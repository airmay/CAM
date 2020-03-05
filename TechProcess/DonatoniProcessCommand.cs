﻿using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public int ToolIndex { get; set; }

        public Location ToolLocation;

        public Curve ToolpathCurve;

        public string GetProgrammLine() => $"{Number} {Text}";
    }
}