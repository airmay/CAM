using System.Collections.Generic;

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
        /// Список технологических операций процесса
        /// </summary>
        public List<SawingTechOperation> TechOperations { get; } = new List<SawingTechOperation>();

        public TechProcess(string name)
        {
            Name = name;
        }
    }
}