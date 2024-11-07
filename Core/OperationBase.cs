using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    public interface IOperation : ITreeNode
    {
        bool Enabled { get; set; }
        IProcessing ProcessingBase { set; }
        MachineType MachineType { get; }
        Machine Machine { get; }
        Tool Tool { get; }
        void AddDuration(double duration);
    }

    [Serializable]
    public abstract class OperationBase : IOperation
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public abstract MachineType MachineType { get; }
        public void AddDuration(double duration) => Duration += duration;

        public IProcessing ProcessingBase { get; set; }

        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public double Duration;

        public abstract Machine Machine { get; }
        public abstract Tool Tool { get; }
        
        public abstract void Execute();

        public void Clear()
        {
            ToolpathGroup = null;
            Duration = 0;
        }

        public virtual bool Validate()
        {
            return ProcessingArea != null;
        }

        public void OnSelect()
        {
            Acad.SelectObjectIds(ProcessingArea?.ObjectIds);
            ProcessingBase?.HideToolpath(this);
        }
    }
}
