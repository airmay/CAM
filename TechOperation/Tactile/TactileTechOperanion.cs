using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.TechOperation.Tactile
{
    [Serializable]
    public class TactileTechOperanion : TechOperationBase
    {
        public override ProcessingType Type => ProcessingType.Tactile;

        public TactileParams TactileParams { get; }

        public override object Params => TactileParams;

        public TactileTechOperanion(TechProcess techProcess, ProcessingArea processingArea, TactileParams tactileParams, string name)
            : base(techProcess, processingArea, name)
        {
            TactileParams = tactileParams;
        }

        public void CalcPassList()
        {
            var toolThickness = TechProcess.TechProcessParams.ToolThickness;
            var periodAll = TactileParams.GutterWidth - toolThickness;
            var periodWidth = toolThickness + toolThickness - 1;
            var count = Math.Ceiling(periodAll / periodWidth);
            periodWidth = periodAll / count;
            TactileParams.PassList = new List<Pass>();
            for (int i = 0; i < count; i++)
                TactileParams.PassList.Add(new Pass(i * periodWidth + toolThickness, CuttingType.Roughing));

            TactileParams.PassList.Add(new Pass(TactileParams.GutterWidth, CuttingType.Roughing));

            var x = periodWidth + (toolThickness - (periodWidth - toolThickness)) / 2;
            for (int i = 0; i < count; i++)
                TactileParams.PassList.Add(new Pass(i * periodWidth + x, CuttingType.Finishing));
        }

        public override List<CuttingSet> GetCutting()
        {
            if (TactileParams.PassList == null)
                CalcPassList();
            var cuttingSets = new List<CuttingSet>();
            var points = ProcessingArea.Curves.Select(p => p.StartPoint).Concat(ProcessingArea.Curves.Select(p => p.EndPoint)).Distinct();
            var xMin = points.Min(p => p.X);
            var basePoint = points.Where(p => p.X == xMin).OrderBy(p => p.Y).First();
            var basePoint2 = ProcessingArea.Curves.Where(p => p.HasPoint(basePoint)).OrderBy(p => Math.Abs(p.StartPoint.Y - p.EndPoint.Y)).First().NextPoint(basePoint);

            var ray = new Ray()
            {
                BasePoint = basePoint,
                SecondPoint = basePoint2
            };
            if (TactileParams.Type.Contains("Диагональные"))
                ray.UnitDir = ray.UnitDir.RotateBy(Math.PI / 4, -Vector3d.ZAxis);

            cuttingSets.Add(CalcCuttingSet(basePoint, ray, TactileParams.FeedRoughing1, TactileParams.FeedFinishing1));

            if (TactileParams.Type.Contains("квадраты"))
            {
                ray.UnitDir = -ray.UnitDir.GetPerpendicularVector();
                if (TactileParams.Type.Contains("Диагональные"))
                {
                    basePoint = basePoint2;
                    ray.UnitDir = -ray.UnitDir;
                }
                cuttingSets.Add(CalcCuttingSet(basePoint, ray, TactileParams.FeedRoughing2, TactileParams.FeedFinishing2));
            }
            return cuttingSets;
        }

        private CuttingSet CalcCuttingSet(Point3d basePoint, Ray ray, int feedRoughing, int feedFinishing)
        {
            var passDir = ray.UnitDir.GetPerpendicularVector();
            var points = new List<Point3d>();
            double offset = TactileParams.TopStart == 0 ? TactileParams.TopWidth : TactileParams.TopStart - TactileParams.GutterWidth;
            bool cuttingFlag = false;
            
            var cuttingPassList = new List<CuttingPass>();
            do
            {
                foreach (var pass in TactileParams.PassList)
                {
                    ray.BasePoint = basePoint + passDir * (offset + pass.Pos);
                    points.Clear();
                    foreach (var curve in ProcessingArea.Curves)
                    {
                        var intersectPoints = new Point3dCollection();
                        ray.IntersectWith(curve, Intersect.ExtendThis, intersectPoints, 0, 0);
                        if (intersectPoints.Count == 1)
                            points.Add(intersectPoints[0]);
                    }
                    if (points.Count == 2)
                    {
                        var vector = (points[1] - points[0]).GetNormal() * TactileParams.Departure;
                        var line = NoDraw.Line(points[0] - vector - Vector3d.ZAxis * TactileParams.Depth, points[1] + vector - Vector3d.ZAxis * TactileParams.Depth);
                        cuttingPassList.Add(new CuttingPass(line, pass.CuttingType == "Гребенка" ? feedRoughing : feedFinishing));
                        cuttingFlag = true;
                    }
                }
                offset += TactileParams.GutterWidth + TactileParams.TopWidth;
            }
            while (points.Count != 0 || !cuttingFlag);

            return new CuttingSet
            {
                Cuttings = cuttingPassList,
                StartCorner = Corner.Start
            };
        }
    }
}
