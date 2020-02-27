using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Полосы")]
    public class BandsTechOperation : TechOperationBase
    {
        public int ProcessingAngle { get; set; }

        public int CuttingFeed { get; set; }

        public int FeedFinishing { get; set; }

        public double BandWidth { get; set; }

        public double BandSpacing { get; set; }

        public double BandStart { get; set; }

        public double Depth { get; set; }

        public double MaxCrestWidth { get; set; }

        public List<Pass> PassList { get; set; }

        public BandsTechOperation(TactileTechProcess techProcess, string caption) : this(techProcess, caption, null, null) { }

        public BandsTechOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart) : base(techProcess, caption)
        {
            BandWidth = techProcess.BandWidth.Value;
            BandSpacing = techProcess.BandSpacing.Value;
            BandStart = bandStart ?? techProcess.BandStart1.Value;
            ProcessingAngle = processingAngle ?? techProcess.ProcessingAngle1.Value;
            Depth = techProcess.TactileTechProcessParams.Depth;
            CuttingFeed = techProcess.TactileTechProcessParams.CuttingFeed;
            FeedFinishing = techProcess.TactileTechProcessParams.FinishingFeed;
            MaxCrestWidth = (TechProcess.Tool?.Thickness).GetValueOrDefault();
        }

        public void CalcPassList()
        {
            if (TechProcess.Tool.Thickness == null)
            {
                Acad.Alert("Не указана толщина инструмента");
                return;
            }
            var toolThickness = TechProcess.Tool.Thickness.Value;
            if (MaxCrestWidth == 0 || MaxCrestWidth > toolThickness)
                MaxCrestWidth = toolThickness;
            var periodAll = BandWidth - toolThickness;
            var periodWidth = toolThickness + MaxCrestWidth;
            var count = Math.Ceiling(periodAll / periodWidth);
            periodWidth = periodAll / count;
            var x = (toolThickness - (periodWidth - toolThickness)) / 2;
            var shift = TechProcess.MachineType == MachineType.ScemaLogic ^ ProcessingAngle == 45? toolThickness : 0;
            PassList = new List<Pass> { new Pass(shift, CuttingType.Roughing) };
            for (int i = 1; i <= count; i++)
            {
                PassList.Add(new Pass(i * periodWidth + shift, CuttingType.Roughing));
                PassList.Add(new Pass(i * periodWidth + x + shift - toolThickness, CuttingType.Finishing));
            }
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            if (PassList?.Any() != true)
                CalcPassList();
            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var contour = tactileTechProcess.GetContour();
            var contourPoints = contour.GetPolyPoints().ToArray();
            var basePoint = ProcessingAngle == 45 ? contourPoints[3] : contourPoints[0];
            var ray = new Ray
            {
                BasePoint = basePoint,
                UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle), Vector3d.ZAxis)
            };
            var passDir = ray.UnitDir.GetPerpendicularVector();
            if (ProcessingAngle >= 90)
                passDir = passDir.Negate();
            var diag = (contourPoints[2] - contourPoints[0]).Length;
            double offset = BandStart - BandSpacing - BandWidth;

            builder.StartTechOperation();
            do
            {
                foreach (var pass in PassList)
                {
                    ray.BasePoint = basePoint + passDir * (offset + pass.Pos);
                    var points = new Point3dCollection();
                    ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
                    if (points.Count == 2 && points[0] != points[1])
                    {
                        var vector = (points[1] - points[0]).GetNormal() * tactileTechProcess.TactileTechProcessParams.Departure;
                        var startPoint = points[0] - vector - Vector3d.ZAxis * Depth;
                        var endPoint = points[1] + vector - Vector3d.ZAxis * Depth;
                        builder.Cutting(startPoint, endPoint, pass.CuttingType == CuttingType.Roughing ? CuttingFeed : FeedFinishing, tactileTechProcess.TactileTechProcessParams.TransitionFeed);
                    }
                }
                offset += BandWidth + BandSpacing;
            }
            while (offset < diag);

            ProcessCommands = builder.FinishTechOperation();
        }
    }
}
