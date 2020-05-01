using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CAM
{
    public class CamDocument
    {
        public int Hash;
        public List<ITechProcess> TechProcessList { get; set; } = new List<ITechProcess>();

        private readonly Dictionary<MachineType, MachineSettings> _machineSettings;    
        private readonly TechProcessFactory _techProcessFactory;

        public CamDocument(Dictionary<MachineType, MachineSettings> machineSettings, TechProcessFactory techProcessFactory)
        {
            _machineSettings = machineSettings;
            _techProcessFactory = techProcessFactory;
        }

        public ITechProcess CreateTechProcess(string techProcessName)
        {
            var techProcess = _techProcessFactory.CreateTechProcess(techProcessName);
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

        public void DeleteTechOperation(ITechOperation techOperation)
        {
            techOperation.Teardown();
            techOperation.TechProcess.TechOperations.Remove(techOperation);
        }

        public void SelectTechProcess(ITechProcess techProcess)
        {
            techProcess.SetToolpathVisible(true);
            Acad.Editor.UpdateScreen();
        }

        public void SelectTechOperation(ITechOperation techOperation)
        {
            if (techOperation.ProcessingArea != null)
                Acad.SelectObjectIds(techOperation.ProcessingArea.ObjectId);
            techOperation.TechProcess.TechOperations.ForEach(p => p.SetToolpathVisible(p == techOperation));
            if (techOperation.ProcessCommands == null)
                Acad.DeleteToolObject();
            Acad.Editor.UpdateScreen();
        }

        public void SelectProcessCommand(ITechProcess techProcess, ProcessCommand processCommand)
        {
            if (processCommand.ToolpathObjectId.HasValue)
                Acad.SelectObjectIds(processCommand.ToolpathObjectId.Value);
            Acad.RegenToolObject(techProcess.Tool, processCommand.HasTool, processCommand.ToolLocation, techProcess.MachineType == MachineType.Donatoni);  //Settongs.IsFrontPlaneZero
        }

        public void DeleteExtraObjects(ITechProcess techProcess)
        {
            techProcess.SetToolpathVisible(false);
            Acad.DeleteExtraObjects();
            //Acad.HideExtraObjects(techProcess.ToolpathCurves);
        }

        public void BuildProcessing(ITechProcess techProcess)
        {
            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Caption} ...");
                var stopwatch = Stopwatch.StartNew();
                Acad.DeleteObjects(techProcess.ToolpathObjectIds);
                Acad.DeleteExtraObjects();

                techProcess.BuildProcessing();

                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                techProcess.DeleteProcessCommands();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
            Acad.Editor.UpdateScreen();
        }

        public void PartialProcessing(ITechProcess techProcess, ProcessCommand processCommand)
        {
            Acad.Write($"Выполняется формирование программы обработки по техпроцессу {techProcess.Caption} с команды номер {processCommand.Number}");

            var toolpathObjectIds = techProcess.ToolpathObjectIds.ToList();
            techProcess.SkipProcessing(processCommand);

            Acad.DeleteObjects(toolpathObjectIds.Except(techProcess.ToolpathObjectIds));
            Acad.Editor.UpdateScreen();
        }

        public void SendProgram(List<ProcessCommand> processCommands, ITechProcess techProcess)
        {
            if (processCommands == null || !processCommands.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = Acad.SaveFileDialog(techProcess.Caption, _machineSettings[techProcess.MachineType.Value].ProgramFileExtension, techProcess.MachineType.ToString());
            if (fileName != null)
                try
                {
                    var contents = processCommands?.Select(p => p.GetProgrammLine(_machineSettings[techProcess.MachineType.Value].ProgramLineNumberFormat)).ToArray();
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
