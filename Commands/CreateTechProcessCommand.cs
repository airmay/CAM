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
    public class CreateTechProcessCommand : CommandBase
    {
        private Context _context;
        private CreateTechOperationCommand _сreateTechOperationCommand;
        private List<TechProcess> _techProcessList;
        private TreeNodeCollection _treeNodeCollection;

        public CreateTechProcessCommand(Context context, List<TechProcess> techProcessList, TreeNodeCollection treeNodeCollection, CreateTechOperationCommand сreateTechOperationCommand)
        {
            _context = context;
            _techProcessList = techProcessList;
            _treeNodeCollection = treeNodeCollection;
            _сreateTechOperationCommand = сreateTechOperationCommand;
        }

        public override void Execute()
        {
            var techProcess = new TechProcess($"Изделие{_techProcessList.Count + 1}");
            _techProcessList.Add(techProcess);
            _treeNodeCollection.Add(techProcess.Id, techProcess.Name);
            _context.TechProcess = techProcess;
            _сreateTechOperationCommand.Execute();
        }
    }
}
