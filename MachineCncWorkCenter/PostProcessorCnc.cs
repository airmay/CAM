using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using CAM.Core.Processing;
using CAM.Utils;

namespace CAM.MachineCncWorkCenter;

public abstract class PostProcessorCnc : PostProcessorBase
{
    public virtual string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
    {
        Params['C'] = angleC.ToParam();
        Params['A'] = angleA.ToParam();

        return null;
    }

    public virtual string Cycle() => null;
    public bool WithThick { get; set; }
    
    public abstract string[] StartEngine(int frequency, bool hasTool);

    public override string Pause(double duration) => $"G4 F{duration}";

    public string GCommand(Point3d? point, double? angleC, double? angleA, int? feed, Point2d? arcCenter)
    {
        var @params = GetParams(point, angleC, angleA, feed, arcCenter);
        return GetGCommand(@params);
    }

    protected virtual List<CommandParam> GetParams(Point3d? point, double? angleC, double? angleA, int? feed, Point2d? arcCenter)
    {
        var commandParams = new List<CommandParam>();

        if (point.HasValue)
        {
            commandParams.Add(new('X', (point.Value.X - Origin.X).ToParam()));
            commandParams.Add(new('Y', (point.Value.Y - Origin.Y).ToParam()));
            var zParam = point.Value.Z.ToParam();
            if (WithThick)
                zParam = $"({zParam} + THICK)";
            commandParams.Add(new('Z', zParam));
        }

        if (arcCenter.HasValue)
        {
            commandParams.Add(new('I', (arcCenter.Value.X - Origin.X).ToParam()));
            commandParams.Add(new('J', (arcCenter.Value.Y - Origin.Y).ToParam()));
        }

        commandParams.Add(new CommandParam('C', angleC.ToParam()));
        commandParams.Add(new CommandParam('A', angleA.ToParam()));
        commandParams.Add(new CommandParam('F', feed?.ToString()));

        return commandParams;
    }
}