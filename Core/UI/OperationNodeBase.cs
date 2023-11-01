using System.Collections.Generic;
using System.Windows.Forms;

namespace CAM
{
    public abstract class OperationNodeBase : TreeNode
    {
        protected OperationNodeBase(OperationBase operation, string caption, int imageIndex) : base(caption, imageIndex, imageIndex)
        {
            Tag = operation;
            Checked = operation.Enabled;
        }

        public abstract void RefreshColor();

        public abstract void MoveUp();

        public abstract void MoveDown();

        protected void Move(TreeNodeCollection nodes, int index)
        {
            (Parent?.Nodes ?? TreeView.Nodes).Remove(this);
            nodes.Insert(index, this);
            TreeView.SelectedNode = this;
        }

        public new abstract void Remove();


        public virtual void SelectAcadObject() { }

        public abstract void ShowToolpath();

        public List<ProcessCommand> ProcessCommands => null; // TechProcess.ProcessCommands;

        public virtual int FirstCommandIndex => 0;


        //public void SetVisibility(bool isToolpathVisible)
        //{
        //    TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        //    TechProcess.GetExtraObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        //}

        //public void SendProgram() => Acad.CamDocument.SendProgram(TechProcess);

    }
}
