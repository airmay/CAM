using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CAM
{
    public class Processing
    {
        public int Hash;
        public GeneralOperation[] GeneralOperations { get; set; }
        public Command[] Commands { get; set; }
        public MachineType MachineType { get; set; }

        private Dictionary<ObjectId, int> _toolpathCommandDictionary;

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
                if (Commands != null)
                    UpdateFromCommands();

                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
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
            foreach (var generalOperation in GeneralOperations)
            foreach (var operation in generalOperation.Operations)
            {
                operation.ToolpathGroup?.DeleteGroup();
                operation.ToolpathGroup = null;
                operation.SupportGroup?.DeleteGroup();
                operation.SupportGroup = null;
            }

            Commands = null;
        }

        private void BuildProcessing()
        {
            var generalParams = GeneralOperations.First(p => p.Enabled);
            var machineType = generalParams.MachineType;
            if (!machineType.CheckNotNull("Станок"))
                return;
            MachineType = machineType.Value;
            var tool = generalParams.Tool;
            if (!tool.CheckNotNull("Инструмент"))
                return;

            using (var processor = ProcessorFactory.Create(MachineType))
            {
                processor.Start(tool);

                foreach (var generalOperation in GeneralOperations.Where(p => p.Enabled))
                {
                    processor.SetGeneralOperarion(generalOperation);
                    foreach (var operation in generalOperation.Operations.Where(p => p.Enabled))
                    {
                        Acad.Write($"расчет операции {operation.Caption}");

                        processor.SetOperarion(operation);
                        operation.SetGeneralParams(generalOperation);
                        operation.Execute(processor);
                    }
                }

                processor.Finish();
                Commands = processor.ProcessCommands.ToArray();
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

        public int? GetCommandIndex(ObjectId id)
        {
            return _toolpathCommandDictionary.TryGetValue(id, out var value) ? (int?)value : null;
        }

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
