using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        /// <summary>
        /// Технологические операции
        /// </summary>
        List<ITechOperation> TechOperations { get; } = new List<ITechOperation>();
    }
}