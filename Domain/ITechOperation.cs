using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Интрефейс технологической операции
    /// </summary>
    public interface ITechOperation
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Создает обработку по технологической операции
        /// </summary>
        /// <returns></returns>
        void BuildProcessing(ScemaLogicProcessBuilder builder);
    }
}