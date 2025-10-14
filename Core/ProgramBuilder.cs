using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Core;

public static class ProgramBuilder
{
    public static List<Command> DwgFileCommands { get; set; }
    public static List<Command> Commands { get; set; }

    public static void Init()
    {
        Commands ??= new List<Command>(100);
        Commands.Clear();
    }

    public static void AddCommand(short operationNumber, string text, ToolPosition toolPosition, double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
    {
        var timeSpan = new TimeSpan(0, 0, 0, (int)Math.Round(duration.GetValueOrDefault()));
        var command = new Command(Commands.Count + 1, text, toolPosition, timeSpan, toolpath1, toolpath2, operationNumber);
        Commands.Add(command);
    }

    public static Program CreateProgram(IProcessing processing, ToolpathBuilder toolpathBuilder = null)
    {
        if (DwgFileCommands != null && toolpathBuilder != null)
            Acad.Write(Commands.SequenceEqual(DwgFileCommands, Command.Comparer) ? "Программа не изменилась" : "Внимание! Программа изменена!");

        if (Commands.Count == 0)
            return null;

        var operationCommands = Commands.ToLookup(p => p.OperationNumber);
        var operationDurations = operationCommands.ToDictionary(p => p.Key, v => v.Aggregate(TimeSpan.Zero, (current, command) => current.Add(command.Duration)));
        processing.Operations.ForEach(p => p.Caption = GetCaption(p.Caption, operationDurations.TryGetAndReturn(p.Number)));
        processing.Caption = GetCaption(processing.Caption, operationDurations.Aggregate(TimeSpan.Zero, (current, p) => current.Add(p.Value)));

        var operations = operationCommands.ToDictionary(p => p.Key, p => processing.Operations.Single(op => op.Number == p.Key));

        var operationNumbers = Commands.Select((p, index) => new { p.OperationNumber, index })
            .GroupBy(x => x.OperationNumber)
            .ToDictionary(g => g.Key, g => g.Min(x => x.index));

        var objectIds = Commands.Take(100)
            .SelectMany((p, ind) => new[] { (ind, p.ObjectId), (ind, p.ObjectId2) })
            .Where(p => p.Item2.HasValue)
            .ToDictionary(p => p.Item2.Value, p => p.ind);

        var operationToolpath = toolpathBuilder != null
            ? operationCommands.Where(p => p.Any(c => c.ObjectId.HasValue))
                .ToDictionary(p => p.Key,
                    p => toolpathBuilder.CreateGroup(p.Key.ToString(),
                        p.SelectMany(c => new[] { c.ObjectId, c.ObjectId2 }).Where(c => c.HasValue).Select(c => c.Value)
                            .ToArray()))
            : null;

        return new Program(Commands, processing, operations, operationNumbers, objectIds, operationToolpath);

        static string GetCaption(string caption, TimeSpan duration) => $"{(caption.IndexOf('(') > 0 ? caption.Substring(0, caption.IndexOf('(')).Trim() : caption)} ({duration})";
    }
}