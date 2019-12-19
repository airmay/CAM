using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    public class CamDocument
    {
        public readonly Document Document;
        public int Hash;
        public List<TechProcess> TechProcessList { get; set; } = new List<TechProcess>();

        public CamDocument(Document document)
        {
            Document = document;
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

        public void SelectTechProcess(TechProcess techProcess) => Acad.SelectObjectIds(techProcess.TechOperations.Select(p => p.ProcessingArea.AcadObjectId).ToArray());

        public void SelectTechOperation(TechOperation techOperation) => Acad.SelectObjectIds(techOperation.ProcessingArea.AcadObjectId);

        public void SelectProcessCommand(ProcessCommand processCommand) => Acad.SelectCurve(processCommand.GetToolpathCurve());

        public void BuildProcessing(TechProcess techProcess, BorderProcessingArea startBorder = null)
        {
            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Name} ...");

                Acad.DeleteHatch();
                Acad.DeleteCurves(techProcess.ToolpathCurves);

                techProcess.BuildProcessing(startBorder);

                Acad.SaveCurves(techProcess.ToolpathCurves);

                Acad.Write("Расчет обработки завершен");
            }
            catch (Exception ex)
            {
                techProcess.DeleteToolpath();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
        }

        public void SwapOuterSide(TechProcess techProcess, TechOperation techOperation)
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
            var filePath = Settings.GetMachineSettings(techProcess.TechProcessParams.Machine).ProgramPath;
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
