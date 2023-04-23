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

            view.AddMachine();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddTextBox(nameof(AC), "a + c");
            view.AddTextBox(nameof(DZ), "dz");
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddTextBox(nameof(Departure));
            view.AddText("______Пирамида_____");
            view.AddTextBox(nameof(PenetrationStep), "Шаг");
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(LastStep), "Последний шаг");
            view.AddTextBox(nameof(LastFeed), "Последняя подача");
            view.AddText("________ПАЗ________");
            view.AddTextBox(nameof(Depth), "Глубина паза");
            view.AddTextBox(nameof(RoughingStep), "Заглубление гребенка");
            view.AddTextBox(nameof(FinishingStep), "Заглубление чистка");
            view.AddTextBox(nameof(RoughingFeed), "Подача гребенка");
            view.AddTextBox(nameof(FinishingFeed), "Подача чистка");
            view.AddControl(passView, 10);
        }
    }
}
