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

        public ToolInfo ToolInfo;

        public string GetProgrammLine() => $"{Number} {Text}";
    }
}