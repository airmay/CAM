using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM.Core
{
    public class Program
    {
        private static Command[] _commands;

        public int Count { get; set; }
        public ArraySegment<Command> ArraySegment { get; private set; }
        public IProcessing Processing { get; }

        private readonly string _programFileExtension;
        private Dictionary<short, int> _operationNumberDict;
        private Dictionary<ObjectId, int> _objectIdDict;

        public Program(IProcessing processing, string programFileExtension)
        {
            Processing = processing;
            _programFileExtension = programFileExtension;
        }

        public void CreateProgram()
        {
            ArraySegment = new ArraySegment<Command>(_commands, 0, Count);
            _operationNumberDict = ArraySegment.Select((p, index) => new { p.OperationNumber, index })
                .GroupBy(x => x.OperationNumber)
                .ToDictionary(g => g.Key, g => g.Min(x => x.index));
            _objectIdDict = ArraySegment.Take(100)
                .SelectMany((p, ind) => new[] { (ind, p.ObjectId), (ind, p.ObjectId2) })
                .Where(p => p.Item2.HasValue)
                .ToDictionary(p => p.Item2.Value, p => p.ind);
        }

        public void AddCommand(Command command)
        {
            if (_commands == null)
                _commands = new Command[1000];

            if (Count == _commands.Length)
            {
                if (Count == 100_000)
                    throw new Exception("Количество команд программы превысило 100 тысяч");

                var newArray = new Command[_commands.Length * 10];
                Array.Copy(_commands, 0, newArray, 0, _commands.Length);
                _commands = newArray;
            }

            _commands[Count++] = command;
            command.Number = Count;
        }

        public Command[] GetCommands()
        {
            var copy = new Command[Count];
            Array.Copy(_commands, 0, copy, 0, Count);
            return copy;
        }

        public bool TryGetCommandIndexByObjectId(ObjectId objectId, out int commandIndex)
        {
            var result = _objectIdDict.TryGetValue(objectId, out var index);
            commandIndex = index;
            return result;
        }

        public bool TryGetCommandIndexByOperationNumber(short operationNumber, out int commandIndex)
        {
            var result = _operationNumberDict.TryGetValue(operationNumber, out var index);
            commandIndex = index;
            return result;
        }


        public void Export()
        {
            if (Count == 0)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var fileName = Acad.SaveFileDialog("program", _programFileExtension, "Экспорт программы в файл");
            if (fileName == null)
                return;

            var contents = ArraySegment.Select(p => $"N{p.Number} {p.Text}").ToArray();
            try
            {
                File.WriteAllLines(fileName, contents);
                Acad.Write($"Создан файл {fileName}");
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