using Autodesk.AutoCAD.Geometry;

namespace CAM.Core
{
    public struct ToolLocationParams
    {
        public ToolLocationParams(double p1, double p2, double p3, double p4, double p5)
        {
            Param1 = p1;
            Param2 = p2;
            Param3 = p3;
            Param4 = p4;
            Param5 = p5;
        }

        public double Param1 { get; }
        public double Param2 { get; }
        public double Param3 { get; }
        public double Param4 { get; }
        public double Param5 { get; }
    }
}
