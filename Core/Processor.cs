using CAM.Program.Generator;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;

namespace CAM
{
    public class Processor : IDisposable
    {
        const double Epsilon = 0.000001;
        public Point3d CenterPoint { get; set; }
        public Vector2d Vector { get; set; } = Vector2d.YAxis;

        public double U { get; set; }
        public double V { get; set; }
        public int Feed { get; set; }
        public int S { get; set; }

        private bool _isSetPosition;
        public bool _isEngineStarted = false;

        public Point2d Center { get; set; }
        public bool IsExtraRotate { get; set; }
        private Vector2d _normal;
        private double _angle = 0;
        private double _signU;

        public void SetToolPosition1(Point3d centerPoint, double angle, double u, double v)
        {
            Center = centerPoint.To2d();
            _angle = angle;
            U = u.Round(6);
            V = v.Round(6);
            _isSetPosition = true;
        }

        private CableToolPosition GetToolPosition1()
        {
            var point = new Point3d(Center.X - U, Center.Y, V);
            var angle = _angle.ToRad(); // Vector2d.YAxis.MinusPiToPiAngleTo(Vector);
            return new CableToolPosition(point, Center.ToPoint3d(), angle);
        }
        protected void StartEngineCommands1()
        {
            Command1($"M03", "Включение");
            _isEngineStarted = true;
        }
        public void Command1(string text, string name = null, double duration = 0)
        {
            AddCommand1(new Command
            {
                Name = name,
                Text = text,
                //HasTool = _hasTool,
                ToolLocation = GetToolPosition1(),
                Duration = duration,
                U = U,
                V = V,
                //A =  Vector2d.YAxis.MinusPiToPiAngleTo(Vector).ToDeg(6)
                A = _angle
            });
        }
        public void AddCommand1(Command command)
        {
            command.Owner = _operation;
            command.Number = CamManager.Commands.Count + 1;
            CamManager.Commands.Add(command);
        }
        public void GCommandAngle1(Vector2d vector, int s)
        {
            var angle = Vector.MinusPiToPiAngleTo(vector);
            if (Math.Abs(angle.ToDeg(6)) >= 90)
            {
                vector = vector.Negate();
                angle = Vector.MinusPiToPiAngleTo(vector);
            }
            if (Math.Abs(angle.ToDeg()) < 0.000001)
                return;

            //var da = angle - Angle;
            //if (da > 180)
            //    da -= 360;
            //if (da < -180)
            //    da += 360;

            Vector = vector;
            Command1($"G05 A{angle.ToDeg(6)} S{s}", "Rotate");
            //Command($"G05 A{angle.ToDeg(6)} S{s} A={Vector.Angle.ToDeg(6)} U={U.Round(6)}", "Rotate");
        }
        public void GCommand1(int gCode, Line2d line2d, double z, bool isRevereseAngle)
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

            if (angle - _angle > 0.01 && IsExtraRotate)
                GCommandA1(angle + 1);

            GCommandA1(angle);

            var u = (_signU * normal.Length).Round(4);
            var v = z.Round(4);

            if (gCode == 0)
            {
                if (u - U < 0 && IsExtraRotate)
                    GCommandUV(0, u - 5, V);
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
                StartEngineCommands1();
            }
        }

        public void GCommandUV(int gCode, double u, double v)
        {
            if (u == U && v == V)
                return;

            var du = (u - U).Round(4);
            var dv = (v - V).Round(4);
            U = u;
            V = v;

            var feed = gCode == 1 ? $"F{Feed}" : "";
            var text = $"G0{gCode} U{du} V{dv} {feed}";
            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Feed) * 60;

