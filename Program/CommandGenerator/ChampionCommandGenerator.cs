﻿using Autodesk.AutoCAD.DatabaseServices;
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

        protected override string GCommandText(int gCode, string paramsString, MillToolPosition position, Point3d point, Curve curve, double? angleC, double? angleA, int? feed, Point2d? center)
        {
            var par = CreateParams(position, feed);
            var textParams = GetTextParams(par);
            return $"G{gCode}{CommandDelimiter}{textParams}";
        }

        public override Dictionary<string, double?> CreateParams(MillToolPosition position, int? feed)
        {
            var angleC = position.AngleC.ToRad();
            var angleA = position.AngleA.ToRad();
            var dl = AC * (1 - Math.Cos(angleA)) + DiskRadius * Math.Sin(angleA);
            var angle = Math.PI * 3 / 2 - angleC;
            var dx = dl * Math.Cos(angle);
            var dy = dl * Math.Sin(angle);
            var dz = AC * Math.Sin(angleA) - DiskRadius * (1 - Math.Cos(angleA));

            return new Dictionary<string, double?>
            {
                ["X"] = position.Point.X + dx,
                ["Y"] = position.Point.Y + dy,
                ["Z"] = position.Point.Z + dz,
                ["A"] = position.AngleC <= 180 ? position.AngleC : (position.AngleC - 360),
                ["C"] = 90 - position.AngleA,
                ["F"] = feed
            };
        }
    }
}
