using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
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

        public CamContainer Container { get; set; }

        public TechProcessView TechProcessView { get; set; }

        private Dictionary<Document, List<TechProcess>> _documentTechProcessList = new Dictionary<Document, List<TechProcess>>();

        private List<TechProcess> TechProcessList => _documentTechProcessList[Acad.Document];

        //public EventHandler<ProgramEventArgs> ProgramGenerated;

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

        public TechOperation[] CreateTechOperations(TechProcess techProcess, TechOperationType techOperationType)
        {
            var curves = Acad.GetSelectedCurves();
            if (curves.Any())
                return techProcess.CreateTechOperations(techOperationType, curves);

            Application.ShowAlertDialog($"Не выбраны элементы чертежа");
            return null;
        }

        internal void SelectTechProcess(TechProcess techProcess) => Acad.SelectObjectIds(techProcess.TechOperations.Select(p => p.ProcessingArea.AcadObjectId).ToArray());

        internal void SelectTechOperation(TechOperation techOperation) => Acad.SelectObjectIds(techOperation.ProcessingArea.AcadObjectId);

        public bool MoveForwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapNext(techOperation);

        public bool MoveBackwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapPrev(techOperation);

        public void DeleteTechProcess(TechProcess techProcess)
        {
            Acad.DeleteCurves(techProcess.ToolpathCurves);
            TechProcessList.Remove(techProcess);
        }

        public void DeleteTechOperation(TechOperation techOperation)
        {
            Acad.DeleteCurves(techOperation.ToolpathCurves);
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void BuildProcessing(TechProcess techProcess)
        {
            try
            {
                Acad.DeleteCurves(techProcess.ToolpathCurves);
                techProcess.BuildProcessing();
                Acad.SaveCurves(techProcess.ToolpathCurves);
                TechProcessView.RefreshView();
            }
            catch (Exception e)
            {
                Application.ShowAlertDialog(e.Message);
            }

            //var programGenerator = new ScemaLogicProgramGenerator();
            //var program = programGenerator.Generate(techProcess);
            //ProgramGenerated?.Invoke(this, new ProgramEventArgs(program));
        }

        public void SelectProcessCommand(ProcessCommand processCommand)
        {
            Acad.SelectCurve(processCommand.ToolpathCurve);
        }

        public void SwapOuterSide(TechProcess techProcess, TechOperation techOperation)
        {
            var to = techOperation ?? techProcess?.TechOperations?.FirstOrDefault();
            if (to?.ProcessingArea is BorderProcessingArea border)
            {
                border.OuterSide = border.OuterSide.Swap();
                to.TechProcess.ProcessBorders(border);
                BuildProcessing(to.TechProcess);
            }
        }

        #region Load/Save TechProsess
        /// <summary>
        /// Загрузить технологические процессы из файла чертежа
        /// </summary>
        private List<TechProcess> LoadTechProsess()
        {
            try
            {
                var techProcessList = (List<TechProcess>)Acad.LoadDocumentData(DataKey);
                if (techProcessList != null)
                {
                    techProcessList.ForEach(tp =>
                    {
                        tp.SetContainer(Container);
                        tp.TechOperations.ForEach(to =>
                        {
                            to.ProcessingArea.AcadObjectId = Acad.GetObjectId(to.ProcessingArea.Handle);
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
                Acad.SaveDocumentData(TechProcessList, DataKey);
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
