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
        public TechProcessView TechProcessView { get; set; }

        private Dictionary<Document, CamDocument> _documents = new Dictionary<Document, CamDocument>();

        private List<TechProcess> TechProcessList;

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
                Acad.DeleteProcessLayer();
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
            techProcess.BuildProcessing();
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
                Acad.Write($"Файл {fileName} сохранен по адресу {filePath}");
                TechProcessView.Cursor = Cursors.Default;
            }
            catch (Exception e)
            {
                TechProcessView.Cursor = Cursors.Default;
                Acad.Alert($"Ошибка при записи файла программы", e);
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
    }
}
