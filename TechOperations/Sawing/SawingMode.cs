using System;

namespace CAM.TechOperations.Sawing
{
    /// <summary>
    /// Режим распиловки
    /// </summary>
    [Serializable]
    public class SawingMode
    {
        /// <summary>
        /// Глубина
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Шаг по глубине
        /// </summary>
        public int DepthStep { get; set; }

        /// <summary>
        /// Подача
        /// </summary>
        public int Feed { get; set; }

        public SawingMode Clone() => (SawingMode)MemberwiseClone();
    }
}
