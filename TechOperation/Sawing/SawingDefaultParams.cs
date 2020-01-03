using System;

namespace CAM.TechOperation.Sawing
{
    /// <summary>
    /// Параметры по-умолчанию технологической операция "Распиловка"
    /// </summary>
    [Serializable]
    public class SawingDefaultParams
    {
        public SawingTechOperationParams SawingLineTechOperationParams { get; set; }

        public SawingTechOperationParams SawingCurveTechOperationParams { get; set; }

    }
}
