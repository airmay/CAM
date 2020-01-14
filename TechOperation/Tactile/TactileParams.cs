using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.TechOperation.Tactile
{
    /// <summary>
    /// Параметры технологической операция "Тактилка"
    /// </summary>
    [Serializable]
    public class TactileParams
    {
        public string Type { get; set; }

        public int TopStart { get; set; }

        public int TopWidth { get; set; }

        public int GutterWidth { get; set; }

        public int Depth { get; set; }

        public int Departure { get; set; }

        public int FeedRoughing1 { get; set; }

        public int FeedFinishing1 { get; set; }

        public int FeedRoughing2 { get; set; }

        public int FeedFinishing2 { get; set; }

        public int Transition { get; set; }

        public List<Pass> PassList { get; set; }

        public TactileParams Clone() => MemberwiseClone() as TactileParams;

        public static TactileParams GetDefault()
        {
            return new TactileParams
            {
                Type = TactileType.StraightStripes,
                TopStart = 50,
                TopWidth = 50,
                GutterWidth = 50,
                Depth = 20,
                Departure = 150
            };
        }
    }
}
