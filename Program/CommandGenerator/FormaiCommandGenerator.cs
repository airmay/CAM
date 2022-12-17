using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Forma.
    /// </summary>
    [MachineType(MachineType.Forma)]
    public class FormaCommandGenerator : MillingCommandGenerator
    {
        public bool IsSupressMoveHome { get; set; } = false;

        protected override void StartMachineCommands(string caption)
        {
            Command("%");
            Command("N2 G17");
            Command("N3 G54.1");
            Command("N4 G66");
            Command("N5R300 = V5004/0");
            Command("N6R301 = V5004/1");
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
        }

        public override void StartEngineCommands()
        {
            Command("M08", "Охлаждение");
            Command($"M03S{_frequency}", "Шпиндель");
            Command("G4F3", "Пауза");
        }

        protected override void StopEngineCommands()
        {
            Command("M05", "Шпиндель откл.");
            Command("G4F3", "Пауза");
            Command("M09", "Охлаждение откл.");
        }

        protected override void StopMachineCommands()
        {
            Command("M02", "Конец");
        }

        public override void Pause(double duration) => Command("G4 F" + duration.ToString(CultureInfo.InvariantCulture), "Пауза", duration);

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G0{gCode}{Format("X", point.X, ToolPosition.Point.X, _originX)}{Format("Y", point.Y, ToolPosition.Point.Y, _originY)}" +
                $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
                $"{FormatZ()}{Format("C", angleC, ToolPosition.AngleC)}{Format("A", angleA, ToolPosition.AngleA)}" +
                $"{Format("F", feed, _feed)}";

            string Format(string label, double? value, double oldValue, double origin = 0) =>
                 (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
                        ? $" {label}{(value.GetValueOrDefault(oldValue) - origin).Round(4)}"
                        : null;

            string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

            string FormatZ() => (paramsString == null || paramsString.Contains("Z")) && ((ThickCommand != null && gCode == 1) || point.Z != ToolPosition.Point.Z)
                ? (WithThick ? $" Z({point.Z.Round(4)} + THICK)" : $" Z{point.Z.Round(4)}")
                : null;
        }
    }
}
