using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM.CncWorkCenter;

public abstract class PostProcessorCnc : PostProcessorBase
{
    public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);
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
        var commandParams = new List<CommandParam>
        {
            new('F', feed?.ToString()),
            new('A', angleA.ToParam()),
            new('C', angleC.ToParam())
        };

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

        return commandParams;
    }
}