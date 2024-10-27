using Autodesk.AutoCAD.DatabaseServices;
using System;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public abstract class OperationBase : ProcessItem
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; } = true;
        public ProcessItem[] Children { get; set; } = null;
        public int CommandIndex { get; set; }
        public abstract void Delete();
        public abstract void Select();
    }

    [Serializable]
    public abstract class Operation : ProcessItem
    {
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public ObjectId? SupportGroup;
        [NonSerialized] public int FirstCommandIndex;
        [NonSerialized] public double Duration;

        public abstract Machine Machine { get; }
        public abstract Tool Tool { get; }
        
        public abstract void Execute(ProcessingBase processingBase, IProcessor processor);

        public void RemoveAcadObjects()
        {
            ToolpathGroup?.DeleteGroup();
            ToolpathGroup = null;
            SupportGroup?.DeleteGroup();
            SupportGroup = null;
        }

        public virtual bool Validate()
        {
            return ProcessingArea != null;
        }
    }
}
