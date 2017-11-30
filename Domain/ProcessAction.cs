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
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public readonly Curve ToolpathAcadObject;

        /// <summary>
        /// Технологическая операция
        /// </summary>
        public TechOperation TechOperation { get; }

        public string Name { get; }

        public string GroupName { get; }

        public Point3d Point { get; }

        public int Feed { get; set; }

        public ProcessAction(TechOperation techOperation, string name, string groupName = null, Curve toolpathAcadObject = null, int feed = 0)
        {
            TechOperation = techOperation;
            TechOperation.ProcessActions.Add(this);

            ToolpathAcadObject = toolpathAcadObject;
            Name = name;
            GroupName = groupName;
            Feed = feed;
        }
    }
}
