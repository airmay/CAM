using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperationBase : ITechOperation
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        [NonSerialized]
        private ITechProcess _techProcess;
        [NonSerialized]
        private ObjectId? _toolpathObjectIds;

        public ITechProcess TechProcess => _techProcess;

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        public TechOperationBase(ITechProcess techProcess, string caption)
        {
            techProcess.TechOperations.Add(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";
            Setup(techProcess);
        }

        public abstract void BuildProcessing(CommandGeneratorBase generator);

        public virtual void PrepareBuild(CommandGeneratorBase generator) { }

        public virtual void Setup(ITechProcess techProcess) => _techProcess = techProcess;

        public virtual void Teardown() { }

        public ObjectId? ToolpathObjectsGroup { get => _toolpathObjectIds; set => _toolpathObjectIds = value; }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

        public int ProcessCommandIndex { get; set; }
    }
}
