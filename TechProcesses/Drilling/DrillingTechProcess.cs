using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM.TechProcesses.Drilling
{
    [Serializable]
    [TechProcess(4, TechProcessNames.Drilling)]
    public class DrillingTechProcess : TechProcessBase
    {
        public double Depth { get; set; }

        public int FeedMax { get; set; }

        public int FeedMin { get; set; }

        public int ZSafety { get; set; }

        public int ZEntry { get; set; }

        public DrillingTechProcess(string caption) : base(caption)
        {
            MachineType = CAM.MachineType.Krea;
        }

        protected override void BuildProcessing(ICommandGenerator generator)
        {
            generator.ZSafety = ZSafety;
            generator.SetTool(1, Frequency);

            ProcessingArea.ForEach(p =>
            {
                var arc = p.GetCurve() as Circle;
                Cutting(generator, arc.Center.X, arc.Center.Y);
            });
        }

        private void Cutting(ICommandGenerator generator, double x, double y)
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
