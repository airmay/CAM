using System;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    [MenuItem("Распиловка по дуге", 2)]
    public class ArcSawingTechOperation : WireSawingTechOperation<CableSawingTechProcess>
    {
        public int StepCount { get; set; } = 100;

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject()
                .AddParam(nameof(StepCount), "Количество шагов");
        }

        public override void BuildProcessing(CableCommandGenerator generator)
        {
            throw new NotImplementedException();
        }
    }
}
