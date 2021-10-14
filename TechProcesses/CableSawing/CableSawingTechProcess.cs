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

        public Point2d Center => new Point2d(OriginX, OriginY);

        public CableSawingTechProcess()
        {
            MachineType = CAM.MachineType.CableSawing;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области")
                .AddOrigin()
                .AddIndent()
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddParam(nameof(ZSafety))
                .AddIndent()
                .AddParam(nameof(Approach), "Заезд")
                .AddParam(nameof(Departure), "Выезд")
                .AddIndent()
                .AddParam(nameof(ToolThickness), "Толщина троса")
                .AddParam(nameof(Delta))
                .AddParam(nameof(Delay), "Задержка");
        }

        public override List<TechOperation> CreateTechOperations()
        {
            return ProcessingArea.ConvertAll(p =>
            {
                var dbObject = p.ObjectId.QOpenForRead();
                var operation = dbObject is Region || dbObject is PlaneSurface
                    ? (TechOperation)new LineSawingTechOperation()
                    : new ArcSawingTechOperation();
                operation.ProcessingArea = p;
                var caption = ((MenuItemAttribute)Attribute.GetCustomAttribute(operation.GetType(), typeof(MenuItemAttribute))).Name;
                operation.Setup(this, caption);
                return operation;
            });
        }

        protected override void BuildProcessing(CableCommandGenerator generator)
        {
            Tool = new Tool { Type = ToolType.Cable, Diameter = ToolThickness, Thickness = ToolThickness };
            //if (OriginObject == null)
            //{
            //    var center = ProcessingArea.Select(p => p.ObjectId).GetCenter();
            //    OriginX = center.X.Round(2);
            //    OriginY = center.Y.Round(2);
            //    OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));
            //}

            var isStarted = false;

            foreach (var operation in TechOperations.FindAll(p => p.Enabled && p.CanProcess).Cast<CableSawingTechOperation>())
            {
                SetOperationParams(operation);

                var entity = operation.ProcessingArea.ObjectId.QOpenForRead<Entity>();
                if (entity is Region region)
                {
                    entity = new PlaneSurface();
                    ((PlaneSurface)entity).CreateFromRegion(region);
                }
                var offsetDistance = ToolThickness / 2 + Delta;
                if (operation.IsRevereseOffset)
                    offsetDistance *= -1;
                var surface = DbSurface.CreateOffsetSurface(entity, offsetDistance) as DbSurface;

                var collection = new DBObjectCollection();
                surface.Explode(collection);
                var curves = collection.Cast<Curve>().ToList();
                var railCurves = operation.GetRailCurves(curves);
                if (railCurves[1].StartPoint.DistanceTo(railCurves[0].StartPoint) > railCurves[1].StartPoint.DistanceTo(railCurves[0].EndPoint))
                    railCurves[1].ReverseCurve();
                var points0 = GetRailPoints(railCurves[0], operation);
                var points1 = GetRailPoints(railCurves[1], operation);

                if (!isStarted)
                {
                    generator.SetToolPosition(new Point3d(OriginX, OriginY, 0), 0, 0, entity.Bounds.Value.MaxPoint.Z + ZSafety);
                    generator.Command($"G92");
                }
                for (int i = 0; i < points0.Count; i++)
                {
                    var line = new Line2d(points0[i].ToPoint2d(), points1[i].ToPoint2d());
                    var pNearest = line.GetClosestPointTo(Center).Point;
                    var vector = pNearest - Center;
                    var u = vector.Length;
                    var z = (points0[i] + (points1[i] - points0[i]) / 2).Z;
                    var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg();

                    //if (operation is ArcSawingTechOperation)
                    //{
                    //    if (angle - generator.Angle > 150)
                    //    {
                    //        angle -= 180;
                    //        u *= -1;
                    //    }
                    //    if (generator.Angle - angle > 150)
                    //    {
                    //        angle += 180;
                    //        u *= -1;
                    //    }
                    //}

                    //if (angle > 180)
                    //{
                    //    angle -= 180;
                    //    u *= -1;
                    //}
                    //if (i != 0 && generator.Angle - angle > 150)
                    //{
                    //    //angle += 360;
                    //    u *= -1;
                    //}
                    //if (operation == TechOperations.Last())
                    //{
                    //    angle += 180;
                    //    //u *= -1;
                    //}

                    if (!isStarted)
                    {
                        generator.GCommandAngle(angle, operation.S);
                        generator.GCommand(0, u);
                        generator.GCommand(0, u, z);
                        generator.Command($"M03", "Включение");
                        isStarted = true;
                    }
                    else
                    {
                        generator.GCommand(1, u, z, operation.CuttingFeed);
                        if (i == 0 || !(entity is PlaneSurface))
                            generator.GCommandAngle(angle, operation.S);
                    }
                }
                generator.Command($"G04 P{operation.Delay}", "Задержка");
                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");
            }

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
            operation.Delay = operation.Delay == 0 ? Delay : operation.Delay;
        }

        public List<Point3d> GetRailPoints(Curve rail, CableSawingTechOperation operation)
        {
            var points = rail.GetPoints(operation.StepCount).ToList();
            if (operation.Approach != 0)
                points.Insert(0, points[0].GetExtendedPoint(points[1], operation.Approach));
            if (operation.Departure != 0)
                points.Add(points[points.Count - 1].GetExtendedPoint(points[points.Count - 2], operation.Departure));
            if (operation.IsRevereseDirection)
                points.Reverse();
            return points;
        }   
    }
}
