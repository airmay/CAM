using System;

namespace CAM.TechProcesses.Tactile
{
    /// <summary>
    /// Параметры техпроцесса "Тактилка"
    /// </summary>
    [Serializable]
    public class TactileTechProcessParams
    {
        public double Depth { get; set; }

        public double Departure { get; set; }

        public int TransitionFeed { get; set; }

        public int PenetrationFeed { get; set; }

        public int CuttingFeed { get; set; }

        public int FinishingFeed { get; set; }

        public TactileTechProcessParams Clone() => MemberwiseClone() as TactileTechProcessParams;

    }
}
