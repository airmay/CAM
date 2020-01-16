using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class CuttingParams
    {
        public Curve Curve { get; set; }

        public List<CuttingMode> CuttingModes { get; set; }

        public bool IsZeroPass { get; set; }

        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public Side ToolSide { get; set; }
        
        public int DepthAll { get; set; }

        public List<KeyValuePair<Curve, int>> ToolpathList { get; set; }

        public Corner StartCorner { get; set; }

        public int Transition { get; set; }

    }
}