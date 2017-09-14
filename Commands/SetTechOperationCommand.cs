using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class SetTechOperationCommand : CommandBase
    {
        private Context _context;
        private List<TechProcess> _techProcessList;
        private TreeView _treeView;

        public SetTechOperationCommand(Context context, List<TechProcess> techProcessList, TreeView treeView)
        {
            Name = CommandNames.SetTechOperationCommand;

            _context = context;
            _techProcessList = techProcessList;
            _treeView = treeView;
        }

        public override void Execute()
        {
            if (_treeView.SelectedNode.Level == 0)
            {
                _context.TechProcess = _techProcessList.Single(p => p.Id == _treeView.SelectedNode.Name);
                _context.TechOperation = null;
            }
            else
            {
                _context.TechProcess = _techProcessList.Single(p => p.Id == _treeView.SelectedNode.Parent.Name);
                _context.TechOperation = _context.TechProcess.TechOperations.Single(p => p.Id == _treeView.SelectedNode.Name);
            }
        }
    }
}
