using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace CAM
{
    public class ToolObject
    {
        public Circle Circle0 { get; set; }

        public Circle Circle1 { get; set; }

        public Line Axis { get; set; }

        //public Point3d Origin { get; set; }

        //public double AngleC { get; set; }

        //public double AngleA { get; set; }

        public ToolInfo ToolInfo;

        public IEnumerable<Curve> GetCurves()
        {
            if (Circle0 != null)
                yield return Circle0;
            if (Circle1 != null)
                yield return Circle1;
            if (Axis != null)
                yield return Axis;
        }
    }
}
