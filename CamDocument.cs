using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM
{
    public class CamDocument
    {
        public readonly Document Document;
        public int Hash;
        public List<TechProcess> TechProcessList { get; set; } = new List<TechProcess>();
        private Settings _settings;

        public CamDocument(Document document, Settings settings)
        {
            Document = document;
            _settings = settings;
        }

        public TechProcess CreateTechProcess()
        {
            var techProcess = new TechProcess($"Обработка{TechProcessList.Count + 1}", _settings);
            TechProcessList.Add(techProcess);
            return techProcess;
        }

        public ITechOperation[] CreateTechOperations(TechProcess techProcess, ProcessingType processingType)
        {
            var curves = Acad.GetSelectedCurves();
            if (curves.Any())
                return techProcess.CreateTechOperations(processingType, curves);

            Acad.Alert($"Не выбраны элементы чертежа");
            return null;
        }

        public void DeleteTechProcess(TechProcess techProcess)
        {
            Acad.DeleteExtraObjects(techProcess.ToolpathCurves, techProcess.ToolModel);
            TechProcessList.Remove(techProcess);
        }

        public void DeleteTechOperation(TechOperationBase techOperation)
        {
            Acad.DeleteCurves(techOperation.ToolpathCurves);
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void SelectTechProcess(TechProcess techProcess) => Acad.SelectObjectIds(techProcess.TechOperations.SelectMany(p => p.ProcessingArea.AcadObjectIds).ToArray());

        public void SelectTechOperation(ITechOperation techOperation) => Acad.SelectObjectIds(techOperation.ProcessingArea.AcadObjectIds.ToArray());

        public void SelectProcessCommand(TechProcess techProcess, ProcessCommand processCommand)
        {
            Acad.SelectCurve(processCommand.ToolpathCurve);
            if (techProcess.ToolModel == null && processCommand.EndPoint != null)
                techProcess.ToolModel = Acad.CreateToolModel(techProcess.TechProcessParams.ToolDiameter, techProcess.TechProcessParams.ToolThickness);
            Acad.DrawToolModel(techProcess.ToolModel, processCommand.EndPoint, processCommand.ToolAngle);
        }
       
        public void BuildProcessing(TechProcess techProcess, BorderProcessingArea startBorder = null)
        {
            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Name} ...");

                Acad.DeleteExtraObjects(techProcess.ToolpathCurves, techProcess.ToolModel); 
                techProcess.ToolModel = null;

                techProcess.BuildProcessing(startBorder);

                Acad.SaveCurves(techProcess.ToolpathCurves);

                Acad.Write("Расчет обработки завершен");
            }
            catch (Exception ex)
            {
                techProcess.DeleteProcessCommands();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
        }

        public void SwapOuterSide(TechProcess techProcess, TechOperationBase techOperation)
        {
            var to = techOperation ?? techProcess?.TechOperations?.FirstOrDefault();
            if (to?.ProcessingArea is BorderProcessingArea border)
            {
                border.OuterSide = border.OuterSide.Swap();
                BuildProcessing(to.TechProcess, border);
            }
        }

        public void SendProgram(TechProcess techProcess)
        {
            var contents = techProcess?.ProcessCommands?.Select(p => p.GetProgrammLine()).ToArray();
            if (contents == null || !contents.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = $"{techProcess.Name}.csv";
            var filePath = _settings.GetMachineSettings(techProcess.TechProcessParams.Machine).ProgramPath;
            var fullPath = Path.Combine(filePath, fileName);
            try
            {
                File.WriteAllLines(fullPath, contents);
                Acad.Write($"Создан файл {fullPath}");
            }
            catch (Exception ex)
            {
                Acad.Alert($"Ошибка при записи файла {fullPath}", ex);
            }
        }
    }
}