            Command1(text, duration: duration);
        }

        public void GCommandA1(double angle)
        {
            if (Math.Abs(angle - _angle) < 0.01)
                return;

            var da = (angle - _angle).Round(4);
            _angle = angle;

            var text = $"G05 A{da} S{S}";
            var duration = Math.Abs(da) / S * 60;

            Command1(text, "Rotate", duration);
        }


        public void GCommand1(int gCode, Line3d line3d, bool isRevereseAngle = false) => GCommand1(gCode, new Line2d(line3d.PointOnLine.To2d(), line3d.Direction.ToVector2d()), line3d.PointOnLine.Z, isRevereseAngle);

        public void GCommand1(int gCode, Point3d point1, Point3d point2, bool isRevereseAngle = false) => GCommand1(gCode, new Line2d(point1.To2d(), point2.To2d()), (point1.Z + point2.Z) / 2, isRevereseAngle);


        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>



        private readonly IPostProcessor _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        private Operation _operation;
        public bool IsEngineStarted;

        public Point3d Position { get; set; }
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        protected int _frequency;
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public Point2d Origin { get; set; }
        public Side EngineSide { get; set; }

        public double ZSafety { get; set; } = 20;
        public double ZMax { get; set; } = 0;
        public double UpperZ => ZMax + ZSafety;
        public bool IsUpperTool => Position.Z > ZMax;
        private const int CommandListCapacity = 10_000;

        public Processor(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
        }

        public void Start(Tool tool)
        {
            //Position = Algorithms.NullPoint3d.WithZ(ZMax + ZSafety * 3);
            //_postProcessor.GCommand(-1, Position, 0, 0, null);

            if (CamManager.Commands == null)
                CamManager.Commands = new List<Command>(CommandListCapacity);
            CamManager.Commands.Clear();

            //AddCommands(_postProcessor.StartMachine());
            //AddCommands(_postProcessor.SetTool(tool.Number, 0, 0, 0));

            _toolpathBuilder = new ToolpathBuilder();
        }

        public void SetGeneralOperarion(GeneralOperation generalOperation)
        {
            _frequency = generalOperation.Frequency;
            CuttingFeed = generalOperation.CuttingFeed;
            PenetrationFeed = generalOperation.PenetrationFeed;
            ZSafety = generalOperation.ZSafety;
            Origin = generalOperation.Origin;
        }

        public void SetOperation(Operation operation)
        {
            _operation = operation;
            _operation.FirstCommandIndex = CamManager.Commands.Count;
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
        }

        public void Cycle() => AddCommand(_postProcessor.Cycle());

        public void Cutting(Line line, CurveTip tip)
        {
            var point = line.GetPoint(tip);
            if (IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, EngineSide);
                Move(point, angleC);
                Cycle();
            }
            Penetration(point);
            Cutting(line, line.NextPoint(point));
        }

        public void Move(Point3d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, point.WithZ(Position.Z));
            if (Position.Z > UpperZ)
                GCommandTo(CommandNames.InitialMove, 0, Position.WithZ(UpperZ));
            if (angleC != null && angleC.Value != AngleC)
                TurnC(angleC.Value);
            if (angleA != null && angleA.Value != angleA)
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(_frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand("Поворот", 0, angleC: angleC);

        public void TurnA(double angleA) => GCommand("Наклон", 1, feed: 500, angleA: angleA);

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, Position.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(CommandNames.Penetration, 1, point, PenetrationFeed);

        public void Cutting(Point3d point) => GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);

        private void Cutting(Line line, Point3d point) => GCommand(CommandNames.Cutting, 1, CuttingFeed, line, point);

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            Line line = null;
            if (!Position.IsNull())
            {
                if (point.IsEqualTo(Position))
                    return;
                line = NoDraw.Line(Position, point);
            }

            GCommand( name, gCode, feed, line, point);
        }

        public void GCommand(string name, int gCode, int? feed = null, Curve curve = null, Point3d? point = null, double? angleC = null, double? angleA = null, Point2d? arcCenter = null)
        {
            Position = point ?? Position;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
            var commandText = _postProcessor.GCommand(gCode, Position, AngleC, AngleA, feed, arcCenter);
            ObjectId? toolpath = null;
            if (curve != null)
            {
                if (curve.Length() > 1)
                    toolpath = _toolpathBuilder.AddToolpath(curve, name);
                _operation.Duration += curve.Length() / feed.GetValueOrDefault(10000) * 60;
            }
            AddCommand(name, commandText, toolpath);
        }

        public void AddCommand(string text, string name = null, ObjectId? toolpath = null)
        {
            CamManager.Commands.Add(new Command
            {
                Name = name,
                Text = text,
                Position = Position,
                AngleA = AngleA,
                AngleC = AngleC,
                Toolpath = toolpath,
                Operation = _operation,
                Number = CamManager.Commands.Count + 1
            });
        }

        private void AddCommands(string[] commands)
        {
            Array.ForEach(commands, p => AddCommand(p));
        }

        public void Dispose()
        {
            _toolpathBuilder.Dispose();
        }
    }
}