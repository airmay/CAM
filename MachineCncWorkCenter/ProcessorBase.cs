using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System;
using System.Linq;

namespace CAM.CncWorkCenter
{
    public abstract class ProcessorBase<TTechProcess, TProcessor>
        where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
        where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
    {
        public TTechProcess Processing { get; set; }
        public IOperation Operation { get; set; }

        protected abstract PostProcessorBase PostProcessor { get; }
        protected ToolpathBuilder ToolpathBuilder;

        protected Tool Tool;

        protected bool IsEngineStarted;
        public double UpperZ;
        public bool IsUpperTool => ToolPoint.Z + 0.1 > UpperZ;

        public Point3d ToolPoint;
        public double AngleC, AngleA;

        public virtual void Start()
        {
            ProgramBuilder.Init();
            ToolpathBuilder = new ToolpathBuilder();
            Operation = null;
            Tool = null;
            IsEngineStarted = false;
        }

        public virtual void StartOperation(double? zMax = null)
        {
            if (IsEngineStarted)
                return;

            if (zMax.HasValue)
                UpperZ = zMax.Value + Processing.ZSafety;
            ToolPoint = Processing.Origin.Point.WithZ(UpperZ + Processing.ZSafety * 5);

            AddCommands(PostProcessor.StartMachine());
        }

        public void Stop()
        {
            AddCommands(PostProcessor.StopEngine());
            AddCommands(PostProcessor.StopMachine());
        }

        public Program CreateProgram()
        {
            var program = ProgramBuilder.CreateProgram(Processing, ToolpathBuilder);

            IsEngineStarted = false;
            Tool = null;
            ToolpathBuilder.Dispose();
            ToolpathBuilder = null;

            return program;
        }

        public ObjectId AddEntity(Entity curve) => ToolpathBuilder.AddEntity(curve);

        public void Pause(double duration) => AddCommand(PostProcessor.Pause(duration));

        public void Cycle() => AddCommand(PostProcessor.Cycle());

        public void AddCommand(string text, Point3d? point = null, double? angleC = null, double? angleA = null,
            double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
        {
            ToolPoint = point ?? ToolPoint;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;

            ProgramBuilder.AddCommand(Operation.Number, text, new ToolPosition(ToolPoint, AngleC, AngleA), duration, toolpath1, toolpath2);
        }

        protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

        public Program PartialProgram(int programPosition, IOperation operation, ToolPosition toolPosition)
        {
            var commands = ProgramBuilder.Commands.Skip(programPosition).ToArray();
            Start();
            Operation = operation;
            StartOperation();
            MoveToPosition(toolPosition);
            ProgramBuilder.Commands.AddRange(commands);

            return CreateProgram();
        }

        protected abstract void MoveToPosition(ToolPosition position);
    }
}