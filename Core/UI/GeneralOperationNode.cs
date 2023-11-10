using System.Linq;

namespace CAM.Core.UI
{
    public class GeneralOperationNode : OperationNodeBase
    {
        public GeneralOperation GeneralOperation => (GeneralOperation)Tag;

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

        public override void RemoveOperation() => TreeView.Nodes.Remove(this);

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
    }
}
