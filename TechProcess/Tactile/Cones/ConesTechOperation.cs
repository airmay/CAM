using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Конусы")]
    public class ConesTechOperation : TechOperationBase
    {
        public double BandWidth { get; set; }

        public double BandSpacing { get; set; }

        public double BandStart1 { get; set; }

        public double BandStart2 { get; set; }

        public int Frequency { get; set; }

        public int ProcessingAngle { get; set; }

        public ConesTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {
            BandWidth = techProcess.BandWidth.Value;
            BandSpacing = techProcess.BandSpacing.Value;
            BandStart1 = techProcess.BandStart1.Value;
            BandStart2 = techProcess.BandStart2.Value;
            Frequency = 5000;
            ProcessingAngle = 45;
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            if (!(TechProcess.MachineType == MachineType.Donatoni || TechProcess.MachineType == MachineType.Krea))
                return;
            var baseCurve = TechProcess.ProcessingArea.Curves.OrderBy(p => p.StartPoint.Y + p.EndPoint.Y).First();
            var ray = new Ray
            {
                BasePoint = baseCurve.StartPoint.X < baseCurve.EndPoint.X ? baseCurve.StartPoint : baseCurve.EndPoint,
                UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle - 90), Vector3d.ZAxis)
            };
            var passDir = ray.UnitDir.GetPerpendicularVector();
            var tactileParams = ((TactileTechProcess)TechProcess).TactileTechProcessParams;
            ray.BasePoint += passDir * (BandStart1 - BandSpacing / 2);

            var step = BandWidth + BandSpacing;
            var curves = TechProcess.ProcessingArea.Curves.ToList();
            var lines = new List<Curve>();
            List<Point3d> points;
            while ((points = ray.Intersect(curves, Intersect.ExtendThis)).Count == 2)
            {
                lines.Add(NoDraw.Line(points[0], points[1]));
                ray.BasePoint += passDir * step;
            }
            ray.UnitDir = passDir;
            passDir = passDir.GetPerpendicularVector();
            ray.BasePoint = (baseCurve.StartPoint.X < baseCurve.EndPoint.X ? baseCurve.EndPoint : baseCurve.StartPoint) + passDir * (BandStart2 - BandSpacing / 2);

            bool flag = false;
            builder.StartTechOperation();
            while ((points = ray.Intersect(lines, Intersect.ExtendThis)).Any())
            {
                if (flag)
                    points.Reverse();
                flag = !flag;
                points.ForEach(Cutting);
                ray.BasePoint += passDir * step;
            }
            ProcessCommands = builder.FinishTechOperation();

            void Cutting(Point3d point)
            {
                //builder.Cutting(new Point3d(point.X, point.Y, tactileParams.Depth), 0, 0);
                builder.Uplifting();
            }
        }

        public void BuildProcessing1(ScemaLogicProcessBuilder builder)
        {
            var tactileTechProcess = (TactileTechProcess)TechProcess;
            var contour = tactileTechProcess.GetContour();
            var contourPoints = contour.GetPolyPoints().ToArray();
            var basePoint = contourPoints[0];
            var ray = new Ray
            {
                BasePoint = basePoint,
                UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle), Vector3d.ZAxis)
            };
            var passDir = ray.UnitDir.GetPerpendicularVector();
            if (ProcessingAngle >= 90)
                passDir = passDir.Negate();
            var diag = (contourPoints[2] - contourPoints[0]).Length;
            var width = (tactileTechProcess.BandWidth.Value + tactileTechProcess.BandSpacing.Value) / Math.Sqrt(2) - tactileTechProcess.BandSpacing.Value;
            var offset = (Math.Min(tactileTechProcess.BandStart1.Value, tactileTechProcess.BandStart2.Value) - tactileTechProcess.BandSpacing.Value / 2) / Math.Sqrt(2)
                - tactileTechProcess.BandSpacing.Value / 2;
            bool flag = true;
            var tactileParams = tactileTechProcess.TactileTechProcessParams;

            builder.StartTechOperation();
            do
            {
                ray.BasePoint = basePoint + passDir * offset;
                var points = new Point3dCollection();
                ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
                if (points.Count == 2 && points[0] != points[1])
                {
                    var vector = (points[1] - points[0]).GetNormal() * tactileParams.Departure;
                    var startPoint = points[0] - vector - Vector3d.ZAxis * tactileParams.Depth;
                    var endPoint = points[1] + vector - Vector3d.ZAxis * tactileParams.Depth;
//                    builder.Cutting(startPoint, endPoint, CuttingFeed, tactileParams.TransitionFeed, 45);
                }
                offset += flag ? tactileTechProcess.BandSpacing.Value : width;
                flag = !flag;
            }
            while (offset < diag);

            ProcessCommands = builder.FinishTechOperation();
        }

    }
}