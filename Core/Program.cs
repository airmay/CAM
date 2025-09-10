using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM.Core
{
    public static class Program
    {
        private static Command[] _commands;
        private static int _capacity = 1_000;
        private static int _count;
        public static ArraySegment<Command> ArraySegment;
        public static IProcessing Processing { get; private set; }
        public static bool IsEmpty => _count == 0;

        private static Dictionary<ObjectId, int> _objectIdDict = new Dictionary<ObjectId, int>();
        private static readonly Dictionary<object, int> _operationDict = new Dictionary<object, int>();

        public static void CreateProgram()
        {
            ArraySegment = new ArraySegment<Command>(_commands, 0, _count);
        }

        public static void Init(IProcessing processing)
        {
            Clear();
            Processing = processing;
        }

        public static void Clear()
        {
            _count = 0;
            _objectIdDict?.Clear();
            _operationDict.Clear();
            Processing = null;
        }

        public static void AddCommand(Command command)
        {
            if (_commands == null)
            {
                _commands = new Command[_capacity];
                _objectIdDict = new Dictionary<ObjectId, int>(1_000);
            }

            if (_count == _capacity)
                if (_capacity < 100_000)
                {
                    _capacity *= 10;
                    var newArray = new Command[_capacity];
                    Array.Copy(_commands, 0, newArray, 0, _commands.Length);
                    _commands = newArray;
                }
                else
                {
                    throw new Exception("Количество команд программы превысило 100 тысяч");
                }

            if (command.ObjectId.HasValue && !_objectIdDict.ContainsKey(command.ObjectId.Value))
                _objectIdDict[command.ObjectId.Value] = _count;
            if (command.ObjectId2.HasValue && !_objectIdDict.ContainsKey(command.ObjectId2.Value))
                _objectIdDict[command.ObjectId2.Value] = _count;

            if (command.Operation != null && !_operationDict.ContainsKey(command.Operation))
                _operationDict[command.Operation] = _count;

            _commands[_count++] = command;
            command.Number = _count;
        }

        public static bool TryGetCommandIndex(ObjectId objectId, out int commandIndex)
        {
            var result = _objectIdDict.TryGetValue(objectId, out var index);
            commandIndex = index;
            return result;
        }

        public static bool TryGetCommandIndex(object operation, out int commandIndex)
        {
            var result = _operationDict.TryGetValue(operation, out var index);
            commandIndex = index;
            return result;
        }

        public static void Export()
        {
            if (_count == 0)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var settings = Settings.Machines[Processing.Machine.Value];
            var fileName = Acad.SaveFileDialog("Программа", settings.ProgramFileExtension, Processing.Machine.ToString());
            if (fileName == null)
                return;
            try
            {
                var lines = ArraySegment
                    .Select(p => $"{string.Format(settings.ProgramLineNumberFormat, p.Number)}{p.Text}")
                    .ToArray();
                File.WriteAllLines(fileName, lines);
                Acad.Write($"Создан файл {fileName}");
                //if (machineType == Machine.CableSawing)
                //    CreateImitationProgramm(contents, fileName);
            }
            catch (Exception ex)
            {
                Acad.Alert($"Ошибка при записи файла {fileName}", ex);
            }
        }

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