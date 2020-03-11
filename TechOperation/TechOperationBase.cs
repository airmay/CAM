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

        public ITechProcess TechProcess
        {
            get => _techProcess;
            set => _techProcess = value;
        }

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

        public abstract void BuildProcessing(ScemaLogicProcessBuilder builder);

        public void SetToolpathVisible(bool visible) => ToolpathObjectIds?.ForEach<Curve>(p => p.Visible = visible);

        public virtual void Setup(ITechProcess techProcess)
        {
            TechProcess = techProcess;
        }

        public virtual void Teardown()
        {
            Acad.DeleteObjects(ToolpathObjectIds);
        }

        public IEnumerable<ObjectId> ToolpathObjectIds => ProcessCommands?.Select(p => p.ToolpathObjectId).Where(p => p != ObjectId.Null);
    }
}
