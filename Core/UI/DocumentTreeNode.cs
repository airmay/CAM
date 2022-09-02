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

        public void BuildProcessing(ProcessCommand processCommand)
        {
            if (processCommand is null)
                Acad.CamDocument.BuildProcessing(TechProcess);
            else
                Acad.CamDocument.PartialProcessing(TechProcess, processCommand);

            var techProcessNode = Parent ?? this;
            if (techProcessNode.Nodes.Count == 0)
                ((TechProcessNode)techProcessNode).CreateTechOperationNodes();

            techProcessNode.Text = TechProcess.Caption;
            foreach (TechOperationNode techOperationNode in techProcessNode.Nodes)
                techOperationNode.Text = techOperationNode.TechOperation.Caption;
        }

        public void SetVisibility(bool isToolpathVisible)
        {
            TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
            TechProcess.GetExtraObjectsGroup()?.SetGroupVisibility(isToolpathVisible);
        }

        public void SendProgram() => Acad.CamDocument.SendProgram(TechProcess);
    }
}
