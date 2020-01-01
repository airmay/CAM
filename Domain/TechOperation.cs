using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperationBase : ITechOperation
    {
        /// <summary>
        /// Вид обработки
        /// </summary>
        public abstract ProcessingType Type { get; }

        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        [NonSerialized]
        private TechProcess _techProcess;

        public TechProcess TechProcess
        {
            get => _techProcess;
            set => _techProcess = value;
        }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public ProcessingArea ProcessingArea { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Команды
        /// </summary>
        [NonSerialized]
        protected List<ProcessCommand> _processCommands;

        public List<ProcessCommand> ProcessCommands => _processCommands;

        public abstract object Params { get; }

        public TechOperationBase(TechProcess techProcess, ProcessingArea processingArea)
        {
            _techProcess = techProcess;
            TechProcess.TechOperations.Add(this);
            ProcessingArea = processingArea;
        }

        public abstract void BuildProcessing(ScemaLogicProcessBuilder builder);

        public void DeleteToolpath() => _processCommands = null;

	    public IEnumerable<Curve> ToolpathCurves => ProcessCommands?.Select(p => p.GetToolpathCurve()).Where(p => p != null);

        public bool MoveDown() => TechProcess.TechOperations.SwapNext(this);

        public bool MoveUp() => TechProcess.TechOperations.SwapPrev(this);
    }
}
