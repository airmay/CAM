using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class MoveUpTechOperationCommand : CommandBase
    {
        private Context _context;
        private TreeNodeCollection _treeNodeCollection;

        public MoveUpTechOperationCommand(Context context, TreeNodeCollection treeNodeCollection)
        {
            _context = context;
            _treeNodeCollection = treeNodeCollection;
        }

        public override void Execute()
        {
            _context.TechProcess.TechOperations.SwapPrev(_context.TechOperation);
            _treeNodeCollection[_context.TechProcess.Id].Nodes.SwapPrev(_treeNodeCollection[_context.TechProcess.Id].Nodes[_context.TechOperation.Id]);
        }

        public override bool CanExecute()
        {
            return _context.TechProcess.TechOperations.IndexOf(_context.TechOperation) > 0;
        }
    }
}
