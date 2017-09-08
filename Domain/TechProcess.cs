using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        List<ITechOperation> TechOperations { get; } = new List<ITechOperation>();
    }
}