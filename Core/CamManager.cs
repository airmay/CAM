using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core.UI;

namespace CAM
{
    public static class CamManager
    {
        private static CamDocument _camDocument;
        public static readonly ProcessingView ProcessingView = Acad.ProcessingView;
        public static List<Command> Commands;

        public static void SetDocument(CamDocument camDocument)
        {
            if (_camDocument != null)
                UpdateProcessing();
            _camDocument = camDocument;
            Commands?.Clear();
            ProcessingView.CreateTree(_camDocument.Processings);
            //Acad.ClearHighlighted();
        }

        public static void RemoveProcessing()
        {
            _camDocument = null;
            ProcessingView.ClearView();
        }

        public static void SaveProcessing()
        {
            //UpdateProcessing();

            //Acad.Documents[sender as Document].GeneralOperations.ForEach(p => p.DeleteProcessing());

            _camDocument.Save();

            //ProcessingView.ClearCommandsView();
            //Acad.DeleteAll();
        }

        private static void UpdateProcessing()
        {
            HideTool();
        }

        private static Dictionary<ObjectId, int> _toolpathCommandDictionary;
        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathId() is ObjectId id && _toolpathCommandDictionary.TryGetValue(id, out var commandIndex))
                ProcessingView.SelectProcessCommand(commandIndex);
        }

        private static ToolObject ToolObject { get; } = new ToolObject();

        public static List<Command> ExecuteProcessing()
        {
            if (!Processings.Any(p => p.Operations.Any()))
                return;

            try
            {
                Acad.Write("Выполняется расчет обработки ...");
                Acad.CreateProgressor("Расчет обработки");
                var stopwatch = Stopwatch.StartNew();
                DeleteProcessing();
                Acad.Editor.UpdateScreen();

                BuildProcessing();
                if (CamManager.Commands != null)
                    UpdateFromCommands();

                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
            {
                Acad.Write("Расчет прерван");
            }
#if !DEBUG  
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
#endif

            Acad.CloseProgressor();
            return Commands;

        }

        private static void DeleteProcessing()
        {
            foreach (var generalOperation in Processings)
                foreach (var operation in generalOperation.Operations)
                {
                    operation.ToolpathGroup?.DeleteGroup();
                    operation.ToolpathGroup = null;
                    operation.SupportGroup?.DeleteGroup();
                    operation.SupportGroup = null;
                }

            CamManager.Commands = null;
            ToolObject.Hide();
        }

        private static void BuildProcessing()
        {
            var generalParams = Processings.First(p => p.Enabled);
            var machineType = generalParams.MachineType;
            if (!machineType.CheckNotNull("Станок"))
                return;
            Machine = machineType.Value;
            var tool = generalParams.Tool;
            if (!tool.CheckNotNull("Инструмент"))
                return;

            using (var processor = ProcessorFactory.Create(Machine))
            {
                processor.Start(tool);

                foreach (var generalOperation in Processings.Where(p => p.Enabled))
                {
                    processor.SetGeneralOperarion(generalOperation);
                    foreach (var operation in generalOperation.Operations.Where(p => p.Enabled))
                    {
                        Acad.Write($"расчет операции {operation.Caption}");

                        processor.SetOperation(operation);
                        operation.Processing = generalOperation;
                        operation.Execute(processor);
                    }
                }
                processor.Finish();
            }
        }

        private void UpdateFromCommands()
        {
            _toolpathCommandDictionary = Commands.Select((command, index) => new { command, index })
                .Where(p => p.command.Toolpath.HasValue)
                .GroupBy(p => p.command.Toolpath.Value)
                .ToDictionary(p => p.Key, p => p.Min(k => k.index));

            foreach (var operationGroup in Commands.GroupBy(p => p.Operation))
                operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.Toolpath).CreateGroup();
        }

        public static void SendProgram()
        {
            if (!Commands.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var machine = Settings.Machines[_camDocument.MachineCodes];
            var fileName = Acad.SaveFileDialog("Программа", machine.ProgramFileExtension, _camDocument.MachineCodes.ToString());
            if (fileName == null)
                return;
            try
            {
                var contents = Commands
                    .Select(p => $"{string.Format(machine.ProgramLineNumberFormat, p.Number)}{p.Text}")
                    .ToArray();
                File.WriteAllLines(fileName, contents);
                Acad.Write($"Создан файл {fileName}");
                //if (machineType == MachineType.CableSawing)
                //    CreateImitationProgramm(contents, fileName);
            }
            catch (Exception ex)
            {
                Acad.Alert($"Ошибка при записи файла {fileName}", ex);
            }
        }

        public static void ShowTool(Command command)
        {
            ToolObject.Set(command?.Operation.Machine, command?.Operation.Tool, command.Position, command.AngleC, command.AngleA);
        }

        public static void HideTool() => ToolObject.Hide();

        //public void PartialProcessing(ITechProcess techProcess, ProcessCommand processCommand)
        //{
        //    Acad.Write($"Выполняется формирование программы обработки по техпроцессу {techProcess.Caption} с команды номер {processCommand.Number}");
        //    techProcess.SkipProcessing(processCommand);
        //    Acad.Editor.UpdateScreen();
        //}

        /*
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
    */
    }
}
