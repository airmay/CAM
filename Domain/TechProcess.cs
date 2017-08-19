using System.Collections.Generic;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    public class TechProcess
    {
        /// <summary>
        /// Управляющие команды техпроцесса
        /// </summary>
        List<TechProcessCommand> Commands;
    }
}