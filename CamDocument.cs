using System;
using System.Collections.Generic;
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
            techProcess.TechOperations.ForEach(p => p.SetToolpathVisible(false));
            Acad.SelectAcadObjects(techProcess.ProcessingArea);
        }

        public void SelectTechOperation(ITechOperation techOperation)
        {
            if (techOperation.ProcessingArea != null)
                Acad.SelectObjectIds(techOperation.ProcessingArea.ObjectId);
            techOperation.TechProcess.TechOperations.ForEach(p => p.SetToolpathVisible(p == techOperation));
            Acad.Editor.UpdateScreen();
        }

        public void SelectProcessCommand(ITechProcess techProcess, ProcessCommand processCommand)
        {
            if (processCommand.ToolpathObjectId.HasValue)
                Acad.SelectObjectIds(processCommand.ToolpathObjectId.Value);
            Acad.ShowToolObject(techProcess.Tool, processCommand.ToolIndex, processCommand.ToolLocation, techProcess.MachineType == MachineType.Donatoni);
        }
       
        public void BuildProcessing(ITechProcess techProcess)
        {
            try
            {
                if (!techProcess.TechOperations.Any())
                {
                    if (!techProcess.Validate())
                        return;
                    Acad.Write($"Создание операций по техпроцессу {techProcess.Caption}");
                    if (!techProcess.CreateTechOperations().Any())
                        return;
                }

                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Caption} ...");
                Acad.DeleteObjects(techProcess.ToolpathObjectIds);
                Acad.DeleteExtraObjects();

                techProcess.BuildProcessing(_machineSettings[techProcess.MachineType.Value].ZSafety);

                Acad.Write("Расчет обработки завершен");
            }
            catch (Exception ex)
            {
                techProcess.DeleteProcessCommands();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
        }

        public void DeleteExtraObjects(ITechProcess techProcess)
        {
            techProcess.TechOperations.ForEach(p => p.SetToolpathVisible(false));
            Acad.DeleteExtraObjects();
            //Acad.HideExtraObjects(techProcess.ToolpathCurves);
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
                    var prefix = techProcess.MachineType.Value == MachineType.ScemaLogic ? "" : "N";
                    var separator = techProcess.MachineType.Value == MachineType.ScemaLogic ? ";" : " ";
                    var contents = processCommands?.Select(p => p.GetProgrammLine(prefix, separator)).ToArray();
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
