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

        [NonSerialized] public ObjectId? Toolpath;
        [NonSerialized] public ObjectId? Support;
        [NonSerialized] public int FirstCommandIndex;

        public MachineType MachineType { get; set; }
        public Tool Tool { get; set; }
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }
        public double ZSafety { get; set; }

        public void SetGeneralParams(GeneralOperation generalOperation)
        {
            MachineType = generalOperation.MachineType.Value;
            Tool = generalOperation.Tool;
            CuttingFeed = generalOperation.CuttingFeed;
            PenetrationFeed = generalOperation.PenetrationFeed;
            ZSafety = generalOperation.ZSafety;
        }

        public virtual void Init() { }

        public virtual void Teardown() { }

        public abstract void Execute(Processor processor);
    }
}
