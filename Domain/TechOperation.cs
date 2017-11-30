using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    public abstract class TechOperation : ITechOperation
    {
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
        /// Действия для выполнения операции
        /// </summary>
        public List<ProcessAction> ProcessActions { get; } = new List<ProcessAction>();

        protected TechOperation(TechProcess techProcess, ProcessingArea processingArea)
        {
            TechProcess = techProcess;
            TechProcess.TechOperations.Add(this);

            ProcessingArea = processingArea;
        }

        public abstract void BuildProcessing(ProcessBuilder actionGenerator);
    }
}
