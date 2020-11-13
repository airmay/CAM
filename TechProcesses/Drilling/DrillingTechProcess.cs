using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM.TechProcesses.Drilling
{
    [Serializable]
    [TechProcess(TechProcessType.Drilling)]
    public class DrillingTechProcess : TechProcess
    {
        public double Depth { get; set; }

        public int FeedMax { get; set; }

        public int FeedMin { get; set; }

        public int ZEntry { get; set; }

        public DrillingTechProcess(string caption) : base(caption)
        {
            MachineType = CAM.MachineType.Krea;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddParam(nameof(Frequency))
                .AddParam(nameof(Depth))
                .AddIndent()
                .AddOrigin()
                .AddAcadObject(nameof(ProcessingArea), "Отверстия", "Выберите окружности", AcadObjectNames.Circle)
                .AddIndent()
                .AddParam(nameof(FeedMax), "Подача макс.")
                .AddParam(nameof(FeedMin), "Подача мин.")
                .AddIndent()
                .AddParam(nameof(ZSafety))
                .AddParam(nameof(ZEntry));
        }

        protected override void BuildProcessing(CommandGeneratorBase generator)
        {
            generator.ZSafety = ZSafety;
            generator.SetTool(1, Frequency, hasTool: false);

            ProcessingArea.ForEach(p =>
            {
                var arc = p.GetCurve() as Circle;
                Cutting(generator, arc.Center.X, arc.Center.Y);
            });
        }

        private void Cutting(CommandGeneratorBase generator, double x, double y)
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
