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
        public List<ITechProcess> TechProcessList { get; set; } = new List<ITechProcess>();
        private Settings _settings;
        private readonly TechProcessFactory _techProcessFactory;

        public CamDocument(Document document, Settings settings)
        {
            Document = document;
            _settings = settings;
            _techProcessFactory = new TechProcessFactory(settings);
        }

        public ITechProcess CreateTechProcess(string techProcessName)
        {
            var techProcess = _techProcessFactory.CreateTechProcess(techProcessName);
            //techProcess.MachineType = MachineType.Donatoni; ////////////////////////
            TechProcessList.Add(techProcess);
            return techProcess;
        }

        public List<ITechOperation> CreateTechOperation(ITechProcess techProcess, string techOperationName) => _techProcessFactory.CreateTechOperations(techProcess, techOperationName);

        public IEnumerable<string> GetTechProcessNames() => _techProcessFactory.GetTechProcessNames();

        public ILookup<Type, string> GetTechOperationNames() => _techProcessFactory.GetTechOperationNames();

        public void DeleteTechProcess(ITechProcess techProcess)
        {
            techProcess.Teardown();
            TechProcessList.Remove(techProcess);
        }

        public void DeleteTechOperation(TechOperationBase techOperation)
        {
            techOperation.Teardown();
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void SelectTechProcess(ITechProcess techProcess)
        {
            techProcess.TechOperations.ForEach(p => p.SetToolpathVisible(false));
            Acad.SelectObjectIds(techProcess.ProcessingArea?.ObjectIds);
        }

        public void SelectTechOperation(ITechOperation techOperation)
        {
            //Acad.SelectObjectIds(techOperation.ProcessingArea.AcadObjectIds.ToArray());
            techOperation.TechProcess.TechOperations.ForEach(p => p.SetToolpathVisible(p == techOperation));
            Acad.Editor.UpdateScreen();
        }

        public void SelectProcessCommand(ITechProcess techProcess, ProcessCommand processCommand)
        {
            Acad.SelectObjectIds(processCommand.ToolpathObjectId);
            Acad.ShowToolObject(techProcess.Tool, processCommand.ToolIndex, processCommand.ToolLocation, techProcess.MachineType == MachineType.Donatoni);
        }
       
        public void BuildProcessing(ITechProcess techProcess, BorderProcessingArea startBorder = null)
        {
            try
            {
                if (!techProcess.TechOperations.Any())
                {
                    Acad.Write($"Создание операций по техпроцессу {techProcess.Caption}");
                    if (!_techProcessFactory.CreateTechOperations(techProcess).Any())
                        return;
                }

                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Caption} ...");

                Acad.DeleteObjects(techProcess.ToolpathObjectIds);
                Acad.DeleteExtraObjects();

                techProcess.BuildProcessing(); // startBorder);

                Acad.Write("Расчет обработки завершен");
            }
            catch (Exception ex)
            {
                techProcess.DeleteProcessCommands();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
        }

        public void HideShowProcessing(ITechProcess techProcess)
        {
            //Acad.HideExtraObjects(techProcess.ToolpathCurves);
        }

        public void SwapOuterSide(ITechProcess techProcess, TechOperationBase techOperation)
        {
            //var to = techOperation ?? techProcess?.TechOperations?.FirstOrDefault();
            //if (to?.ProcessingArea is BorderProcessingArea border)
            //{
            //    border.OuterSide = border.OuterSide.Swap();
            //    BuildProcessing(to.TechProcess, border);
            //}
        }

        public void SendProgram(List<ProcessCommand> processCommands, ITechProcess techProcess)
        {
            if (processCommands == null || !processCommands.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = Acad.SaveFileDialog(techProcess.Caption, techProcess.MachineSettings.ProgramFileExtension, techProcess.MachineType.ToString());
            if (fileName != null)
                try
                {
                    var contents = processCommands?.Select(p => p.GetProgrammLine()).ToArray();
                    File.WriteAllLines(fileName, contents);
                    Acad.Write($"Создан файл {fileName}");
                }
                catch (Exception ex)
                {
                    Acad.Alert($"Ошибка при записи файла {fileName}", ex);
                }
        }
    }
}
