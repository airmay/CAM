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
    public abstract class TechOperation : ITechOperation
    {
        /// <summary>
        /// Вид технологической операции
        /// </summary>
        public abstract TechOperationType Type { get; }

        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        public TechProcess TechProcess { get; }

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
        public List<ProcessCommand> ProcessCommands { get; set; } = new List<ProcessCommand>();

        protected TechOperation(TechProcess techProcess, ProcessingArea processingArea)
        {
            TechProcess = techProcess;
            TechProcess.TechOperations.Add(this);

            ProcessingArea = processingArea;
        }

        public abstract void BuildProcessing(ScemaLogicProcessBuilder builder);

	    public IEnumerable<Curve> ToolpathCurves => ProcessCommands.Select(p => p.ToolpathCurve).Where(p => p != null);

	}
}
