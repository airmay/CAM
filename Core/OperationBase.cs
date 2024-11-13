using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    public interface IOperation : ITreeNode
    {
        bool Enabled { get; set; }
        MachineType MachineType { get; }
        Machine Machine { get; }
        Tool Tool { get; }
        ObjectId? ToolpathGroupId { get; set; }
    }

    [Serializable]
    public abstract class OperationBase : IOperation
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public abstract MachineType MachineType { get; }
        [NonSerialized] public IProcessing ProcessingBase;

        [NonSerialized] private ObjectId? _toolpathGroupId;
        public ObjectId? ToolpathGroupId
        {
            get => _toolpathGroupId;
            set => _toolpathGroupId = value;
        }

        public AcadObject ProcessingArea { get; set; }
        public abstract Machine Machine { get; }
        public abstract Tool Tool { get; }
        
        public abstract void Execute();

        public virtual bool Validate()
        {
            return ProcessingArea.CheckNotNull("Объекты автокада");
        }

        void ITreeNode.OnSelect()
        {
            ProcessingBase?.HideToolpath(this);
            Acad.SelectObjectIds(ProcessingArea?.ObjectIds);
        }
    }
}
