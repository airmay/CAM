using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI
{
    public static class TreeViewExtensions
    {
        public static TechProcessNode SelectedTechProcessNode(this TreeView treeView)
        {
            return (TechProcessNode)treeView.SelectedNode;
        }
    }
}
