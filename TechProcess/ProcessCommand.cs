using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string GCode { get; set; }

        public string MCode { get; set; }

        public string Axis { get; set; }

        public string Feed { get; set; }

        public string X { get; set; }

        public string Y { get; set; }

        public string Param1 { get; set; }

        public string Param2 { get; set; }

        /// <summary>
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public Curve ToolpathCurve;

        public Point3d? EndPoint;

        public double? ToolAngle;

        public string GetProgrammLine() => $"{Number};{GCode};{MCode};{Axis};{Feed};{X};{Y};{Param1};{Param2};";

    }
}
