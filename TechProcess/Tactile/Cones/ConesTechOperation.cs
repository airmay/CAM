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
        public int Frequency { get; set; }

        public int ProcessingAngle { get; set; }

        public double BandStart1 { get; set; }

        public double BandStart2 { get; set; }

        public ConesTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {
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
            ray.BasePoint += passDir * (BandStart1 - tactileParams.BandSpacing / 2);

            var step = tactileParams.BandWidth + tactileParams.BandSpacing;
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
            ray.BasePoint = (baseCurve.StartPoint.X < baseCurve.EndPoint.X ? baseCurve.EndPoint : baseCurve.StartPoint) + passDir * (BandStart2 - tactileParams.BandSpacing / 2);

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
                builder.Cutting(new Point3d(point.X, point.Y, tactileParams.Depth), 0, 0);
                builder.Uplifting();
            }
        }
    }
}