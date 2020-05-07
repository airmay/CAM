using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CAM.Core;
using CAM.Disk3D;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public LongCleaningTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void PrepareBuild(ICommandGenerator generator)
        {
            var bounds = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents();
            generator.ZSafety = bounds.MaxPoint.Z + TechProcess.ZSafety;
            generator.ToolLocation.Point += Vector3d.ZAxis * generator.ZSafety;
        }

        public override void BuildProcessing(ICommandGenerator generator)
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

            var progressor = new Progressor("Диск 3D", (int)((endY - startY) / StepPass));
            for (var y = startY; y < endY; y += StepPass)
            {
                progressor.Progress();
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
            progressor.Stop();

            matrix = matrix.Inverse();
            PassList.ForEach(p =>
            {
                var tp = disk3DTechProcess.Angle != 0 ? p.ConvertAll(x => x.TransformBy(matrix)) : p;
                var loc = generator.ToolLocation;
                if (loc.IsDefined && loc.Point.DistanceTo(tp.First()) > loc.Point.DistanceTo(tp.Last()))
                    tp.Reverse();

                BuildPass(generator, tp);

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
            if (TechProcess.MachineType == MachineType.ScemaLogic)  //Settongs.IsFrontPlaneZero
                points = points.ConvertAll(p => new Point3d(p.X, p.Y + TechProcess.Tool.Thickness.Value, p.Z));

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
