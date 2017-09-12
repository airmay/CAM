using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class MoveDownTechOperationCommand : CommandBase
    {
        private Context _context;
        private TreeNodeCollection _treeNodeCollection;

        public MoveDownTechOperationCommand(Context context, TreeNodeCollection treeNodeCollection)
        {
            _context = context;
            _treeNodeCollection = treeNodeCollection;
        }

        public override void Execute()
        {
            _context.TechProcess.TechOperations.SwapNext(_context.TechOperation);
            _treeNodeCollection[_context.TechProcess.Id].Nodes.SwapNext(_treeNodeCollection[_context.TechProcess.Id].Nodes[_context.TechOperation.Id]);
        }

        public override bool CanExecute()
        {
            return _context.TechProcess.TechOperations.IndexOf(_context.TechOperation) < _context.TechProcess.TechOperations.Count - 1;
        }
    }
}
