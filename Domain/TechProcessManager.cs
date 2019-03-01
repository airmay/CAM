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
    public class TechProcessManager
    {
        private List<TechProcess> _techProcessList = new List<TechProcess>();
        private SawingTechOperationParams _sawingLineTechOperationParamsDefault = new SawingTechOperationParams();
        private SawingTechOperationParams _sawingCurveTechOperationParamsDefault = new SawingTechOperationParams();
        private TechProcessParams _techProcessParams = new TechProcessParams();
        private SawingTechOperationFactory _techOperationFactory;
        private IAcadGateway _acad;

        public EventHandler<ProgramEventArgs> ProgramGenerated;

        public TechProcessManager(IAcadGateway acad)
        {
            _acad = acad;

            _sawingLineTechOperationParamsDefault.Modes.Add(new SawingMode { Depth = 30, DepthStep = 5, Feed = 2000 });
            _sawingCurveTechOperationParamsDefault.Modes.AddRange(new SawingMode[3] {
                new SawingMode { Depth = 10, DepthStep = 5, Feed = 2000 },
                new SawingMode { Depth = 20, DepthStep = 2, Feed = 1000 },
                new SawingMode { Depth = 30, DepthStep = 1, Feed = 500 } });
            _techOperationFactory = new SawingTechOperationFactory(_sawingLineTechOperationParamsDefault, _sawingCurveTechOperationParamsDefault);

            _techProcessParams.Tool = new Tool();
            _techProcessParams.Tool.Number = 1;
            // TODO убрать
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Изделие{_techProcessList.Count + 1}", _techProcessParams);
            _techProcessList.Add(techProcess);
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

        public IEnumerable<SawingTechOperation> CreateTechOperations(TechProcess techProcess, TechOperationType techOperationType)
        {
	        var factory = techProcess.TechOperationFactorys.SingleOrDefault(p => p.TechOperationType == techOperationType);
	        if (factory == null)
	        {
		        switch (techOperationType)
		        {
			        case TechOperationType.Sawing:
				        factory = new SawingTechOperationFactory(_sawingLineTechOperationParamsDefault, _sawingCurveTechOperationParamsDefault);
						break;
		        }

		        techProcess.TechOperationFactorys.Add(factory);
	        }

	        return _acad.GetSelectedEntities().Select(p => _techOperationFactory.Create(techProcess, p));
        }

        public bool MoveForwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapNext(techOperation);

	    public bool MoveBackwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapPrev(techOperation);


        public void RemoveTechProcess(TechProcess techProcess)
        {
            DeleteToolpath(techProcess);
            _techProcessList.Remove(techProcess);
        }

        private void DeleteToolpath(TechProcess techProcess)
        {
            _acad.DeleteEntities(techProcess.TechOperations.SelectMany(p => p.ProcessCommands).Select(p => p.ToolpathAcadObject?.ObjectId).Where(p => p != null).ToList());
        }

        public void RemoveTechOperation(TechOperation techOperation)
        {
            _acad.DeleteEntities(techOperation.ProcessCommands.ConvertAll(p => p.ToolpathAcadObject?.ObjectId).FindAll(p => p != null));
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void RemoveProcessAction(TechOperation techOperation, ProcessAction processAction)
        {
            _acad.DeleteEntities(new List<ObjectId> { processAction.ToolpathAcadObject.ObjectId });
            techOperation.ProcessActions.Remove(processAction);
        }

        public void BuildProcessing(TechProcess techProcess)
        {
            DeleteToolpath(techProcess);
	        var currentPoint = Point3d.Origin;
			techProcess.TechOperations.ForEach(p => currentPoint = p.BuildProcessing(currentPoint, p == techProcess.TechOperations.Last()));
            _acad.CreateEntities(techProcess.TechOperations.SelectMany(p => p.ProcessCommands).Select(p => p.ToolpathAcadObject));

            var programGenerator = new ScemaLogicProgramGenerator();
            var program = programGenerator.Generate(techProcess);
            ProgramGenerated?.Invoke(this, new ProgramEventArgs(program));
        }        
    }

    public class ProgramEventArgs : EventArgs
    {
        public string Program { get; set; }

        public ProgramEventArgs(string program)
        {
            Program = program;
        }
    }
}
