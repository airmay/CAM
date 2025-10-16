using System;

namespace CAM
{
    [Serializable]
    public class SawingMode
    {
        /// <summary>
        /// Глубина до которой действуют параметры шага и подачи
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

        //public SawingMode Clone() => (SawingMode)MemberwiseClone();
    }
}
