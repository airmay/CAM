using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class RemoveTechOperationCommand : CommandBase
    {
        private Context _context;
        private List<TechProcess> _techProcessList;
        private TreeNodeCollection _treeNodeCollection;

        public RemoveTechOperationCommand(Context context, List<TechProcess> techProcessList, TreeNodeCollection treeNodeCollection)
        {
            _context = context;
            _techProcessList = techProcessList;
            _treeNodeCollection = treeNodeCollection;
        }

        public override void Execute()
        {
            if (_context.TechOperation == null)
            {
                _techProcessList.Remove(_context.TechProcess);
                _treeNodeCollection.RemoveByKey(_context.TechProcess.Id);
                _context.TechProcess = null;
            }
            else
            {
                _context.TechProcess.TechOperations.Remove(_context.TechOperation);
                _treeNodeCollection[_context.TechProcess.Id].Nodes.RemoveByKey(_context.TechOperation.Id);
                _context.TechOperation = null;
            }
        }
    }
}
