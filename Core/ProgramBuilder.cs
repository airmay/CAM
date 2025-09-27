using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;

namespace CAM.Core;

public class ProgramBuilder
{
    private static Command[] _commands;
    private int _count;

    public ProgramBuilder() { }

    public ProgramBuilder(Command[] commands) => _commands = commands;

    public void AddCommand(short operationNumber, string text, ToolPosition toolPosition, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
    {
        _commands ??= new Command[100];
        if (_count == _commands.Length)
        {
            if (_count > 100_000)
                throw new Exception("Количество команд программы превысило 100 тысяч");

            var newArray = new Command[_commands.Length * 5];
            Array.Copy(_commands, newArray, _commands.Length);
            _commands = newArray;
        }

        _commands[_count++] = new Command
        {
            Number = _count + 1,
            Text = text,
            ToolPosition = toolPosition,
            Duration = new TimeSpan(0, 0, 0, (int)Math.Round(duration.GetValueOrDefault())),
            ObjectId = toolpath1,
            ObjectId2 = toolpath2,
            OperationNumber = operationNumber,
        };
    }

    public void AddCommandsFromPosition(int position, int count)
    {
        Array.Copy(_commands, position, _commands, _count, count);
        _count += count;
    }

    public Program CreateProgram(IProcessing processing, string programFileExtension, ToolpathBuilder toolpathBuilder = null)
    {
        if (_count == 0)
            return null;

        var arraySegment = new ArraySegment<Command>(_commands, 0, _count);
        var program = new Program(arraySegment, processing, programFileExtension);

        program.OperationNumbers = arraySegment.Select((p, index) => new { p.OperationNumber, index })
            .GroupBy(x => x.OperationNumber)
            .ToDictionary(g => g.Key, g => g.Min(x => x.index));

        program.ObjectIds = arraySegment.Take(100)
            .SelectMany((p, ind) => new[] { (ind, p.ObjectId), (ind, p.ObjectId2) })
            .Where(p => p.Item2.HasValue)
            .ToDictionary(p => p.Item2.Value, p => p.ind);

        var operationCommands = arraySegment.ToLookup(p => p.OperationNumber);

        if (toolpathBuilder != null)
            program.OperationToolpath = operationCommands.ToDictionary(p => p.Key,
                p => toolpathBuilder.CreateGroup(p.Key.ToString(),
                    p.SelectMany(c => new[] { c.ObjectId, c.ObjectId2 }).Where(c => c.HasValue).Select(c => c.Value).ToArray()));

        var operationDurations = operationCommands.ToDictionary(p => p.Key, v => v.Aggregate(TimeSpan.Zero, (current, command) => current.Add(command.Duration)));
        processing.Operations.ForEach(p => p.Caption = GetCaption(p.Caption, operationDurations.TryGetAndReturn(p.Number)));
        processing.Caption = GetCaption(processing.Caption, operationDurations.Aggregate(TimeSpan.Zero, (current, p) => current.Add(p.Value)));

        return program;

        static string GetCaption(string caption, TimeSpan duration) => $"{(caption.IndexOf('(') > 0 ? caption.Substring(0, caption.IndexOf('(')).Trim() : caption)} ({duration})";
    }
}