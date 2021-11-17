﻿using Autodesk.AutoCAD.DatabaseServices;
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

        public void BuildProcessing(CableCommandGenerator generator)
        {
            var offsetDistance = TechProcess.ToolThickness / 2 + Delta;
            var dbObject = AcadObjects.First().ObjectId.QOpenForRead();

            if (AcadObjects.Count == 2 && dbObject is Line)
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance);
                var railCurves = AcadObjects.Select(p => (Line)p.ObjectId.QOpenForRead<Entity>().GetTransformedCopy(matrix)).ToArray();

                if (railCurves[0].StartPoint.GetVectorTo(railCurves[0].EndPoint).GetAngleTo(railCurves[1].StartPoint.GetVectorTo(railCurves[1].EndPoint)) > Math.PI / 2)
                    railCurves[1].ReverseCurve();

                if (IsRevereseDirection)
                {
                    railCurves[0].ReverseCurve();
                    railCurves[1].ReverseCurve();
                }

                var points = new List<Point3d[]>();
                if (Approach > 0)
                    points.Add(railCurves.Select(p => p.StartPoint.GetExtendedPoint(p.EndPoint, Approach)).ToArray());
                //if (Approach < 0)
                //    zStart += Approach;
                points.Add(railCurves.Select(p => p.StartPoint).ToArray());
                points.Add(railCurves.Select(p => p.EndPoint).ToArray());
                if (Departure > 0)
                    points.Add(railCurves.Select(p => p.EndPoint.GetExtendedPoint(p.StartPoint, Departure)).ToArray());

                generator.S = S;
                generator.Feed = CuttingFeed;
                var z00 = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + TechProcess.ZSafety;
                generator.SetToolPosition(new Point3d(TechProcess.OriginX, TechProcess.OriginY, 0), 0, 0, z00);
                generator.Command($"G92");

                generator.GCommand(0, points[0][0], points[0][1]);
                generator.Command($"M03", "Включение");

                for (int i = 1; i < points.Count; i++)
                {
                    generator.GCommand(1, points[i][0], points[i][1]);
                }
                generator.Command($"G04 P{Delay}", "Задержка");
                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");

                return;
            }

            if (dbObject is Line rail)
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance);
                rail = (Line)rail.GetTransformedCopy(matrix);

                //if (railCurves[0].StartPoint.GetVectorTo(railCurves[0].EndPoint).GetAngleTo(railCurves[1].StartPoint.GetVectorTo(railCurves[1].EndPoint)) > Math.PI / 2)
                //    railCurves[1].ReverseCurve();

                var points = new List<Point3d>();
                if (Approach > 0)
                    points.Add(rail.StartPoint.GetExtendedPoint(rail.EndPoint, Approach));
                //if (Approach < 0)
                //    zStart += Approach;
                points.Add(rail.StartPoint);
                points.Add(rail.EndPoint);
                if (Departure > 0)
                    points.Add(rail.EndPoint.GetExtendedPoint(rail.StartPoint, Departure));

                generator.S = S;
                generator.Feed = CuttingFeed;
                var z00 = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + TechProcess.ZSafety;
                generator.SetToolPosition(new Point3d(TechProcess.OriginX, TechProcess.OriginY, 0), 0, 0, z00);
                generator.Command($"G92");

                foreach (var point in points)
                {
                    var line = new Line3d(point, rail.Delta.GetPerpendicularVector());
                    if (point == points[0])
                    {
                        generator.GCommand(0, line);
                        generator.Command($"M03", "Включение");
                    }
                    else
                    {
                        generator.GCommand(1, line);
                    }
                }
                generator.Command($"G04 P{Delay}", "Задержка");
                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");

                return;
            }


            var surface = dbObject as PlaneSurface;
            if (dbObject is Region region)
            {
                surface = new PlaneSurface();
                surface.CreateFromRegion(region);
            }            
            if (IsRevereseOffset)
                offsetDistance *= -1;
            var offsetSurface = DbSurface.CreateOffsetSurface(surface, offsetDistance);
            var curves = new DBObjectCollection();
            offsetSurface.Explode(curves);
            if (curves[0] is Region r)
            {
                curves.Clear();
                r.Explode(curves);
            }
            var plane = offsetSurface.GetPlane();

            var zl = curves.Cast<Curve>().SelectMany(p => p.GetStartEndPoints().Select(x => x.Z)).ToList();
            var zStart = zl.Max();
            var zEnd = zl.Min();
            var zPos = new List<double>();
            if (Approach > 0)
                zPos.Add(zStart + Approach);
            if (Approach < 0)
                zStart += Approach;
            zPos.Add(zStart);
            zPos.Add(zEnd);
            if (Departure > 0)
                zPos.Add(zEnd - Departure);

            generator.S = S;
            generator.Feed = CuttingFeed;
            var z0 = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + TechProcess.ZSafety;
            generator.SetToolPosition(new Point3d(TechProcess.OriginX, TechProcess.OriginY, 0), 0, 0, z0);
            generator.Command($"G92");

            foreach (var z in zPos)
            {
                var line = plane.IntersectWith(new Plane(new Point3d(0, 0, z), Vector3d.ZAxis));
                var u = line.GetDistanceTo(new Point3d(TechProcess.OriginX, TechProcess.OriginY, z));
                if (z == zPos[0])
                {
                    //var angle = line.Direction.ToVector2d().MinusPiToPiAngleTo(Vector2d.YAxis);
                    //generator.GCommandAngle(line.Direction.ToVector2d(), S);
                    //generator.GCommand(0, u);
                    //generator.GCommand(0, u, z);
                    generator.GCommand(0, line);
                    generator.Command($"M03", "Включение");
                }
                else
                {
                    generator.GCommand(1, line);
                    //generator.GCommand(1, u, z, CuttingFeed);
                }
            }
            generator.Command($"G04 P{Delay}", "Задержка");
            generator.Command($"M05", "Выключение");
            generator.Command($"M00", "Пауза");
        }
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
