using CAM.CncWorkCenter;

namespace CAM;

public class FormaPostProcessor : PostProcessorCnc
{
    public override string[] StartMachine()
    {
        return
        [
            "%",
            "N2 G17",
            "N3 G54.1",
            "N4 G66",
            "N5R300 = V5004/0",
            "N6R301 = V5004/1"
        ];
    }

    public override string[] StopMachine()
    {
        return
        [
            "M02"
        ];
    }

    public override string[] StartEngine(int frequency, bool hasTool)
    {
        return
        [
            "M08",
            $"M03S{frequency}",
            "G4F3",
        ];
    }

    public override string[] StopEngine()
    {
        return
        [
            "M05",
            "G4F3",
            "M09"
        ];
    }

    public override string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber)
    {
        return
        [
        ];
    }

    //protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
    //{
    //    return $"G0{gCode}{Format("X", point.X, ToolPosition.Point.X, _originX)}{Format("Y", point.Y, ToolPosition.Point.Y, _originY)}" +
    //        $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
    //        $"{FormatZ()}{Format("C", angleC, ToolPosition.AngleC)}{Format("A", angleA, ToolPosition.AngleA)}" +
    //        $"{Format("F", feed, _feed)}";

    //    string Format(string label, double? value, double oldValue, double origin = 0) =>
    //         (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
    //                ? $" {label}{(value.GetValueOrDefault(oldValue) - origin).Round(4)}"
    //                : null;

    //    string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

    //    string FormatZ() => (paramsString == null || paramsString.Contains("Z")) && ((ThickCommand != null && gCode == 1) || point.Z != ToolPosition.Point.Z)
    //        ? (WithThick ? $" Z({point.Z.Round(4)} + THICK)" : $" Z{point.Z.Round(4)}")
    //        : null;
    //}
}