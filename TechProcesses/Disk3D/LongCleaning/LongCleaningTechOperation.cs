using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CAM.Core;
using CAM.Disk3D;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CAM.TechProcesses.Disk3D
{
    [Serializable]
    [TechOperation(2, TechProcessNames.Disk3D, "Продольная чистка")]
    public class LongCleaningTechOperation : TechOperationBase
    {
        public double StepPass { get; set; }

        public double StartPass { get; set; }

        public double Delta { get; set; }

        public double StepLong { get; set; }

        public int CuttingFeed { get; set; }

        public double Departure { get; set; }

        public bool IsDepartureOnBorderSection { get; set; }

        public bool IsUplifting { get; set; }

        public double StepZ { get; set; }

        public LongCleaningTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
            StepPass = 1;
            StepLong = 1;
            StepZ = 1;
        }

        public override void PrepareBuild(ICommandGenerator generator)
        {
            var bounds = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents();
            generator.ZSafety = bounds.MaxPoint.Z + TechProcess.ZSafety;
            generator.ToolLocation.Point += Vector3d.ZAxis * generator.ZSafety;
        }

        public override void BuildProcessing(ICommandGenerator generator)
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


            var disk3DTechProcess = (Disk3DTechProcess)TechProcess;

            var offsetSurface = CreateOffsetSurface();

            var matrix = Matrix3d.Rotation(disk3DTechProcess.Angle.ToRad(), Vector3d.ZAxis, Point3d.Origin);
            if (disk3DTechProcess.Angle != 0)
                offsetSurface.TransformBy(matrix);

            var minPoint = offsetSurface.GeometricExtents.MinPoint;
            var maxPoint = offsetSurface.GeometricExtents.MaxPoint; // - Vector3d.XAxis * 1000;

            var polylines = GetSurfacePolylines(offsetSurface, minPoint, maxPoint);

            offsetSurface.Dispose();

            var pointsArray = GetPointsArray(polylines, minPoint, maxPoint);

            var passList = CalcCuttingPass(pointsArray);

            matrix = matrix.Inverse();
            passList.ForEach(p =>
            {
                var points = p;
                if (TechProcess.MachineType == MachineType.Krea) //Settongs.IsFrontPlaneZero
                    points = points.ConvertAll(x => new Point3d(x.X, x.Y - TechProcess.Tool.Thickness.Value, x.Z));
                if (disk3DTechProcess.Angle != 0)
                    points = points.ConvertAll(x => x.TransformBy(matrix));
                var loc = generator.ToolLocation;
                if (loc.IsDefined && loc.Point.DistanceTo(points.First()) > loc.Point.DistanceTo(points.Last()))
                    points.Reverse();

                BuildPass(generator, points);

                if (IsUplifting)
                    generator.Uplifting();
            });
        }

        private List<List<Point3d>> CalcCuttingPass(Point3d[,] pointsArray)
        {
            var passList = new List<List<Point3d>>();

            var w = (int)(TechProcess.Tool.Thickness / StepPass);
            var w08 = (int)(0.8 * TechProcess.Tool.Thickness / StepPass);
            int k08 = 0;
            var zArrayLast = new double[pointsArray.GetLength(1)];
            var zArray = new double[pointsArray.GetLength(1)];

            Acad.SetLimitProgressor(pointsArray.GetLength(0));

            for (int j = 0; j < pointsArray.GetLength(1); j++)
            {
                zArrayLast[j] = pointsArray[0, j].Z;
            }
            for (int i = 1; i < pointsArray.GetLength(0); i++)
            {
                Acad.ReportProgressor();
                var isCutting = false;
                k08++;

                for (int j = 0; j < pointsArray.GetLength(1); j++)
                {
                    var z = double.MinValue;
                    for (int k = 0; k <= w && i - k >= 0; k++)
                    {
                        if (i - k < pointsArray.GetLength(0) && pointsArray[i - k, j].Z > z)
                            z = pointsArray[i - k, j].Z;
                    }
                    zArray[j] = z;
                    if (zArrayLast[j] > double.MinValue && z > double.MinValue && Math.Abs(z - zArrayLast[j]) > StepZ)
                        isCutting = true;
                }
                if (isCutting || k08 == w08)
                {
                    k08 = 0;
                    var pass = new List<Point3d>();
                    for (int j = 0; j < pointsArray.GetLength(1); j++)
                    {
                        if (zArray[j] > double.MinValue)
                            pass.Add(new Point3d(pointsArray[i, j].X, pointsArray[i, j].Y, zArray[j]));
                        zArrayLast[j] = zArray[j];
                        zArray[j] = double.MinValue;
                    }
                    passList.Add(pass);
                }
            }
            
            return passList;
        }

        private Point3d[,] GetPointsArray(List<PolylineCurve3d> polylines, Point3d minPoint, Point3d maxPoint)
        {
            var distance = TechProcess.Tool.Diameter / 2;
            var stepX = StepLong;
            var countX = (int)((maxPoint.X - minPoint.X) / stepX) + 1;
            var pointsArray = new Point3d[polylines.Count, countX];
            var cInt = new CurveCurveIntersector3d();
            Acad.SetLimitProgressor(polylines.Count);
            var lr = Acad.GetProcessLayerId();
            for (int i = 0; i < polylines.Count; i++)
            {
                Acad.ReportProgressor();

                var polyline = polylines[i];
                var y = polyline.StartPoint.Y;
                for (int j = 0; j < countX; j++)
                {
                    var x = minPoint.X + j * stepX;
                    var offsetCurve = polyline.GetTrimmedOffset(distance, -Vector3d.YAxis, OffsetCurveExtensionType.Fillet)[0];
                    var line = new Line3d(new Point3d(x, y, minPoint.Z), new Point3d(x, y, maxPoint.Z));
                    cInt.Set(offsetCurve, line, Vector3d.ZAxis);

                    pointsArray[i, j] = cInt.NumberOfIntersectionPoints == 1 ? cInt.GetIntersectionPoint(0) - Vector3d.ZAxis * distance : new Point3d(x, y, double.MinValue);

                    //var pt = new DBPoint(pointsArray[i, j]);
                    //pt.LayerId = lr;
                    //pt.AddToCurrentSpace();
                }
                polyline.Dispose();
            }
            return pointsArray;
        }

        private List<PolylineCurve3d> GetSurfacePolylines(DbSurface offsetSurface, Point3d minPoint, Point3d maxPoint)
        {
            var startY = minPoint.Y;
            var endY = maxPoint.Y;
            var stepY = StepPass;
            var countY = (int)((endY - startY) / stepY);

            var startX = minPoint.X;
            var endX = (int)maxPoint.X;
            var stepX = 10;
            var countX = (int)((endX - startX) / stepX);

            var polylines = new List<PolylineCurve3d>();
            var collection = new Point3dCollection();

            Acad.SetLimitProgressor(countY);

            for (var i = 0; i < countY; i++)
            {
                Acad.ReportProgressor();

                for (var j = 0; j < countX; j++)
                {
                    var y = startY + i * stepY;
                    var x = startX + j * stepX;
                    offsetSurface.RayTest(new Point3d(x, y, minPoint.Z), Vector3d.ZAxis, 0.0001, out SubentityId[] col, out DoubleCollection par);
                    if (par.Count > 0)
                    {
                        var z = par[par.Count - 1];
                        //if (x > 500)
                        //    z += 100;

                        var point = new Point3d(x, y, minPoint.Z + z);
                        var ind = collection.Count - 1;
                        if (ind > 0 && collection[ind - 1].GetVectorTo(collection[ind]).IsCodirectionalTo(collection[ind].GetVectorTo(point)))
                            collection[ind] = point;
                        else
                            collection.Add(point);
                    }   
                }
                polylines.Add(new PolylineCurve3d(collection));
                collection.Clear();
            }
            return polylines;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void BuildProcessing1(ICommandGenerator generator)
        {
            var disk3DTechProcess = (Disk3DTechProcess)TechProcess;

            var offsetSurface = CreateOffsetSurface();

            var matrix = Matrix3d.Rotation(disk3DTechProcess.Angle.ToRad(), Vector3d.ZAxis, Point3d.Origin);
            if (disk3DTechProcess.Angle != 0)
                offsetSurface.TransformBy(matrix);
            var bounds = offsetSurface.GeometricExtents;
            var minPoint = bounds.MinPoint;
            var maxPoint = bounds.MaxPoint;

            var PassList = new List<List<Point3d>>();
            var startY = minPoint.Y - (disk3DTechProcess.IsExactlyBegin ? 0 : (TechProcess.Tool.Thickness.Value - StepPass));
            var endY = maxPoint.Y - (disk3DTechProcess.IsExactlyEnd ? TechProcess.Tool.Thickness : 0);

            Acad.SetLimitProgressor((int)((endY - startY) / StepPass));
            for (var y = startY; y < endY; y += StepPass)
            {
                Acad.ReportProgressor();
                var points = new Point3dCollection();
                for (var x = minPoint.X; x <= maxPoint.X; x += StepLong)
                {
                    double z = 0;
                    for (var s = 0; s <= TechProcess.Tool.Thickness; s += 1)
                    {
                        if (y + s < minPoint.Y + Consts.Epsilon || y + s > maxPoint.Y - Consts.Epsilon)
                            continue;
                        offsetSurface.RayTest(new Point3d(x, y + s, minPoint.Z), Vector3d.ZAxis, 0.0001, out SubentityId[] col, out DoubleCollection par);
                        if (par.Count == 0)
                        {
                            z = 0;
                            break;
                        }
                        z = par[par.Count - 1] > z ? par[par.Count - 1] : z;
                    }
                    if (z > 0)
                    {
                        var point = new Point3d(x, y, minPoint.Z + z);
                        var ind = points.Count - 1;
                        if (ind > 0 && points[ind - 1].GetVectorTo(points[ind]).IsCodirectionalTo(points[ind].GetVectorTo(point)))
                            points[ind] = point;
                        else
                            points.Add(point);
                    }
                }
                if (points.Count > 1)
                    PassList.Add(CalcOffsetPoints(points, bounds));
            }
            offsetSurface.Dispose();

            matrix = matrix.Inverse();
            PassList.ForEach(p =>
            {
                var points = p;
                if (TechProcess.MachineType == MachineType.ScemaLogic) //Settongs.IsFrontPlaneZero
                    points = points.ConvertAll(x => new Point3d(x.X, x.Y + TechProcess.Tool.Thickness.Value, x.Z));
                if (disk3DTechProcess.Angle != 0)
                    points = points.ConvertAll(x => x.TransformBy(matrix));
                var loc = generator.ToolLocation;
                if (loc.IsDefined && loc.Point.DistanceTo(points.First()) > loc.Point.DistanceTo(points.Last()))
                    points.Reverse();

                BuildPass(generator, points);

                if (IsUplifting)
                    generator.Uplifting();
            });
        }

        private List<Point3d> CalcOffsetPoints(Point3dCollection points, Extents3d bounds)
        {
            var pline = new PolylineCurve3d(points);
            var distance = TechProcess.Tool.Diameter / 2;
            var offsetCurves = pline.GetTrimmedOffset(distance, -Vector3d.YAxis, OffsetCurveExtensionType.Fillet);
            var matrix = Matrix3d.Displacement(Vector3d.ZAxis.Negate() * distance);
            offsetCurves.ForEach(p => p.TransformBy(matrix));

            var firstPoint = offsetCurves[0].StartPoint;
            var departurePoint = new Point3d((IsDepartureOnBorderSection ? firstPoint.X : bounds.MinPoint.X) - Departure, firstPoint.Y, firstPoint.Z);
            var offsetPoints = new List<Point3d> { departurePoint, firstPoint };

            switch (offsetCurves[0])
            {
                case LineSegment3d line:
                    offsetPoints.Add(line.EndPoint);
                    break;
                case CircularArc3d arc:
                    offsetPoints.AddRange(GetArcPoints(arc));
                    break;
                case CompositeCurve3d curve:
                    curve.GetCurves().ForEach(p =>
                    {
                        if (p is CircularArc3d arc)
                            offsetPoints.AddRange(GetArcPoints(arc));
                        else
                            offsetPoints.Add(p.EndPoint);
                    });
                    break;
                default:
                    throw new Exception(ErrorStatus.NotImplementedYet, $"Полученный тип кривой не может быть обработан {offsetCurves[0].GetType()}");
            }

            var lastPoint = offsetPoints.Last();
            offsetPoints.Add(new Point3d((IsDepartureOnBorderSection ? lastPoint.X : bounds.MaxPoint.X) + Departure, lastPoint.Y, lastPoint.Z));
            return offsetPoints;

            IEnumerable<Point3d> GetArcPoints(CircularArc3d arc)
            {
                var num = (int)((arc.EndAngle - arc.StartAngle) / (Math.PI / 36)) + 4;
                return arc.GetSamplePoints(num).Skip(1).Select(p => p.Point);
            }
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
            var offsetSurface = DbSurface.CreateOffsetSurface(unionSurface, Delta) as DbSurface;
            unionSurface.Dispose();
            return offsetSurface;
        }

        private void BuildPass(ICommandGenerator generator, List<Point3d> points)
        {
            var point0 = Algorithms.NullPoint3d;
            var point = Algorithms.NullPoint3d;

            foreach (var p in points)
            {
                if (generator.IsUpperTool)
                    generator.Move(p.X, p.Y, angleC: ((Disk3DTechProcess)TechProcess).Angle);

                if (point.IsNull())
                {
                    if (generator.ToolLocation.Point != p)
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
    }
}
