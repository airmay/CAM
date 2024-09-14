using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;
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
        public double Duration { get; set; }
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? ToolpathGroup;
        [NonSerialized] public ObjectId? SupportGroup;
        [NonSerialized] public int FirstCommandIndex;
        [NonSerialized] public Processing Processing;

        public Machine Machine => Processing.Machine.Value;
        public Tool Tool => Processing.Tool;
        public int CuttingFeed => Processing.CuttingFeed;
        public int PenetrationFeed => Processing.PenetrationFeed;
        public double ZSafety => Processing.ZSafety;
        public Point2d Origin => Processing.Origin;

        public virtual void Init() { }

        public virtual void Teardown() { }

        public abstract void Execute(Processor processor);

        public void RemoveAcadObjects()
        {
            ToolpathGroup?.DeleteGroup();
            ToolpathGroup = null;
            SupportGroup?.DeleteGroup();
            SupportGroup = null;
        }

        public override string ToString() => Caption;
    }
}
