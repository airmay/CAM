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
        public bool Across { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject("AcadObjects")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddIndent()
                .AddParam(nameof(Approach), "Заезд")
                .AddParam(nameof(Departure), "Выезд")
                .AddIndent()
                .AddParam(nameof(Across), "Поперек")
                .AddParam(nameof(IsRevereseDirection), "Обратное напр.")
                .AddParam(nameof(IsRevereseAngle), "Обратный угол")
                .AddParam(nameof(IsRevereseOffset), "Обратный Offset")
                .AddIndent()
                .AddParam(nameof(Delta))
                .AddParam(nameof(Delay), "Задержка")
                .AddParam(nameof(StepCount), "Количество шагов")
                .AddParam(nameof(DU), "dU");
        }

        public LineSawingTechOperation()
        {
            StepCount = 100;
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

            if (AcadObjects.Count == 1 && dbObject is DbSurface surface1) // ----- Стелла ---------------------------
            {
                //if (o is Region region1)
                //{
                //    var planeSurface = new PlaneSurface();
                //    planeSurface.CreateFromRegion(region1);
                //    surface1 = planeSurface;
                //}

                if (IsRevereseOffset)
                    offsetDistance *= -1;
                var offsetSurface1 = DbSurface.CreateOffsetSurface(surface1, offsetDistance);

                //if (curves[0] is Region r)
                //{
                //    curves.Clear();
                //    r.Explode(curves);
                //}
                //var plane = offsetSurface.GetPlane();

                var curves1 = new DBObjectCollection();
                offsetSurface1.Explode(curves1);
                var railCurves = curves1.Cast<Curve>().OrderByDescending(p => Math.Abs(p.EndPoint.Z - p.StartPoint.Z)).Take(2).ToList();
                foreach(var curve in railCurves)
                    if (curve.StartPoint.Z < curve.EndPoint.Z ^ IsRevereseDirection)
                        curve.ReverseCurve();

                //if (Approach > 0)
                //    points.Add(railCurves.Select(p => p.StartPoint + Vector3d.ZAxis * Approach).ToArray());
                generator.DU = (DU / StepCount).Round(4);
                generator.GCommand(0, railCurves[0].StartPoint, railCurves[1].StartPoint, IsRevereseAngle);

                var stepCurves = railCurves.ConvertAll(p => new { Curve = p, step = (p.EndParam - p.StartParam) / StepCount });
                for (var i = 0; i <= StepCount; i++)
                {
                    var points = stepCurves.ConvertAll(p => p.Curve.GetPointAtParameter(i * p.step));

                    generator.GCommand(1, points[0], points[1]);
                }

                if (Departure > 0)
                {
                    var point0 = railCurves[0].EndPoint.GetExtendedPoint(railCurves[0].StartPoint, Departure);
                    var point1 = railCurves[1].EndPoint + (point0 - railCurves[0].EndPoint);
                    generator.DU = 0;
                    generator.GCommand(1, point0, point1);
                }

                return;
            }            

            if (AcadObjects.Count == 2)
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance);
                var railCurves = AcadObjects.Select(p => (Curve)p.ObjectId.QOpenForRead<Curve>().GetTransformedCopy(matrix))
                    .Select(p => new Line(p.StartPoint, p.EndPoint)).ToArray();
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
                points.Add(railCurves.Select(p => Departure >= 0 ? p.EndPoint : p.GetPointAtDist(p.Length + Departure)).ToArray());
                if (Departure > 0)
                    points.Add(railCurves.Select(p => p.EndPoint.GetExtendedPoint(p.StartPoint, Departure)).ToArray());

                generator.GCommand(0, points[0][0], points[0][1], IsRevereseAngle);

                for (int i = 1; i < points.Count; i++)
                {
                    generator.GCommand(1, points[i][0], points[i][1]);
                }

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

                foreach (var point in points)
                {
                    var line = new Line3d(point, rail.Delta.GetPerpendicularVector());
                    if (point == points[0])
                    {
                        generator.GCommand(0, line);
                    }
                    else
                    {
                        generator.GCommand(1, line);
                    }
                }

                return;
            }


            var surface = dbObject as DbSurface;
            if (dbObject is Region region)
            {
                var planeSurface = new PlaneSurface();
                planeSurface.CreateFromRegion(region);
                surface = planeSurface;
            }
            
            surface.GeometricExtents.GetCenter();
            var basePoint = surface.GeometricExtents.GetCenter();

            if (IsRevereseOffset)
                offsetDistance *= -1;
            var offsetSurface = DbSurface.CreateOffsetSurface(surface, offsetDistance);
            var basePointOffset = offsetSurface.GeometricExtents.GetCenter();

            //if (curves[0] is Region r)
            //{
            //    curves.Clear();
            //    r.Explode(curves);
            //}
            //var plane = offsetSurface.GetPlane();

            var curves = new DBObjectCollection();
            offsetSurface.Explode(curves);
            var pts = curves.Cast<Curve>().SelectMany(p => p.GetStartEndPoints()).OrderBy(x => x.Z).ToList();
            var maxPoint = pts.Last();
            var minPoint = pts.First();

            if (maxPoint.Z - minPoint.Z < 10) // горизонтальная
            {
                var matrix = Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance);
                var railCurvesAll = curves.Cast<Curve>().ToList();
                var railCurves = new Curve[2];
                var rc = railCurvesAll.Where(p => p.HasPoint(maxPoint)).OrderBy(p => p.Length());
                railCurves[0] = Across ?  rc.First() : rc.Last();
                railCurves[1] = railCurvesAll.Where(p => !p.HasPoint(railCurves[0].StartPoint) && !p.HasPoint(railCurves[0].EndPoint)).First();

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
                points.Add(railCurves.Select(p => Approach >= 0 ? p.StartPoint : p.GetPointAtDist(-Approach)).ToArray());
                points.Add(railCurves.Select(p => p.EndPoint).ToArray());
                if (Departure > 0)
                    points.Add(railCurves.Select(p => p.EndPoint.GetExtendedPoint(p.StartPoint, Departure)).ToArray());

                generator.GCommand(0, points[0][0], points[0][1], IsRevereseAngle);

                for (int i = 1; i < points.Count; i++)
                {
                    generator.GCommand(1, points[i][0], points[i][1]);
                }

                return;
            }


            var baseCurves = curves.Cast<Curve>().Where(p => p.HasPoint(maxPoint)).ToArray();

            var plane = new Plane(maxPoint, baseCurves[0].EndPoint - baseCurves[0].StartPoint, baseCurves[1].EndPoint - baseCurves[1].StartPoint);

            var zStart = pts.Last().Z;
            var zEnd = pts.First().Z;
            var zPos = new List<double>();
            if (Approach > 0)
                zPos.Add(zStart + Approach);
            if (Approach < 0)
                zStart += Approach;
            zPos.Add(zStart);
            if (Departure < 0)
                zEnd -= Departure;
            zPos.Add(zEnd);
            if (Departure > 0)
                zPos.Add(zEnd - Departure);

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
                    generator.GCommand(0, line, IsRevereseAngle);
                }
                else
                {
                    generator.GCommand(1, line);
                    //generator.GCommand(1, u, z, CuttingFeed);
                }
            }
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
