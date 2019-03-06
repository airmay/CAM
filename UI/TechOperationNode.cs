using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
	public class TechOperationNode : TreeNode
	{
		public TechOperation TechOperation { get; }

		public TechOperationNode(TechOperation techOperation) : base(techOperation.Name)
		{
			TechOperation = techOperation;
			ImageIndex = 1;
			SelectedImageIndex = 1;
		}
	}
}