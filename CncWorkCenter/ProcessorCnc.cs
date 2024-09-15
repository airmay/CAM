using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using CAM.Program.Generator;
using Dreambuild.AutoCAD;

namespace CAM.CncWorkCenter
{
    public class ProcessorCnc : IDisposable
    {
        private readonly IPostProcessor _postProcessor;
        private ToolpathBuilder _toolpathBuilder;
        private OperationCnc _operation;
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
        public Program<CommandCnc> Program { get; set; }

        private const int CommandListCapacity = 10_000;

        public ProcessorCnc(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;
            CamManager.Program.Reset();
        }

        public void Start(Tool tool)
        {
            Position = Algorithms.NullPoint3d.WithZ(ZMax + ZSafety * 3);
            _postProcessor.GCommand(-1, Position, 0, 0, null);

            if (CamManager.Commands == null)
                CamManager.Commands = new List<CommandCnc>(CommandListCapacity);
            CamManager.Commands.Clear();

            AddCommands(_postProcessor.StartMachine());
            AddCommands(_postProcessor.SetTool(tool.Number, 0, 0, 0));

            _toolpathBuilder = new ToolpathBuilder();
        }

        public void SetGeneralOperarion(ProcessingCnc processing)
        {
            _frequency = processing.Frequency;
            CuttingFeed = processing.CuttingFeed;
            PenetrationFeed = processing.PenetrationFeed;
            ZSafety = processing.ZSafety;
            Origin = processing.Origin;
        }

        public void SetOperation(OperationCnc operation)
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
        public void Penetration(double z) => Penetration(Position.WithZ(z));

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
            AddCommand(commandText, name, toolpath);
        }

        public void AddCommand(string text, string name = null, ObjectId? toolpath = null)
        {
            if (text == null)
                return;

            CamManager.Program.Add(new CommandCnc
            {
                Name = name,
                Text = text,
                Position = Position,
                AngleA = AngleA,
                AngleC = AngleC,
                ObjectId = toolpath,
                Operation = _operation,
                Number = CamManager.Program.Count + 1
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