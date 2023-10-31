using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Core.UI
{
    public class GeneralOperationNode : OperationNodeBase
    {
        public readonly TechOperation TechOperation;

        public GeneralOperationNode() : base(new GeneralOperation(), "Обработка", 1)
        {
            //Checked = techOperation.Enabled;
            //ForeColor = techOperation.Enabled ? Color.Black : Color.Gray;

            //Tag = techOperation;
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

        public override TreeNode MoveUp()
        {
            Move(-1);
            return this;
        }

        public override TreeNode MoveDown()
        {
            if (TechOperation.TryMoveBackward())
                if (TechOperation.TryMoveForward())
                    Move(1);
            return this;
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }

        private void Move(int shift)
        {
            var treeView = TreeView;
            treeView.Nodes.Remove(this);
            treeView.Nodes.Insert(Index + shift, this);
        }
    }
}
