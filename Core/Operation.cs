﻿using Autodesk.AutoCAD.DatabaseServices;
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
        public int Duration { get; set; }
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized] public ObjectId? Toolpath;
        [NonSerialized] public ObjectId? ExtraObjectsGroup;
        [NonSerialized] public int FirstCommandIndex;

        public virtual void Init() { }
        public virtual void Teardown() { }
        
        public abstract void Execute(GeneralOperation generalOperation, Processor processor);
    }
}
