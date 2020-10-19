using System;
using System.Collections.Generic;

namespace CAM.TechProcesses.Sawing
{
    /// <summary>
    /// Параметры техпроцесса "Распиловка"
    /// </summary>
    [Serializable]
    public class SawingTechProcessParams
    {
        public int PenetrationFeed { get; set; }

        public List<SawingMode> SawingLineModes { get; set; }

        public List<SawingMode> SawingCurveModes { get; set; }


        public SawingTechProcessParams Clone() => MemberwiseClone() as SawingTechProcessParams;
    }
}
