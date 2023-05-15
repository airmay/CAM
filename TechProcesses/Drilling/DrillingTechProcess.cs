using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;

namespace CAM.TechProcesses.Drilling
{
    [Serializable]
    [MenuItem("Сверление", 5)]
    public class DrillingTechProcess : MillingTechProcess
    {
        public double Depth { get; set; }

        public int FeedMax { get; set; }

        public int FeedMin { get; set; }

        public int ZEntry { get; set; }

        public DrillingTechProcess()
        {
            MachineType = CAM.MachineType.Krea;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTextBox(nameof(Frequency));
            view.AddTextBox(nameof(Depth));
            view.AddIndent();
            view.AddOrigin();
            view.AddAcadObject(nameof(ProcessingArea), "Отверстия", "Выберите окружности", AcadObjectNames.Circle);
            view.AddIndent();
            view.AddTextBox(nameof(FeedMax), "Подача макс.");
            view.AddTextBox(nameof(FeedMin), "Подача мин.");
            view.AddIndent();
            view.AddTextBox(nameof(ZSafety));
            view.AddTextBox(nameof(ZEntry));
        }

        protected override void BuildProcessing(MillingCommandGenerator generator)
        {
            generator.ZSafety = ZSafety;
            generator.SetTool(1, Frequency, hasTool: false);

            ProcessingArea.ObjectIds.ToList().ForEach(p =>
            {
                var arc = Acad.OpenForRead(p) as Circle;
                Cutting(generator, arc.Center.X, arc.Center.Y);
            });
        }

        private void Cutting(MillingCommandGenerator generator, double x, double y)
        {
            generator.Move(x, y);
            generator.Move(z: ZEntry);
            var zMin = -Depth;
            double z = 0;
            int counter = 0;
            while (z >= zMin)
            {
                var feed = (int)((FeedMax - FeedMin) / zMin * (z * z / zMin - 2 * z) + FeedMax);
                generator.Cutting(x, y, z, feed);
                if (z <= -2)
                {
                    if (++counter == 5) // Подъем на 1мм для охлаждения
                    {
                        counter = 0;
                        generator.Pause(0.2);
                        generator.Uplifting(z + 1);
                        generator.Cutting(x, y, z + 0.2, (int)(FeedMax * 1.5));
                        generator.Cutting(x, y, z, feed);
                    }
                }
                z -= 0.2;
            }
            generator.Pause(0.2);
            generator.Uplifting();
        }
    }
}
