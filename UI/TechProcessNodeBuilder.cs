using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.UI
{
    /// <summary>
    /// Конструктор узлов дерева
    /// </summary>
    public class TechProcessNodeBuilder
    {
        /// <summary>
        /// Создает узел техпроцесса со всеми дочерними узлами
        /// </summary>
        /// <param name="techProcess"></param>
        /// <returns></returns>
        public TechProcessNode CreateTechProcessNode(TechProcess techProcess)
        {
            var techProcessNode = new TechProcessNode(techProcess);
            techProcess.TechOperations.ForEach(p => techProcessNode.Nodes.Add(CreateTechOperationNode(p)));
            return techProcessNode;
        }

        /// <summary>
        /// Создает узел техоперации со всеми дочерними узлами
        /// </summary>
        /// <param name="techOperation"></param>
        /// <returns></returns>
        public TechProcessNode CreateTechOperationNode(TechOperation techOperation)
        {
            var techOperationNode = new TechProcessNode(techOperation);
            TechProcessNode groupNode = null;
            foreach (var processAction in techOperation.ProcessActions)
            {
                if (!string.IsNullOrEmpty(processAction.GroupName))
                {
                    if (groupNode?.Name != processAction.GroupName)
                    {
                        groupNode = new TechProcessNode(processAction.GroupName);
                        techOperationNode.Nodes.Add(groupNode);
                    }
                }
                else
                    groupNode = null;

                var processActionNode = new TechProcessNode(processAction);
                (groupNode ?? techOperationNode).Nodes.Add(processActionNode);
            }
            return techOperationNode;
        }
    }
}
