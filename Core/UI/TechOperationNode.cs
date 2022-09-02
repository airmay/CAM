using System.Drawing;
using System.Windows.Forms;

namespace CAM
{
    public class TechOperationNode : DocumentTreeNode
    {
        public readonly TechOperation TechOperation;

        public TechOperationNode(ITechProcess techProcess, TechOperation techOperation) : base(techOperation, techOperation.Caption, 1)
        {
            Checked = techOperation.Enabled;
            ForeColor = techOperation.Enabled ? Color.Black : Color.Gray;

            TechProcess = techProcess;
            TechOperation = techOperation;
            Tag = techOperation;
        }

        public override void ShowToolpath()
        {
            TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
            TechOperation.ToolpathObjectsGroup?.SetGroupVisibility(true);
            Acad.Editor.UpdateScreen();

        }

        public override void SelectAcadObject()
        {
            if (TechOperation.ProcessingArea != null)
                Acad.SelectObjectIds(TechOperation.ProcessingArea.ObjectId);
        }

        public override int FirstCommandIndex => TechOperation.FirstCommandIndex.GetValueOrDefault();

        public override TreeNode MoveUp()
        {
            if (TechOperation.TryMoveBackward())
                Move(-1);
            return this;
        }

        public override TreeNode MoveDown()
        {
            if (TechOperation.TryMoveForward())
                Move(1);
            return this;
        }

        private void Move(int shift)
        {
            var parent = Parent;
            parent.Nodes.Remove(this);
            parent.Nodes.Insert(Index + shift, this);
        }

        public override void Remove() => TechOperation.Remove();

    }
}
