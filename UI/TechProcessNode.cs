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

        public TechProcessNodeType Type { get; }

        public TechProcessNode(TechProcess techProcess) : this(TechProcessNodeType.TechProcess, techProcess.Name, techProcess) { }

        public TechProcessNode(TechOperation techOperation) : this(TechProcessNodeType.TechOperation, techOperation.Name, techOperation) { }

        public TechProcessNode(ProcessAction processAction) : this(TechProcessNodeType.ProcessAction, processAction.Name, processAction) { }

        public TechProcessNode(string processActionGroupName) : this(TechProcessNodeType.ProcessActionGroup, processActionGroupName) { }

        public TechProcessNode(TechProcessNodeType type, string name, object domainObject = null) : base(name)
        {
            Type = type;
            _domainObject = domainObject;
            ImageIndex = (int)Type;
            SelectedImageIndex = (int)Type;
        }
    }
}
