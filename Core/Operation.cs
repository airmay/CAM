using Autodesk.AutoCAD.DatabaseServices;
using System;

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
        [NonSerialized] public GeneralOperation GeneralOperation;
        public MachineType MachineType => GeneralOperation.MachineType.Value;
        public Tool Tool => GeneralOperation.Tool;
        public int CuttingFeed => GeneralOperation.CuttingFeed;
        public int PenetrationFeed => GeneralOperation.PenetrationFeed;
        public double ZSafety => GeneralOperation.ZSafety;

        public virtual void Init() { }

        public virtual void Teardown() { }

        public abstract void Execute(Processor processor);
    }
}
