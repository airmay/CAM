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
            if (!(TechProcess.MachineType == MachineType.ScemaLogic || TechProcess.MachineType == MachineType.Donatoni))
                return;

            if (PassList?.Any() != true)
                CalcPassList();
            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var thickness = TechProcess.Tool.Thickness.Value;
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
            double offset = BandStart - BandSpacing - BandWidth;
            var size = (contourPoints[ProcessingAngle == 0 ? 3 : ProcessingAngle == 90 ? 1 : 2] - contourPoints[0]).Length;

            if (ProcessingAngle == 45 ^ TechProcess.MachineType == MachineType.Donatoni)
                Cutting(0.8 * thickness, CuttingFeed, -thickness);

            if (offset > 0)
            {
                var count = (int)Math.Ceiling(offset / (0.8 * thickness));
                Algorithms.Range(-0.8 * thickness * count, -0.1, 0.8 * thickness).ForEach(p => Cutting(offset + PassList[0].Pos + p, CuttingFeed));
            }

            do
            {
                foreach (var pass in PassList)
                    Cutting(offset + pass.Pos, pass.CuttingType == CuttingType.Roughing ? CuttingFeed : FeedFinishing);

                offset += BandWidth + BandSpacing;
            }
            while (offset < size);

            if (offset - BandSpacing < size)
                Algorithms.Range(offset - BandSpacing, size, 0.8 * thickness).ForEach(p => Cutting(p, CuttingFeed));

            if (ProcessingAngle == 45 ^ TechProcess.MachineType == MachineType.ScemaLogic)
                Cutting(size - 0.8 * thickness, CuttingFeed, thickness);

            void Cutting(double pos, int feed, double s = 0)
            {
                if (pos < 0 || pos > size)
                    return;
                ray.BasePoint = basePoint + passDir * pos;
                var points = new Point3dCollection();
                ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
                if (points.Count == 2)
                {
                    var vector = (points[1] - points[0]).GetNormal() * tactileTechProcess.TactileTechProcessParams.Departure;
                    var startPoint = points[0] + passDir * s - vector - Vector3d.ZAxis * Depth;
                    var endPoint = points[1] + passDir * s + vector - Vector3d.ZAxis * Depth;
                    builder.Cutting(startPoint, endPoint, feed, tactileTechProcess.TactileTechProcessParams.TransitionFeed);
                }
            }
            ray.Dispose();
            contour.Dispose();
        }
    }
}
