using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System;

namespace CAM.CncWorkCenter
{
    public abstract class ProcessorBase : IProcessor
    {
        protected ToolpathBuilder ToolpathBuilder;
        protected abstract ProcessingBase Processing { get; }
        protected abstract PostProcessorBase PostProcessor { get; }

        private IOperation _operation;
        private double _processDuration;
        private double _operationDuration;

        protected bool IsEngineStarted;
        protected double UpperZ;
        public bool IsUpperTool => ToolPoint.Z + 0.1 > UpperZ;

        public Point3d ToolPoint;
        public double AngleC;
        public double AngleA;

        public void Start()
        {
            Program.Init(Processing);
            ToolpathBuilder = new ToolpathBuilder();
        }

        public void SetOperation(IOperation operation)
        {
            if (_operation != null)
                FinishOperation();
            _operation = operation;
            ToolpathBuilder.CreateGroup();
        }

        public virtual void StartOperation(double zMax = 0)
        {
            if (IsEngineStarted)
                return;

            UpperZ = zMax + Processing.ZSafety;
            ToolPoint = Processing.Origin.Point.WithZ(UpperZ);

            AddCommands(PostProcessor.StartMachine());
        }

        private void FinishOperation()
        {
            _operation.ToolpathGroupId = ToolpathBuilder.AddGroup(_operation.Caption);
            _operation.Caption = GetCaption(_operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
        }

        public void Finish()
        {
            AddCommands(PostProcessor.StopEngine());
            AddCommands(PostProcessor.StopMachine());
            Program.CreateProgram();
            FinishOperation();
            Processing.Caption = GetCaption(Processing.Caption, _processDuration);
        }

        private static string GetCaption(string caption, double duration)
        {
            var ind = caption.IndexOf('(');
            var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
            return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
        }

        public void Dispose()
        {
            ToolpathBuilder.Dispose();
        }

        public ObjectId AddEntity(Entity curve) => ToolpathBuilder.AddEntity(curve);

        public void Pause(double duration) => AddCommand(PostProcessor.Pause(duration));

        public void Cycle() => AddCommand(PostProcessor.Cycle());

        public void AddCommand(string text, Point3d? point = null, double? angleC = null, double? angleA = null, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
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
                Operation = _operation,
            };
            if (duration.HasValue)
                command.Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.Value)).ToString();

            Program.AddCommand(command);
            _operationDuration += duration.GetValueOrDefault();
        }

        protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));
    }
}