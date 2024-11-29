using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorWireSaw : IProcessor
    {
        private readonly PostProcessorWireSaw _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        private readonly ProcessingWireSaw _processing;
        public bool IsEngineStarted;
        private IOperation _operation;
        private double _processDuration;
        private double _operationDuration;

        public ToolLocationWireSaw Location { get; } = new ToolLocationWireSaw();
        public int CuttingFeed { get; set; }

        public Side EngineSide { get; set; }

        public double ZSafety => _processing.ZSafety;
        public double ZMax { get; set; } = 0;
        public double UpperZ => ZMax + ZSafety;

        public ProcessorWireSaw(ProcessingWireSaw processing, PostProcessorWireSaw postProcessor)
        {
            _processing = processing;
            _postProcessor = postProcessor;
            CuttingFeed = _processing.CuttingFeed;
        }

        public void SetOperation(IOperation operation)
        {
            if (_operation != null)
                FinishOperation();
            _operation = operation;
            _toolpathBuilder.CreateGroup();
        }

        public void SetToolPosition(Point3d centerPoint, double angle, double u, double v)
        {
            Center = centerPoint.To2d();
            _angle = angle;
            U = u.Round(6);
            V = v.Round(6);
            _isSetPosition = true;
        }

        private void FinishOperation()
        {
            _operation.ToolpathGroupId = _toolpathBuilder.AddGroup(_operation.Caption);
            _operation.Caption = GetCaption(_operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
        }

        private static string GetCaption(string caption, double duration)
        {
            var ind = caption.IndexOf('(');
            var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
            return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
        }

        public ObjectId AddEntity(Entity curve) => _toolpathBuilder.AddEntity(curve);

        public void Dispose()
        {
            _toolpathBuilder.Dispose();
        }

        public void Start()
        {
            ProcessingBase.Program.Init(_processing);
            _toolpathBuilder = new ToolpathBuilder();

            Location.Z = ZMax + ZSafety * 3;
            //_postProcessor.GCommand(-1, Position, 0, 0, null);

            AddCommands(_postProcessor.StartMachine());
            AddCommands(_postProcessor.SetTool(Tool.Number, 0, 0, 0));
        }


        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
            ProcessingBase.Program.CreateProgram();
            FinishOperation();
            _processing.Caption = GetCaption(_processing.Caption, _processDuration);
        }

        public void Cycle() => AddCommand(_postProcessor.Cycle());

        public void AddCommand(string text, string name = null, ObjectId? toolpath = null)
        {
            if (text == null)
                return;

            ProcessingBase.Program.AddCommand(new Command
            {
                Name = name,
                Text = text,
                ToolLocationParams = Location.GetParams(),
                ObjectId = toolpath,
                Operation = _operation,
            });
        }

        private void AddCommands(string[] commands)
        {
            Array.ForEach(commands, p => AddCommand(p));
        }

        #region GCommands

        public void Cutting(Line line, CurveTip tip)
        {
            var point = line.GetPoint(tip);
            if (Location.Z > ZMax)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, EngineSide);
                Move(point, angleC);
                //Cycle();
            }
            Penetration(point);
            Cutting(line, line.NextPoint(point));
        }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, point.WithZ(Location.Z));
            if (Location.Z > UpperZ)
                GCommandTo(CommandNames.InitialMove, 0, point.WithZ(UpperZ));
            if (angleC.HasValue)
                TurnC(angleC.Value);
            if (angleA.HasValue && angleA.Value != Location.AngleA)
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(Frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand("Поворот", 0, angleC: angleC);

        public void TurnA(double angleA) => GCommand("Наклон", 1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, Location.Point.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(CommandNames.Penetration, 1, point, PenetrationFeed);

        public void Penetration(double z) => Penetration(Location.Point.WithZ(z));

        public void Cutting(Point3d point) => GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);

        private void Cutting(Line line, Point3d point) => GCommand(CommandNames.Cutting, 1, CuttingFeed, line, point);

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            Line line = null;
            if (Location.IsDefined)
            {
                if (point.IsEqualTo(Location.Point))
                    return;
                line = NoDraw.Line(Location.Point, point);
            }

            GCommand( name, gCode, feed, line, point);
        }

        public void GCommand(string name, int gCode, int? feed = null, Curve curve = null, Point3d? point = null,
            double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            Location.Set(point, angleC, angleA);
            var commandText = _postProcessor.GCommand(gCode, Location, feed, arcCenter);
            ObjectId? toolpath = null;
            if (curve != null)
            {
                if (curve.IsNewObject)
                    _operationDuration += curve.Length() / (feed ?? 10000) * 60;

                if (curve.Length() > 1)
                    toolpath = _toolpathBuilder.AddToolpath(curve, name);
            }

            AddCommand(commandText, name, toolpath);
        }

        #endregion

        public void GCommand(int gCode, Line3d line3d, bool isRevereseAngle = false) => GCommand(gCode, new Line2d(line3d.PointOnLine.To2d(), line3d.Direction.ToVector2d()), line3d.PointOnLine.Z, isRevereseAngle);

        public void GCommand(int gCode, Point3d point1, Point3d point2, bool isRevereseAngle = false) => GCommand(gCode, new Line2d(point1.To2d(), point2.To2d()), (point1.Z + point2.Z) / 2, isRevereseAngle);

        public void GCommand(int gCode, Line2d line2d, double z, bool isRevereseAngle)
        {
            var angle = Vector2d.YAxis.MinusPiToPiAngleTo(line2d.Direction).ToDeg(4);
            var da = Math.Sign(_angle - angle) * 180;
            while (Math.Abs(angle - _angle) > 90)
                angle += da;
            angle = angle.Round(4);

            if (isRevereseAngle)
                angle += -Math.Sign(angle) * 180;

            var normal = !line2d.IsOn(Center) ? line2d.GetClosestPointTo(Center).Point - Center : new Vector2d();
            if (!_normal.IsZeroLength() && !normal.IsZeroLength())
            {
                if (Math.Abs(angle - _angle) == 90)
                    angle = _angle + normal.MinusPiToPiAngleTo(_normal) > 0 ? 90 : -90;
                else
                    if (normal.GetAngleTo(_normal) > Math.PI / 2)
                    _signU *= -1;
            }
            if (_normal.IsZeroLength())
                _signU = -Math.Sign(angle) * Math.Sign(normal.Y);

            if (_signU == 0)
                _signU = -1;

            if (gCode == 0 && Math.Abs(angle - _angle) > 0.01 && IsExtraRotate)
                GCommandA(angle + Math.Sign(angle - _angle));

            GCommandA(angle);

            var u = (_signU * normal.Length).Round(4);
            var v = z.Round(4);

            if (gCode == 0)
            {
                if (IsExtraMove)
                    GCommandUV(0, u + Math.Sign(u - U) * 20, V);
                GCommandUV(0, u, V);
                GCommandUV(0, u, v);
            }
            else
            {
                GCommandUV(1, u, v);
            }
            _normal = normal;

            if (!_isEngineStarted)
            {
                StartEngineCommands();
            }
        }

        public void GCommandUV(int gCode, double u, double v)
        {
            if (u == U && v == V)
                return;

            var du = -(u - U).Round(4);
            if (gCode == 1 && DU != 0)
                du = (Math.Abs(du) + DU) * Math.Sign(du);

            var dv = (v - V).Round(4);
            U = u;
            V = v;

            var feed = gCode == 1 ? $"F{Feed}" : "";
            var text = $"G0{gCode} U{du} V{dv} {feed}";
            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;

            Command(text, duration: duration);
        }

        public void GCommandA(double angle)
        {
            if (Math.Abs(angle - Location.Angle) < 0.01)
                return;

            var da = (angle - Location.Angle).Round(4);
            Location.Angle += da;

            var text = $"G05 A{da} S{_processing.S}";
            var duration = Math.Abs(da) / _processing.S * 60;

            Command(text, "Rotate", duration);
        }

        public void Command(string text, string name = null, double duration = 0)
        {
            AddCommand(new ProcessCommand
            {
                Name = name,
                Text = text,
                //HasTool = _hasTool,
                ToolLocation = GetToolPosition(),
                Duration = duration,
                U = U,
                V = V,
                //A =  Vector2d.YAxis.MinusPiToPiAngleTo(Vector).ToDeg(6)
                A = _angle
            });
        }

        private CableToolPosition GetToolPosition()
        {
            var point = new Point3d(Center.X - U, Center.Y, V);
            var angle = _angle.ToRad(); // Vector2d.YAxis.MinusPiToPiAngleTo(Vector);
            return new CableToolPosition(point, Center.ToPoint3d(), angle);
        }
    }
}