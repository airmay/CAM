using System;

namespace CAM.Domain
{
    /// <summary>
    /// Технология обработки изделия
    /// </summary>
    [Obsolete]
    public interface ITechnonogy
    {
        /// <summary>
        /// Создает технологический процесс 
        /// </summary>
        /// <returns></returns>
        TechProcess CreateTechProcess();
    }
}