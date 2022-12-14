using CAM.TechProcesses.Tactile;
using System;
using System.Collections.Generic;

namespace CAM.TechProcesses.Stolb
{
    [Serializable]
    [MenuItem("Столб", 9)]
    public class StolbTechProcess : MillingTechProcess
    {
        public double Depth { get; set; }

        public double RoughingStep { get; set; }

        public double FinishingStep { get; set; }

        public double Departure { get; set; }

        public int CuttingFeed { get; set; }

        public double PenetrationStep { get; set; }

        public int LastFeed { get; set; }

        public double LastStep { get; set; }

        public int RoughingFeed { get; set; }

        public int FinishingFeed { get; set; }

        public double AC { get; set; }

        public double DZ { get; set; }

        public List<Pass> PassList { get; set; } = new List<Pass>();

        public StolbTechProcess()
        {
            MachineType = CAM.MachineType.Champion;
            Material = CAM.Material.Granite;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            var passView = new PassListControl(view.BindingSource);
            view.BindingSource.DataSourceChanged += (s, e) => passView.BindingSource.DataSource = view.GetParams<StolbTechProcess>().PassList;

            view.AddMachine()
                .AddTool()
                .AddParam(nameof(Frequency))
                .AddParam(nameof(AC), "a + c")
                .AddParam(nameof(DZ), "dz")
                .AddParam(nameof(PenetrationFeed))
                .AddParam(nameof(Departure))
                .AddText("______Пирамида_____")
                .AddParam(nameof(PenetrationStep), "Шаг")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(LastStep), "Последний шаг")
                .AddParam(nameof(LastFeed), "Последняя подача")
                .AddText("________ПАЗ________")
                .AddParam(nameof(Depth), "Глубина паза")
                .AddParam(nameof(RoughingStep), "Заглубление гребенка")
                .AddParam(nameof(FinishingStep), "Заглубление чистка")
                .AddParam(nameof(RoughingFeed), "Подача гребенка")
                .AddParam(nameof(FinishingFeed), "Подача чистка")
                .AddControl(passView, 10);
            ;
        }
    }
}
