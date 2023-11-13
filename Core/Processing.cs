using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.Windows.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CAM
{
    public class Processing
    {
        public int Hash;
        public GeneralOperation[] GeneralOperations { get; set; }
        public Command[] Commands { get; set; }
        public Dictionary<ObjectId, int> ToolpathCommandDictionary;
        public ObjectId? ToolpathGroup;

        public void Execute()
        {
            if (!GeneralOperations.Any(p => p.Operations.Any()))
                return;

            try
            {
                Acad.Write("Выполняется расчет обработки ...");
                Acad.CreateProgressor("Расчет обработки");
                var stopwatch = Stopwatch.StartNew();
                Acad.DeleteToolObject();
                DeleteProcessing();
                Acad.Editor.UpdateScreen();

                BuildProcessing();

                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) 
                when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
            {
                Acad.Write("Расчет прерван");
            }
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }

            Acad.CloseProgressor();
        }

        private void DeleteProcessing()
        {
            
        }

        private void BuildProcessing()
        {
            var machineType = GeneralOperations.First(p => p.Enabled).MachineType.Value;
            var processor = ProcessorFactory.Create(machineType);
            processor.Start();

            foreach (var generalOperation in GeneralOperations.Where(p => p.Enabled))
            foreach (var operation in generalOperation.Operations.Where(p => p.Enabled))
            {
                Acad.Write($"расчет операции {operation.Caption}");

                processor.SetOperarion(operation);
                operation.Execute(processor);

                //    if (!generator.IsUpperTool)
                //        generator.Uplifting();
            }

            processor.Finish();
            Commands = processor.ProcessCommands.ToArray();
            UpdateFromCommands();
        }

        private void UpdateFromCommands()
        {
            ToolpathCommandDictionary = Commands.Select((command, index) => new { command, index })
                .Where(p => p.command.ToolpathId.HasValue)
                .GroupBy(p => p.command.ToolpathId.Value)
                .ToDictionary(p => p.Key, p => p.Min(k => k.index));

            var operationGroups = Commands.GroupBy(p => p.Operation).ToList();
            foreach (var operationGroup in operationGroups)
            {
                var operation = operationGroup.Key;
                operation.ToolpathId = operationGroup.Select(p => p.ToolpathId).CreateGroup();
                operation.Caption = GetCaption(operation.Caption, operationGroup.Sum(p => p.Duration));
            }

            foreach (var generalOperationGroup in Commands.GroupBy(p => p.Operation.GeneralOperation))
                generalOperationGroup.Key.Caption = GetCaption(generalOperationGroup.Key.Caption, generalOperationGroup.Sum(p => p.Duration));

            return;

            string GetCaption(string caption, double duration)
            {
                var ind = caption.IndexOf('(');
                var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
            }
        }

        public void PartialProcessing(ITechProcess techProcess, ProcessCommand processCommand)
        {
            Acad.Write($"Выполняется формирование программы обработки по техпроцессу {techProcess.Caption} с команды номер {processCommand.Number}");
            techProcess.SkipProcessing(processCommand);
            Acad.Editor.UpdateScreen();
        }

        public void SendProgram()
        {
            if (Commands == null)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var machineType = GeneralOperations.First(p => p.Enabled).MachineType;
            var fileName = Acad.SaveFileDialog("Программа", Settings.GetMachineSettings(machineType.Value).ProgramFileExtension, machineType.ToString());
            if (fileName != null)
                try
                {
                    var contents = Commands.Select(p => p.GetProgrammLine(Settings.GetMachineSettings(machineType.Value).ProgramLineNumberFormat)).ToArray();
                    File.WriteAllLines(fileName, contents);
                    Acad.Write($"Создан файл {fileName}");
                    if (machineType == MachineType.CableSawing)
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
