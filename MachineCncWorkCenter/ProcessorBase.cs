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
        public Program Program { get; private set; }

        protected abstract PostProcessorBase PostProcessor { get; }
        protected ToolpathBuilder ToolpathBuilder;

        protected IOperation Operation;
        private double _processDuration;
        private double _operationDuration;
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

        public void SetOperation(IOperation operation)
        {
            if (Operation != null)
                FinishOperation();  // TODO move
            Operation = operation;
            ToolpathBuilder.CreateGroup();
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

        private void FinishOperation()
        {
            Operation.ToolpathGroupId = ToolpathBuilder.AddGroup(Operation.Caption);
            Operation.Caption = GetCaption(Operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
        }

        public void Finish()
        {
            AddCommands(PostProcessor.StopEngine());
            AddCommands(PostProcessor.StopMachine());
            FinishOperation();
            Processing.Caption = GetCaption(Processing.Caption, _processDuration);

            IsEngineStarted = false;
            Tool = null;
            Program.CreateProgram();

            ToolpathBuilder.Dispose();
            ToolpathBuilder = null;
        }

        private static string GetCaption(string caption, double duration)
        {
            var ind = caption.IndexOf('(');
            var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
            return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
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

            var command = new Command
            {
                Text = text,
                ToolPosition = new ToolPosition(ToolPoint, AngleC, AngleA),
                ObjectId = toolpath1,
                ObjectId2 = toolpath2,
                OperationNumber = Operation.Number,
            };
            if (duration.HasValue)
                command.Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.Value));

            Program.AddCommand(command);
            _operationDuration += duration.GetValueOrDefault();
        }

        protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

        public void PartialProgram(int programPosition)
        {

            var command = Program.GetCommand(programPosition-1);
            Operation = Program.Processing.Operations.FirstOrDefault(p => p.Number == command.OperationNumber);
            if (Operation == null) 
                return;

            var count = Program.Count;
            Program.Count = 0;
            AngleA = 0;
            AngleC = 0;

            using (ToolpathBuilder = new ToolpathBuilder())
            {
                ToolpathBuilder.CreateGroup();
                StartOperation();
                MoveToPosition(command.ToolPosition);
            }

            Program.AddCommandsFromPosition(programPosition, count - programPosition);
            Program.CreateProgram();
        }

        protected abstract void MoveToPosition(ToolPosition position);
    }
}