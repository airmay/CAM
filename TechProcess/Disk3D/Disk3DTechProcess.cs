using System;

namespace CAM.Disk3D
{
    [Serializable]
    [TechProcess(3, TechProcessNames.Disk3D)]
    public class Disk3DTechProcess : TechProcessBase
    {
        public int PenetrationFeed { get; set; }

        public double? Thickness { get; set; }

        public Disk3DTechProcess(string caption) : base(caption)
        {
        }
    }
}
