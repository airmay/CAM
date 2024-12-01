/*
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
    [MenuItem("Распиловка тросом", 8)]
    public class CableSawingTechProcess : CableTechProcess
    {
        public double ToolThickness { get; set; } = 10;
        public int CuttingFeed { get; set; } = 10;
        public int S { get; set; } = 100;
        public double Approach { get; set; }
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 0;
        public double Delay { get; set; } = 60;
        public bool IsExtraMove { get; set; }
        public bool IsExtraRotate { get; set; }

        public Point2d Center => new Point2d(OriginX, OriginY);

        public CableSawingTechProcess()
        {
            //MachineType = CAM.MachineType.CableSawing;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            //view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области")
            //    .AddOrigin()
            //    .AddIndent()
            //    .AddParam(nameof(CuttingFeed))
            //    .AddParam(nameof(S), "Угловая скорость")
            //    .AddParam(nameof(ZSafety))
            //    .AddText("Z безопасности отсчитывается от верха выбранных объектов техпроцесса")
            //    .AddIndent()
            //    .AddParam(nameof(Approach), "Заезд")
            //    .AddParam(nameof(Departure), "Выезд")
            //    .AddIndent()
            //    .AddParam(nameof(ToolThickness), "Толщина троса")
            //    .AddParam(nameof(Delta))
            //    .AddParam(nameof(Delay), "Задержка")
            //    .AddParam(nameof(IsExtraMove), "Возврат")
            //    .AddParam(nameof(IsExtraRotate), "Поворот");
        }

        //public override List<TechOperation> CreateTechOperations()
        //{
        //    return ProcessingArea.ConvertAll(p =>
        //    {
        //        var dbObject = p.ObjectId.QOpenForRead();
        //        var operation = dbObject is Region || dbObject is PlaneSurface
        //            ? (TechOperation)new LineSawingTechOperation()
        //            : new ArcSawingTechOperation();
        //        operation.ProcessingArea = p;
        //        var caption = ((MenuItemAttribute)Attribute.GetCustomAttribute(operation.GetType(), typeof(MenuItemAttribute))).Name;
        //        operation.Setup(this, caption);
        //        return operation;
        //    });
        //}

        protected override void BuildProcessing(CableCommandGenerator generator)
        {
            Tool = new Tool { Type = ToolType.Cable, Diameter = ToolThickness, Thickness = ToolThickness };
            //var z0 = ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + ZSafety;
            //generator.IsExtraMove = IsExtraMove;
            //generator.IsExtraRotate = IsExtraRotate;

            //if (OriginObject == null)
            //{
            //    var center = ProcessingArea.Select(p => p.ObjectId).GetCenter();
            //    OriginX = center.X.Round(2);
            //    OriginY = center.Y.Round(2);
            //    OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));
            //}


            //var surface1 = ProcessingArea.First().ObjectId.QOpenForRead<Autodesk.AutoCAD.DatabaseServices.Surface>();
            //var bounds = surface1.Bounds;
            //var min = bounds.Value.MinPoint;
            //var max = bounds.Value.MaxPoint;
            //var cnt = 500;
            //var cntz = 20;
            //for (int i = 0; i < cntz; i++)
            //{
            //    for (int j = 0; j < cnt; j++)
            //    {
            //        var pt = new Point3d(min.X, min.Y + (max.Y - min.Y) / cnt * j, min.Z + (max.Z - min.Z) / cntz * i);
            //        surface1.RayTest(pt, Vector3d.XAxis, 0.0001, out SubentityId[] col, out DoubleCollection par);
            //        if (par.Count == 1)
            //        {
            //            App.LockAndExecute(() => Draw.Circle(pt + Vector3d.XAxis * par[0], 1));
            //        }
            //    }
            //}
            //return;
            //surface1.Intersect(new)







            generator.S = S;
            generator.Feed = CuttingFeed;
            var z00 = ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + ZSafety;
            generator.SetToolPosition(new Point3d(OriginX, OriginY, 0), 0, 0, z00);
            generator.Command($"G92");
            CableSawingTechOperation last = null; 

            foreach (var operation in TechOperations.FindAll(p => p.Enabled && p.CanProcess).Cast<CableSawingTechOperation>())
            {
                SetOperationParams(operation);
                generator.Feed = operation.CuttingFeed;

                if (operation is LineSawingTechOperation lineSawingTechOperation)
                {
                    lineSawingTechOperation.BuildProcessing(generator);
                    continue;
                }

                if (operation is ArcSawingTechOperation arcSawingTechOperation)
                {
                    arcSawingTechOperation.BuildProcessing(generator);
                    continue;
                }

                //Curve[] railCurves = new Curve[2];
                //var entityes = operation.AcadObjects.ConvertAll(p => p.ObjectId.QOpenForRead<Entity>());
                //var offsetDistance = ToolThickness / 2 + operation.Delta;

                //if (entityes.Count == 2)
                //{
                //    //railCurves = entityes.Cast<Curve>().ToArray();
                //    //var offsetCurve0 = railCurves[0].GetOffsetCurves(offsetDistance)[0] as Curve;
                //    //if (offsetCurve0.StartPoint.DistanceTo(Center.ToPoint3d()) < railCurves[0].StartPoint.DistanceTo(Center.ToPoint3d()))
                //    //    offsetCurve0 = railCurves[0].GetOffsetCurves(-offsetDistance)[0] as Curve;
                //    //var offsetCurve1 = railCurves[1].GetOffsetCurves(offsetDistance)[0] as Curve;
                //    //if (offsetCurve1.StartPoint.DistanceTo(Center.ToPoint3d()) < railCurves[1].StartPoint.DistanceTo(Center.ToPoint3d()))
                //    //    offsetCurve1 = railCurves[1].GetOffsetCurves(-offsetDistance)[0] as Curve;
                //    var matrix = Matrix3d.Displacement(Vector3d.ZAxis * offsetDistance);
                //    railCurves = entityes.Select(p => (Curve)p.GetTransformedCopy(matrix)).ToArray();
                //}
                //else
                //{
                //    var entity = entityes.First();
                //    if (entity is Region region)
                //    {
                //        entity = new PlaneSurface();
                //        ((PlaneSurface)entity).CreateFromRegion(region);
                //    }
                //    if (operation.IsRevereseOffset)
                //        offsetDistance *= -1;
                //    var surface = DbSurface.CreateOffsetSurface(entity, offsetDistance) as DbSurface;

                //    var collection = new DBObjectCollection();
                //    surface.Explode(collection);
                //    var curves = collection.Cast<Curve>().ToList();
                //    railCurves = operation.GetRailCurves(curves);
                //}
                ////if (railCurves[1].StartPoint.DistanceTo(railCurves[0].StartPoint) > railCurves[1].StartPoint.DistanceTo(railCurves[0].EndPoint))
                //if (railCurves[0].StartPoint.GetVectorTo(railCurves[0].EndPoint).GetAngleTo(railCurves[1].StartPoint.GetVectorTo(railCurves[1].EndPoint)) > Math.PI / 2)
                //    railCurves[1].ReverseCurve();

                //if (railCurves[0] is Line)
                //{
                //    var dz = Math.Abs(railCurves[0].StartPoint.Z - railCurves[1].StartPoint.Z); // TODO dz
                //    if (dz > 1)
                //    {
                //        if (railCurves[0].StartPoint.Z > railCurves[1].StartPoint.Z)
                //            railCurves[1].StartPoint = railCurves[1].StartPoint.GetExtendedPoint(railCurves[1].EndPoint, dz);
                //        else
                //            railCurves[0].StartPoint = railCurves[0].StartPoint.GetExtendedPoint(railCurves[0].EndPoint, dz);
                //    }
                //}

                //var points0 = GetRailPoints(railCurves[0], operation);
                //var points1 = GetRailPoints(railCurves[1], operation);

                //var direction = new Line2d(points0[1].ToPoint2d(), points1[1].ToPoint2d()).Direction;

                //if (!isStarted)
                //{
                //    generator.SetToolPosition(new Point3d(OriginX, OriginY, 0), 0, 0, z0);
                //    generator.Command($"G92");
                //}
                //int? xSign = null;
                //for (int i = 0; i < points0.Count; i++)
                //{
                //    var line = new Line2d(points0[i].ToPoint2d(), points1[i].ToPoint2d());
                //    var pNearest = line.GetClosestPointTo(Center).Point;
                //    var vector = pNearest - Center;
                //    if (xSign == null)
                //        xSign = vector.X.Round(6) > 0 ? -1 : 1;
                //    var xSignNew = vector.X.Round(6) > 0 ? -1 : 1;
                //    if (xSignNew != xSign)
                //        xSign *= -1;
                //    var u = vector.Length * xSign.Value;
                //    var z = (points0[i] + (points1[i] - points0[i]) / 2).Z;
                //    //var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg();

                //    //if (operation is ArcSawingTechOperation)
                //    //{
                //    //    if (angle - generator.Angle > 150)
                //    //    {
                //    //        angle -= 180;
                //    //        u *= -1;
                //    //    }
                //    //    if (generator.Angle - angle > 150)
                //    //    {
                //    //        angle += 180;
                //    //        u *= -1;
                //    //    }
                //    //}

                //    //if (angle > 180)
                //    //{
                //    //    angle -= 180;
                //    //    u *= -1;
                //    //}
                //    //if (i != 0 && generator.Angle - angle > 150)
                //    //{
                //    //    //angle += 360;
                //    //    u *= -1;
                //    //}
                //    //if (operation == TechOperations.Last())
                //    //{
                //    //    angle += 180;
                //    //    //u *= -1;
                //    //}

                //    if (!(railCurves[0] is Line))
                //        direction = line.Direction;

                //    if (!isStarted)
                //    {
                //        generator.GCommandAngle(direction, operation.S);
                //        //generator.GCommand(0, u);
                //        //generator.GCommand(0, u, z);
                //        generator.Command($"M03", "Включение");
                //        isStarted = true;
                //    }
                //    else
                //    {
                //        //generator.GCommand(1, u, z, operation.CuttingFeed);
                //        generator.GCommandAngle(direction, operation.S);
                //    }
                //}
            }
                generator.Command($"G04 P{last?.Delay ?? Delay}", "Задержка");
                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");

            //foreach (Region region in regions)
            //{
            //    var z1 = region.Bounds.Value.MinPoint.Z;
            //    var z2 = region.Bounds.Value.MaxPoint.Z;

            //    var collection = new DBObjectCollection();
            //    region.Explode(collection);
            //    var ofsset = region.Normal * (ToolThickness / 2 + Delta);
            //    var lines = collection.Cast<Line>()
            //        .Where(p => Math.Abs(p.StartPoint.Z - p.EndPoint.Z) < 1)
            //        .Where(p => Math.Abs(z1 - p.StartPoint.Z) < 1 || Math.Abs(z2 - p.StartPoint.Z) < 1)
            //        .OrderByDescending(p => p.Length)
            //        .Take(2)
            //        .Select(p => p.GetStartEndPoints().Select(pt => pt + ofsset).ToArray())
            //        .OrderBy(p => p[0].Z)
            //        .ToList();
            //    //if (lines.Count != 2)
            //    //    throw new Exception("Задана некорректная область");

            //    var line1 = new Line(lines[0][0].ToPoint2d().ToPoint3d(), lines[0][1].ToPoint2d().ToPoint3d());
            //    var pNearest = line1.GetClosestPointTo(origin, true);
            //    var vector = (pNearest - origin).ToVector2d();
            //    var u1 = vector.Length;
            //    var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg(2);
            //    generator.GCommandAngle(angle, S);

            //    var line2 = new Line2d(lines[1][0].ToPoint2d(), lines[1][1].ToPoint2d());
            //    var u2 = line2.GetDistanceTo(origin.ToPoint2d());

            //    var coeff = (u2 - u1) / (z2 - z1);
            //    var u3 = u2 + ZSafety * coeff;
            //    var z3 = z2 + ZSafety;
            //    generator.GCommand(0, u3, z3);

            //    generator.Command($"M03", "Включение");

            //    var u0 = u1 - Departure * coeff;
            //    var z0 = z1 - Departure;
            //    generator.GCommand(1, u0, z0, PenetrationFeed);

            //    generator.Command($"G04 P60", "Задержка");
            //    generator.Command($"M05", "Выключение");
            //    generator.Command($"M00", "Пауза");

            //    generator.GCommand(1, u3, z3, CuttingFeed);
            //}
        }

        public void SetOperationParams(CableSawingTechOperation operation)
        {
            operation.CuttingFeed = operation.CuttingFeed == 0 ? CuttingFeed : operation.CuttingFeed;
            operation.S = operation.S == 0 ? S : operation.S;
            operation.Approach = operation.Approach == 0 ? Approach : operation.Approach;
            operation.Departure = operation.Departure == 0 ? Departure : operation.Departure;
            operation.Delta = operation.Delta == 0 ? Delta : operation.Delta;
            operation.Delay = operation.Delay == 0 ? Delay : operation.Delay;
        }

        public List<Point3d> GetRailPoints(Curve rail, CableSawingTechOperation operation)
        {
            var points = rail.GetPoints(operation.StepCount).ToList();

            if (operation.Approach > 0)
                points.Insert(0, points[0].GetExtendedPoint(points[1], operation.Approach));
            if (operation.Approach < 0)
                points[0] += (points[0] - points[1]).GetNormal() * operation.Approach;
            if (operation.Departure != 0)
                points.Add(points[points.Count - 1].GetExtendedPoint(points[points.Count - 2], operation.Departure));
            if (operation.IsRevereseDirection)
                points.Reverse();
            return points;
        }   
    }
}
*/