using System;
using System.Collections.Generic;

namespace CAM.Sawing
{
    /// <summary>
    /// Параметры техпроцесса "Распиловка"
    /// </summary>
    [Serializable]
    public class SawingTechProcessParams
    {
        public List<SawingMode> SawingLineModes { get; set; } = new List<SawingMode>();

        public List<SawingMode> SawingCurveModes { get; set; } = new List<SawingMode>();

        public SawingTechProcessParams Clone() => MemberwiseClone() as SawingTechProcessParams;
    }
}
