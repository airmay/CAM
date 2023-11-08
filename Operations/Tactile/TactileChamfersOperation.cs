using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.TechProcesses.Tactile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Tactile
{
    [Serializable]
    public class TactileChamfersOperation : Operation
    {
        public double BandStart { get; set; }

        public int ProcessingAngle { get; set; }

        public int CuttingFeed { get; set; }

        //public TactileChamfersOperation(TactileTechProcess techProcess, string caption) : base(techProcess, caption) { }

        //public TactileChamfersOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart) : base(techProcess, caption)
        //{
        //    BandStart = bandStart ?? BandStart;
        //    ProcessingAngle = processingAngle ?? ProcessingAngle;
        //}

        //public void Init()
        //{
        //    BandStart = TechProcess.BandStart1.Value;
        //    ProcessingAngle = TechProcess.ProcessingAngle1.Value;
        //}

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTextBox(nameof(ProcessingAngle), "Угол полосы");
            view.AddTextBox(nameof(CuttingFeed));
        }

        //public bool Validate() => ToolService.Validate(TechProcess.Tool, ToolType.Disk);

        //public void BuildProcessing(MillingCommandGenerator generator)
        //{
        //    const int AngleA = 45;
        //    var tactileTechProcess = (TactileTechProcess)TechProcess;
        //    var contour = tactileTechProcess.GetContour();
        //    var contourPoints = contour.GetPolyPoints().ToArray();
        //    var ray = new Ray
        //    {
        //        BasePoint = ProcessingAngle == 45 ? contourPoints.Last() : contourPoints.First(),
        //        UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle), Vector3d.ZAxis)
        //    };
        //    var passDir = ray.UnitDir.GetPerpendicularVector();
        //    if (ProcessingAngle >= 90)
        //        passDir = passDir.Negate();
        //    var engineSide = ProcessingAngle < 90 ? Side.Right : Side.Left;

        //    var start = tactileTechProcess.BandStart1.Value - tactileTechProcess.BandSpacing.Value;
        //    if (start <= 0)
        //        start += tactileTechProcess.BandWidth.Value + tactileTechProcess.BandSpacing.Value;
        //    ray.BasePoint += passDir * start;

        //    var pointCollections = new List<Point3dCollection[]>();
        //    while (true)
        //    {
        //        var pts = new Point3dCollection[]
        //        {
        //            new Point3dCollection(),
        //            new Point3dCollection()
        //        };
        //        ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), pts[0], IntPtr.Zero, IntPtr.Zero);
        //        if (pts[0].Count != 2)
        //            break;

        //        ray.BasePoint += passDir * tactileTechProcess.BandSpacing.Value;
        //        ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), pts[1], IntPtr.Zero, IntPtr.Zero);
        //        if (pts[1].Count != 2)
        //            break;

        //        ray.BasePoint += passDir * tactileTechProcess.BandWidth.Value;

        //        pointCollections.Add(pts);
        //    }

        //    for (int index = 0; index < 2; index++)
        //    {
        //        foreach (var points in pointCollections)
        //        {
        //            var vector = (points[index][1] - points[index][0]).GetNormal() * tactileTechProcess.Departure;
        //            var startPoint = points[index][0] - vector - Vector3d.ZAxis * tactileTechProcess.Depth;
        //            var endPoint = points[index][1] + vector - Vector3d.ZAxis * tactileTechProcess.Depth;

        //            generator.Cutting(startPoint, endPoint, CuttingFeed, tactileTechProcess.TransitionFeed, BuilderUtils.CalcToolAngle(ProcessingAngle.ToRad(), engineSide), AngleA, engineSide: engineSide);
        //        }
        //        pointCollections.Reverse();
        //        generator.Uplifting();
        //        engineSide = engineSide.Opposite();
        //    }
        //}

        public override void Execute(Processor processor)
        {
            throw new NotImplementedException();
        }
    }
}
