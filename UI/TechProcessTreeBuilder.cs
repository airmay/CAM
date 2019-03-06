using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI
{
    /// <summary>
    /// Конструктор дерева техпроцесса
    /// </summary>
    //public class TechProcessTreeBuilder
    //{
    //    /// <summary>
    //    /// Создает дерево техпроцесса
    //    /// </summary>
    //    /// <param name="techProcess"></param>
    //    /// <returns></returns>
    //    public TechProcessNode CreateTechProcessTree(TechProcess techProcess)
    //    {
    //        var techProcessNode = new TechProcessNode(techProcess);
    //        techProcess.TechOperations.ForEach(p => techProcessNode.Nodes.Add(new TechProcessNode(p)));
    //        return techProcessNode;
    //    }

    //    /// <summary>
    //    /// Перестраивает дерево техпроцесса
    //    /// </summary>
    //    /// <param name="rootNode"></param>
    //    public void RebuildTechProcessTree(TreeNode rootNode)
    //    {
    //        foreach (TechProcessNode techOperationNode in rootNode.Nodes)
    //        {
    //            TechProcessNode groupNode = null;
    //            techOperationNode.Nodes.Clear();
    //            foreach (var processAction in techOperationNode.TechOperation.ProcessActions)
    //            {
    //                if (!string.IsNullOrEmpty(processAction.GroupName))
    //                {
    //                    if (groupNode?.Text != processAction.GroupName)
    //                    {
    //                        groupNode = new TechProcessNode(processAction.GroupName);
    //                        techOperationNode.Nodes.Add(groupNode);
    //                    }
    //                }
    //                else
    //                    groupNode = null;

    //                var processActionNode = new TechProcessNode(processAction);
    //                (groupNode ?? techOperationNode).Nodes.Add(processActionNode);
    //            }
    //        }
    //    }
    //}
}
