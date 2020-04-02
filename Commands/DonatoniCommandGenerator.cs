using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Donatoni
    /// </summary>
    [MachineType(MachineType.Donatoni)]
    public class DonatoniCommandGenerator : CommandGeneratorBase
    {
        protected override void StartMachineCommands(string caption)
        {
            Command($"; Donatoni \"{caption}\"");
            Command($"; DATE {DateTime.Now}");

            Command("%300");
            Command("RTCP=1");
            Command("G600 X0 Y-2500 Z-370 U3800 V0 W0 N0");
            Command("G601");
        }

        protected override void StopMachineCommands()
        {
            Command("G0 G53 Z0 ");
            Command("G0 G53 A0 C0");
            Command("G0 G53 X0 Y0");
            Command("M30", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA)
        {
            Command("G0 G53 Z0");
            Command($"G0 G53 C0 A{angleA}");
            Command("G64");
            Command("G154O10");
            Command($"T{toolNo}", "Инструмент№");
            Command("M6", "Инструмент");
            Command("G172 T1 H1 D1");
            Command("M300");
        }

        protected override void StartEngineCommands()
        {
            Command(_toolIndex == 2 ? "M8" : "M7", "Охлаждение");
            Command($"M3 S{_frequency}", "Шпиндель");
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

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            return $"G{gCode}{Format("X", point.X, _location.Point.X, _originX)}{Format("Y", point.Y, _location.Point.Y, _originY)}" +
                $"{FormatIJ("I", center?.X, _originX)}{FormatIJ("J", center?.Y, _originY)}" +
                $"{Format("Z", point.Z, _location.Point.Z, withThick: WithThick)}{Format("C", angleC, _location.AngleC)}{Format("A", angleA, _location.AngleA)}" +
                $"{Format("F", feed, _feed)}";

            string Format(string label, double? value, double oldValue, double origin = 0, bool withThick = false) =>
                 (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
                        ? $" {label}{FormatValue((value.GetValueOrDefault(oldValue) - origin).Round(4), withThick)}"
                        : null;

            string FormatIJ(string label, double? value, double origin) => value.HasValue ? $" {label}{(value.Value - origin).Round(4)}" : null;

            string FormatValue(double value, bool withThick) => withThick ? $"({value} + THICK)" : value.ToString();
        }
    }
}
