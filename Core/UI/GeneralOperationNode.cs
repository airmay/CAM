using System.Drawing;

namespace CAM.Core.UI
{
    public class GeneralOperationNode : OperationNodeBase
    {
        public readonly TechOperation TechOperation;

        public GeneralOperationNode() : base(new GeneralOperation(), "Обработка", 0)
        {
        }

        public override void RefreshColor()
        {
            ForeColor = Checked ? Color.Black : Color.Gray;
            foreach (OperationNodeBase node in Nodes) 
                node.RefreshColor();
        }

        public override void MoveUp() => Move(TreeView.Nodes, Index - 1);

        public override void MoveDown() => Move(TreeView.Nodes, Index + 1);

        public override void Remove() => TreeView.Nodes.Remove(this);

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
