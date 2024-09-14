using System.Linq;
using CAM.CncWorkCenter;

namespace CAM.Core.UI
{
    public class ProcessingNode : OperationNodeBase
    {
        public IProcessing Processing => (IProcessing)Tag;
        private OperationNode FirstOperationNode => Nodes.Count > 0 ? (OperationNode) Nodes[0] : null;

        public ProcessingNode() : base(new ProcessingCnc(), "Обработка", 0)
        {
        }
        public ProcessingNode(IProcessing processing) : base(processing, processing.Caption, 0)
        {
        }

        public IProcessing GetProcessing()
        {
            UpdateOperation();
            Processing.Operations = Nodes.Cast<OperationNode>()
                .Select(c => (OperationCnc)c.UpdateOperation())
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

        //public override void RemoveOperation() => Processing.Teardown();

        public override void ShowToolpath()
        {
            //TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
            //TechOperation.ObjectId?.SetGroupVisibility(true);
            //Acad.Editor.UpdateScreen();

        }

        public override void RemoveOperation()
        {
            throw new System.NotImplementedException();
        }

        public override void SelectAcadObject() => FirstOperationNode?.SelectAcadObject();

        public override int FirstCommandIndex => FirstOperationNode?.FirstCommandIndex ?? 0;
    }
}
