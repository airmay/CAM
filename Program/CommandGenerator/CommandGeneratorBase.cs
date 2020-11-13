using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;

namespace CAM
{
    public abstract class CommandGeneratorBase: IDisposable
    {
        private ITechProcess _techProcess;
        private ITechOperation _techOperation;

        protected bool _hasTool;
        protected int _GCode;
        protected int _feed;
        protected double _originX, _originY;
        protected int _frequency;

        private const int UpperZ = 100;
        private bool _isEngineStarted = false;

        private DocumentLock _documentLock;
        private Transaction _transaction;
        private BlockTableRecord _currentSpace;
        private ObjectId _layerId = Acad.GetProcessLayerId();
        private readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };

        public double ZSafety { get; set; }

        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public bool IsUpperTool => !ToolLocation.IsDefined || ToolLocation.Point.Z >= ZSafety;
        public bool WithThick { get; set; }
        public Location ToolLocation { get; set; } = new Location();
        public string ThickCommand { get; set; }

        public void StartTechProcess(ITechProcess techProcess)
        {
            _techProcess = techProcess;
            _originX = techProcess.OriginX;
            _originY = techProcess.OriginY;
            ZSafety = techProcess.ZSafety;

            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

            StartMachineCommands(_techProcess.Caption);
        }

        /// <summary>
        /// Запуск станка
        /// </summary>
        protected abstract void StartMachineCommands(string caption);

        public void FinishTechProcess()
        {
            StopEngine();
            StopMachineCommands();
        }

        public void AddCommand(ProcessCommand command)
        {
            command.Owner = _techOperation ?? (object)_techProcess;
            command.Number = ProcessCommands.Count + 1;
            ProcessCommands.Add(command);

            if (_techOperation != null && !_techOperation.ProcessCommandIndex.HasValue)
                _techOperation.ProcessCommandIndex = ProcessCommands.Count - 1;
        }

