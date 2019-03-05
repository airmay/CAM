using System;
using System.Collections.Generic;
using System.Linq;

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

	    public IEnumerable<Curve> ToolpathCurves => TechOperations.SelectMany(p => p.ProcessCommands).Select(p => p.ToolpathAcadObject).Where(p => p != null);

	    public TechProcess(string name, TechProcessParams techProcessParams)
        {
            Name = name;
            TechProcessParams = techProcessParams;
        }

	    public void BuildProcessing()
	    {
			BorderProcessingArea.SetupBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList());
			var currentPoint = Point3d.Origin;
		    TechOperations.ForEach(p => currentPoint = p.BuildProcessing(currentPoint, p == TechOperations.Last()));
		}
	}
}