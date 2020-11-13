using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperation
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        [NonSerialized]
        private TechProcess _techProcess;
        [NonSerialized]
        private ObjectId? _toolpathObjectIds;

        public TechProcess TechProcess => _techProcess;

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        public TechOperation(TechProcess techProcess, string caption)
        {
            techProcess.TechOperations.Add(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";
            Setup(techProcess);
        }

        public abstract void BuildProcessing(CommandGeneratorBase generator);

        public virtual void PrepareBuild(CommandGeneratorBase generator) { }

        public virtual void Setup(TechProcess techProcess) => _techProcess = techProcess;

        public virtual void Teardown() { }

        public ObjectId? ToolpathObjectsGroup { get => _toolpathObjectIds; set => _toolpathObjectIds = value; }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

        public int? ProcessCommandIndex { get; set; }
    }
}
