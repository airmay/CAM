using CAM.Domain;
using System.Windows.Forms;

namespace CAM.UI
{
	public class TechProcessNode : TreeNode
	{
		public TechProcess TechProcess { get; }

		public TechProcessNode(TechProcess techProcess) : base(techProcess.Name)
		{
			TechProcess = techProcess;
			ImageIndex = 0;
			SelectedImageIndex = 0;
		}
	}
}
