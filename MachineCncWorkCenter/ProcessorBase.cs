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
        public Program Program { get; private set; }
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
            Program = new Program(Processing, PostProcessor.ProgramFileExtension);
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

        public void Finish()
        {
            AddCommands(PostProcessor.StopEngine());
            AddCommands(PostProcessor.StopMachine());

            IsEngineStarted = false;
            Tool = null;
            Program.CreateProgram(ToolpathBuilder);

            ToolpathBuilder.Dispose();
            ToolpathBuilder = null;
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

            Program.AddCommand(Operation, text, new ToolPosition(ToolPoint, AngleC, AngleA), duration, toolpath1, toolpath2);
        }

        protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

        public void PartialProgram(int programPosition)
        {
            var command = Program.GetCommand(programPosition-1);
            Operation = Program.Operations[command.OperationNumber];
            var count = Program.Count;
            Program.Count = 0;
            AngleA = 0;
            AngleC = 0;

            Program.ArraySegment.Take(programPosition).SelectMany(p => new[] { p.ObjectId, p.ObjectId2 }).Delete();
            using (ToolpathBuilder = new ToolpathBuilder())
            {
                StartOperation();
                MoveToPosition(command.ToolPosition);
                Program.AddCommandsFromPosition(programPosition, count - programPosition);
                Program.CreateProgram(ToolpathBuilder);
            }
        }

        protected abstract void MoveToPosition(ToolPosition position);
    }
}