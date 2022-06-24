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

        public void DeleteTechOperation(ITechProcess techProcess, int index) => techProcess.RemoveTechOperation(index);

        public void BuildProcessing(ITechProcess techProcess)
        {
            if (!techProcess.TechOperations.Any())
                techProcess.CreateTechOperations();

            if (!techProcess.Validate() || techProcess.TechOperations.Any(p => p.Enabled && p.CanProcess && !p.Validate()))
                return;

            try
            {
                Acad.DeleteToolObject();
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
                    if (techProcess.MachineType == MachineType.CableSawing)
                        CreateImitationProgramm(contents, fileName);
                }
                catch (Exception ex)
                {
                    Acad.Alert($"Ошибка при записи файла {fileName}", ex);
                }
        }

        private void CreateImitationProgramm(string[] contents, string fileName)
        {
            List<string> result = new List<string>(contents.Length * 2);
            foreach (var item in contents)
            {
                if (item.StartsWith("M03"))
                    continue;

                var line = item.Replace("G01", "G00");
                var vi = line.IndexOf('V');
                if (vi > 0)
                    line = line.Substring(0, vi) + "V0";

                if (line == "G00 U0 V0")
                    continue;

                result.Add(line);

                if (line.StartsWith("G00"))
                    result.Add("M00");
            }
            var parts = fileName.Split('.');
            fileName = parts[0] + "_i." + parts[1];
            File.WriteAllLines(fileName, result);
            Acad.Write($"Создан файл с имитацией {fileName}");
        }
    }
}
