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
        private AcadGateway _acad = new AcadGateway();

        /// <summary>
        /// Текущий техпроцесс
        /// </summary>
        public TechProcess CurrentTechProcess { get; set; }

        /// <summary>
        /// Текущая техоперация
        /// </summary>
        public SawingTechOperation CurrentTechOperation { get; set; }

        public TechProcessService()
        {
            _techOperationFactory = new SawingTechOperationFactory(_techProcessParams, _sawingLineTechOperationParamsDefault, _sawingArcTechOperationParamsDefault);
        }

        public TechProcess CreateTechProcess()
        {
            CurrentTechProcess = new TechProcess($"Изделие{_techProcessList.Count + 1}");
            _techProcessList.Add(CurrentTechProcess);
            CreateTechOperation();
            return CurrentTechProcess;
        }

        public List<SawingTechOperation> CreateTechOperation()
        {
            var operations = _acad.GetSelectedCurves().Select(p => _techOperationFactory.Create(p)).ToList();
            CurrentTechProcess.TechOperations.AddRange(operations);
            CurrentTechOperation = operations.Last();
            return operations;
        }

        public void Remove()
        {
            if (CurrentTechOperation == null)
            {
                _techProcessList.Remove(CurrentTechProcess);
                CurrentTechProcess = null;
            }
            else
            {
                CurrentTechProcess.TechOperations.Remove(CurrentTechOperation);
                CurrentTechOperation = null;
            }
        }

        public bool MoveForwardTechOperation()
        {
            var flag = CurrentTechOperation != null && CurrentTechOperation != CurrentTechProcess.TechOperations.Last();
            if (flag)
                CurrentTechProcess.TechOperations.SwapNext(CurrentTechOperation);
            return flag;
        }

        public bool MoveBackwardTechOperation()
        {
            var flag = CurrentTechOperation != null && CurrentTechOperation != CurrentTechProcess.TechOperations.First();
            if (flag)
                CurrentTechProcess.TechOperations.SwapPrev(CurrentTechOperation);
            return flag;
        }

        public void SetCurrentTechProcess(string techProcessId)
        {
            CurrentTechProcess = _techProcessList.Single(p => p.Id == techProcessId);
            CurrentTechOperation = null;
        }

        public void SetCurrentTechOperation(string techProcessId, string techOperationId)
        {
            SetCurrentTechProcess(techProcessId);
            CurrentTechOperation = CurrentTechProcess.TechOperations.Single(p => p.Id == techOperationId);
        }

        public void RenameTechProcess(string name)
        {
            CurrentTechProcess.Name = name;
        }

        public void RenameTechOperation(string name)
        {
            CurrentTechOperation.Name = name;
        }
    }
}
