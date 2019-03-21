using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Параметры технологического процесса
        /// </summary>
        public TechProcessParams TechProcessParams { get; set; }

        public List<ITechOperationFactory> TechOperationFactorys { get; set; } = new List<ITechOperationFactory>();

        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        public List<TechOperation> TechOperations { get; } = new List<TechOperation>();

	    public IEnumerable<Curve> ToolpathCurves => TechOperations.SelectMany(p => p.ToolpathCurves);

        /// <summary>
        /// Команды
        /// </summary>
        public List<ProcessCommand> ProcessCommands { get; set; } = new List<ProcessCommand>();

        public TechProcess(string name)
        {
            Name = name;
            TechProcessParams = new TechProcessParams();
        }

	    public void BuildProcessing()
	    {
			BorderProcessingArea.SetupBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList());
	        var builder = new ScemaLogicProcessBuilder(TechProcessParams);
            TechOperations.ForEach(p => p.BuildProcessing(builder));
	        ProcessCommands = builder.FinishTechProcess();
	    }
	}
}