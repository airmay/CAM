using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Интрефейс технологической операции
    /// </summary>
    internal interface ITechOperation
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Получает программу обработки по технологической операции
        /// </summary>
        /// <returns></returns>
        List<TechProcessCommand> GetProcessing();
    }
}