using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
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

        public ConesTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {
            FeedMax = 200;
            FeedMin = 80;
            Frequency = 5000;
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            if (!(TechProcess.MachineType == MachineType.Donatoni || TechProcess.MachineType == MachineType.Krea))
                return;

            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var tactileParams = ((TactileTechProcess)TechProcess).TactileTechProcessParams;
            var isDiag = tactileTechProcess.ProcessingAngle1 != 0;

            var contour = tactileTechProcess.GetContour();
            var contourPoints = contour.GetPolyPoints().ToArray();

            var point = tactileTechProcess.Objects.GetCurves().OfType<Circle>().Select(p => p.Center).OrderBy(p => (int)p.Y).ThenBy(p => p.X).First();
            var x = point.X;
            var y = point.Y;
            var stepX = tactileTechProcess.BandSpacing.Value + tactileTechProcess.BandWidth.Value;
            var stepY = stepX;
            if (isDiag)
            {
                stepY /= Math.Sqrt(2);
                stepX = stepY * 2;
            }
            builder.SetTool(2, Frequency, 90);

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
                var zMin = -tactileParams.Depth;
                double z = 0;
                int counter = 0;
                while (z >= zMin)
                {
                    var feed = (int)((FeedMax - FeedMin) / zMin * (z * z / zMin - 2 * z) + FeedMax);
                    builder.Cutting(x, y, z, 0, cuttingFeed: feed);
                    if (z <= -2)
                    {
                        if (++counter == 5) // Подъем на 1мм для охлаждения
                        {
                            counter = 0;
                            builder.Pause();
                            builder.Uplifting(z + 1);
                            builder.Cutting(x, y, z + 0.2, 0, cuttingFeed: 300);
                            builder.Cutting(x, y, z, 0, cuttingFeed: feed);
                        }
                    }
                    z -= 0.2;
                }
                builder.Pause();
                builder.Uplifting();
            }

            contour.Dispose();
        }
    }
}