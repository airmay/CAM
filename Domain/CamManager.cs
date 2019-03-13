using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Управляющий класс
    /// </summary>
    public class CamManager
    {
        public List<TechProcess> TechProcessList = new List<TechProcess>();
        private SawingTechOperationParams _sawingLineTechOperationParamsDefault = new SawingTechOperationParams();
        private SawingTechOperationParams _sawingCurveTechOperationParamsDefault = new SawingTechOperationParams();
        private SawingTechOperationFactory _techOperationFactory;
        private IAcadGateway _acad;

        public EventHandler<ProgramEventArgs> ProgramGenerated;

        public CamManager(IAcadGateway acad)
        {
            _acad = acad;

            _sawingLineTechOperationParamsDefault.Modes.Add(new SawingMode { Depth = 30, DepthStep = 5, Feed = 2000 });
            _sawingCurveTechOperationParamsDefault.Modes.AddRange(new SawingMode[3] {
                new SawingMode { Depth = 10, DepthStep = 5, Feed = 2000 },
                new SawingMode { Depth = 20, DepthStep = 2, Feed = 1000 },
                new SawingMode { Depth = 30, DepthStep = 1, Feed = 500 } });
            _techOperationFactory = new SawingTechOperationFactory(_sawingLineTechOperationParamsDefault, _sawingCurveTechOperationParamsDefault);

	        CreateTechProcess();
	        // TODO убрать
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Обработка{TechProcessList.Count + 1}");
            TechProcessList.Add(techProcess);

            return techProcess;
        }

        internal void SelectTechProcess(TechProcess techProcess) => _acad.SelectEntities(techProcess.TechOperations.ConvertAll(p => p.ProcessingArea.AcadObjectId));

	    internal void SelectTechOperation(TechOperation techOperation) => _acad.SelectEntities(new List<ObjectId> { techOperation.ProcessingArea.AcadObjectId });

	    public List<SawingTechOperation> CreateTechOperations(TechProcess techProcess, TechOperationType techOperationType)
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

	        var operations = _acad.GetSelectedEntities().Select(p => _techOperationFactory.Create(techProcess, p)).ToList();

	        return operations;
        }

        public bool MoveForwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapNext(techOperation);

	    public bool MoveBackwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapPrev(techOperation);
		
        public void DeleteTechProcess(TechProcess techProcess)
        {
			_acad.DeleteEntities(techProcess.ToolpathCurves);
			TechProcessList.Remove(techProcess);
        }

	    public void DeleteTechOperation(TechOperation techOperation)
        {
            _acad.DeleteEntities(techOperation.ToolpathCurves);
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void BuildProcessing(TechProcess techProcess)
        {
			_acad.DeleteEntities(techProcess.ToolpathCurves);
			techProcess.BuildProcessing();
            _acad.CreateEntities(techProcess.ToolpathCurves);

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
