using Autodesk.AutoCAD.BoundaryRepresentation;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CAM
{
    public class OperationNode : OperationNodeBase
    {
        public readonly TechOperation TechOperation;

        public OperationNode(OperationBase operation, string caption) : base(operation, caption, 1)
        {
        }

        public override void RefreshColor()
        {
            ForeColor = Checked && Parent.Checked ? Color.Black : Color.Gray;
        }

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

        public override void Remove() => TechOperation.Remove();

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
