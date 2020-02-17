using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public Curve ToolpathCurve;

        public Point3d? EndPoint;

        public double? ToolAngle;

        public string GetProgrammLine() => $"{Number} {Text}";
    }
}