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
        /// Вид обработки
        /// </summary>
        public abstract ProcessingType Type { get; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

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
        /// Команды
        /// </summary>
        [NonSerialized]
        private List<ProcessCommand> _processCommands;

        public List<ProcessCommand> ProcessCommands
        {
            get => _processCommands;
            set => _processCommands = value;
        }

        public abstract object Params { get; }

        public TechOperationBase(TechProcess techProcess, ProcessingArea processingArea, string name)
        {
            _techProcess = techProcess;
            _techProcess.TechOperations.Add(this);
            ProcessingArea = processingArea;
            Name = name;
        }

        public abstract List<CuttingParams> GetCuttingParams();

        public IEnumerable<Curve> ToolpathCurves => ProcessCommands?.Select(p => p.ToolpathCurve).Where(p => p != null);
    }
}
