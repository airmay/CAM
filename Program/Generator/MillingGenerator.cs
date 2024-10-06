using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Colors;

namespace CAM
{
    public class MillingGenerator : IDisposable
    {
        private readonly IPostProcessor _postProcessor;
        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public MillingGenerator(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
        }

        protected ITechProcess _techProcess;
        protected TechOperation _techOperation;
        public void SetTechOperation(TechOperation techOperation)
        {
            _techOperation = techOperation;
            _techOperation.FirstCommandIndex = null;
        }

        public void AddCommand(ProcessCommand command)
        {
            command.Owner = _techOperation ?? (object)_techProcess;
            command.Number = ProcessCommands.Count + 1;
            ProcessCommands.Add(command);

            if (_techOperation?.FirstCommandIndex.HasValue == false)
                _techOperation.FirstCommandIndex = ProcessCommands.Count - 1;
        }

        private void AddCommands(IEnumerable<string> commands)
        {
            ProcessCommands.AddRange(commands.Select(p => new ProcessCommand { Text = p }));
        }

        //public void WithThick() => _postProcessor.WithThick = true;

        public string GCommand(int gCode, MillToolPosition position, int feed, Point2d? arcCenter = null)
        {
            return null;// _postProcessor.GCommand(gCode, position, feed, arcCenter);
        }

        private void StopMachine() => AddCommands(_postProcessor.StopMachine());

        public void SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            AddCommands(_postProcessor.SetTool(toolNo, angleA, angleC, originCellNumber));
        }

        public void Pause(double duration) => AddCommands(new[] { _postProcessor.Pause(duration) });

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

        protected bool _hasTool;
        protected int _feed;
        protected int _frequency;

        private const int UpperZ = 100;
        public bool _isEngineStarted;

        public double ZSafety { get; set; }
        public bool IsUpperTool => !ToolPosition.IsDefined || ToolPosition.Point.Z >= ZSafety;
        public MillToolPosition ToolPosition { get; set; }
        public string ThickCommand { get; set; }
        public int CuttingFeed { get; set; }
        public int SmallFeed { get; set; }
        public Side EngineSide { get; set; }
        public Tool Tool { get; set; }

        public void StartTechProcess(ITechProcess techProcess)
        {
            _techProcess = techProcess;
            ZSafety = techProcess.ZSafety;
            _postProcessor.Origin = new Point2d(techProcess.OriginX, techProcess.OriginY);

            _documentLock = Acad.ActiveDocument.LockDocument();
            _transaction = Acad.Database.TransactionManager.StartTransaction();
            _currentSpace = (BlockTableRecord)_transaction.GetObject(Acad.Database.CurrentSpaceId, OpenMode.ForWrite, false);

            AddCommands(_postProcessor.StartMachine());
        }

        public void FinishTechProcess()
        {
            StopEngine();
            StopMachine();
        }

        public void Dispose()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _documentLock.Dispose();
        }

        public void SetTool(int toolNo, int frequency, double angleA = 0, bool hasTool = true, double angleC = 0, int originCellNumber = 10)
        {
            StopEngine();
            SetTool(toolNo, angleA, angleC, originCellNumber);
            InitToolPosition(angleC, angleA);

            _frequency = frequency;
            _hasTool = hasTool;
        }

        private void InitToolPosition(double angleC, double angleA)
        {
            ToolPosition = new MillToolPosition
            {
                Z = ZSafety + UpperZ,
                AngleC = angleC,
                AngleA = angleA
            };
            // _postProcessor.SetParams(ToolPosition);
        }

        public void SetZMax(double zMax)
        {
            ZSafety += zMax;
            ToolPosition.Z = ZSafety + UpperZ;
            // _postProcessor.SetParams(ToolPosition);
        }

        private void StopEngine()
        {
            if (_isEngineStarted)
                AddCommands(_postProcessor.StopEngine());
            _isEngineStarted = false;
        }

        public void Uplifting() => GCommand(CommandNames.Uplifting, 0, z: ZSafety);

        public void Move(double? x = null, double? y = null, double? z = null, double? angleC = null, double? angleA = null)
        {
            var name = _isEngineStarted ? CommandNames.Fast : CommandNames.InitialMove;
            GCommand(name, 0, x: x, y: y, z: z, angleC: angleC);
            if (!_isEngineStarted)
                GCommand(CommandNames.InitialMove, 0, z: ZSafety);

            if (angleA.HasValue && !angleA.Value.IsEqual(ToolPosition.AngleA))
                GCommand("Наклон", 1, angleA: angleA, feed: 500);

            if (!_isEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(_frequency, _hasTool));
                _isEngineStarted = true;
            }
        }

        public Matrix3d? Matrix { get; set; }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            if (Matrix.HasValue)
                point = point.TransformBy(Matrix.Value);

            Move(point.X, point.Y, point.Z, angleC, angleA);
        }

        public void Transition(double? x = null, double? y = null, double? z = null, int? feed = null)
        {
            GCommand(CommandNames.Transition, 1, x: x, y: y, z: z, feed: feed ?? SmallFeed);
        }

        public void Penetration(double x, double y, double z, int feed)
        {
            GCommand(CommandNames.Penetration, 1, x: x, y: y, z: z, feed: feed);
        }

        public void Cutting(Point3d startPoint, Point3d endPoint, int cuttingFeed, int transitionFeed, double? angleC = null, double? angleA = null, Side engineSide = Side.None)
        {
            Cutting(NoDraw.Line(startPoint, endPoint), cuttingFeed, transitionFeed, engineSide, angleC, angleA: angleA);
        }

        public void Cutting(Point3d startPoint, Vector3d delta, int cuttingFeed, int smallFeed, double? angleA = null)
        {
            Cutting(startPoint, startPoint + delta, cuttingFeed, smallFeed, angleA);
        }

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
            var point = ToolPosition.IsDefined ? curve.GetClosestPoint(ToolPosition.Point) : curve.StartPoint;
            var calcAngleC = angleC == null;

            if (IsUpperTool)
            {
                if (IsChangeStart)
                    point = curve.NextPoint(point);
                Move(point.X, point.Y, angleC: angleC ?? BuilderUtils.CalcToolAngle(curve, point, engineSide), angleA: angleA);
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
                ToolLocation = ToolPosition.IsDefined ? ToolPosition : null,
                Duration = duration
            });
        }

        public void GCommand(string name, int gCode, string paramsString = null, Point3d? point = null, double? x = null, double? y = null, double? z = null,
            double? angleC = null, double? angleA = null, Curve curve = null, int? feed = null, Point2d? center = null)
        {
            var command = new ProcessCommand { Name = name };

            if (Matrix.HasValue && point.HasValue)
                point = point.Value.TransformBy(Matrix.Value);

            var position = ToolPosition.Create(point, x, y, z, angleC, angleA);

            //if (point.Value.IsEqual(ToolPosition.Point))
            //    return;

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
            // command.Text = _postProcessor.GCommand(gCode, position, feed, center);
            command.HasTool = _hasTool;
            AddCommand(command);

            _feed = feed ?? _feed;
        }

    }
}
