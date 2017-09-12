//using Autodesk.AutoCAD.DatabaseServices;
using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Commands
{
    public class CreateTechOperationCommand : CommandBase
    {
        private Context _context;
        private SawingTechOperationFactory _techOperationFactory;
        private TreeNodeCollection _treeNodeCollection;
        private Func<Curve[]> _curveProvider;

        public CreateTechOperationCommand(Context context, SawingTechOperationFactory techOperationFactory, TreeNodeCollection treeNodeCollection, Func<Curve[]> curveProvider)
        {
            _context = context;
            _techOperationFactory = techOperationFactory;
            _treeNodeCollection = treeNodeCollection;
            _curveProvider = curveProvider;
        }

        public override void Execute()
        {
            foreach (var curve in _curveProvider())
            {
                var techOperation = _techOperationFactory.Create(curve);
                _context.TechProcess.TechOperations.Add(techOperation);
                _context.TechOperation = techOperation;
                _treeNodeCollection[_context.TechProcess.Id].Nodes.Add(techOperation.Id, techOperation.Name);
            }
        }
    }
}
