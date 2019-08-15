using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        public TechProcessView TechProcessView { get; set; }

        private Dictionary<Document, CamDocument> _documents = new Dictionary<Document, CamDocument>();

        private List<TechProcess> TechProcessList;
        private TechProcess _currentTechProcess;

        public void SetActiveDocument(Document document)
        {
            if (!_documents.ContainsKey(document))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                _documents[document] = new CamDocument(document);
            }
            Acad.ClearHighlighted();
            TechProcessList = _documents[document].TechProcessList;
            TechProcessView.Refresh(TechProcessList);
            _currentTechProcess = TechProcessList.FirstOrDefault();
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            var document = sender as Document;
            document.CommandWillStart -= Document_CommandWillStart;
            document.BeginDocumentClose -= Document_BeginDocumentClose;
            _documents.Remove(document);
            if (!_documents.Any())
                TechProcessView.Refresh();
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT")
            {
                _documents[sender as Document].TechProcessList.ForEach(p => p.DeleteToolpath());
                Acad.DeleteAll();
                TechProcessView.ClearCommandsView();
                _documents[sender as Document].SaveTechProsess();
            }
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Обработка{TechProcessList.Count + 1}");
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

        internal void SelectTechProcess(TechProcess techProcess)
        {
            _currentTechProcess = techProcess;
            Acad.SelectObjectIds(techProcess.TechOperations.Select(p => p.ProcessingArea.AcadObjectId).ToArray());
        }

        internal void SelectTechOperation(TechOperation techOperation)
        {
            _currentTechProcess = techOperation.TechProcess;
            Acad.SelectObjectIds(techOperation.ProcessingArea.AcadObjectId);
        }

        public bool MoveForwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapNext(techOperation);

        public bool MoveBackwardTechOperation(TechOperation techOperation) => techOperation.TechProcess.TechOperations.SwapPrev(techOperation);

        public void DeleteTechProcess(TechProcess techProcess)
        {
            Acad.DeleteHatch();
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
            techProcess.BuildProcessing();
            TechProcessView.RefreshView();
        }

        #region Program
        public string[] GetProgramm() => _currentTechProcess?.ProcessCommands?.Select(p => p.GetProgrammLine()).ToArray();

        public void SendProgramm() => SendProgram(GetProgramm());

        internal void SendProgram(string[] lines)
        {
            if (lines == null || !lines.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = $"{_currentTechProcess.Name}.csv";
            //var filePath = @"\\192.168.137.59\ssd\Automatico\";
            var filePath = @"\\CATALINA\public\Программы станок\CodeRepository";
            var fullPath = Path.Combine(filePath, fileName);
            try
            {
                TechProcessView.Cursor = Cursors.WaitCursor;
                File.WriteAllLines(fullPath, lines);
                Acad.Write($"Записан файл {fullPath}");
                TechProcessView.Cursor = Cursors.Default;
            }
            catch (Exception e)
            {
                TechProcessView.Cursor = Cursors.Default;
                Acad.Alert($"Ошибка при записи файла {fullPath}", e);
            }
        } 
        #endregion

        public void SelectProcessCommand(ProcessCommand processCommand) => Acad.SelectCurve(processCommand.GetToolpathCurve());

        public void SwapOuterSide(TechProcess techProcess, TechOperation techOperation)
        {
            var to = techOperation ?? techProcess?.TechOperations?.FirstOrDefault();
            if (to?.ProcessingArea is BorderProcessingArea border)
            {
                border.OuterSide = border.OuterSide.Swap();
                to.TechProcess.BuildProcessing(border);
                TechProcessView.RefreshView();
            }
        }
    }
}
