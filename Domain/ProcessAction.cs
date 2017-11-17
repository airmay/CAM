using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Действие процесса обработки
    /// </summary>
    public class ProcessAction
    {
        /// <summary>
        /// Идентификатор графического примитива автокада представляющего траекторию инструмента
        /// </summary>
        public readonly ObjectId ToolpathAcadObjectId;

        public string Name { get; }

        public string GroupName { get; }

        public ProcessAction(string name, string groupName = null)
        {
            Name = name;
            GroupName = groupName;
        }
    }
}
