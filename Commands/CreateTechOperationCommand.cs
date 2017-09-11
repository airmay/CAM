using Autodesk.AutoCAD.DatabaseServices;
using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class CreateTechOperationCommand
    {
        private Context _context;
        private SawingTechOperationFactory _techOperationFactory;
        private TreeNodeCollection _treeNodeCollection;

        public CreateTechOperationCommand(Context context, SawingTechOperationFactory techOperationFactory, TreeNodeCollection treeNodeCollection)
        {
            _context = context;
            _techOperationFactory = techOperationFactory;
            _treeNodeCollection = treeNodeCollection;
        }

        public void Execute(Curve curve)
        {
            var techOperation = _techOperationFactory.Create(curve);
            _context.TechProcess.TechOperations.Add(techOperation);
            _treeNodeCollection[_context.TechProcess.Name].Nodes.Add(techOperation.Name);
        }
    }
}
