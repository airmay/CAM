using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace CAM
{
    [Serializable]
    public abstract class Operation : OperationBase
    {
        public int Duration { get; set; }
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized]
        public Dictionary<ObjectId, int> ToolpathCommandDictionary;
        [NonSerialized]
        public ObjectId? ToolpathId;
        [NonSerialized]
        public ObjectId? ExtraObjectsGroup;

        [NonSerialized]
        public int? FirstCommandIndex;

        public virtual void SerializeInit() { }
        public virtual void Teardown() { }
        
        public abstract void Execute(GeneralOperation generalOperation, Processor processor);

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
