using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(3, TechProcessNames.Tactile, "Конусы")]
    public class ConesTechOperation : TechOperationBase
    {
        public int CuttingFeed { get; set; }

        public int FeedMax { get; set; }

        public int FeedMin { get; set; }

        public int Frequency { get; set; }

        public int ZSafety { get; set; }

        public int ZEntry { get; set; }
        
        public ConesTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {
            FeedMax = 200;
            FeedMin = 80;
            Frequency = 5000;
            ZSafety = 5;
            ZEntry = 1;
        }

        public override bool CanProcess => TechProcess.MachineType == MachineType.Donatoni || TechProcess.MachineType == MachineType.Krea;

        public override void BuildProcessing(ICommandGenerator generator)
        {
            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var tactileParams = ((TactileTechProcess)TechProcess).TactileTechProcessParams;
            var isDiag = tactileTechProcess.ProcessingAngle1 != 0;

            var contour = tactileTechProcess.GetContour();
            var contourPoints = contour.GetPolyPoints().ToArray();

            var point = tactileTechProcess.Objects.Select(p => p.GetCurve()).OfType<Circle>().Select(p => p.Center).OrderBy(p => (int)p.Y).ThenBy(p => p.X).First();
            var x = point.X;
            var y = point.Y;
            var stepX = tactileTechProcess.BandSpacing.Value + tactileTechProcess.BandWidth.Value;
            var stepY = stepX;
            if (isDiag)
            {
                stepY /= Math.Sqrt(2);
                stepX = stepY * 2;
            }
            generator.SetTool(2, Frequency, 90);
            generator.ZSafety = ZSafety;

            while (y < contourPoints[1].Y)
            {
                Cutting();
                x += stepX;
                if (x > contourPoints[3].X)
                {
                    y += stepY;
                    x -= stepY;
                    stepX = -stepX;
                    if (x > contourPoints[3].X)
                        x += stepX;
                }
                if (x < contourPoints[1].X)
                {
                    y += stepY;
                    x += stepY;
                    stepX = -stepX;
                    if (x < contourPoints[1].X)
                        x += stepX;
                }
            }

            void Cutting()
            {
                generator.Move(x, y);
                generator.Move(z: ZEntry);
                var zMin = -tactileParams.Depth;
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
                            Pause();
                            generator.Uplifting(z + 1);
                            generator.Cutting(x, y, z + 0.2, (int)(FeedMax * 1.5));
                            generator.Cutting(x, y, z, feed);
                        }
                    }
                    z -= 0.2;
                }
                Pause();
                generator.Uplifting();

                void Pause() => generator.Command("G4 F0.2", "Пауза");
            }

            contour.Dispose();
        }
    }
}