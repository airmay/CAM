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
    [TechOperation(TechProcessType.Disk3D, "Поперечная чистка", 3)]
    public class CrossCleaningTechOperation : TechOperation
    {
        private Disk3DTechProcess _disk3DTechProcess;

        public double StepY { get; set; }

        public double StartY { get; set; }

        public double Delta { get; set; }

        public double StepX1 { get; set; }

        public double StepX2 { get; set; }

        public int CuttingFeed { get; set; }

        public double Departure { get; set; }

        public bool IsDepartureOnBorderSection { get; set; }

        public bool IsUplifting { get; set; }

        public CrossCleaningTechOperation(TechProcess techProcess, string caption) : base(techProcess, caption)
        {
            StepX1 = 1;
            StepX2 = 1;
            StepY = 1;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddParam(nameof(StepX1), "Шаг X1")
                .AddParam(nameof(StepX2), "Шаг X2")
                .AddParam(nameof(StepY), "Шаг Y")
                .AddIndent()
                .AddParam(nameof(Departure))
                .AddParam(nameof(IsDepartureOnBorderSection), "Выезд по границе сечения")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(Delta))
                .AddParam(nameof(IsUplifting));
        }

        public override void PrepareBuild(CommandGeneratorBase generator)
        {
            var bounds = TechProcess.ProcessingArea.Select(p => p.ObjectId).GetExtents();
            generator.ZSafety = bounds.MaxPoint.Z + TechProcess.ZSafety;
            generator.ToolLocation.Point += Vector3d.ZAxis * generator.ZSafety;
        }

        public override void BuildProcessing(CommandGeneratorBase generator)
        {
            _disk3DTechProcess = (Disk3DTechProcess)TechProcess;

            var offsetSurface = CreateOffsetSurface();

            var matrix = Matrix3d.Rotation(_disk3DTechProcess.Angle.ToRad(), Vector3d.ZAxis, Point3d.Origin);
            if (_disk3DTechProcess.Angle != 0)
                offsetSurface.TransformBy(matrix);

            var minPoint = offsetSurface.GeometricExtents.MinPoint;
            var maxPoint = offsetSurface.GeometricExtents.MaxPoint;

            var collections = GetPointCollections(offsetSurface, minPoint, maxPoint);
             
            offsetSurface.Dispose();

            var zArray = GetZArray(collections, (maxPoint - minPoint).X);

            var passList = CalcPassList(zArray, minPoint);

            matrix = matrix.Inverse();
            passList.ForEach(p =>
            {
                var points = p;
                if (Departure > 0)
                {
                    points.Insert(0, new Point3d((IsDepartureOnBorderSection ? points.First().X : minPoint.X) - Departure, points.First().Y, points.First().Z));
                    points.Add(new Point3d((IsDepartureOnBorderSection ? points.Last().X : maxPoint.X) + Departure, points.Last().Y, points.Last().Z));
                }
                if (TechProcess.MachineType == MachineType.Donatoni) //Settongs.IsFrontPlaneZero
                    points = points.ConvertAll(x => new Point3d(x.X, x.Y - TechProcess.Tool.Thickness.Value, x.Z));
                if (_disk3DTechProcess.Angle != 0)
                    points = points.ConvertAll(x => x.TransformBy(matrix));
                var loc = generator.ToolLocation;
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
            var offsetSurface = DbSurface.CreateOffsetSurface(unionSurface, Delta) as DbSurface;
            unionSurface.Dispose();
            return offsetSurface;
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
                polylene.Dispose();
                offsetCurve.Dispose();
            }
            rays.ForEach(p => p.Dispose());
            intersector.Dispose();

            return zArray;
        }

        private List<List<Point3d>> CalcPassList(double?[,] zArray, Point3d minPoint)
        {
            var w = (int)(TechProcess.Tool.Thickness / StepY);
            var maxDist = (int)(0.8 * TechProcess.Tool.Thickness / StepY);
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

            for (int j = 0; j < zArray.GetLength(1); j++)
            {
                var pass = new List<Point3d>();
                var x = minPoint.X + j * StepX2;

                for (int i = startIndex; i < passZArray.Length; i++)
                {
                    if (passZArray[i][j].HasValue)
                    {
                        pass.Add(new Point3d(x, minPoint.Y + i * StepY, minPoint.Z + passZArray[i][j].Value));
                    }
                }
                if (pass.Any())
                    passList.Add(pass);
            }
        
            return passList;
        }

        private void BuildPass(CommandGeneratorBase generator, List<Point3d> points)
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
