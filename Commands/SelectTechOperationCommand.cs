using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class SelectTechOperationCommand : CommandBase
    {
        private Context _context;
        private TreeView _treeView;

        public SelectTechOperationCommand(Context context, TreeView treeView)
        {
            Name = CommandNames.SelectTechOperationCommand;

            _context = context;
            _treeView = treeView;
        }

        public override void Execute()
        {
            _treeView.SelectedNode = _treeView.Nodes.Find(_context.TechOperation?.Id, true).FirstOrDefault();
        }
    }
}
