using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(2, TechProcessNames.Tactile, "Фаска")]
    public class ChamfersTechOperation : TechOperationBase
    {
        public double BandStart { get; set; }

        public int ProcessingAngle { get; set; }

        public int CuttingFeed { get; set; }

        public ChamfersTechOperation(TactileTechProcess techProcess, string caption) : this(techProcess, caption, null, null) { }

        public ChamfersTechOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart) : base(techProcess, caption)
        {
            BandStart = bandStart ?? techProcess.BandStart1.Value;
            ProcessingAngle = processingAngle ?? techProcess.ProcessingAngle1.Value;
            CuttingFeed = techProcess.TactileTechProcessParams.CuttingFeed;
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            if (TechProcess.MachineType != MachineType.Donatoni)
                return;

            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var contour = tactileTechProcess.GetContour();
            var contourPoints = contour.GetPolyPoints().ToArray();
            var ray = new Ray
            {
                BasePoint = ProcessingAngle == 45 ? contourPoints.Last() : contourPoints.First(),
                UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle), Vector3d.ZAxis)
            };
            var passDir = ray.UnitDir.GetPerpendicularVector();
            if (ProcessingAngle >= 90)
                passDir = passDir.Negate();
            ray.BasePoint += passDir * tactileTechProcess.BandStart1.Value;
            var step = tactileTechProcess.BandWidth.Value + tactileTechProcess.BandSpacing.Value;
            var tactileParams = tactileTechProcess.TactileTechProcessParams;

            while(true)
            {
                var points = new Point3dCollection();
                ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
                if (points.Count == 2 && !points[0].IsEqualTo(points[1]))
                {
                    var vector = (points[1] - points[0]).GetNormal() * tactileParams.Departure;
                    var startPoint = points[0] - vector - Vector3d.ZAxis * tactileParams.Depth;
                    var endPoint = points[1] + vector - Vector3d.ZAxis * tactileParams.Depth;
                    generator.Cutting(startPoint, endPoint, CuttingFeed, tactileParams.TransitionFeed, 45, step > 0 ^ ProcessingAngle == 45 ? Side.Right : Side.Left);
                }
                else if (step > 0)
                {
                    ray.BasePoint -= passDir * tactileTechProcess.BandSpacing.Value;
                    step = -step;
                    generator.Uplifting();
                }
                else
                    break;
                ray.BasePoint += passDir * step;
            }
        }
    }
}
