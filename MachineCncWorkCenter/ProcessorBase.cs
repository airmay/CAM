using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System;

namespace CAM.CncWorkCenter
{
    public abstract class ProcessorBase<TTechProcess, TProcessor>
        where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
        where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
    {
        public TTechProcess Processing { get; set; }
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
            Program.Init(Processing);
            ToolpathBuilder = new ToolpathBuilder();
        }

        public void SetOperation(IOperation operation)
        {
            if (Operation != null)
                FinishOperation();
            Operation = operation;
            ToolpathBuilder.CreateGroup();
        }

        public virtual void StartOperation(double? zMax = null)
        {
            if (IsEngineStarted)
                return;

            if (zMax.HasValue)
                UpperZ = zMax.Value + Processing.ZSafety;
            ToolPoint = Processing.Origin.Point.WithZ(UpperZ);
            AngleC = 0;
            AngleA = 0;

            AddCommands(PostProcessor.StartMachine());
            IsEngineStarted = true;
        }

        private void FinishOperation()
        {
            Operation.ToolpathGroupId = ToolpathBuilder.AddGroup(Operation.Caption);
            Operation.Caption = GetCaption(Operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
            Operation = null;
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
                command.Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.Value)).ToString();

            Program.AddCommand(command);
            _operationDuration += duration.GetValueOrDefault();
        }

        protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));
    }
}