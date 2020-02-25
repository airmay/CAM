using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public Curve ToolpathCurve { get; set; }

        public ToolPosition ToolPosition { get; set; }

        public string GetProgrammLine() => $"{Number} {Text}";
    }

    public struct ToolPosition
    {
        public Point3d Point { get; set; }
        public double AngleC { get; set; }
        public double AngleA { get; set; }

        public void Set(Point3d? point, double? angleC, double? angleA)
        {
            Point = point ?? Point;
            AngleC = angleC ?? AngleC;
            AngleA = angleA ?? AngleA;
        }

        //public ToolPosition Clone() => (ToolPosition)this.MemberwiseClone();
    }
}