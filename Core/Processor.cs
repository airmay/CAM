using CAM.Program.Generator;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM
{
    public class Processor
    {
        private readonly IPostProcessor _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        public bool IsEngineStarted;
        public bool IsUpperTool { get; set; }
        public List<Command> ProcessCommands { get; } = new List<Command>();
        public Point3d ToolPosition { get; set; }
        public double AngleA { get; set; }
        public double AngleC { get; set; }

        private Operation _operation;
        protected int _frequency;
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; }
        public double UpperZ => ZMax + ZSafety;
        public double ZMax { get; set; }
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public Side EngineSide { get; set; }


        public Processor(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
        }

        public void Start(Tool tool)
        {
            ToolPosition = Algorithms.NullPoint3d.WithZ(ZMax + ZSafety * 3);
            _postProcessor.GCommand(-1, ToolPosition, 0, 0, null);

            _postProcessor.StartMachine();
            _postProcessor.SetTool(tool.Number, 0, 0, 0);
            _toolpathBuilder = new ToolpathBuilder();
        }

        public void SetGeneralOperarion(GeneralOperation generalOperation)
        {
            _frequency = generalOperation.Frequency;
            CuttingFeed = generalOperation.CuttingFeed;
            PenetrationFeed = generalOperation.PenetrationFeed;
            ZSafety = generalOperation.ZSafety;
            OriginX = generalOperation.OriginX;
            OriginY = generalOperation.OriginY;
        }

        public void SetOperarion(Operation operation)
        {
            _operation = operation;
            _operation.FirstCommandIndex = ProcessCommands.Count;
        }

        public void Finish() => _postProcessor.Finish();

        public void Cycle() => _postProcessor.Cycle();

        public void Cutting(Line line, CurveTip tip)
        {
            var point = line.GetPoint(tip);
            if (IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, EngineSide);
                Move(point.ToPoint2d(), angleC);
                Cycle();
            }
            Penetration(point);
            Cutting(line, line.NextPoint(point));
        }

        public void Cutting(Arc arc, CurveTip tip)
        {
            var point = arc.GetPoint(tip);
            if (IsUpperTool)
            {
                var moveAngleC = BuilderUtils.CalcToolAngle(arc, point, EngineSide);
                Move(point.ToPoint2d(), moveAngleC);
                Cycle();
            }
            Penetration(point);
            point = arc.NextPoint(point);
            var angleC = BuilderUtils.CalcToolAngle(arc, point, EngineSide);
            Cutting(arc, point, angleC);
        }

        public void Move(Point2d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, ToolPosition.WithPoint2d(point));
            if (!IsEngineStarted)
                GCommandTo(CommandNames.InitialMove, 0, ToolPosition.WithZ(UpperZ));
            if (angleC.HasValue)
                TurnC(angleC.Value);
            if (angleA.HasValue && angleA.Value != AngleA)
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                _postProcessor.StartEngine(_frequency, true);
                IsEngineStarted = true;
            }
        }

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, ToolPosition.WithZ(UpperZ));

        public void Penetration(Point3d point)
        {
            GCommandTo(CommandNames.Penetration, 1, point, PenetrationFeed);
        }

        public void Cutting(Point3d point)
        {
            GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);
        }

        public void Cutting(Line line, Point3d point)
        {
            GCommand(CommandNames.Cutting, 1, line, point, AngleC, AngleA, CuttingFeed);
        }

        public void Cutting(Arc arc, Point3d point, double angleC)
        {
            var gCode = point == arc.StartPoint ? 3 : 2;
            var commandText = _postProcessor.GCommand(gCode, point, angleC, AngleA, CuttingFeed, arc.Center.ToPoint2d());
            ToolPosition = point;
            AngleC = angleC;
            AddCommand(CommandNames.Cutting, commandText, arc, CuttingFeed);
        }

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            Line line = null;
            if (!ToolPosition.IsNull())
            {
                if (point.IsEqualTo(ToolPosition))
                    return;
                line = NoDraw.Line(ToolPosition, point);
            }

            GCommand( name, gCode, line, point, AngleC, AngleA, feed);
        }

        public void TurnC(double angleC)
        {
            GCommand("Поворот", 0, null, ToolPosition, angleC, AngleA, null);
        }

        public void TurnA(double angleA)
        {
            GCommand("Наклон", 1, null, ToolPosition, AngleC, angleA, 500);
        }

        private void GCommand(string name, int gCode, Curve curve, Point3d point, double angleC, double angleA, int? feed)
        {
            var commandText = _postProcessor.GCommand(gCode, point, angleC, angleA, feed);
            ToolPosition = point;
            AngleC = angleC;
            AngleA = angleA;
            AddCommand(name, commandText, curve, feed);
        }

        public void AddCommand(string name, string text, Curve curve = null, int? feed = null)
        {
            var command = new Command
            {
                Name = name,
                Text = text,
                Point = ToolPosition,
                AngleA = AngleA,
                AngleC = AngleC,
                Operation = _operation,
                Number = ProcessCommands.Count + 1
            };
            if (curve != null)
            {
                command.Toolpath = _toolpathBuilder.AddToolpath(curve, name);
                _operation.Duration += curve.Length() / feed.GetValueOrDefault(10000) * 60;
            }
            ProcessCommands.Add(command);
        }
    }
}