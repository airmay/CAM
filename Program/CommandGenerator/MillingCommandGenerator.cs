using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Генератор команд для фрезерных станков
    /// </summary>
    public abstract class MillingCommandGenerator: CommandGeneratorBase
    {
        protected bool _hasTool;
        protected int _feed;
        protected double _originX, _originY;
        protected int _frequency;

        private const int UpperZ = 100;
        public bool _isEngineStarted = false;

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

        //public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public bool IsUpperTool => !ToolPosition.IsDefined || ToolPosition.Point.Z >= ZSafety;
        public bool WithThick { get; set; }

        public MillToolPosition ToolPosition { get; set; }
        public string ThickCommand { get; set; }
        public int CuttingFeed { get; set; }
        public int SmallFeed { get; set; }
        public Side EngineSide { get; set; }

        public double AC => 165 + Tool.Thickness.GetValueOrDefault();
        public double AC_V => 169 + Tool.Thickness.GetValueOrDefault();
        public double DZ { get; set; }
        public Tool Tool { get; set; }

        public override void StartTechProcess(ITechProcess techProcess)
        {
            _techProcess = techProcess;
            _originX = techProcess.OriginX;
            _originY = techProcess.OriginY;
            ZSafety = techProcess.ZSafety;

            ToolPosition = new MillToolPosition();
            Params = CreateParams(ToolPosition, 0, null);

            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

            StartMachineCommands(_techProcess.Caption);
        }

        /// <summary>
        /// Запуск станка
        /// </summary>
        //protected abstract void StartMachineCommands(string caption);

        public override void FinishTechProcess()
        {
            StopEngine();
            StopMachineCommands();
        }

        public override void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
        }

        /// <summary>
        /// Остановка станка
        /// </summary>
        //protected abstract void StopMachineCommands();
        public void SetTool(int toolNo, int frequency, double angleA = 0, bool hasTool = true, double angleC = 0, int originCellNumber = 10)
        {
            StopEngine();
            //ToolPosition.Set(new Point3d(double.NaN, double.NaN, ZSafety + UpperZ), 0, 0);
            ToolPosition = new MillToolPosition
            {
                Z = ZSafety + UpperZ,
                AngleC = angleC,
                AngleA = angleA
            };
            SetToolCommands(toolNo, angleA, angleC, originCellNumber);

            _frequency = frequency;
            _hasTool = hasTool;
        }

        public void SetZSafety(double zSafety, double zMax = 0)
        {
            ZSafety = zSafety + zMax;
            ToolPosition.Z = ZSafety + UpperZ;
            Params = CreateParams(ToolPosition, 0, null);

            //ToolPosition = new MillToolPosition(new Point3d(double.NaN, double.NaN, ZSafety + UpperZ), 0, 0);
        }

        protected abstract void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber);

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

        public void Uplifting(Vector3d vector) => GCommand(CommandNames.Uplifting, 0, point: ToolPosition.Point + vector);

        /// <summary>
        /// Подвод
        /// </summary>
        public void Move(double? x = null, double? y = null, double? z = null, double? angleC = null, double? angleA = null)
        {
            //if (ToolPosition == null)
            //    ToolPosition = new MillToolPosition(new Point3d(double.NaN, double.NaN, ZSafety + 500), 0, 0);

            //if (IsParamNotChanged(x, ToolPosition.Point.X) && IsParamNotChanged(y, ToolPosition.Point.Y) && IsParamNotChanged(z, ToolPosition.Point.Z)) return;

            GCommand(_isEngineStarted ? CommandNames.Fast : CommandNames.InitialMove, 0, x: x, y: y, z: z, angleC: angleC);
            if (!_isEngineStarted)
                GCommand(CommandNames.InitialMove, 0, z: ZSafety);

            if (angleA.HasValue && !angleA.Value.IsEqual(ToolPosition.AngleA))
                GCommand("Наклон", 1, angleA: angleA, feed: 500);

            if (!_isEngineStarted)
            {
                StartEngineCommands();
                _isEngineStarted = true;
            }
        }

        private bool IsParamNotChanged(double? param, double toolParam) => !param.HasValue || Math.Abs(param.Value - toolParam) < Consts.Epsilon;

        public Matrix3d? Matrix { get; set; }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            if (Matrix.HasValue)
                point = point.TransformBy(Matrix.Value);

            Move(point.X, point.Y, point.Z, angleC, angleA);
        }

        public void Transition(double? x = null, double? y = null, double? z = null, int? feed = null) => GCommand(CommandNames.Transition, 1, x: x, y: y, z: z, feed: feed ?? SmallFeed);

        public abstract void StartEngineCommands();

        /// <summary>
        /// Рез к точке
        /// </summary>
        public void Cutting(double x, double y, double z, int feed) => GCommand(CommandNames.Penetration, 1, x: x, y: y, z: z, feed: feed);

        /// <summary>
        /// Рез между точками
        /// </summary>
        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed, double? angleC = null, double? angleA = null, Side engineSide = Side.None) 
            => Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed, engineSide, angleC, angleA: angleA);

        public void Cutting(Point3d startPoint, Vector3d delta, int cuttingFeed, int smallFeed, double? angleA = null) => Cutting(startPoint, startPoint + delta, cuttingFeed, smallFeed, angleA);

        public void Cutting(Point3d startPoint, Vector3d delta, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double? angleA = null)
        {
            using (var line = NoDraw.Line(startPoint, startPoint + delta))
                Cutting(line, zArray, cuttingFeed, smallFeed, engineSide, angleC, angleA);
        }

        public void Cutting(Curve curve, double[] zArray, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double? angleA = null)
        {
            foreach (var z in zArray)
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * z);
                var zCurve = (Curve)curve.GetTransformedCopy(matrix);
                Cutting(zCurve, cuttingFeed, smallFeed, engineSide, angleC, angleA);
            }
        }

        public void Cutting(Curve curve, double offset = 0, double? dz = null, double? angleC = null, double? angleA = null, bool IsChangeStart = false)
        {
            var toolpathCurve = curve.GetOffsetCurves(offset)[0] as Curve;
            if (dz.HasValue)
                toolpathCurve.TransformBy(Matrix3d.Displacement(Vector3d.ZAxis * dz.Value));
            Cutting(toolpathCurve, CuttingFeed, SmallFeed, EngineSide, angleC: angleC, angleA: angleA, IsChangeStart: IsChangeStart);
        }

        /// <summary>
        /// Рез по кривой
        /// </summary>
        public void Cutting(Curve curve, int cuttingFeed, int smallFeed, Side engineSide = Side.None, double? angleC = null, double? angleA = null, bool IsChangeStart = false)
        {
            if (ToolPosition == null)
                ToolPosition = new MillToolPosition(new Point3d(double.NaN, double.NaN, ZSafety + 500), 0, 0);

            var point = ToolPosition.IsDefined ? curve.GetClosestPoint(ToolPosition.Point) : curve.StartPoint;
            var calcAngleC = angleC == null;

            if (IsUpperTool) // && (angleA ?? AngleA).GetValueOrDefault() == 0)
            {
                if (IsChangeStart)
                    point = curve.NextPoint(point);
                //var upperPoint = new Point3d(point.X, point.Y, ZSafety);
                //if (!ToolPosition.Point.IsEqualTo(upperPoint))
                //{
                if (!angleC.HasValue)
                    angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

                Move(point.X, point.Y, angleC: angleC, angleA: angleA);
                //}
            }
            else if (!(curve is Line) && calcAngleC)
                angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

            if (!point.IsEqual(ToolPosition.Point))
                GCommand(point.Z != ToolPosition.Point.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, angleA: angleA, feed: smallFeed);

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
                    if (calcAngleC)
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
                if (arc != null && calcAngleC)
                    angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
                var gCode = curve is Line ? 1 : point == curve.StartPoint ? 3 : 2;
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
                ToolLocation = ToolPosition.IsDefined ? ToolPosition : null,
                Duration = duration
            });
        }

        public virtual void Pause(double duration) { }

        public virtual void Cycle() { }

        public void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null,
            double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null, Point2d? center = null)
        {
            var command = new ProcessCommand { Name = name };

            if (Matrix.HasValue && point.HasValue)
                point = point.Value.TransformBy(Matrix.Value);

            var position = ToolPosition.Create(point, x, y, z, angleC, angleA);

            //if (point.Value.IsEqual(ToolPosition.Point))
            //    return;

            if (ThickCommand != null && (position.Point.X != ToolPosition.Point.X || position.Point.Y != ToolPosition.Point.Y)) // TODO ThickCommand
                AddCommand(
                    new ProcessCommand
                    {
                        Text = string.Format(ThickCommand, position.Point.X.Round(), position.Point.Y.Round()),
                        HasTool = _hasTool,
                        ToolLocation = ToolPosition.Clone()
                    });
            
            if (ToolPosition.IsDefined)
            {
                double length;
                if (curve == null)
                {
                    length = ToolPosition.Point.DistanceTo(position.Point);
                    if (length > 1)
                        curve = NoDraw.Line(ToolPosition.Point, position.Point);
                }
                else
                    length = curve.Length();

                if (curve != null && curve.IsNewObject)
                {
                    if (Colors.ContainsKey(name))
                        curve.Color = Colors[name];
                    curve.LayerId = _layerId;
                    curve.Visible = false;
                    _currentSpace.AppendEntity(curve);
                    _transaction.AddNewlyCreatedDBObject(curve, true);
                }
                command.ToolpathObjectId = curve?.ObjectId;

                if ((feed ?? _feed) != 0)
                    command.Duration = length / (gCode == 0 ? 10000 : feed ?? _feed) * 60;
            }

            if (position.IsDefined)
                command.ToolLocation = position;

            ToolPosition = position;
            //command.Text = GCommandText(gCode, paramsString, point.Value, curve, angleC, angleA, feed, center);
            //command.Text = GCommandText(gCode, paramsString, position, position.Point, curve, angleC, angleA, feed, center);
            command.Text = GetGCommand(gCode, feed, center);
            command.HasTool = _hasTool;
            AddCommand(command);

            _feed = feed ?? _feed;
        }

        protected virtual string GetGCommand(int gCode, int? feed, Point2d? center = null)
        {
            var par = CreateParams(ToolPosition, feed, center);
            var textParams = GetTextParams(par);
            return $"G{gCode}{CommandDelimiter}{textParams}";
        }

        public virtual Dictionary<string, double?> CreateParams(MillToolPosition position, int? feed, Point2d? center)
        {
            var newParams = position.GetParams();
            newParams["F"] = feed;
            newParams["I"] = center?.X.Round(4);
            newParams["J"] = center?.Y.Round(4);
            return newParams;
        }

        protected abstract string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center);
    }
}
