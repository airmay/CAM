using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    [MenuItem("Распиловка по прямой", 1)]
    public class LineSawingTechOperation : CableSawingTechOperation
    {
        public override int StepCount => 1;

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject("AcadObjects")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddIndent()
                .AddParam(nameof(Approach), "Заезд")
                .AddParam(nameof(Departure), "Выезд")
                .AddIndent()
                .AddParam(nameof(IsRevereseDirection), "Обратное напр.")
                .AddParam(nameof(IsRevereseOffset), "Обратный Offset")
                .AddIndent()
                .AddParam(nameof(Delta))
                .AddParam(nameof(Delay), "Задержка");
        }

        public override Curve[] GetRailCurves(List<Curve> curves)
        {
            var bounds = AcadObjects.First().ObjectId.QOpenForRead<Entity>().Bounds.Value;

            var pts = curves.SelectMany(p => p.GetStartEndPoints()).ToList();
            var p0 = pts.OrderBy(p => p.DistanceTo(bounds.MaxPoint)).First();
            var rail0 = curves.Where(p => p.HasPoint(p0)).OrderBy(p => Math.Min(p.StartPoint.Z, p.EndPoint.Z)).First();
            if (rail0.StartPoint.Z < rail0.EndPoint.Z)
                rail0.ReverseCurve();
            var p1 = pts.OrderBy(p => p.DistanceTo(bounds.MinPoint)).First();
            var rail1 = curves.Where(p => p.HasPoint(p1)).OrderBy(p => Math.Max(p.StartPoint.Z, p.EndPoint.Z)).Last();

            return new Curve[] { rail0, rail1 };
        }

        //public override void BuildProcessing(CableCommandGenerator generator)
        //{
        //    CuttingFeed = CuttingFeed ?? TechProcess.CuttingFeed;
        //    S = S ?? TechProcess.S;
        //    Departure = Departure ?? TechProcess.Departure;

        //    var entity = ProcessingArea.ObjectId.QOpenForRead<Entity>();
        //    if (entity is Region region)
        //    {
        //        entity = new PlaneSurface();
        //        ((PlaneSurface)entity).CreateFromRegion(region);
        //    }
        //    var offsetDistance = TechProcess.ToolThickness / 2 + TechProcess.Delta;
        //    if (IsRevereseOffset)
        //        offsetDistance *= -1;
        //    var surface = DbSurface.CreateOffsetSurface(entity, offsetDistance) as DbSurface;

        //    var collection = new DBObjectCollection();
        //    surface.Explode(collection);

        //    var lines = collection.Cast<Line>().ToList();
        //    var pts = lines.SelectMany(p => p.GetStartEndPoints()).ToList();
        //    var p0 = pts.OrderBy(p => p.DistanceTo(surface.Bounds.Value.MaxPoint)).First();
        //    var rail0 = lines.Where(p => p.HasPoint(p0)).OrderBy(p => Math.Min(p.StartPoint.Z, p.EndPoint.Z)).First();

        //    //var v1 = lines.Where(p => p.HasPoint(region.Bounds.Value.MinPoint));
        //    //var c = lines.Select(p => new { start = $"{p.StartPoint.X.Round(2)} {p.StartPoint.Y.Round(2)} {p.StartPoint.Z.Round(2)}", end = $"{p.EndPoint.X.Round(2)} {p.EndPoint.Y.Round(2)} {p.EndPoint.Z.Round(2)}" }).ToList();
        //    var p1 = pts.OrderBy(p => p.DistanceTo(surface.Bounds.Value.MinPoint)).First();
        //    var rail1 = lines.Where(p => p.HasPoint(p1)).OrderBy(p => Math.Max(p.StartPoint.Z, p.EndPoint.Z)).Last();

        //    //var regionPpoints = collection.Cast<Line>().SelectMany(p => p.GetPoints()).ToList();
        //    //var railPoints0 = new Point3d[2];
        //    //var railPoints1 = new Point3d[2];
        //    //railPoints0[0] = region.Bounds.Value.MaxPoint;
        //    //railPoints1[1] = region.Bounds.Value.MinPoint;
        //    //railPoints0[1] = regionPpoints.Where(p => p.Z < railPoints1[1].Z + 10).OrderBy(p => p.DistanceTo(railPoints1[1])).Last();
        //    //railPoints1[0] = regionPpoints.Where(p => p.Z > railPoints0[0].Z - 10).OrderBy(p => p.DistanceTo(railPoints0[0])).Last();


        //    //var offsetVector = region.Normal * (TechProcess.ToolThickness / 2 + TechProcess.Delta);
        //    //if (IsRevereseOffset)
        //    //    offsetVector = offsetVector.Negate();

        //    var points0 = GetRailPoints(rail0, 1, surface.Bounds.Value.MaxPoint, Departure.Value, IsRevereseDirection);
        //    var points1 = GetRailPoints(rail1, 1, surface.Bounds.Value.MaxPoint, Departure.Value, IsRevereseDirection);

        //    for (int i = 0; i < points0.Length; i++)
        //    {
        //        var line = new Line2d(points0[i].ToPoint2d(), points1[i].ToPoint2d());
        //        var pNearest = line.GetClosestPointTo(TechProcess.Center).Point;
        //        var vector = pNearest - TechProcess.Center;
        //        var u = vector.Length;
        //        var z = (points0[i] + (points1[i] - points0[i]) / 2).Z;
        //        var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg(6);

        //        if (i == 0)
        //        {
        //            generator.GCommand(0, u, surface.Bounds.Value.MaxPoint.Z + TechProcess.ZSafety);
        //            generator.GCommandAngle(angle, S.Value);
        //            generator.GCommand(0, u, z);
        //            generator.Command($"M03", "Включение");
        //        }
        //        else
        //        {
        //            generator.GCommand(1, u, z, CuttingFeed);
        //            generator.GCommandAngle(angle, S.Value);
        //        }
        //    }
        //    generator.Command($"M05", "Выключение");
        //}

        //public Point3d[] GetRailPoints(Curve rail, int divs, Point3d startPoint, double departure, bool IsRevereseDirection)
        //{
        //    return AddDeparture(rail.GetPoints(divs).ToArray(), Departure.Value)
        //        .If(rail.StartPoint.DistanceTo(startPoint) > rail.EndPoint.DistanceTo(startPoint) ^ IsRevereseDirection, p => p.Reverse())
        //        .ToArray();
        //}

        //public IEnumerable<Point3d> AddDeparture(Point3d[] points, double departure)
        //{
        //    yield return points[0].GetExtendedPoint(points[1], departure);
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        yield return points[i];
        //    }
        //    yield return points[points.Length - 1].GetExtendedPoint(points[points.Length - 2], departure);
        //}
    }
}
