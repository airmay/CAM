using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    public abstract class ProcessorBase : IProcessor
    {
        protected ToolpathBuilder _toolpathBuilder;
        protected PostProcessorCnc _postProcessor;
        protected ProcessingCnc _processing;

        protected IOperation _operation;
        private double _processDuration;
        private double _operationDuration;
        protected bool IsEngineStarted;

        public Point3d ToolPoint;
        public double AngleC;
        public double AngleA;

        protected double UpperZ;
        public bool IsUpperTool => ToolPoint.Z + 0.1 > UpperZ;

        //protected ProcessorBase(ProcessingCnc processing, PostProcessorCnc postProcessor)
        //{
        //    _processing = processing;
        //    _postProcessor = postProcessor;
        //    CuttingFeed = _processing.CuttingFeed;
        //}

        public void Start()
        {
            Program.Init(_processing);
            _toolpathBuilder = new ToolpathBuilder();
        }

        public void SetOperation(IOperation operation)
        {
            if (_operation != null)
                FinishOperation();
            _operation = operation;
            _toolpathBuilder.CreateGroup();
        }

        public void StartOperation(double zMax = 0)
        {
            if (IsEngineStarted)
                return;

            UpperZ = zMax + _processing.ZSafety;
            ToolPoint = _processing.Origin.Point.WithZ(UpperZ);

            AddCommands(_postProcessor.StartMachine());
            AddCommands(_postProcessor.SetTool(_processing.Tool.Number, 0, 0, 0));
        }

        private void FinishOperation()
        {
            _operation.ToolpathGroupId = _toolpathBuilder.AddGroup(_operation.Caption);
            _operation.Caption = GetCaption(_operation.Caption, _operationDuration);
            _processDuration += _operationDuration;
            _operationDuration = 0;
        }

        public void Finish()
        {
            AddCommands(_postProcessor.StopEngine());
            AddCommands(_postProcessor.StopMachine());
            Program.CreateProgram();
            FinishOperation();
            _processing.Caption = GetCaption(_processing.Caption, _processDuration);
        }

        private static string GetCaption(string caption, double duration)
        {
            var ind = caption.IndexOf('(');
            var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
            return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
        }

        public void Dispose()
        {
            _toolpathBuilder.Dispose();
        }

        public ObjectId AddEntity(Entity curve) => _toolpathBuilder.AddEntity(curve);

        public void Pause(double duration) => AddCommand(_postProcessor.Pause(duration));

        public void Cycle() => AddCommand(_postProcessor.Cycle());

        public void AddCommand(string text, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
        {
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