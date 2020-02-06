using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
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

        public int Feed { get; set; }

        public int FeedFinishing { get; set; }

        public double BandWidth { get; set; }

        public double BandSpacing { get; set; }

        public double BandStart { get; set; }

        public double Depth { get; set; }

        public double MaxCrestWidth { get; set; }

        public List<Pass> PassList { get; set; }

        public BandsTechOperation(TactileTechProcess techProcess, string name) : base(techProcess, name)
        {
            BandWidth = techProcess.TactileTechProcessParams.BandWidth;
            BandSpacing = techProcess.TactileTechProcessParams.BandSpacing;
            BandStart = techProcess.TactileTechProcessParams.BandStart;
            Depth = techProcess.TactileTechProcessParams.Depth;
            Feed = techProcess.TactileTechProcessParams.CuttingFeed;
            FeedFinishing = techProcess.TactileTechProcessParams.FinishingFeed;
            MaxCrestWidth = (TechProcess.Tool?.Thickness).GetValueOrDefault();
        }

        public void CalcPassList()
        {
            if (TechProcess.Tool == null)
            {
                Acad.Alert("Не указан инструмент");
                return;
            }
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
            PassList = new List<Pass> { new Pass(toolThickness, CuttingType.Roughing) };
            for (int i = 1; i <= count; i++)
            {
                PassList.Add(new Pass(i * periodWidth + toolThickness, CuttingType.Roughing));
                PassList.Add(new Pass(i * periodWidth + x, CuttingType.Finishing));
            }
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            if (PassList?.Any() != true)
                CalcPassList();
            var baseCurve = TechProcess.ProcessingArea.Curves.OrderBy(p => p.StartPoint.Y + p.EndPoint.Y).First();
            var basePoint = baseCurve.StartPoint.X < baseCurve.EndPoint.X ^ ProcessingAngle < 90 ? baseCurve.StartPoint : baseCurve.EndPoint;
            var ray = new Ray
            {
                BasePoint = basePoint,
                UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle - (ProcessingAngle < 90 ? 0 : 180)), Vector3d.ZAxis)
            };
            var passDir = ray.UnitDir.GetPerpendicularVector();
            var points = new List<Point3d>();
            double offset = BandStart - BandSpacing - BandWidth;
            bool cuttingFlag = false;
            var TactileTechProcessParams = ((TactileTechProcess)TechProcess).TactileTechProcessParams;

            builder.StartTechOperation();
            do
            {
                foreach (var pass in PassList)
                {
                    ray.BasePoint = basePoint + passDir * (offset + pass.Pos);
                    points.Clear();
                    foreach (var curve in TechProcess.ProcessingArea.Curves)
                    {
                        var intersectPoints = new Point3dCollection();
                        ray.IntersectWith(curve, Intersect.ExtendThis, intersectPoints, 0, 0);
                        if (intersectPoints.Count == 1)
                            points.Add(intersectPoints[0]);
                    }
                    if (points.Count == 2)
                    {
                        var vector = (points[1] - points[0]).GetNormal() * TactileTechProcessParams.Departure;
                        var startPoint = points[0] - vector - Vector3d.ZAxis * Depth;
                        var endPoint = points[1] + vector - Vector3d.ZAxis * Depth;
                        builder.Cutting(startPoint, endPoint, pass.CuttingType == CuttingType .Roughing ? Feed : FeedFinishing, TactileTechProcessParams.TransitionFeed);
                        cuttingFlag = true;
                    }
                }
                offset += BandWidth + BandSpacing;
            }
            while ((points.Count != 0 || !cuttingFlag) && offset < 10000);

            ProcessCommands = builder.FinishTechOperation();
        }
    }
}
