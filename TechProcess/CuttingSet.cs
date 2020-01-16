using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class CuttingSet
    {
        public List<CuttingPass> Cuttings{ get; set; }

        public Corner StartCorner { get; set; }

    }
}
