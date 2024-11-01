using Autodesk.AutoCAD.DatabaseServices;
using System;
using CAM.Core;

namespace CAM
{
    [Serializable]
    public abstract class OperationBase : ProcessItem
    {
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public ObjectId? SupportGroup;
        [NonSerialized] public int CommandIndex;
        [NonSerialized] public double Duration;

        public abstract Machine Machine { get; }
        public abstract Tool Tool { get; }
        
        public abstract void Execute(ProcessingBase processingBase, IProcessor processor);

        public override int GetCommandIndex() => CommandIndex;

        public override void OnSelect() => Acad.SelectObjectIds(ProcessingArea?.ObjectIds);

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
