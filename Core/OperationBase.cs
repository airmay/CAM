using Autodesk.AutoCAD.DatabaseServices;
using System;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public abstract class OperationBase : ProcessItem
    {
        public abstract ProcessingBase ProcessingBase { get; set; }
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

        public override void OnSelect()
        {
            Acad.SelectObjectIds(ProcessingArea?.ObjectIds);
            ProcessingBase?.HideToolpath(this);
        }
    }
}
