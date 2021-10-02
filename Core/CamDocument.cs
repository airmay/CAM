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

        private readonly TechProcessFactory _techProcessFactory;

        public CamDocument(TechProcessFactory techProcessFactory)
        {
            _techProcessFactory = techProcessFactory;
        }

        public ITechProcess CreateTechProcess(string techProcessName)
        {
            var techProcess = _techProcessFactory.CreateTechProcess(techProcessName);
            TechProcessList.Add(techProcess);
            return techProcess;
        }

        public List<TechOperation> CreateTechOperation(ITechProcess techProcess, string techOperationName) => _techProcessFactory.CreateTechOperations(techProcess, techOperationName);

        public IEnumerable<string> GetTechProcessNames() => _techProcessFactory.GetTechProcessNames();

        public ILookup<Type, string> GetTechOperationNames() => _techProcessFactory.GetTechOperationNames();

        public void DeleteTechProcess(ITechProcess techProcess)
        {
            techProcess.DeleteProcessing();
            techProcess.Teardown();
            TechProcessList.Remove(techProcess);
        }

        public void DeleteTechOperation(MillingTechOperation techOperation)
        {
            techOperation.TechProcessBase.DeleteProcessing();
            techOperation.Teardown();
            techOperation.TechProcessBase.TechOperations.Remove(techOperation);
        }
       
        public void BuildProcessing(ITechProcess techProcess)
        {
            if (!techProcess.TechOperations.Any())
                techProcess.CreateTechOperations();

            if (!techProcess.Validate() || techProcess.TechOperations.Any(p => p.Enabled && p.CanProcess && !p.Validate()))
                return;

            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {techProcess.Caption} ...");
                var stopwatch = Stopwatch.StartNew();
                Acad.CreateProgressor($"Расчет обработки по техпроцессу \"{techProcess.Caption}\"");
                techProcess.DeleteProcessing();
                Acad.Editor.UpdateScreen();

                techProcess.BuildProcessing();

                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
                    Acad.Write("Расчет прерван");
                else
                    Acad.Alert("Ошибка при выполнении расчета", ex);
            }
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
            Acad.CloseProgressor();
        }

        public void PartialProcessing(ITechProcess techProcess, ProcessCommand processCommand)
        {
            Acad.Write($"Выполняется формирование программы обработки по техпроцессу {techProcess.Caption} с команды номер {processCommand.Number}");
            techProcess.SkipProcessing(processCommand);
            Acad.Editor.UpdateScreen();
        }

        public void SendProgram(ITechProcess techProcess)
        {
            if (techProcess.ProcessCommands == null)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }
            var fileName = Acad.SaveFileDialog(techProcess.Caption, Settings.GetMachineSettings(techProcess.MachineType.Value).ProgramFileExtension, techProcess.MachineType.ToString());
            if (fileName != null)
                try
                {
                    var contents = techProcess.ProcessCommands.Select(p => p.GetProgrammLine(Settings.GetMachineSettings(techProcess.MachineType.Value).ProgramLineNumberFormat)).ToArray();
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
