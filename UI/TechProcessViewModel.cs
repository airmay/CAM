using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI
{
    public class TechProcessViewModel : TreeNode
    {
        public TechProcess TechProcess { get; }

        public SawingTechOperation TechOperation { get; }

        public TechProcessViewModel(TechProcess techProcess) : base(techProcess.Name)
        {
            TechProcess = techProcess;
            ImageIndex = 0;
            SelectedImageIndex = 0;
        }

        public TechProcessViewModel(SawingTechOperation techOperation, TechProcess techProcess) : base(techOperation.Name)
        {
            TechProcess = techProcess;
            TechOperation = techOperation;
            ImageIndex = 1;
            SelectedImageIndex = 1;
        }
    }
}
