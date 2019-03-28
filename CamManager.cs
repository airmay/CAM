using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Domain;

namespace CAM
{
    /// <summary>
    /// Управляющий класс
    /// </summary>
    public class CamManager
    {
        public List<TechProcess> TechProcessList = new List<TechProcess>();
        private IAcadGateway _acad;
        private CamContainer _container = CamContainer.Load();

        public EventHandler<ProgramEventArgs> ProgramGenerated;

        public CamManager(IAcadGateway acad)
        {
            _acad = acad;
	        CreateTechProcess();
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
				        factory = new SawingTechOperationFactory(_container.SawingLineTechOperationParams, _container.SawingCurveTechOperationParams);
						break;
		        }

		        techProcess.TechOperationFactorys.Add(factory);
	        }

	        var operations = _acad.GetSelectedEntities().Select(p => factory.Create(techProcess, p)).ToList();

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

            //var programGenerator = new ScemaLogicProgramGenerator();
            //var program = programGenerator.Generate(techProcess);
            //ProgramGenerated?.Invoke(this, new ProgramEventArgs(program));
        }

        public void SelectProcessCommand(ProcessCommand processCommand)
        {
            if (processCommand.ToolpathCurve != null)
                _acad.SelectCurve(processCommand.ToolpathCurve);
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
