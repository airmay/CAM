using CAM.CncWorkCenter;

namespace CAM;

public class KreaPostProcessor : PostProcessorCnc
{
    public override string[] StartMachine()
    {
        return
        [
            "(UAO,E30)",
            "(UIO,Z(E31))",
        ];
    }

    public override string[] StopMachine()
    {
        return
        [
            "G0 G79 Z(@ZUP)",
            "M30"
        ];
    }

    public override string[] StartEngine(int frequency, bool hasTool)
    {
        return
        [
            "M7",
            "M8",
            $"S{frequency}",
            "M3",
        ];
    }

    public override string[] StopEngine()
    {
        return
        [
            "M5",
            "M9 M10"
        ];
    }

    public override string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
    {
        return
        [
            "T1",
            "G17",
            "G79Z(@ZUP)"
        ];
    }

    public override string Pause(double duration) => $"(DLY,{duration}";

    //protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
    //{
    //    return $"G{gCode}{Format("X", point.X, ToolPosition.Point.X, _originX)}{Format("Y", point.Y, ToolPosition.Point.Y, _originY)}" +
    //        $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
    //        $"{Format("Z", point.Z, ToolPosition.Point.Z, withThick: WithThick)}{Format("C", angleC, ToolPosition.AngleC)}{Format("A", angleA, ToolPosition.AngleA)}" +
    //        $"{Format("F", feed, _feed)}";

    //    string Format(string label, double? value, double oldValue, double origin = 0, bool withThick = false) =>
    //         (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
    //                ? $" {label}{FormatValue((value.GetValueOrDefault(oldValue) - origin).Round(4), withThick)}"
    //                : null;

    //    string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

    //    string FormatValue(double value, bool withThick) => withThick ? $"({value} + THICK)" : value.ToString();
    //}
}