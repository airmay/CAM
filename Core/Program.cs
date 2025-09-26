using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM.Core
{
    public class Program
    {
        public static Command[] DwgFileCommands { get; set; }
        private static Command[] _commands;

        public int Count { get; set; }
        public ArraySegment<Command> ArraySegment { get; private set; }
        public IProcessing Processing { get; }
        public Dictionary<short, IOperation> Operations { get; } = new Dictionary<short, IOperation>();

        public readonly string ProgramFileExtension;
        private Dictionary<short, int> _operationNumberDict;
        private Dictionary<ObjectId, int> _objectIdDict;
        private Dictionary<short, ObjectId> _operationToolpath;

        public Program(IProcessing processing, string programFileExtension)
        {
            Processing = processing;
            ProgramFileExtension = programFileExtension;
        }

        public Program(IProcessing processing, string programFileExtension, Command[] commands) : this(processing, programFileExtension)
        {
            var count = 1000;
            Count = commands.Length;
            if (Count >= 1000)
            {
                var digits = (int)Math.Floor(Math.Log10(Count)) + 1;
                count = (int)Math.Pow(10, digits);
            }

            _commands = new Command[count];
            Array.Copy(commands, 0, _commands, 0, Count);
            CreateProgram();
        }

        public static Command GetCommand(int index) => _commands[index];

        public void CreateProgram(ToolpathBuilder toolpathBuilder = null)
        {
            ArraySegment = new ArraySegment<Command>(_commands, 0, Count);

            _operationNumberDict = ArraySegment.Select((p, index) => new { p.OperationNumber, index })
                .GroupBy(x => x.OperationNumber)
                .ToDictionary(g => g.Key, g => g.Min(x => x.index));

            _objectIdDict = ArraySegment.Take(100)
                .SelectMany((p, ind) => new[] { (ind, p.ObjectId), (ind, p.ObjectId2) })
                .Where(p => p.Item2.HasValue)
                .ToDictionary(p => p.Item2.Value, p => p.ind);

            var operationCommands = ArraySegment.ToLookup(p => p.OperationNumber);

            if (toolpathBuilder != null)
                _operationToolpath = operationCommands.ToDictionary(p => p.Key,
                    p => toolpathBuilder.CreateGroup(p.Key.ToString(),
                        p.SelectMany(c => new[] { c.ObjectId, c.ObjectId2 }).Where(c => c.HasValue).Select(c => c.Value).ToArray()));

            var operationDurations = operationCommands.Select(p =>
                    (OperationNumber: p.Key, Duration: p.Aggregate(TimeSpan.Zero, (current, command) => current.Add(command.Duration)))).ToList();
            operationDurations.ForEach(p =>
                Operations[p.OperationNumber].Caption = GetCaption(Operations[p.OperationNumber].Caption, p.Duration));
            Processing.Caption = GetCaption(Processing.Caption,
                operationDurations.Aggregate(TimeSpan.Zero, (current, p) => current.Add(p.Duration)));

            return;

            string GetCaption(string caption, TimeSpan duration)
            {
                var ind = caption.IndexOf('(');
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({duration})";
            }
        }

        public void AddCommand(IOperation operation, string text, ToolPosition toolPosition, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
        {
            AddCommand(new Command
            {
                Text = text,
                ToolPosition = toolPosition,
                Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.GetValueOrDefault())),
                ObjectId = toolpath1,
                ObjectId2 = toolpath2,
                OperationNumber = operation.Number,
            });

            if (!Operations.ContainsKey(operation.Number))
                Operations.Add(operation.Number, operation);
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

            _commands[Count] = command;
            command.Number = ++Count;
        }

        public void AddCommandsFromPosition(int position, int count)
        {
            Array.Copy(_commands, position, _commands, Count, count);
            Count += count;
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

        public void SetToolpathVisibility(bool value) => _operationToolpath?.ForAll(p => p.Value.SetGroupVisibility(value));

        public void ShowOperationToolpath(IOperation operation) => _operationToolpath?.ForAll(p => p.Value.SetGroupVisibility(p.Key == operation.Number));

        public void Export()
        {
            if (Count == 0)
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var fileName = Acad.SaveFileDialog("program", ProgramFileExtension, "Экспорт программы в файл");
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