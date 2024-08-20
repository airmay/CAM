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
        public static ProcessingView ProcessingView = new ProcessingView();
        public static List<Command> Commands;
        private static ToolObject ToolObject { get; } = new ToolObject();
        private static Processing _processing;

        public static void SetDocument(CamDocument camDocument)
        {
            if (_camDocument != null)
                _camDocument.Processings = ProcessingView.GetProcessings();
            _camDocument = camDocument;
            ToolObject.Hide();
            Commands?.Clear();
            ProcessingView.CreateTree(camDocument.Processings);
            Acad.ClearHighlighted();
        }

        public static void RemoveDocument()
        {
            _camDocument = null;
            ProcessingView.ClearView();
        }

        public static void SaveDocument()
        {
            DeleteGenerated();
            _camDocument.Save(ProcessingView.GetProcessings());

            ProcessingView.ClearCommandsView();
            Acad.DeleteAll();
        }

        public static List<Command> ExecuteProcessing(Processing processing)
        {
            DeleteGenerated();
            Acad.Editor.UpdateScreen();
            _processing = processing;
            Acad.Write($"Выполняется расчет обработки {processing.Caption}");
            Acad.CreateProgressor("Расчет обработки");
            try
            {
                var stopwatch = Stopwatch.StartNew();
                processing.Execute();
                if (CamManager.Commands != null)
                    UpdateFromCommands();
                stopwatch.Stop();
                Acad.Write($"Расчет обработки  {processing.Caption} завершен {stopwatch.Elapsed}");
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
            //Acad.Editor.Regen();//UpdateScreen();
            return Commands;
        }

        private static void DeleteGenerated()
        {
            ToolObject.Hide();
            _processing?.RemoveAcadObjects();
            Commands = null;
        }

        private static Dictionary<ObjectId, int> _toolpathCommandDictionary;

        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathId() is ObjectId id && _toolpathCommandDictionary.TryGetValue(id, out var commandIndex))
                ProcessingView.SelectProcessCommand(commandIndex);
        }

        private static void UpdateFromCommands()
        {
            _toolpathCommandDictionary = Commands.Select((command, index) => new { command, index })
                .Where(p => p.command.Toolpath.HasValue)
                .GroupBy(p => p.command.Toolpath.Value)
                .ToDictionary(p => p.Key, p => p.Min(k => k.index));

            foreach (var operationGroup in Commands.Where(p => p.Operation != null).GroupBy(p => p.Operation))
                operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.Toolpath).CreateGroup();
        }

        public static void ShowTool(Command command)
        { 
            ToolObject.Set(command?.Operation?.Processing.Machine, command?.Operation?.Tool, command.Position, command.AngleC, command.AngleA);
        }

        public static void SendProgram()
        {
            if (!Commands.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var machine = Settings.Machines[_processing.Machine.Value];
            var fileName = Acad.SaveFileDialog("Программа", machine.ProgramFileExtension, _processing.Machine.ToString());
            if (fileName == null)
                return;
            try
            {
                var contents = Commands
                    .Select(p => $"{string.Format(machine.ProgramLineNumberFormat, p.Number)}{p.Text}")
                    .ToArray();
                File.WriteAllLines(fileName, contents);
                Acad.Write($"Создан файл {fileName}");
                //if (machineType == Machine.CableSawing)
                //    CreateImitationProgramm(contents, fileName);
            }
            catch (Exception ex)
            {
                Acad.Alert($"Ошибка при записи файла {fileName}", ex);
            }
        }

        //public static void HideTool() => ToolObject.Hide();

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
