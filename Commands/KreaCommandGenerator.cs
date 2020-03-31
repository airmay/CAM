using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Krea
    /// </summary>
    [MachineType(MachineType.Krea)]
    public class KreaCommandGenerator : CommandGeneratorBase
    {
        protected override void StartMachineCommands(string caption)
        {
            Command($"; Krea \"{caption}\"");
            Command($"; DATE {DateTime.Now}");
            Command("(UAO,E30)");
            Command("(UIO,Z(E31))");
        }

        protected override void StopMachineCommands()
        {
            Command("G0 G79 Z(@ZUP)", "Подъем");
            Command("G0 Y300");
            Command("M30", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA)
        {
            Command($"T1", "Инструмент№");
            Command("G17", "Плоскость");
            Command("G79Z(@ZUP)");
        }

        protected override void StartEngineCommands()
        {
            Command("M7", "Охлаждение");
            Command("M8", "Охлаждение");
            Command($"M3 S{_frequency}", "Шпиндель");
        }

        protected override void StopEngineCommands()
        {
            Command("M5", "Шпиндель откл.");
            Command("M9 M10", "Охлаждение откл.");
        }

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed)
        {
            var IJ = "";
            if (curve is Arc arc)
            {
                gCode = point == arc.EndPoint ? 2 : 3;
                IJ = $" I{(arc.Center.X - _originX).Round(4)} J{(arc.Center.Y - _originY).Round(4)}";
            }
            return $"G{gCode}{Format("X", point.X, _location.Point.X, _originX)}{Format("Y", point.Y, _location.Point.Y, _originY)}{IJ}" +
                $"{Format("Z", point.Z, _location.Point.Z, withThick: WithThick)}{Format("C", angleC, _location.AngleC)}{Format("A", angleA, _location.AngleA)}" +
                $"{Format("F", feed, _feed)}";

            string Format(string label, double? value, double oldValue, double origin = 0, bool withThick = false) =>
                 (paramsString == null || paramsString.Contains(label)) && (value.GetValueOrDefault(oldValue) != oldValue) // || (_GCode == 0 ^ gCode == 0))
                        ? $" {label}{FormatValue((value.GetValueOrDefault(oldValue) - origin).Round(4), withThick)}"
                        : null;

            string FormatValue(double value, bool withThick) => withThick ? $"({value} + THICK)" : value.ToString();
        }
    }
}
