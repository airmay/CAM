namespace CAM
{
    public class OperationNode : OperationNodeBase
    {
        public Operation Operation => (Operation)Tag;

        public OperationNode(OperationBase operation) : base(operation, operation.Caption, 1)
        {
        }
        public OperationNode(OperationBase operation, string caption) : base(operation, caption, 1)
        {
        }

        public override void RefreshColor() => SetNodeColor();

        public override void MoveUp()
        {
            if (PrevNode != null)
                Move(Parent.Nodes, Index - 1);
            else if (Parent.PrevNode != null)
                Move(Parent.PrevNode.Nodes, Parent.PrevNode.Nodes.Count);
        }

        public override void MoveDown()
        {
            if (NextNode != null)
                Move(Parent.Nodes, Index + 1);
            else if (Parent.NextNode != null)
                Move(Parent.NextNode.Nodes, 0);
        }

        public override void RemoveOperation() => Operation.Teardown();

        public override void ShowToolpath()
        {
            //TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
            //TechOperation.Toolpath?.SetGroupVisibility(true);
            //Acad.Editor.UpdateScreen();

        }

        public override void SelectAcadObject() => Acad.SelectObjectIds(Operation.ProcessingArea.ObjectIds);

        public override int FirstCommandIndex => Operation.FirstCommandIndex;
    }
}
