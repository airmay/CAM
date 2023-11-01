using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CAM.Core.UI
{
    public class GeneralOperationNode : OperationNodeBase
    {
        public readonly TechOperation TechOperation;

        public GeneralOperationNode() : base(new GeneralOperation(), "Обработка", 0)
        {
        }

        public override void RefreshColor()
        {
            ForeColor = Checked ? Color.Black : Color.Gray;
            foreach (OperationNodeBase node in Nodes)
            {
                node.RefreshColor();
            }
        }

        public override void ShowToolpath()
        {
            //TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
            //TechOperation.ToolpathObjectsGroup?.SetGroupVisibility(true);
            //Acad.Editor.UpdateScreen();

        }

        public override void SelectAcadObject()
        {
            //if (TechOperation.ProcessingArea != null)
            //    Acad.SelectObjectIds(TechOperation.ProcessingArea.ObjectId);
        }

        public override int FirstCommandIndex => 0; //TechOperation.FirstCommandIndex.GetValueOrDefault();

        public override TreeNode MoveUp() => Move(-1);

        public override TreeNode MoveDown() => Move(1);

        private TreeNode Move(int shift)
        {
            var treeView = TreeView;
            treeView.Nodes.Remove(this);
            treeView.Nodes.Insert(Index + shift, this);
            return this;
        }

        public override void Remove() => TreeView.Nodes.Remove(this);
    }
}
