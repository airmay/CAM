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
            _acad.SelectCurvies(techProcess.TechOperations.ConvertAll(p => p.ProcessingArea.AcadObjectId));
        }

        internal void SelectTechOperation(SawingTechOperation techOperation)
        {
            _acad.SelectCurvies(new List<ObjectId> { techOperation.ProcessingArea.AcadObjectId });
        }

        public List<SawingTechOperation> CreateTechOperations(TechProcess techProcess)
        {
            var operations = _acad.GetSelectedCurves().Select(p => _techOperationFactory.Create(techProcess, p)).ToList();
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

        internal void RemoveTechProcess(TechProcess techProcess)
        {
            _techProcessList.Remove(techProcess);
        }

        internal void RemoveTechOperation(TechOperation techOperation)
        {
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        internal void RemoveProcessAction(ProcessAction processAction)
        {
            processAction.TechOperation.ProcessActions.Remove(processAction);
        }
    }
}
