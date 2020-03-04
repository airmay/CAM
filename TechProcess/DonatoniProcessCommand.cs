using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public Curve ToolpathCurve { get; set; }

        public int ToolIndex { get; set; }

        public Location ToolLocation;

        public string GetProgrammLine() => $"{Number} {Text}";
    }
}