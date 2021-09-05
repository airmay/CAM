using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    [MenuItem("Фаска", 2)]
    public class ChamfersTechOperation : TechOperation<TactileTechProcess>
    {
        public double BandStart { get; set; }

        public int ProcessingAngle { get; set; }

        public int CuttingFeed { get; set; }

        public ChamfersTechOperation() { }

        public ChamfersTechOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart)
        {
            Setup(techProcess, caption);

            BandStart = bandStart ?? BandStart;
            ProcessingAngle = processingAngle ?? ProcessingAngle;
        }

        public override void Init()
        {
            BandStart = TechProcess.BandStart1.Value;
            ProcessingAngle = TechProcess.ProcessingAngle1.Value;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddParam(nameof(ProcessingAngle), "Угол полосы")
                .AddParam(nameof(CuttingFeed));
        }

        public override bool CanProcess => TechProcess.MachineType == MachineType.Donatoni;

        public override bool Validate() => ToolService.Validate(TechProcess.Tool, ToolType.Disk);

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            const int AngleA = 45;
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
            ray.BasePoint += passDir * (tactileTechProcess.BandStart1.Value - tactileTechProcess.BandSpacing.Value);
            var step = tactileTechProcess.BandWidth.Value + tactileTechProcess.BandSpacing.Value;
            var engineSide = ProcessingAngle < 90 ? Side.Right : Side.Left;
            while (true)
            {
                var points = new Point3dCollection();
                ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
                if (points.Count == 2 && !points[0].IsEqualTo(points[1]))
                {
                    var vector = (points[1] - points[0]).GetNormal() * tactileTechProcess.Departure;
                    var startPoint = points[0] - vector - Vector3d.ZAxis * tactileTechProcess.Depth;
                    var endPoint = points[1] + vector - Vector3d.ZAxis * tactileTechProcess.Depth;
                    if (generator.IsUpperTool)
                        generator.Move(startPoint.X, startPoint.Y, angleC: BuilderUtils.CalcToolAngle(ProcessingAngle.ToRad(), engineSide), angleA: AngleA);
                    generator.Cutting(startPoint, endPoint, CuttingFeed, tactileTechProcess.TransitionFeed);
                }
                else if (step > 0)
                {
                    ray.BasePoint += passDir * tactileTechProcess.BandSpacing.Value;
                    step = -step;
                    generator.Uplifting();
                    engineSide = engineSide.Opposite();
                }
                else
                    break;
                ray.BasePoint += passDir * step;
            }
        }
    }
}
