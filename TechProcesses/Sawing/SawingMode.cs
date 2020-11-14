using System;

namespace CAM.TechProcesses.Sawing
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
        public double? Depth { get; set; }

        /// <summary>
        /// Шаг по глубине
        /// </summary>
        public double DepthStep { get; set; }

        /// <summary>
        /// Подача
        /// </summary>
        public int Feed { get; set; }

        public SawingMode Clone() => (SawingMode)MemberwiseClone();
    }
}
