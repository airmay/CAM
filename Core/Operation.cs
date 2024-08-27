using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    [Serializable]
    public abstract class OperationBase
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; } = true;
    }

    [Serializable]
    public abstract class Operation : OperationBase
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
