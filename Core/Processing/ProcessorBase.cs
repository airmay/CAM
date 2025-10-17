using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;
using System;
using System.Linq;

namespace CAM.CncWorkCenter;

public abstract class ProcessorBase<TTechProcess, TProcessor>
    where TTechProcess : TechProcessBase<TTechProcess, TProcessor>
    where TProcessor : ProcessorBase<TTechProcess, TProcessor>, new()
{
    protected abstract PostProcessorBase PostProcessor { get; }
    public TTechProcess TechProcess { get; set; }
    public IOperation Operation { get; set; }
    protected Tool Tool;
    protected bool IsEngineStarted;
    public double UpperZ;
    public bool IsUpperTool => ToolPoint.Z + 0.1 > UpperZ;
    public Point3d ToolPoint;
    public double AngleC, AngleA;

    protected abstract void CreatePostProcessor();

    public virtual void Start()
    {
        CreatePostProcessor();
        PostProcessor.Origin = TechProcess.Origin.Point;

        ProgramBuilder.Init();
        ProcessingObjectBuilder.Start();

        Operation = null;
        Tool = null;
        IsEngineStarted = false;
        AngleA = 0;
        AngleC = 0;
    }

    public virtual void StartOperation(double? zMax = null)
    {
        if (!IsEngineStarted)
        {
            if (zMax.HasValue)
                UpperZ = zMax.Value + TechProcess.ZSafety;
            ToolPoint = TechProcess.Origin.Point.WithZ(UpperZ + TechProcess.ZSafety * 5);
            AddCommands(PostProcessor.StartMachine());
        }
    }

    public void Stop()
    {
        AddCommands(PostProcessor.StopEngine());
        AddCommands(PostProcessor.StopMachine());
    }

    public Program CreateProgram()
    {
        ProcessingObjectBuilder.Stop();
        if (ProgramBuilder.DwgFileCommands != null)
            Acad.Write(ProgramBuilder.Commands.SequenceEqual(ProgramBuilder.DwgFileCommands, Command.Comparer)
                ? "Программа не изменилась"
                : "Внимание! Программа изменена!");

        //for (var i = 0; i < ProgramBuilder.Commands.Count; i++)
        //{
        //    if (!Command.Comparer.Equals(ProgramBuilder.Commands[i], ProgramBuilder.DwgFileCommands[i]))
        //        Acad.Write($"Изменена строка {ProgramBuilder.Commands[i].Number}");
        //}

        return ProgramBuilder.CreateProgram(TechProcess);
    }

    public void Pause(double duration) => AddCommand(PostProcessor.Pause(duration));

    protected void AddCommand(string text, Point3d? point = null, double? angleC = null, double? angleA = null,
        double? duration = null, ObjectId? toolpath1 = null, ObjectId? toolpath2 = null)
    {
        ToolPoint = point ?? ToolPoint;
        AngleC = angleC ?? AngleC;
        AngleA = angleA ?? AngleA;

        ProgramBuilder.AddCommand(Operation.Number, text, new ToolPosition(ToolPoint, AngleC, AngleA), duration, toolpath1, toolpath2);
    }

    protected void AddCommands(string[] commands) => Array.ForEach(commands, p => AddCommand(p));

    public Program PartialProgram(int programPosition, IOperation operation, ToolPosition toolPosition)
    {
        var commands = ProgramBuilder.Commands.Skip(programPosition).ToArray();
        Start();
        Operation = operation;
        StartOperation();
        MoveToPosition(toolPosition);
        ProgramBuilder.Commands.AddRange(commands);

        return CreateProgram();
    }

    protected abstract void MoveToPosition(ToolPosition position);
}