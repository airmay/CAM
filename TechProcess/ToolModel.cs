using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM
{
    public class ToolModel
    {
        public Circle Circle0 { get; set; }

        public Circle Circle1 { get; set; }

        public Line Axis { get; set; }

        //public Point3d Origin { get; set; }

        //public double AngleC { get; set; }

        //public double AngleA { get; set; }

        public ToolPosition ToolPosition { get; set; }

        public IEnumerable<Curve> GetCurves()
        {
            yield return Circle0;
            yield return Circle1;
            yield return Axis;
        }
    }
}
