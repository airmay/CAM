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

        /// <summary>
        /// Технологическая операция
        /// </summary>
        public TechOperation TechOperation { get; }

        public string Name { get; }

        public string GroupName { get; }

        public ProcessAction(TechOperation techOperation, string name, string groupName = null)
        {
            TechOperation = techOperation;
            TechOperation.ProcessActions.Add(this);

            Name = name;
            GroupName = groupName;
        }
    }
}
