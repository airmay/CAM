using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    [Serializable]
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

        public void Remove()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public abstract class OperationBase
    {
        public string Caption { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
