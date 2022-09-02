using System.Drawing;
using System.Linq;

namespace CAM
{
    public class TechProcessNode : DocumentTreeNode
    {
        public TechProcessNode(ITechProcess techProcess) : base(techProcess, techProcess.Caption + "   ", 0)
        {
            Checked = true;
            NodeFont = new Font(Acad.CamView.Font, FontStyle.Bold);

            TechProcess = techProcess;
            Tag = techProcess;
        }

        public void CreateTechOperationNodes()
        {
            var techOperationsNodes = TechProcess.TechOperations.Select(p => new TechOperationNode(TechProcess, p)).ToArray();
            Nodes.AddRange(techOperationsNodes);
            Expand();
        }

        public override void ShowToolpath()
        {
            TechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(true);
        }

        public override void Remove() => Acad.CamDocument.DeleteTechProcess(TechProcess);

    }
}
