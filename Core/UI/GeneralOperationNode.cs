using System.Linq;

namespace CAM.Core.UI
{
    public class GeneralOperationNode : OperationNodeBase
    {
        public GeneralOperation GeneralOperation => (GeneralOperation)Tag;
        private OperationNode FirstOperationNode => Nodes.Count > 0 ? (OperationNode) Nodes[0] : null;

        public GeneralOperationNode() : base(new GeneralOperation(), "Обработка", 0)
        {
        }
        public GeneralOperationNode(GeneralOperation generalOperation) : base(generalOperation, generalOperation.Caption, 0)
        {
        }

        public GeneralOperation UpdateGeneralOperation()
        {
            UpdateOperation();
            GeneralOperation.Operations = Nodes.Cast<OperationNode>()
                .Select(c => (Operation)c.UpdateOperation())
                .ToArray();
            return GeneralOperation;
        }

        public override void RefreshColor()
        {
            SetNodeColor();
            foreach (OperationNodeBase node in Nodes) 
                node.RefreshColor();
        }

        public override void MoveUp() => Move(TreeView.Nodes, Index - 1);

        public override void MoveDown() => Move(TreeView.Nodes, Index + 1);

        public override void RemoveOperation() => GeneralOperation.Teardown();

        public override void ShowToolpath()
        {
            //TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
            //TechOperation.Toolpath?.SetGroupVisibility(true);
            //Acad.Editor.UpdateScreen();

        }

        public override void SelectAcadObject() => FirstOperationNode?.SelectAcadObject();

        public override int FirstCommandIndex => FirstOperationNode?.FirstCommandIndex ?? 0;
    }
}
