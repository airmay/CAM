using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class CuttingPass
    {
        public Curve Toolpath { get; set; }

        public int Feed { get; set; }

        public CuttingPass(Curve toolpath, int feed = 0)
        {
            Toolpath = toolpath;
            Feed = feed;
        }
    }
}
