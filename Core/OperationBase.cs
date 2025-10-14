using System;
using CAM.CncWorkCenter;

namespace CAM;

public interface IOperation
{
    string Caption { get; set; }
    bool Enabled { get; set; }
    short Number { get; set; }
    Tool GetTool();
    AcadObject ProcessingArea { get; }
}

[Serializable]
public abstract class OperationBase<TTechProcess, TProcessor> : IOperation
    where TTechProcess : ProcessingBase<TTechProcess, TProcessor>
    where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
{
    [NonSerialized] public TTechProcess Processing;
    protected TProcessor Processor => Processing.Processor;

    public string Caption { get; set; }
    public bool Enabled { get; set; }
    public short Number { get; set; }
    public AcadObject ProcessingArea { get; set; }

    public Tool Tool { get; set; }
    public Tool GetTool() => Tool ?? Processing?.Tool;
    public double ToolDiameter => GetTool().Diameter;
    public double ToolThickness => GetTool().Thickness.Value;

    public abstract void Execute();
}