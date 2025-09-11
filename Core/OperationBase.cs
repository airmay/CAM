using Autodesk.AutoCAD.DatabaseServices;
using System;
using CAM.CncWorkCenter;

namespace CAM
{
    public interface IOperation : ITreeNode
    {
        bool Enabled { get; set; }
        Tool GetTool();
        ObjectId? ToolpathGroupId { get; set; }
        short Number { get; set; }
    }

    [Serializable]
    public abstract class OperationBase<TTechProcess, TProcessor> : IOperation
        where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
        where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
    {
        [NonSerialized] public TTechProcess Processing;
        protected TProcessor Processor => Processing.Processor;

        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public short Number { get; set; }

        [NonSerialized] private ObjectId? _toolpathGroupId;
        public ObjectId? ToolpathGroupId
        {
            get => _toolpathGroupId;
            set => _toolpathGroupId = value;
        }

        public AcadObject ProcessingArea { get; set; }

        public Tool Tool { get; set; }
        public Tool GetTool() => Tool ?? Processing.Tool;
        public double ToolDiameter => GetTool().Diameter;
        public double ToolThickness => GetTool().Thickness.Value;

        public abstract void Execute();

        public virtual bool Validate()
        {
            return ProcessingArea.CheckNotNull("Объекты автокада");
        }

        void ITreeNode.OnSelect()
        {
            Processing?.HideToolpath(this);
            Acad.SelectObjectIds(ProcessingArea?.ObjectIds);
        }
    }
}
