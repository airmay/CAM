using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM;

public abstract class PostProcessorBase
{
    public Point2d Origin { get; set; }

    protected readonly record struct CommandParam(char Code, string Value)
    {
        public readonly char Code = Code;
        public readonly string Value = Value;

        public override string ToString() => $"{Code}{Value}";
    }

    #region GCommand
    public Dictionary<char, string> Params { get; } = new();

    protected virtual string GetGCommand(List<CommandParam> commandParams)
    {
        var changed = GetChangedParams(commandParams);
        ApplyParams(changed);
        var command = CreateCommand(changed);
        return command;
    }

    protected virtual List<CommandParam> GetChangedParams(List<CommandParam> commandParams)
    {
        return commandParams.FindAll(p => p.Value != null && (!Params.TryGetValue(p.Code, out var value) || value != p.Value));
    }

    protected virtual string CreateCommand(List<CommandParam> changed)
    {
        return string.Join(" ", changed);
    }

    protected virtual void ApplyParams(List<CommandParam> changed)
    {
        foreach (var param in changed)
            Params[param.Code] = param.Value;
    }
    #endregion

    public abstract string[] StartMachine();
    public abstract string[] StopMachine();
    public abstract string[] StopEngine();
    public abstract string Pause(double duration);
}