using System;
using System.Collections.Generic;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    [MenuItem("Столб", 9)]
    public class StolbTechProcess : MillingTechProcess
    {
        public double Depth { get; set; }

        public double? PenetrationStep { get; set; }

        public double Departure { get; set; }

        public int CuttingFeed { get; set; }

        public double AC { get; set; }


        public StolbTechProcess()
        {
            MachineType = CAM.MachineType.Forma;
            Material = CAM.Material.Granite;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine()
                .AddTool()
                .AddParam(nameof(Frequency))
                .AddIndent()
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(PenetrationFeed))
                .AddIndent()
                .AddParam(nameof(Depth))
                .AddParam(nameof(PenetrationStep), "Шаг заглубления")
                .AddParam(nameof(Departure))
                .AddParam(nameof(AC), "a + c")
                ;
        }
    }
}
