using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperation<T> : TechOperation where T : CommandGeneratorBase
    {
        public abstract void BuildProcessing(T generator);

        public virtual void PrepareBuild(T generator) { }
    }

    [Serializable]
    public abstract class MillingTechOperation<T> : TechOperation<MillingCommandGenerator> where T : ITechProcess
    {
        public T TechProcess => (T)TechProcessBase;
    }

    [Serializable]
    public abstract class TechOperation
    {
        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        [NonSerialized]
        public ITechProcess TechProcessBase;

        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;

        [NonSerialized]
        public int? ProcessCommandIndex;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        public void Setup(ITechProcess techProcess, string caption)
        {
            TechProcessBase = techProcess;
            TechProcessBase.AddTechOperation(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";

            Init();
        }

        public virtual void Init() { }

        public virtual void SerializeInit() { }

        public virtual void Teardown() { }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

    }
}
