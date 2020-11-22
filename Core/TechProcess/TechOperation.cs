using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperation<T> : TechOperation where T: TechProcess
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
        public TechProcess TechProcessBase;

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

        public void Setup(TechProcess techProcess, string caption)
        {
            TechProcessBase = techProcess;
            TechProcessBase.TechOperations.Add(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";

            Init();
        }

        public virtual void Init() { }

        public virtual void SerializeInit() { }

        public abstract void BuildProcessing(CommandGeneratorBase generator);

        public virtual void PrepareBuild(CommandGeneratorBase generator) { }

        public virtual void Teardown() { }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

    }
}
