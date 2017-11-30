using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Сервисный класс для работы с техпроцессами
    /// </summary>
    public class TechProcessService
    {
        private List<TechProcess> _techProcessList = new List<TechProcess>();
        private SawingTechOperationParams _sawingLineTechOperationParamsDefault = new SawingTechOperationParams();
        private SawingTechOperationParams _sawingArcTechOperationParamsDefault = new SawingTechOperationParams();
        private TechProcessParams _techProcessParams = new TechProcessParams();
        private SawingTechOperationFactory _techOperationFactory;
        private IAcadGateway _acad;

        public TechProcessService(IAcadGateway acad)
        {
            _acad = acad;

            _sawingLineTechOperationParamsDefault.Modes.Add(new SawingMode { Depth = 30, DepthStep = 5, Feed = 2000 });
            _sawingArcTechOperationParamsDefault.Modes.AddRange(new SawingMode[3] {
                new SawingMode { Depth = 10, DepthStep = 5, Feed = 2000 },
                new SawingMode { Depth = 20, DepthStep = 2, Feed = 1000 },
                new SawingMode { Depth = 30, DepthStep = 1, Feed = 500 } });
            _techOperationFactory = new SawingTechOperationFactory(_sawingLineTechOperationParamsDefault, _sawingArcTechOperationParamsDefault);
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Изделие{_techProcessList.Count + 1}", _techProcessParams);
            _techProcessList.Add(techProcess);
            CreateTechOperations(techProcess);
            return techProcess;
        }

        internal void SelectTechProcess(TechProcess techProcess)
        {
            _acad.SelectEntities(techProcess.TechOperations.ConvertAll(p => p.ProcessingArea.AcadObjectId));
        }

        internal void SelectTechOperation(SawingTechOperation techOperation)
        {
            _acad.SelectEntities(new List<ObjectId> { techOperation.ProcessingArea.AcadObjectId });
        }

        public List<SawingTechOperation> CreateTechOperations(TechProcess techProcess)
        {
            var operations = _acad.GetSelectedEntities().Select(p => _techOperationFactory.Create(techProcess, p)).ToList();
            return operations;
        }

        public bool MoveForwardTechOperation(TechOperation techOperation)
        {
            var flag = techOperation != techOperation.TechProcess.TechOperations.Last();
            if (flag)
                techOperation.TechProcess.TechOperations.SwapNext(techOperation);
            return flag;
        }

        public bool MoveBackwardTechOperation(TechOperation techOperation)
        {
            var flag = techOperation != techOperation.TechProcess.TechOperations.First();
            if (flag)
                techOperation.TechProcess.TechOperations.SwapPrev(techOperation);
            return flag;
        }

        public void RemoveTechProcess(TechProcess techProcess)
        {
            DeleteToolpath(techProcess);
            _techProcessList.Remove(techProcess);
        }

        private void DeleteToolpath(TechProcess techProcess)
        {
            _acad.DeleteEntities(techProcess.TechOperations.SelectMany(p => p.ProcessActions).Select(p => p.ToolpathAcadObject?.ObjectId).Where(p => p != null).ToList());
        }

        public void RemoveTechOperation(TechOperation techOperation)
        {
            _acad.DeleteEntities(techOperation.ProcessActions.ConvertAll(p => p.ToolpathAcadObject.ObjectId));
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void RemoveProcessAction(ProcessAction processAction)
        {
            _acad.DeleteEntities(new List<ObjectId> { processAction.ToolpathAcadObject.ObjectId });
            processAction.TechOperation.ProcessActions.Remove(processAction);
        }

        public void BuildProcessing(TechProcess techProcess)
        {
            DeleteToolpath(techProcess);
            var actionGenerator = new ProcessBuilder(techProcess.TechProcessParams);
            techProcess.TechOperations.ForEach(p => 
            {
                // TODO переделать 
                actionGenerator.SetTechOperation(p);
                p.BuildProcessing(actionGenerator);
            });
            _acad.CreateEntities(actionGenerator.Entities);
        }        
    }
}
