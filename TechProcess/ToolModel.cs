using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace CAM
{
    public class ToolObject
    {
        public Circle Circle0 { get; set; }

        public Circle Circle1 { get; set; }

        public Line Axis { get; set; }

        public Tool Tool { get; set; }

        public int Index { get; set; }

        public Location Location;

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
