using System.Linq;

namespace CAM.Core.UI
{
    public class ProcessingNode : OperationNodeBase
    {
        public Processing Processing => (Processing)Tag;
        private OperationNode FirstOperationNode => Nodes.Count > 0 ? (OperationNode) Nodes[0] : null;

        public ProcessingNode() : base(new Processing(), "Обработка", 0)
        {
        }
        public ProcessingNode(Processing processing) : base(processing, processing.Caption, 0)
        {
        }

        public Processing GetProcessing()
        {
            UpdateOperation();
            Processing.Operations = Nodes.Cast<OperationNode>()
                .Select(c => (Operation)c.UpdateOperation())
                .ToArray();
            return Processing;
        }

        public override void RefreshColor()
        {
            SetNodeColor();
            foreach (OperationNodeBase node in Nodes) 
                node.RefreshColor();
        }

        public override void MoveUp() => Move(TreeView.Nodes, Index - 1);

        public override void MoveDown() => Move(TreeView.Nodes, Index + 1);

        public override void RemoveOperation() => Processing.Teardown();

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
