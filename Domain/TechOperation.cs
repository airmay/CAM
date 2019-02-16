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
        /// Действия для выполнения операции
        /// </summary>
        public List<ProcessAction> ProcessActions { get; } = new List<ProcessAction>();

        /// <summary>
        /// Команды
        /// </summary>
        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        protected TechOperation(TechProcess techProcess, ProcessingArea processingArea)
        {
            TechProcess = techProcess;
            TechProcess.TechOperations.Add(this);

            ProcessingArea = processingArea;
        }

        public abstract void BuildProcessing(ScemaLogicProgramBuilder actionGenerator);
    }
}
