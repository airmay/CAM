using System.Windows.Forms;
using System.Drawing;
using CAM.Core;

namespace CAM
{
    public abstract class OperationNodeBase : TreeNode
    {
        protected OperationNodeBase(IProcessing operation, string caption, int imageIndex) : base(caption, imageIndex, imageIndex)
        {
            Tag = operation;
            //Checked = operation.Enabled;
            SetNodeColor();
        }

        public OperationBase UpdateOperation()
        {
            var operation = (OperationBase)Tag;
            operation.Caption = Text;
            operation.Enabled = Checked;
            return operation;
        }

        public abstract void RefreshColor();

        protected void SetNodeColor() => ForeColor = Checked ? Color.Black : Color.Gray;

        public abstract void MoveUp();

        public abstract void MoveDown();

        protected void Move(TreeNodeCollection nodes, int index)
        {
            (Parent?.Nodes ?? TreeView.Nodes).Remove(this);
            nodes.Insert(index, this);
            TreeView.SelectedNode = this;
        }

        public abstract void RemoveOperation();

        public virtual void SelectAcadObject() { }

        public abstract void ShowToolpath();

        public virtual int FirstCommandIndex => 0;

        //public void SetVisibility(bool isToolpathVisible)
        //{
        //    TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        //    TechProcess.GetExtraObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        //}
    }
}
