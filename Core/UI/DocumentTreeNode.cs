using System.Collections.Generic;
using System.Windows.Forms;

namespace CAM
{
    public abstract partial class DocumentTreeNode : TreeNode
    {
        public ITechProcess TechProcess;

        public readonly object Data;

        public static TreeNode Create(ITechProcess techProcess)
        {
            var node = new TechProcessNode(techProcess);
            node.CreateTechOperationNodes();
            return node;
        }

        public static TreeNode Create(ITechProcess techProcess, TechOperation techOperation) => new TechOperationNode(techProcess, techOperation);

        public DocumentTreeNode(object data, string text, int imageIndex) : base(text, imageIndex, imageIndex)
        {
            Data = data;
        }

        public virtual void SelectAcadObject() { }

        public abstract void ShowToolpath();

        public List<ProcessCommand> ProcessCommands => TechProcess.ProcessCommands;

        public virtual int FirstCommandIndex => 0;

        public virtual TreeNode MoveUp() => this;

        public virtual TreeNode MoveDown() => this;

        public new abstract void Remove();

        public void SetVisibility(bool isToolpathVisible)
        {
            TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
            TechProcess.GetExtraObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        }
    }
}
