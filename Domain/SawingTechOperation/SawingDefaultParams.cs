namespace CAM.Domain
{
    /// <summary>
    /// Параметры по-умолчанию технологической операция "Распиловка"
    /// </summary>
    public class SawingDefaultParams
    {
        public SawingTechOperationParams SawingLineTechOperationParams { get; set; }

        public SawingTechOperationParams SawingCurveTechOperationParams { get; set; }
    }
}
