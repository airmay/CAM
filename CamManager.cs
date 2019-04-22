using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using CAM.Domain;
using CAM.UI;
using Dreambuild.AutoCAD;

namespace CAM
{
    /// <summary>
    /// Управляющий класс
    /// </summary>
    public class CamManager
    {
        private const string DataKey = "TechProcessList";

        private AcadGateway _acad;

        public CamContainer Container { get; set; }

        public TechProcessView TechProcessView { get; set; }

        private Dictionary<Document, List<TechProcess>> _documentTechProcessList = new Dictionary<Document, List<TechProcess>>();

        private List<TechProcess> TechProcessList => _documentTechProcessList[_acad.Document];

        //public EventHandler<ProgramEventArgs> ProgramGenerated;

        public CamManager(AcadGateway acad)
        {
            _acad = acad;
        }

        public void SetActiveDocument(Document document)
        {
            if (!_documentTechProcessList.ContainsKey(document))
                _documentTechProcessList[document] = LoadTechProsess() ?? new List<TechProcess>();
            TechProcessView.SetTechProcessList(TechProcessList);
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Обработка{TechProcessList.Count + 1}", Container);
            TechProcessList.Add(techProcess);
            return techProcess;
        }

        public List<SawingTechOperation> CreateTechOperations(TechProcess techProcess, TechOperationType techOperationType) =>
            techProcess.CreateTechOperations(techOperationType, _acad.GetSelectedCurves());

        internal void SelectTechProcess(TechProcess techProcess) => _acad.SelectCurves(techProcess.TechOperations.Select(p => p.ProcessingArea.AcadObjectId));

        internal void SelectTechOperation(TechOperation techOperation) => _acad.SelectCurve(techOperation.ProcessingArea.AcadObjectId);

        public bool MoveForwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapNext(techOperation);

        public bool MoveBackwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapPrev(techOperation);

        public void DeleteTechProcess(TechProcess techProcess)
        {
            _acad.DeleteCurves(techProcess.ToolpathCurves);
            TechProcessList.Remove(techProcess);
        }

        public void DeleteTechOperation(TechOperation techOperation)
        {
            _acad.DeleteCurves(techOperation.ToolpathCurves);
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void BuildProcessing(TechProcess techProcess)
        {
            _acad.DeleteCurves(techProcess.ToolpathCurves);
            techProcess.BuildProcessing();
            _acad.SaveCurves(techProcess.ToolpathCurves);

            //var programGenerator = new ScemaLogicProgramGenerator();
            //var program = programGenerator.Generate(techProcess);
            //ProgramGenerated?.Invoke(this, new ProgramEventArgs(program));
        }

        public void SelectProcessCommand(ProcessCommand processCommand)
        {
            if (processCommand?.ToolpathCurve != null)
                _acad.SelectCurve(processCommand.ToolpathCurve.ObjectId);
        }

        #region Load/Save TechProsess
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
                    techProcessList.ForEach(tp =>
                    {
                        tp.SetContainer(Container);
                        tp.TechOperations.ForEach(to =>
                        {
                            to.ProcessingArea.AcadObjectId = _acad.GetObjectId(to.ProcessingArea.Handle);
                            to.TechProcess = tp;
                        });
                    });
                    Interaction.WriteLine($"Загружены техпроцессы: {string.Join(", ", techProcessList.Select(p => p.Name))}");
                }
                return techProcessList;
            }
            catch (Exception e)
            {
                Application.ShowAlertDialog($"Ошибка при загрузке техпроцессов:\n{e.Message}");
                return null;
            }
        }

        public void SaveTechProsess()
        {
            try
            {
                _acad.SaveDocumentData(TechProcessList, DataKey);
                Interaction.WriteLine("Техпроцессы успешно сохранены в файле чертежа");
            }
            catch (Exception e)
            {
                Application.ShowAlertDialog($"Ошибка при записи техпроцессов:\n{e.Message}");
            }
        }

        #endregion    }

        //public class ProgramEventArgs : EventArgs
        //{
        //    public string Program { get; set; }

        //    public ProgramEventArgs(string program)
        //    {
        //        Program = program;
        //    }
        //}
    }
}
