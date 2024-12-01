using Autodesk.AutoCAD.DatabaseServices;
/*
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    [MenuItem("Распиловка по дуге", 2)]
    public class ArcSawingTechOperation : CableSawingTechOperation
    {
        public static void ConfigureParamsView(ParamsView view)
        {
            //view.AddAcadObject("AcadObjects")
            //    //.AddParam(nameof(CuttingFeed))
            //    .AddParam(nameof(S), "Угловая скорость")
            //    .AddIndent()
            //    .AddParam(nameof(Approach), "Заезд")
            //    .AddParam(nameof(Departure), "Выезд")
            //    .AddIndent()
            //    .AddParam(nameof(IsRevereseDirection), "Обратное напр.")
            //    .AddParam(nameof(IsRevereseOffset), "Обратный Offset")
            //    .AddIndent()
            //    .AddParam(nameof(Delta))
            //    .AddParam(nameof(Delay), "Задержка")
            //    .AddParam(nameof(StepCount), "Количество шагов");
        }

        public ArcSawingTechOperation()
        {
            StepCount = 100;
        }

        public override Curve[] GetRailCurves(List<Curve> curves)
        {
            var cv = curves.Cast<Curve>().Where(p => !(p is Line)).ToArray();
            if (cv.Length > 2)
                //cv = cv.Where(p => !(p is Arc)).ToArray();
                return new Curve[2] { cv.First(p => p is Arc), cv.OrderBy(p => p.Length()).First(p => p is Spline) };
            else
                return cv;
        }

        private Polyline3d GetRail(Point3d startPoint, Curve curve1, Curve curve2)
        {
                if (!curve1.StartPoint.IsEqualTo(startPoint))
                curve1.ReverseCurve();

            Polyline3d polyline = null;

            if (curve1 is Line line)
            {
                polyline = line.ToPolyline3d();
            }
            else if (curve1 is Spline spline)
            {
                polyline = (Polyline3d)spline.ToPolyline();
            }

            ObjectId objectId = ObjectId.Null;
            App.LockAndExecute(() => objectId = polyline.AddToCurrentSpace());

            var ep = polyline.EndPoint;
            if (!curve2.StartPoint.IsEqualTo(ep))
                curve2.ReverseCurve();

            App.LockAndExecute(() =>
            {
                objectId.QOpenForWrite<Polyline3d>(pl =>
                {
                    if (curve2 is Line line2)
                    {
                        var pt = line2.NextPoint(ep);
                        pl.AppendVertex(new PolylineVertex3d(pt));
                    }
                    else if (curve2 is Spline spline2)
                    {
                        foreach (var vertex in (Polyline3d)spline2.ToPolyline())
                            if (!((PolylineVertex3d)vertex).Position.IsEqualTo(ep))
                            {
                                var v = new PolylineVertex3d(((PolylineVertex3d)vertex).Position);

                                pl.AppendVertex(v);

                            }
                    }
                });
            });
            return objectId.QOpenForRead<Polyline3d>();
        }

        public override void BuildProcessing(CableCommandGenerator generator)
        {
            //var dbObject = ProcessingArea.ObjectId.QOpenForRead();
            var surface = AcadObjects.First().ObjectId.QOpenForRead<DbSurface>();
            var offsetDistance = TechProcess.ToolThickness / 2 + Delta;
            if (IsRevereseOffset)
                offsetDistance *= -1;
            //var offsetSurface = DbSurface.CreateOffsetSurface(surface, offsetDistance);
            var collection = new DBObjectCollection();
            surface.Explode(collection);

            var curves = collection.Cast<Curve>().ToList();
            var vertex = curves.SelectMany(p => p.GetStartEndPoints()).OrderBy(p => p.Z).ToList();
            var maxPoint = vertex.Last();
            var minPoint = vertex.First();
            var maxCurves = curves.FindAll(p => p.HasPoint(maxPoint));
            var minCurves = curves.FindAll(p => p.HasPoint(minPoint));

            //var rail1s = new List<Curve>
            //{
            //    maxCurves[0],
            //    minCurves.Single(p => p.HasPoint(maxCurves[0].StartPoint) || p.HasPoint(maxCurves[0].EndPoint))
            //};
            //var rail2s = new List<Curve>
            //{
            //    maxCurves[1],
            //    minCurves.Single(p => p != rail1s[1])
            //};

            //var rail1 = ((Spline)maxCurves[0]).ToPolyline(); // GetRail(maxPoint, maxCurves[0], minCurves.Single(p => p.HasPoint(maxCurves[0].StartPoint) || p.HasPoint(maxCurves[0].EndPoint)));
            //var rail2 = ((Spline)maxCurves[1]).ToPolyline(); //  GetRail(maxPoint, maxCurves[1], minCurves.Single(p => p.HasPoint(maxCurves[1].StartPoint) || p.HasPoint(maxCurves[1].EndPoint)));

            var rail1 = new Rail(maxPoint, maxCurves[0], curves);
            var rail2 = new Rail(maxPoint, maxCurves[1], curves);

            //var length1 = rail1.GetDistanceAtParameter(rail1.EndParam);
            //var length2 = rail2.GetDistanceAtParameter(rail2.EndParam);
            //double dist1 = 0;
            //double dist2 = 0;
            double step = 2;

            generator.Feed = CuttingFeed;
            generator.S = S;
            generator.Center = new Point2d(TechProcess.OriginX, TechProcess.OriginY);

            while (!rail1.IsEnd && !rail2.IsEnd)
            {
                var pt1 = rail1.Step(step);
                var pt2 = rail2.Step(step);

                if (pt2.Z - pt1.Z > 2)
                    rail2.StepToZ(pt1.Z, 0.1);
                else
                    if (pt1.Z - pt2.Z > 2)
                        rail1.StepToZ(pt2.Z, 0.1);

                //dist1 += step;
                //if (dist1 >= length1)
                //    break;
                //var pt1 = rail1.GetPointAtDist(dist1);

                //dist2 += step;
                //if (dist2 >= length2)
                //    break;
                //var pt2 = rail2.GetPointAtDist(dist2);

                //if (pt1.Z < pt2.Z)
                //{
                //    var dist = dist2;
                //    var pt = pt2;
                //    while(dist < length2 && pt.Z > pt1.Z)
                //    {
                //        dist2 = dist;
                //        pt2 = pt;

                //        dist += step;
                //        pt = rail2.GetPointAtDist(dist);
                //    }
                //    if (dist < length2 && pt1.Z - pt.Z < pt2.Z - pt1.Z)
                //    {
                //        dist2 = dist;
                //        pt2 = pt;
                //    }
                //}
                //else
                //{
                //    var dist = dist1;
                //    var pt = pt1;
                //    while (dist < length1 && pt.Z > pt2.Z)
                //    {
                //        dist1 = dist;
                //        pt1 = pt;

                //        dist += step;
                //        pt = rail1.GetPointAtDist(dist);
                //    }
                //    if (dist < length1 && pt2.Z - pt.Z < pt1.Z - pt2.Z)
                //    {
                //        dist1 = dist;
                //        pt1 = pt;
                //    }
                //}

                generator.GCommand(1, rail1.Point, rail2.Point);
            }
        }

        public class Rail
        {
            private readonly Curve _curve1;
            private readonly Curve _curve2;
            private readonly double _length1;
            private readonly double _allLength;
            private double _dist;
            public Point3d Point;

            public bool IsEnd;

            public Rail(Point3d startPoint, Curve curve1, List<Curve> curves)
            {
                _curve1 = curve1;
                if (!_curve1.StartPoint.IsEqualTo(startPoint))
                    _curve1.ReverseCurve();

                _curve2 = curves.Single(p => p != curve1 && p.HasPoint(_curve1.EndPoint));
                if (!_curve2.StartPoint.IsEqualTo(_curve1.EndPoint))
                    _curve2.ReverseCurve();

                _length1 = _curve1.GetDistanceAtParameter(_curve1.EndParam);
                _allLength = _length1 + _curve2.GetDistanceAtParameter(_curve2.EndParam);// - 50;

                Point = startPoint;
                _dist = 0;
                IsEnd = false;
            }

            public Point3d Step(double step)
            {
                _dist += step;
                if (_dist >= _allLength)
                {
                    IsEnd = true;
                    _dist = _allLength;
                }
                return Point = _dist < _length1 ? _curve1.GetPointAtDist(_dist) : _curve2.GetPointAtDist(_dist - _length1);
            }

            public Point3d StepToZ(double z, double step)
            {
                var dist = _dist;
                var point = Point;

                while (Point.Z - z > 2 && !IsEnd)
                {
                    dist = _dist;
                    point = Point;

                    Step(step);
                }

                if (point.Z - z < z - Point.Z)
                {
                    _dist = dist;
                    Point = point;
                }
                return Point;
            }
        }

        //public void BuildProcessing(CableCommandGenerator generator)
        //{
        //    CuttingFeed = CuttingFeed ?? TechProcess.CuttingFeed;
        //    S = S ?? TechProcess.S;
        //    Departure = Departure ?? TechProcess.Departure;

        //    var surfaceOrigin = ProcessingArea.ObjectId.QOpenForRead<Entity>();
        //    var surface = DbSurface.CreateOffsetSurface(surfaceOrigin, TechProcess.ToolThickness / 2 + TechProcess.Delta) as DbSurface;

        //    var collection = new DBObjectCollection();
        //    surface.Explode(collection);
        //    var curves = collection.Cast<Curve>().Where(p => !(p is Line)).ToList();
        //    var dir0 = ((curves[0].EndPoint - Point3d.Origin).Length > (curves[0].StartPoint - Point3d.Origin).Length);// ^ IsReverese;
        //    var dir1 = ((curves[1].EndPoint - Point3d.Origin).Length > (curves[1].StartPoint - Point3d.Origin).Length);// ^ IsReverese;

        //    var pt0 = new Point3d[StepCount + 3];
        //    var pt1 = new Point3d[StepCount + 3];
        //    for (int i = 0; i <= StepCount; i++)
        //    {
        //        pt0[i + 1] = curves[0].GetPointAtDist(curves[0].Length() / StepCount * (dir0 ? i : StepCount - i));
        //        pt1[i + 1] = curves[1].GetPointAtDist(curves[1].Length() / StepCount * (dir1 ? i : StepCount - i));
        //    }
        //    pt0[0] = pt0[1].GetExtendedPoint(pt0[2], Departure.Value);
        //    pt1[0] = pt1[1].GetExtendedPoint(pt1[2], Departure.Value);
        //    pt0[StepCount + 2] = pt0[StepCount + 1].GetExtendedPoint(pt0[StepCount], Departure.Value);
        //    pt1[StepCount + 2] = pt1[StepCount + 1].GetExtendedPoint(pt1[StepCount], Departure.Value);

        //    for (int i = 0; i < pt0.Length; i++)
        //    {
        //        var line = new Line2d(pt0[i].ToPoint2d(), pt1[i].ToPoint2d());
        //        var pNearest = line.GetClosestPointTo(TechProcess.Center).Point;
        //        var vector = pNearest - TechProcess.Center;
        //        var u = vector.Length;
        //        var z = (pt0[i] + (pt1[i] - pt0[i]) / 2).Z;
        //        var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg();

        //        if (i == 0)
        //        {
        //            generator.GCommand(0, u, 0);
        //            generator.GCommandAngle(angle, S.Value);
        //            generator.GCommand(0, 0, z);
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

    }
}
*/