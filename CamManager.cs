using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using CAM.Domain;
using CAM.UI;

namespace CAM
{
    /// <summary>
    /// Управляющий класс
    /// </summary>
    public class CamManager
    {
        private const string DataKey = "TechProcessList";
        public List<TechProcess> TechProcessList => _documentTeshProcessList[_acad.Document];
        private AcadGateway _acad;
        private CamContainer _container = CamContainer.Load();
        public TechProcessView TechProcessView { get; set; }

        public EventHandler<ProgramEventArgs> ProgramGenerated;

        public CamManager(AcadGateway acad)
        {
            _acad = acad;
            //CreateTechProcess();
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Обработка{TechProcessList.Count + 1}");
            TechProcessList.Add(techProcess);

            return techProcess;
        }

        internal void SelectTechProcess(TechProcess techProcess) => _acad.SelectCurves(techProcess.TechOperations.Select(p => p.ProcessingArea.AcadObjectId));

        private Dictionary<Document, List<TechProcess>> _documentTeshProcessList = new Dictionary<Document, List<TechProcess>>();

        //private Document _activeDocument;

        public void SetActiveDocument(Document document)
        {
            if (!_documentTeshProcessList.ContainsKey(document))
                _documentTeshProcessList[document] = LoadTechProsess() ?? new List<TechProcess>();

            //_activeDocument = document;
            TechProcessView.Reset();
        }

        internal void SelectTechOperation(TechOperation techOperation) => _acad.SelectCurve(techOperation.ProcessingArea.AcadObjectId);

        public List<SawingTechOperation> CreateTechOperations(TechProcess techProcess, TechOperationType techOperationType)
        {
            // select operation Autodesk.AutoCAD.ApplicationServices.Application  ShowModalDialog и ShowModelessDialog.
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
                _acad.SelectCurve(processCommand.ToolpathCurve.ObjectId);
        }

        /// <summary>
        /// Загрузить технологические процессы из файла чертежа
        /// </summary>
        private List<TechProcess> LoadTechProsess()
        {
            try
            {
                var techProcessList = (List<TechProcess>)_acad.LoadDocumentData(DataKey);
                if (techProcessList != null)
                {
                    techProcessList.ForEach(tp => tp.TechOperations.ForEach(to => to.ProcessingArea.AcadObjectId = _acad.GetObjectId(to.ProcessingArea.Handle)));
                    _acad.WriteMessage($"Загружены техпроцессы: {string.Join(", ", techProcessList.Select(p => p.Name))}");
                }
                return techProcessList;
            }
            catch (System.Exception e)
            {
                _acad.WriteMessage(string.Format($"Ошибка при загрузке техпроцессов:\n{e}"));
                return null;
            }
        }

        public void SaveTechProsess()
        {
            try
            {
                _acad.SaveDocumentData(TechProcessList, DataKey);
                _acad.WriteMessage("Техпроцессы успешно сохранены в файле чертежа");
            }
            catch (System.Exception e)
            {
                Application.ShowAlertDialog($"Ошибка при записи техпроцессов:\n{e}");
            }
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
