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

        public override TreeNode MoveUp()
        {
            var prev = PrevNode;
            var parent = Parent;
            parent.Nodes.Remove(this);

            if (prev != null)
                parent.Nodes.Insert(Index - 1, this);
            else if (parent.PrevNode != null) 
                parent.PrevNode.Nodes.Insert(parent.PrevNode.Nodes.Count, this);
            else
                parent.Nodes.Insert(0, this);

            return this;
        }

        public override TreeNode MoveDown()
        {
            var next = NextNode;
            var parent = Parent;
            parent.Nodes.Remove(this);

            if (next != null)
                parent.Nodes.Insert(Index + 1, this);
            else if (parent.NextNode != null)
                parent.NextNode.Nodes.Insert(0, this);
            else
                parent.Nodes.Add(this);

            return this;
        }

        public override void Remove() => TechOperation.Remove();

    }
}
