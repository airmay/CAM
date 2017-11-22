using CAM.Domain;
using System.Windows.Forms;

namespace CAM.UI
{
    public class TechProcessNode : TreeNode
    {
        private object _domainObject;

        public TechProcess TechProcess => (TechProcess)_domainObject;

        public SawingTechOperation TechOperation => (SawingTechOperation)_domainObject;

        public ProcessAction ProcessAction => (ProcessAction)_domainObject;

        public TreeNodeType Type { get; }

        public TechProcessNode(TechProcess techProcess) : this(TreeNodeType.TechProcess, techProcess.Name, techProcess) { }

        public TechProcessNode(TechOperation techOperation) : this(TreeNodeType.TechOperation, techOperation.Name, techOperation) { }

        public TechProcessNode(ProcessAction processAction) : this(TreeNodeType.ProcessAction, processAction.Name, processAction) { }

        public TechProcessNode(string processActionGroupName) : this(TreeNodeType.ProcessActionGroup, processActionGroupName) { }

        public TechProcessNode(TreeNodeType type, string name, object domainObject = null) : base(name)
        {
            Type = type;
            _domainObject = domainObject;
            ImageIndex = (int)Type;
            SelectedImageIndex = (int)Type;
        }
    }
}
