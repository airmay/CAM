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
    }
}
