using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CAM
{
    /// <summary>
    /// Генератор команд для станка типа Champion
    /// </summary>
    [MachineType(MachineType.Champion)]
    public class ChampionCommandGenerator : MillingCommandGenerator
    {
        public bool IsSupressMoveHome { get; set; } = false;

        protected override void StartMachineCommands(string caption)
        {
            Command("G90");
            Command("#MCS G0Z0.0");
            Command("C90 A0");
            Command("V.A.ORGT[1].Z=190");
            Command("#FLUSH");
            Command("G54");
            Command("M22");
        }

        protected override void StopMachineCommands()
        {
            Command("#MCS G0Z0.0");
            Command("M23");
            Command("D0");
            Command("#MCS G0Z0.0");
            Command("#MCS G0Y-1");
            Command("M30", "Конец");
        }

        protected override void SetToolCommands(int toolNo, double angleA, double angleC, int originCellNumber)
        {
            Command($"T{toolNo}D1", "Инструмент№");
            Command("M6", "Смена инструмента");
        }

        public override void StartEngineCommands()
        {
            Command($"S{_frequency}", "Шпиндель");
            Command($"M3", "Шпиндель");
        }

        protected override void StopEngineCommands()
        {
            Command("M05", "Шпиндель откл.");
        }

        public override void Pause(double duration) => Command("G4 F" + duration.ToString(CultureInfo.InvariantCulture), "Пауза", duration);

        public override Dictionary<string, double?> CreateParams(MillToolPosition position, int? feed, Point2d? center)
        {
            var newParams = base.CreateParams(position, feed, center);

            newParams["A"] = position.AngleC < 180 ? -position.AngleC : (360 - position.AngleC);
            newParams["C"] = 90 - position.AngleA;

            if (position.X.HasValue || position.Y.HasValue || position.Z.HasValue)
            {
                var angleC = position.AngleC.ToRad();
                var angleA = position.AngleA.ToRad();
                var dl = AC * (1 - Math.Cos(angleA)) + Tool.Diameter / 2 * Math.Sin(angleA);
                var angle = Math.PI * 3 / 2 - angleC;
                if (position.X.HasValue)
                    newParams["X"] = position.X.Value + dl * Math.Cos(angle) + AC_V * Math.Sin(angleC);
                if (position.Y.HasValue)
                    newParams["Y"] = position.Y.Value + dl * Math.Sin(angle) + AC_V * Math.Cos(angleC); ;
                if (position.Z.HasValue)
                    newParams["Z"] = position.Z.Value + AC * Math.Sin(angleA) - Tool.Diameter / 2 * (1 - Math.Cos(angleA)) + DZ;
            }

            return newParams;
        }

        protected override string GCommandText(int gCode, string paramsString, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            throw new NotImplementedException();
        }
    }
}
