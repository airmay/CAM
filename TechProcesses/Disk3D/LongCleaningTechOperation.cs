using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CAM.TechProcesses.Disk3D
{
    [Serializable]
    [MenuItem("Продольная чистка", 2)]
    public class LongCleaningTechOperation : MillingTechOperation<Disk3DTechProcess>
    {
        private Disk3DTechProcess _disk3DTechProcess;

        public double StartPass { get; set; }

        public double EndPass { get; set; }

        public double StepY { get; set; }

        public double YMax { get; set; }

        public bool IsReverse { get; set; }

        public double Delta { get; set; }

        public double StepX1 { get; set; }

        public double StepX2 { get; set; }

        public int CuttingFeed { get; set; }

        public double Departure { get; set; }

        public bool IsDepartureOnBorderSection { get; set; }

        public bool IsUplifting { get; set; }

        public double StepZ { get; set; }

        public LongCleaningTechOperation(Disk3DTechProcess techProcess, string caption) : base(techProcess, caption)
        {
            StepX1 = 1;
            StepX2 = 1;
            StepY = 1;
            YMax = 4;
            StepZ = 1;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddParam(nameof(StepX1), "Шаг X1")
                .AddParam(nameof(StepX2), "Шаг X2")
                .AddIndent()
                .AddParam(nameof(StartPass), "Начало")
                .AddParam(nameof(EndPass), "Конец")
                .AddParam(nameof(StepY), "Шаг Y мин.")
                .AddParam(nameof(YMax), "Шаг Y макс.")
                .AddParam(nameof(IsReverse), "Обратно")
                .AddIndent()
                .AddParam(nameof(StepZ), "Шаг Z")
                .AddIndent()
                .AddParam(nameof(Departure))
                .AddParam(nameof(IsDepartureOnBorderSection), "Выезд по границе сечения")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(Delta))
                .AddParam(nameof(IsUplifting));
        }

        public override void PrepareBuild(MillingCommandGenerator generator)
        {
            var bounds = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents();
            generator.ZSafety = bounds.MaxPoint.Z + TechProcess.ZSafety;
            generator.ToolPosition.Point += Vector3d.ZAxis * generator.ZSafety;
        }

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            //var progressMeter = new ProgressMeter();
            //progressMeter.Start($"test");
            //progressMeter.SetLimit(100);
            //for (int i = 0; i < 100; i++)
            //{
            //    progressMeter.MeterProgress();
            //    Thread.Sleep(100);
            //}
            ////progressMeter.Start($"test111");
            //progressMeter.SetLimit(100);
            //for (int i = 0; i < 100; i++)
            //{
            //    progressMeter.MeterProgress();
            //    Thread.Sleep(100);
            //}
            //progressMeter.Stop();
            //return;

            //Draw.Pline(new Point3d(0, 1000, 0), new Point3d(1000, 1000, 0), new Point3d(1000, 1200, 0), new Point3d(2000, 1200, 0));
            //var points1 = new Point3dCollection(new Point3d[] { new Point3d(0, 1000, 0), new Point3d(1000, 1000, 0), new Point3d(1000, 1200, 0), new Point3d(2000, 1200, 0) });
            //var pline = new PolylineCurve3d(points1);

            //    var distance = TechProcess.Tool.Diameter / 2;
            //    var offsetCurves = pline.GetTrimmedOffset(100, Vector3d.ZAxis, OffsetCurveExtensionType.Fillet);

            //var cInt = new CurveCurveIntersector3d(offsetCurves[0], new Line3d(new Point3d(0, 0, 0), new Point3d(0, 3000, 0)), Vector3d.ZAxis);
            //var dbp = new List<DBPoint>();
            //for (int i = 0; i < 2000; i++)
            //{
            //    cInt.Set(offsetCurves[0], new Line3d(new Point3d(i, 0, 0), new Point3d(i, 3000, 0)), Vector3d.ZAxis);

            //    Point3d pnt3d = cInt.GetIntersectionPoint(0);

            //    dbp.Add(new DBPoint(pnt3d - Vector3d.YAxis * 100));

            //}
            //cInt.Dispose();
            //dbp.AddToCurrentSpace();


            _disk3DTechProcess = (Disk3DTechProcess)TechProcess;

            var offsetSurface = CreateOffsetSurface();

            var matrix = Matrix3d.Rotation(_disk3DTechProcess.Angle.ToRad(), Vector3d.ZAxis, Point3d.Origin);
            if (_disk3DTechProcess.Angle != 0)
                offsetSurface.TransformBy(matrix);

            var minPoint = offsetSurface.GeometricExtents.MinPoint;
            if (StartPass != 0)
                minPoint = new Point3d(minPoint.X, StartPass, minPoint.Z);
            var maxPoint = offsetSurface.GeometricExtents.MaxPoint; // - Vector3d.XAxis * 1000;
            if (EndPass != 0)
                maxPoint = new Point3d(maxPoint.X, EndPass, maxPoint.Z);

            var collections = GetPointCollections(offsetSurface, minPoint, maxPoint);

            offsetSurface.Dispose();

            var zArray = GetZArray(collections, (maxPoint - minPoint).X);
            //var pointsArray = CalcOffsetPoints(polylines, minPoint, maxPoint);

            var passList = CalcPassList(zArray, minPoint);

            matrix = matrix.Inverse();
            if (IsReverse)
                passList.Reverse();

            passList.ForEach(p =>
            {
                var points = p;

                if (!points.Any())
                    return;

                if (Departure > 0)
                {
                    points.Insert(0, new Point3d((IsDepartureOnBorderSection ? points.First().X : minPoint.X) - Departure, points.First().Y, points.First().Z));
                    points.Add(new Point3d((IsDepartureOnBorderSection ? points.Last().X : maxPoint.X) + Departure, points.Last().Y, points.Last().Z));
                }
                if (TechProcess.MachineType == MachineType.Donatoni) //Settongs.IsFrontPlaneZero
                    points = points.ConvertAll(x => new Point3d(x.X, x.Y - TechProcess.Tool.Thickness.Value, x.Z));
                if (_disk3DTechProcess.Angle != 0)
                    points = points.ConvertAll(x => x.TransformBy(matrix));
                var loc = generator.ToolPosition;
                if (loc.IsDefined && loc.Point.DistanceTo(points.First()) > loc.Point.DistanceTo(points.Last()))
                    points.Reverse();

                BuildPass(generator, points);

                if (IsUplifting)
                    generator.Uplifting();
            });
        }

        private DbSurface CreateOffsetSurface()
        {
            DbSurface unionSurface = null;
            foreach (var dBObject in TechProcess.ProcessingArea.Select(p => p.ObjectId.QOpenForRead()))
            {
                DbSurface surface;
                switch (dBObject)
                {
                    case DbSurface sf:
                        surface = sf.Clone() as DbSurface;
                        break;
                    case Region region:
                        surface = new PlaneSurface();
                        ((PlaneSurface)surface).CreateFromRegion(region);
                        break;
                    default:
                        throw new Exception(ErrorStatus.NotImplementedYet, $"Объект типа {dBObject.GetType()} не может быть обработан (1)");
                }
                if (unionSurface == null)
                    unionSurface = surface;
                else
                {
                    var res = unionSurface.BooleanUnion(surface);
                    if (res != null)
                    {
                        unionSurface.Dispose();
                        unionSurface = res;
                    }
                    surface.Dispose();
                }
            }
            if (Delta == 0)
                return unionSurface;

            try
            {
                var offsetSurface = DbSurface.CreateOffsetSurface(unionSurface, Delta) as DbSurface;
                unionSurface.Dispose();
                return offsetSurface;
            }
            catch
            {
                unionSurface.TransformBy(Matrix3d.Displacement(Vector3d.ZAxis * Delta));
                return unionSurface;
            }
        }

        private Point2dCollection[] GetPointCollections(DbSurface offsetSurface, Point3d minPoint, Point3d maxPoint)
        {
            var countY = (int)((maxPoint.Y - minPoint.Y) / StepY) + 1;
            var countX = (int)((maxPoint.X - minPoint.X) / StepX1) + 1;
            var collections = new Point2dCollection[countY];

            Acad.SetLimitProgressor(countY);

            for (var i = 0; i < countY; i++)
            {
                Acad.ReportProgressor();
                var collection = new Point2dCollection();
                var dy = i * StepY;

                for (var j = 0; j < countX; j++)
                {
                    var dx = j * StepX1;

                    offsetSurface.RayTest(minPoint + new Vector3d(dx, dy, 0), Vector3d.ZAxis, 0.0001, out SubentityId[] col, out DoubleCollection par);

                    if (par.Count == 1)
                    {
                        var point = new Point2d(dx, Math.Round(par[0], 2));
                        var ind = collection.Count - 1;
                        if (ind > 0 && collection[ind - 1].GetVectorTo(collection[ind]).IsCodirectionalTo(collection[ind].GetVectorTo(point)))
                            collection[ind] = point;
                        else
                            collection.Add(point);
                    }
                }
                if (collection.Count > 1)
                    collections[i] = collection;
            }
            return collections;
        }

        private double?[,] GetZArray(Point2dCollection[] collections, double sizeX)
        {
            var distance = TechProcess.Tool.Diameter / 2;
            var countX = (int)(sizeX / StepX2) + 1;
            var zArray = new double?[collections.Length, countX];
            var intersector = new CurveCurveIntersector2d();

            Acad.SetLimitProgressor(collections.Length);

            var rays = Enumerable.Range(0, countX).Select(p => new Ray2d(new Point2d(p * StepX2, 0), Vector2d.YAxis)).ToList();

            for (int i = 0; i < collections.Length; i++)
            {
                Acad.ReportProgressor();

                if (collections[i] == null)
                    continue;

                var polylene = new PolylineCurve2d(collections[i]);
                var offsetCurve = polylene.GetTrimmedOffset(distance, OffsetCurveExtensionType.Fillet)[0];

                for (int j = 0; j < countX; j++)
                {
                    intersector.Set(offsetCurve, rays[j]);
                    if (intersector.NumberOfIntersectionPoints == 1)
                        zArray[i, j] = intersector.GetIntersectionPoint(0).Y - distance;
                }
                //if (i < 50)
                //{
                //    Draw.Pline(polylene.GetSamplePoints(10).Select(p => new Point3d(p.X + i * 100, p.Y + 1000, 0)));
                //    Draw.Pline(offsetCurve.GetSamplePoints(10).Select(p => new Point3d(p.X + i * 100, p.Y + 1000, 0)));
                //}
                //else
                //{
                    polylene.Dispose();
                    offsetCurve.Dispose();
                //}
            }
            rays.ForEach(p => p.Dispose());
            intersector.Dispose();

            return zArray;
        }

        private List<List<Point3d>> CalcPassList(double?[,] zArray, Point3d minPoint)
        {
            var w = (int)(TechProcess.Tool.Thickness / StepY);
            var maxDist = (int)(YMax / StepY);
            var passZArray = new double?[zArray.GetLength(0) + (_disk3DTechProcess.IsExactlyEnd ? 0 : maxDist)][];

            Parallel.For(0, passZArray.Length, i =>
            {
                passZArray[i] = new double?[zArray.GetLength(1)];

                for (int j = 0; j < zArray.GetLength(1); j++)
                {
                    for (int k = 0; k <= w && i - k >= 0; k++)
                    {
                        if (i - k < zArray.GetLength(0) && zArray[i - k, j].GetValueOrDefault(double.MinValue) > passZArray[i][j].GetValueOrDefault(double.MinValue))
                            passZArray[i][j] = zArray[i - k, j];
                    }
                }
            });

            var passList = new List<List<Point3d>>();

            var startIndex = _disk3DTechProcess.IsExactlyBegin ? w : 1;
            int lastPassIndex = 0;
            int lastPassDist = startIndex - 1;

            for (int i = startIndex; i < passZArray.Length; i++)
            {
                lastPassDist++;

                if (lastPassDist > maxDist)
                {
                    AddPass(i-1);
                    continue;
                }
                for (int j = 0; j < zArray.GetLength(1); j++)
                {
                    if (passZArray[i][j].HasValue && passZArray[lastPassIndex][j].HasValue && Math.Abs(passZArray[i][j].Value - passZArray[lastPassIndex][j].Value) > StepZ)
                    {
                        AddPass(i-1);
                        break;
                    }
                }
            }
            if (_disk3DTechProcess.IsExactlyEnd && lastPassIndex != passZArray.Length - 1)
                AddPass(passZArray.Length - 1);

            void AddPass(int index)
            {
                var y = minPoint.Y + index * StepY;
                passList.Add(passZArray[index].Select((z, ind) => new {z, ind}).Where(p => p.z.HasValue).Select(p => new Point3d(minPoint.X + p.ind * StepX2, y, minPoint.Z + p.z.Value)).ToList());
                lastPassIndex = index + 1;
                lastPassDist = 0;
            }
        
            return passList;
        }

        private void BuildPass(MillingCommandGenerator generator, List<Point3d> points)
        {
            var point0 = Algorithms.NullPoint3d;
            var point = Algorithms.NullPoint3d;

            foreach (var p in points)
            {
                if (generator.IsUpperTool)
                    generator.Move(p.X, p.Y, angleC: ((Disk3DTechProcess)TechProcess).Angle);

                if (point.IsNull())
                {
                    if (generator.ToolPosition.Point != p)
                        generator.GCommand(CommandNames.Penetration, 1, point: p, feed: TechProcess.PenetrationFeed);
                }
                else if (point0 != point && point != p && !point0.GetVectorTo(point).IsCodirectionalTo(point.GetVectorTo(p)))
                {
                    generator.GCommand(CommandNames.Cutting, 1, point: point, feed: CuttingFeed);
                    point0 = point;
                }
                if (point0.IsNull())
                    point0 = p;
                point = p;
            }
            generator.GCommand(CommandNames.Cutting, 1, point: point, feed: CuttingFeed);
        }

        //private Point3d[,] CalcOffsetPoints(List<PolylineCurve3d> polylines, Point3d minPoint, Point3d maxPoint)
        //{
        //    //var pline = new PolylineCurve3d(points);
        //    var distance = TechProcess.Tool.Diameter / 2;
        //    //var matrix = Matrix3d.Displacement(Vector3d.ZAxis.Negate() * distance);
        //    //offsetCurves.ForEach(p => p.TransformBy(matrix));

        //    //var firstPoint = offsetCurve.StartPoint;
        //    //var departurePoint = new Point3d((IsDepartureOnBorderSection ? firstPoint.X : bounds.MinPoint.X) - Departure, firstPoint.Y, firstPoint.Z);
        //    //var offsetPoints = new List<Point3d> { departurePoint, firstPoint };

        //    var countX = (int)((maxPoint.X - minPoint.X) / StepLong) + 1;
        //    var pointsArray = new Point3d[polylines.Count, countX];
        //    Acad.SetLimitProgressor(polylines.Count);

        //    for (int i = 0; i < polylines.Count; i++)
        //    {
        //        Acad.ReportProgressor();

        //        var polyline = polylines[i];
        //        var offsetCurves = polyline.GetTrimmedOffset(distance, -Vector3d.YAxis, OffsetCurveExtensionType.Fillet);
        //        if (offsetCurves[0] is CompositeCurve3d compositeCurve)
        //            offsetCurves = compositeCurve.GetCurves();

        //        var index = 0;
        //        var y = polyline.StartPoint.Y;
        //        Point3d[] pt = null;
        //        for (int j = 0; j < countX; j++)
        //        {
        //            var x = minPoint.X + j * StepLong;
        //            var line = new Line3d(new Point3d(x, y, minPoint.Z), new Point3d(x, y, maxPoint.Z + 100));

        //            if (offsetCurves[index] is CircularArc3d arc)
        //                pt = arc.IntersectWith(line);

        //            if (offsetCurves[index] is Line3d line3d)
        //                pt = line3d.IntersectWith(line);

        //            if (pt?.Length == 1)
        //                pointsArray[i, j] = pt[0];
        //            else
        //                pointsArray[i, j] = new Point3d(x, y, double.MinValue);

        //        }
        //    }
        //    return pointsArray;
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------------------------

        //public void BuildProcessing1(CommandGeneratorBase generator)
        //{
        //    var disk3DTechProcess = (Disk3DTechProcess)TechProcess;

        //    var offsetSurface = CreateOffsetSurface();

        //    var matrix = Matrix3d.Rotation(disk3DTechProcess.Angle.ToRad(), Vector3d.ZAxis, Point3d.Origin);
        //    if (disk3DTechProcess.Angle != 0)
        //        offsetSurface.TransformBy(matrix);
        //    var bounds = offsetSurface.GeometricExtents;
        //    var minPoint = bounds.MinPoint;
        //    var maxPoint = bounds.MaxPoint;

        //    var PassList = new List<List<Point3d>>();
        //    var startY = minPoint.Y - (disk3DTechProcess.IsExactlyBegin ? 0 : (TechProcess.Tool.Thickness.Value - StepY));
        //    var endY = maxPoint.Y - (disk3DTechProcess.IsExactlyEnd ? TechProcess.Tool.Thickness : 0);

        //    Acad.SetLimitProgressor((int)((endY - startY) / StepY));
        //    for (var y = startY; y < endY; y += StepY)
        //    {
        //        Acad.ReportProgressor();
        //        var points = new Point3dCollection();
        //        for (var x = minPoint.X; x <= maxPoint.X; x += StepX2)
        //        {
        //            double z = 0;
        //            for (var s = 0; s <= TechProcess.Tool.Thickness; s += 1)
        //            {
        //                if (y + s < minPoint.Y + Consts.Epsilon || y + s > maxPoint.Y - Consts.Epsilon)
        //                    continue;
        //                offsetSurface.RayTest(new Point3d(x, y + s, minPoint.Z), Vector3d.ZAxis, 0.0001, out SubentityId[] col, out DoubleCollection par);
        //                if (par.Count == 0)
        //                {
        //                    z = 0;
        //                    break;
        //                }
        //                z = par[par.Count - 1] > z ? par[par.Count - 1] : z;
        //            }
        //            if (z > 0)
        //            {
        //                var point = new Point3d(x, y, minPoint.Z + z);
        //                var ind = points.Count - 1;
        //                if (ind > 0 && points[ind - 1].GetVectorTo(points[ind]).IsCodirectionalTo(points[ind].GetVectorTo(point)))
        //                    points[ind] = point;
        //                else
        //                    points.Add(point);
        //            }
        //        }
        //        if (points.Count > 1)
        //            PassList.Add(CalcOffsetPoints(points, bounds));
        //    }
        //    offsetSurface.Dispose();

        //    matrix = matrix.Inverse();
        //    PassList.ForEach(p =>
        //    {
        //        var points = p;
        //        if (TechProcess.MachineType == MachineType.ScemaLogic) //Settongs.IsFrontPlaneZero
        //            points = points.ConvertAll(x => new Point3d(x.X, x.Y + TechProcess.Tool.Thickness.Value, x.Z));
        //        if (disk3DTechProcess.Angle != 0)
        //            points = points.ConvertAll(x => x.TransformBy(matrix));
        //        var loc = generator.ToolLocation;
        //        if (loc.IsDefined && loc.Point.DistanceTo(points.First()) > loc.Point.DistanceTo(points.Last()))
        //            points.Reverse();

        //        BuildPass(generator, points);

        //        if (IsUplifting)
        //            generator.Uplifting();
        //    });
        //}

        //private List<Point3d> CalcOffsetPoints(Point3dCollection points, Extents3d bounds)
        //{
        //    var pline = new PolylineCurve3d(points);
        //    var distance = TechProcess.Tool.Diameter / 2;
        //    var offsetCurves = pline.GetTrimmedOffset(distance, -Vector3d.YAxis, OffsetCurveExtensionType.Fillet);
        //    var matrix = Matrix3d.Displacement(Vector3d.ZAxis.Negate() * distance);
        //    offsetCurves.ForEach(p => p.TransformBy(matrix));

        //    var firstPoint = offsetCurves[0].StartPoint;
        //    var departurePoint = new Point3d((IsDepartureOnBorderSection ? firstPoint.X : bounds.MinPoint.X) - Departure, firstPoint.Y, firstPoint.Z);
        //    var offsetPoints = new List<Point3d> { departurePoint, firstPoint };

        //    switch (offsetCurves[0])
        //    {
        //        case LineSegment3d line:
        //            offsetPoints.Add(line.EndPoint);
        //            break;
        //        case CircularArc3d arc:
        //            offsetPoints.AddRange(GetArcPoints(arc));
        //            break;
        //        case CompositeCurve3d curve:
        //            curve.GetCurves().ForEach(p =>
        //            {
        //                if (p is CircularArc3d arc)
        //                    offsetPoints.AddRange(GetArcPoints(arc));
        //                else
        //                    offsetPoints.Add(p.EndPoint);
        //            });
        //            break;
        //        default:
        //            throw new Exception(ErrorStatus.NotImplementedYet, $"Полученный тип кривой не может быть обработан {offsetCurves[0].GetType()}");
        //    }

        //    var lastPoint = offsetPoints.Last();
        //    offsetPoints.Add(new Point3d((IsDepartureOnBorderSection ? lastPoint.X : bounds.MaxPoint.X) + Departure, lastPoint.Y, lastPoint.Z));
        //    return offsetPoints;

        //    IEnumerable<Point3d> GetArcPoints(CircularArc3d arc)
        //    {
        //        var num = (int)((arc.EndAngle - arc.StartAngle) / (Math.PI / 36)) + 4;
        //        return arc.GetSamplePoints(num).Skip(1).Select(p => p.Point);
        //    }
        //}


    }
}