        public void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
        }

        /// <summary>
        /// Остановка станка
        /// </summary>
        protected abstract void StopMachineCommands();

        public void SetTechOperation(ITechOperation techOperation) => _techOperation = techOperation;

        public void SetTool(int toolNo, int frequency, double angleA = 0, bool hasTool = true)
        {
            StopEngine();
            ToolLocation.Set(new Point3d(double.NaN, double.NaN, ZSafety + UpperZ), 0, 0);
            SetToolCommands(toolNo, angleA);

            _frequency = frequency;
            _hasTool = hasTool;
        }

        public void SetZSafety(double zSafety, double zMax = 0)
        {
            ZSafety = zSafety + zMax;
            ToolLocation.Set(new Point3d(double.NaN, double.NaN, ZSafety + UpperZ), 0, 0);
        }

        protected abstract void SetToolCommands(int toolNo, double angleA);

        private void StopEngine()
        {
            if (_isEngineStarted)
                StopEngineCommands();
            _isEngineStarted = false;
        }

        protected abstract void StopEngineCommands();

        /// <summary>
        /// Поднятние
        /// </summary>
        public void Uplifting(double? z = null) => GCommand(CommandNames.Uplifting, 0, z: z ?? ZSafety);

        public void Uplifting(Vector3d vector) => GCommand(CommandNames.Uplifting, 0, point: ToolLocation.Point + vector);

        /// <summary>
        /// Подвод
        /// </summary>
        public void Move(double? x = null, double? y = null, double? z = null, double? angleC = null, double angleA = 0)
        {
            GCommand(_isEngineStarted ? CommandNames.Fast : CommandNames.InitialMove, 0, x: x, y: y, z: z, angleC: angleC);
            if (!_isEngineStarted)
                GCommand(CommandNames.InitialMove, 0, z: ZSafety);

            if (angleA != ToolLocation.AngleA)
                GCommand("Наклон", 1, angleA: angleA, feed: 500);

            if (!_isEngineStarted)
            {
                StartEngineCommands();
                _isEngineStarted = true;
            }
        }

        public void Transition(double? x = null, double? y = null, double? z = null, int? feed = null) => GCommand(CommandNames.Transition, 1, x: x, y: y, z: z);

        protected abstract void StartEngineCommands();

        /// <summary>
        /// Рез к точке
        /// </summary>
        public void Cutting(double x, double y, double z, int feed) => GCommand(CommandNames.Penetration, 1, x: x, y: y, z: z, feed: feed);

        /// <summary>
        /// Рез между точками
        /// </summary>
        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed, double angleA = 0) => Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed, angleA: angleA);

        public void Cutting(Point3d startPoint, Vector3d delta, int cuttingFeed, int smallFeed) => Cutting(startPoint, startPoint + delta, cuttingFeed, smallFeed);

        public void Cutting(Point3d startPoint, Vector3d delta, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0)
        {
            using (var line = NoDraw.Line(startPoint, startPoint + delta))
                Cutting(line, zArray, cuttingFeed, smallFeed, engineSide, angleC, angleA);
        }

        public void Cutting(Curve curve, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0)
        {
            foreach (var z in zArray)
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * z);
                var zCurve = (Curve)curve.GetTransformedCopy(matrix);
                Cutting(zCurve, cuttingFeed, smallFeed, engineSide, angleC, angleA);
            }
        }

        /// <summary>
        /// Рез по кривой
        /// </summary>
        public void Cutting(Curve curve, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double angleA = 0)
        {
            var point = curve.GetClosestPoint(ToolLocation.Point);

            if (IsUpperTool && angleA == 0)
            {
                var upperPoint = new Point3d(point.X, point.Y, ZSafety);
                if (!ToolLocation.Point.IsEqualTo(upperPoint))
                {
                    Move(upperPoint.X, upperPoint.Y, angleC: angleC ?? BuilderUtils.CalcToolAngle(curve, point, engineSide));
                }
            }
            else if (!(curve is Line))
                angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

            GCommand(point.Z != ToolLocation.Point.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, angleA: angleA, feed: smallFeed);

            if (curve is Polyline polyline)
            {
                if (point == polyline.EndPoint)
                {
                    polyline.ReverseCurve();
                    engineSide = engineSide.Opposite();
                }
                for (int i = 1; i < polyline.NumberOfVertices; i++)
                {
                    point = polyline.GetPoint3dAt(i);
                    angleC = BuilderUtils.CalcToolAngle(polyline, point, engineSide);
                    if (polyline.GetSegmentType(i - 1) == SegmentType.Arc)
                    {
                        var arcSeg = polyline.GetArcSegment2dAt(i - 1);
                        GCommand(CommandNames.Cutting, arcSeg.IsClockWise ? 2 : 3, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed, center: arcSeg.Center);
                    }
                    else
                        GCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed);
                }
            }
            else
            {
                var arc = curve as Arc;
                if (arc != null)
                    angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
                var gCode = curve is Line ? 1 : point == curve.StartPoint ? 2 : 3;
                GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());
            }
        }

        public void Command(string text, string name = null, double duration = 0)
        {
            AddCommand(new ProcessCommand
            {
                Name = name,
                Text = text,
                HasTool = _hasTool,
                ToolLocation = ToolLocation.Clone(),
                Duration = duration
            });
        }

        public virtual void Pause(double duration) { }

        public virtual void Cycle() { }

        public void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null,
            double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null, Point2d? center = null)
        {
            var command = new ProcessCommand { Name = name };

            if (point == null)
                point = new Point3d(x ?? ToolLocation.Point.X, y ?? ToolLocation.Point.Y, z ?? ToolLocation.Point.Z);

            if (ThickCommand != null && (point.Value.X != ToolLocation.Point.X || point.Value.Y != ToolLocation.Point.Y))
                AddCommand(
                    new ProcessCommand
                    {
                        Text = string.Format(ThickCommand, point.Value.X.Round(), point.Value.Y.Round()),
                        HasTool = _hasTool,
                        ToolLocation = ToolLocation.Clone()
                    });

            command.Text = GCommandText(gCode, paramsString, point.Value, curve, angleC, angleA, feed, center);
            
            if (ToolLocation.IsDefined)
            {
                if (curve == null && ToolLocation.Point.DistanceTo(point.Value) > 1)
                    curve = NoDraw.Line(ToolLocation.Point, point.Value);

                double length = 0;
                if (curve != null && curve.IsNewObject)
                {
                    if (Colors.ContainsKey(name))
                        curve.Color = Colors[name];
                    curve.LayerId = _layerId;
                    curve.Visible = false;
                    _currentSpace.AppendEntity(curve);
                    _transaction.AddNewlyCreatedDBObject(curve, true);
                    length = curve.Length();
                }
                command.ToolpathObjectId = curve?.ObjectId;

                if ((feed ?? _feed) != 0)
                    command.Duration = (curve != null ? length : ToolLocation.Point.DistanceTo(point.Value)) / (gCode == 0 ? 10000 : feed ?? _feed) * 60;
            }

            _GCode = gCode;
            ToolLocation.Set(point.Value, angleC, angleA);
            _feed = feed ?? _feed;

            command.HasTool = _hasTool;
            command.ToolLocation = ToolLocation.Clone();

            AddCommand(command);
        }

        protected abstract string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center);
    }
}
