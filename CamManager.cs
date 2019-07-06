using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                _documentTechProcessList[document] = LoadTechProsess(document) ?? new List<TechProcess>();
            }
            Acad.ClearHighlighted();
            TechProcessView.Refresh(TechProcessList);
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            var document = sender as Document;
            document.CommandWillStart -= Document_CommandWillStart;
            document.BeginDocumentClose -= Document_BeginDocumentClose;
            _documentTechProcessList.Remove(document);
            if (!_documentTechProcessList.Any())
                TechProcessView.Refresh();
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT")
            {
                _documentTechProcessList[sender as Document].ForEach(p => p.DeleteToolpath());
                Acad.DeleteProcessLayer();
                TechProcessView.SetCommands(null);
                SaveTechProsess(sender as Document);
            }
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

            Acad.Alert($"Не выбраны элементы чертежа");
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
                techProcess.BuildProcessing();
            }
            catch (Exception e)
            {
                Acad.Alert(e.Message);
            }
            TechProcessView.RefreshView();
        }

        public void SendProgramm(TechProcess techProcess)
        {
            if (techProcess.ProcessCommands == null)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = $"{techProcess.Name}.csv";                
            //var filePath = @"\\192.168.137.59\ssd\Automatico\";
            var filePath = @"\\CATALINA\public\Программы станок\CodeRepository";
            var programmLines = techProcess.ProcessCommands.Select(p => p.ProgrammLine).ToArray();
            try
            {
                TechProcessView.Cursor = Cursors.WaitCursor;
                File.WriteAllLines(Path.Combine(filePath, fileName), programmLines);
                Acad.WriteMessage($"Файл {fileName} сохранен по адресу {filePath}");
                TechProcessView.Cursor = Cursors.Default;
            }
            catch (Exception e)
            {
                TechProcessView.Cursor = Cursors.Default;
                Acad.Alert($"Ошибка при записи файла программы: {e.Message}");
            }
        }

        public void SelectProcessCommand(ProcessCommand processCommand)
        {
            if (processCommand != null)
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
        private List<TechProcess> LoadTechProsess(Document document)
        {
            try
            {
                var techProcessList = (List<TechProcess>)Acad.LoadDocumentData(document, DataKey);
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
                Acad.Alert($"Ошибка при загрузке техпроцессов:\n{e.Message}");
                return null;
            }
        }

        public void SaveTechProsess(Document document)
        {
            try
            {
                Acad.SaveDocumentData(document, TechProcessList, DataKey);
            }
            catch (Exception e)
            {
                Acad.Alert($"Ошибка при записи техпроцессов:\n{e.Message}");
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
