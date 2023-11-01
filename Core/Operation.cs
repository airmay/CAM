using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    public abstract class Operation : OperationBase
    {
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;

        [NonSerialized]
        public int? FirstCommandIndex;

        public virtual void SerializeInit() { }
        public virtual void Teardown() { }
        
        public abstract void Execute(Processor processor);

    }

    public abstract class OperationBase
    {
        public string Caption { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
