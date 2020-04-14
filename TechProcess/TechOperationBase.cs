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

        public ITechProcess TechProcess => _techProcess;

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        /// <summary>
        /// Команды
        /// </summary>
        [NonSerialized]
        private List<ProcessCommand> _processCommands;

        public List<ProcessCommand> ProcessCommands
        {
            get => _processCommands;
            set => _processCommands = value;
        }

        public TechOperationBase(ITechProcess techProcess, string caption)
        {
            techProcess.TechOperations.Add(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";
            Setup(techProcess);
        }

        public abstract void BuildProcessing(ICommandGenerator generator);

        public void SetToolpathVisible(bool visible) => ToolpathObjectIds?.ForEach<Curve>(p => p.Visible = visible);

        public virtual void Setup(ITechProcess techProcess) => _techProcess = techProcess;

        public virtual void Teardown() => Acad.DeleteObjects(ToolpathObjectIds);

        public IEnumerable<ObjectId> ToolpathObjectIds => ProcessCommands?.Where(p => p.ToolpathObjectId != null).Select(p => p.ToolpathObjectId.Value);

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;
    }
}
