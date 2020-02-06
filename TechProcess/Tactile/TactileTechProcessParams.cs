using System;

namespace CAM.Tactile
{
    /// <summary>
    /// Параметры техпроцесса "Тактилка"
    /// </summary>
    [Serializable]
    public class TactileTechProcessParams
    {
        public double BandWidth { get; set; }

        public double BandSpacing { get; set; }

        public double BandStart { get; set; }

        public double Depth { get; set; }

        public double Departure { get; set; }

        public int TransitionFeed { get; set; }

        public int PenetrationFeed { get; set; }

        public int CuttingFeed { get; set; }

        public int FinishingFeed { get; set; }

        public TactileTechProcessParams Clone() => MemberwiseClone() as TactileTechProcessParams;

        public static TactileTechProcessParams GetDefault()
        {
            return new TactileTechProcessParams
            {
                BandWidth = 25,
                BandSpacing = 25,
                BandStart = 37.5,
                Depth = 20,
                Departure = 150,
                TransitionFeed = 2000,
                PenetrationFeed = 200
            };
        }
    }
}
