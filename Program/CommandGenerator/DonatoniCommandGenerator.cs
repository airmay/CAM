using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Donatoni
    /// </summary>
    [MachineType(MachineType.Donatoni)]
    public class DonatoniCommandGenerator : CommandGeneratorBase
    {
        public bool IsSupressMoveHome { get; set; } = false;

        protected override void StartMachineCommands(string caption)
        {
            //Command($"; Donatoni \"{caption}\"");
            //Command($"; DATE {DateTime.Now}");

            Command("%300");
            Command("RTCP=1");
            Command("G600 X0 Y-2500 Z-370 U3800 V0 W0 N0");
            Command("G601");
        }

        protected override void StopMachineCommands()
        {
            Command("G0 G53 Z0 ");
            if (!IsSupressMoveHome)
            {
                Command("G0 G53 A0 C0");
                Command("G0 G53 X0 Y0");
            }
            Command("M30", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            Command("G0 G53 Z0");
            Command($"G0 G53 C{angleC} A{angleA}");
            Command("G64");
            Command($"G154O{originCellNumber}");
            Command($"T{toolNo}", "Инструмент№");
            Command("M6", "Инструмент");
            Command("G172 T1 H1 D1");
            Command("M300");
        }

        protected override void StartEngineCommands()
        {
            Command(_hasTool ? "M7" : "M8", "Охлаждение");
            Command($"S{_frequency}", "Шпиндель");
            Command($"M3", "Шпиндель");
        }

        protected override void StopEngineCommands()
        {
            Command("M5", "Шпиндель откл.");
            Command("M9", "Охлаждение откл.");
            Command("G61");
            Command("G153");
            Command("G0 G53 Z0");
            Command("SETMSP=1");
        }

        public override void Pause(double duration) => Command("G4 F" + duration.ToString(CultureInfo.InvariantCulture), "Пауза", duration);

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G{gCode}{Format("X", point.X, ToolLocation.Point.X, _originX)}{Format("Y", point.Y, ToolLocation.Point.Y, _originY)}" +
                $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
                $"{FormatZ()}{Format("C", angleC, ToolLocation.AngleC)}{Format("A", angleA, ToolLocation.AngleA)}" +
                $"{Format("F", feed, _feed)}";

            string Format(string label, double? value, double oldValue, double origin = 0) =>
                 (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
                        ? $" {label}{(value.GetValueOrDefault(oldValue) - origin).Round(4)}"
                        : null;

            string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

            string FormatZ() => (paramsString == null || paramsString.Contains("Z")) && ((ThickCommand != null && gCode == 1) || point.Z != ToolLocation.Point.Z)
                ? (WithThick ? $" Z({point.Z.Round(4)} + THICK)" : $" Z{point.Z.Round(4)}")
                : null;
        }
    }
}
