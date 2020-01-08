using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Интрефейс технологической операции
    /// </summary>
    public interface ITechOperation
    {
        ProcessingType Type { get; }

        TechProcess TechProcess { get; set; }

        object Params { get; }

        ProcessingArea ProcessingArea { get; }

        List<ProcessCommand> ProcessCommands { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Создает обработку по технологической операции
        /// </summary>
        /// <returns></returns>
        void BuildProcessing(ScemaLogicProcessBuilder builder);

        IEnumerable<Curve> ToolpathCurves { get; }

        List<CuttingParams> GetCuttingParams();

    }
}