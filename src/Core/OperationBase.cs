using System;
using CAM.Autocad;
using CAM.Core.Processing;
using CAM.Core.Tools;

namespace CAM.Core;

[Serializable]
public abstract class OperationBase<TTechProcess, TProcessor> : IOperation
    where TTechProcess : TechProcessBase<TTechProcess, TProcessor>
    where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
{
    [NonSerialized] public TTechProcess TechProcess;
    protected TProcessor Processor => TechProcess.Processor;

    public string Caption { get; set; }
    public bool Enabled { get; set; }
    public short Number { get; set; }
    public AcadObject ProcessingArea { get; set; }

    public Tool Tool { get; set; }
    public Tool GetTool() => Tool ?? TechProcess?.Tool;
    public double ToolDiameter => GetTool().Diameter;
    public double ToolThickness => GetTool().Thickness.Value;
    public double Delta => TechProcess.Delta;

    public abstract void Execute();
}