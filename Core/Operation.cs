using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM.Core
{
    public abstract class Operation
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;

        [NonSerialized]
        public int? FirstCommandIndex;

        public virtual void SerializeInit() { }
        public virtual void Teardown() { }
        
        public abstract void Execute(Processor processor);

    }
}
