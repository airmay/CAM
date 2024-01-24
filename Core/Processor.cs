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

            AddCommands(_postProcessor.StartMachine());
            AddCommands(_postProcessor.SetTool(tool.Number, 0, 0, 0));
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

        public void Finish() => AddCommands(_postProcessor.Finish());

        public void Cycle() => AddCommand(_postProcessor.Cycle());

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

        public void Move(Point2d point, double? angleC = null, double? angleA = null)
        {
            GCommandTo(CommandNames.Fast, 0, ToolPosition.WithPoint2d(point));
            if (!IsEngineStarted)
                GCommandTo(CommandNames.InitialMove, 0, ToolPosition.WithZ(UpperZ));
            if (angleC.HasValue)
                TurnC(angleC.Value);
            if (angleA.HasValue)
                TurnA(angleA.Value);

            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine(_frequency, true));
                IsEngineStarted = true;
            }
        }

        public void TurnC(double angleC) => GCommand("Поворот", 0, angleC: angleC);

        public void TurnA(double angleA) => GCommand("Наклон", 1, angleA: angleA, feed: 500);

        public void Uplifting() => GCommandTo(CommandNames.Uplifting, 0, ToolPosition.WithZ(UpperZ));

        public void Penetration(Point3d point) => GCommandTo(CommandNames.Penetration, 1, point, PenetrationFeed);

        public void Cutting(Point3d point) => GCommandTo(CommandNames.Cutting, 1, point, CuttingFeed);

        public void Cutting(Line line, Point3d point) => GCommand(CommandNames.Cutting, 1, line, point, feed: CuttingFeed);

        public void GCommandTo(string name, int gCode, Point3d point, int? feed = null)
        {
            Line line = null;
            if (!ToolPosition.IsNull())
            {
                if (point.IsEqualTo(ToolPosition))
                    return;
                line = NoDraw.Line(ToolPosition, point);
            }

            GCommand( name, gCode, line, point, feed: feed);
        }

        public void GCommand(string name, int gCode, Curve curve = null, Point3d? point = null, double? angleC = null, double? angleA = null, int? feed = null, Point3d? arcCenter = null)
        {
            ToolPosition = point ?? ToolPosition;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
            var commandText = _postProcessor.GCommand(gCode, ToolPosition, AngleC, AngleA, feed, arcCenter?.ToPoint2d());
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
            ProcessCommands.Add(new Command
            {
                Name = name,
                Text = text,
                Point = ToolPosition,
                AngleA = AngleA,
                AngleC = AngleC,
                Toolpath = toolpath,
                Operation = _operation,
                Number = ProcessCommands.Count + 1
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