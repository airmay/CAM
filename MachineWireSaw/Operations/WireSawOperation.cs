using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using System;
using System.Linq;
using System.Security.Cryptography;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CAM
{
    [Serializable]
    public class WireSawOperation : OperationWireSawBase
    {

        public bool IsReverseDirection { get; set; }
        public bool Across { get; set; }
        public bool IsReverseOffset { get; set; }
        public int? StepCount { get; set; }
        public bool IsReverseU { get; set; }
        public bool IsReverseAngle { get; set; }
        //public double DU { get; set; }
        //public double Delay { get; set; } = 60;
        //public bool IsExtraMove { get; set; }
        //public bool IsExtraRotate { get; set; }
        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject();
            view.AddCheckBox(nameof(IsReverseDirection), "Обратное направление");
            view.AddCheckBox(nameof(Across), "Поперек");
            view.AddCheckBox(nameof(IsReverseOffset), "Обратный Offset");
            view.AddTextBox(nameof(StepCount), "Количество шагов");
            view.AddIndent();
            view.AddCheckBox(nameof(IsReverseU), "Обратное U");
            view.AddCheckBox(nameof(IsReverseAngle), "Обратный угол");
            //view.AddIndent();
            //view.AddTextBox(nameof(Delay), "Задержка");
            //view.AddCheckBox(nameof(IsExtraMove), "Возврат");
            //view.AddCheckBox(nameof(IsExtraRotate), "Поворот");
            //view.AddTextBox(nameof(DU), "dU");
        }

        public override void Execute()
        {
            var entities = ProcessingArea.ObjectIds.QOpenForRead<Entity>();
            var extents = entities.GetExtents();
            var offset = (Processing.ToolThickness / 2 + Processing.Delta) * IsReverseOffset.GetSign(-1);

            Processor.StartOperation(extents.MaxPoint.Z + Processing.ZSafety);

            //var surface = dBObject as DbSurface;
            //var plane = surface.GetPlane();
            //var n = plane.Normal;
            //if (surf is Plane)
            //{
            //    Plane plane = (Plane)dBObject;
            //    Vector3d normal = plane.Normal;
            //}

            if (entities[0] is Region region)
            {
                var normal = region.Normal;
                Point3d point;
                using (var exploded = new DBObjectCollection())
                {
                    region.Explode(exploded);
                    point = ((Curve)exploded[0]).StartPoint;
                }

                PlaneCutting(extents, normal, point, offset);
            }
            else if (entities.Length > 1 && entities[0] is Line linе1 && entities[1] is Line linе2)
            {
                var vecBetween = linе1.StartPoint - (linе1.Delta.IsParallelTo(linе2.StartPoint - linе1.StartPoint) ? linе2.EndPoint : linе2.StartPoint);
                var normal = linе1.Delta.CrossProduct(vecBetween);
                // Если нормаль нулевая (отрезки коллинеарны или один из них - точка)
                if (normal.Length > Tolerance.Global.EqualPoint)
                {
                    // Смешанное произведение
                    var tripleProduct = linе2.Delta.DotProduct(normal);
                    // Если смешанное произведение близко к нулю - отрезки компланарны
                    if (Math.Abs(tripleProduct) < Tolerance.Global.EqualPoint)
                        PlaneCutting(extents, normal, linе1.StartPoint, offset);
                }
            }
            else if (entities[0] is DbSurface dbSurface)
            {
                using (var exploded = new DBObjectCollection())
                {
                    dbSurface.Explode(exploded);
                    var railCurves = exploded.Cast<Curve>()
                        .OrderByDescending(p => Math.Abs(p.EndPoint.Z - p.StartPoint.Z))
                        .Take(2)
                        .ToList();
                    foreach (var curve in railCurves)
                        if (curve.StartPoint.Z < curve.EndPoint.Z ^ IsReverseDirection)
                            curve.ReverseCurve();

                    var points = railCurves.Select(c =>
                        c.GetGeCurve().GetSamplePoints(StepCount.Value).Select(p => p.Point).ToArray()).ToArray();

                    //if (Approach > 0)
                    //    points.Add(railCurves.Select(p => p.StartPoint + Vector3d.ZAxis * Approach).ToArray());
                    Processor.Move(points[0][0], points[1][0], IsReverseAngle, IsReverseU);

                    //var stepCurves = railCurves.ConvertAll(p => new
                    //    { Curve = p, step = (p.EndParam - p.StartParam) / StepCount });
                    for (var i = 0; i < StepCount; i++)
                    {
                        //var points = stepCurves.ConvertAll(p1 => p1.Curve.GetPointAtParameter(i * p.step.Value));

                        Processor.Cutting(points[0][i], points[1][i]);
                    }
                }
            }
            else
                throw new Exception(ErrorStatus.NotImplementedYet, "Нет реализации обработки для данной области");
        }

        private void PlaneCutting(Extents3d extents, Vector3d normal, Point3d point, double offset)
        {
            normal = normal.GetNormal();
            var offsetVector = offset * normal;
            var maxPoint = extents.MaxPoint + offsetVector;
            var minPoint = extents.MinPoint + offsetVector;
            point += offsetVector;
            var (startPoint, endPoint) = IsReverseDirection ? (minPoint, maxPoint) : (maxPoint, minPoint);

            Vector3d cuttingDirection;
            if (normal.IsParallelTo(Vector3d.ZAxis))
            {
                cuttingDirection = (Across ? Vector3d.YAxis : Vector3d.XAxis) * IsReverseDirection.GetSign();
            }
            else
            {
                // Вычисляем проекцию оси Z на нормаль: (Z · n) * n
                var projOnNormal = normal * Vector3d.ZAxis.DotProduct(normal);
                // Вычитаем из оси Z её проекцию на нормаль → получаем проекцию на плоскость
                cuttingDirection = (Vector3d.ZAxis - projOnNormal).GetNormal() * IsReverseDirection.GetSign();
                //wireDirection = new Vector3d(normal.Y, -normal.X, 0);

                startPoint = GetPoint(startPoint.Z);
                endPoint = GetPoint(endPoint.Z);
            }

            var wireDirection = cuttingDirection.GetPerpendicularVector();
            var approachPoint = startPoint - Processing.Approach * cuttingDirection;
            if (approachPoint.Z > Processor.UpperZ)
                approachPoint = GetPoint(Processor.UpperZ);
            
            Processor.Move(approachPoint, wireDirection, IsReverseAngle, IsReverseU);
            Processor.Cutting(startPoint, wireDirection);
            Processor.Cutting(endPoint, wireDirection);
            Processor.Cutting(endPoint + Processing.Departure * cuttingDirection, wireDirection);

            return;

            Point3d GetPoint(double z)
            {
                // Уравнение плоскости: N · (P - P₀) = 0  =>  planeNormal.x * (x - x0) + planeNormal.y * (y - y0) + planeNormal.z * (z - z0) = 0
                return Math.Abs(normal.Y) < Tolerance.Global.EqualPoint
                    ? new Point3d(point.X - normal.Z * (z - point.Z) / normal.X, 0, z)
                    : new Point3d(point.X, point.Y - normal.Z * (z - point.Z) / normal.Y, z);
            }
        }

        public void Execute1()
        {
            var z0 = ProcessingArea.ObjectIds.GetExtents().MaxPoint.Z + Processing.ZSafety;
                //Processor.StartOperation(0, 0, z0);
            var offsetDistance = Processing.ToolThickness / 2 + Processing.Delta;

            if (ProcessingArea.ObjectIds.Length == 1)
            {
                var dBObject = ProcessingArea.ObjectId.QOpenForRead();
                var surface = dBObject as DbSurface;
                if (dBObject is Region region)
                {
                    var planeSurface = new PlaneSurface();
                    planeSurface.CreateFromRegion(region);
                    surface = planeSurface;
                }

                if (IsReverseOffset)
                    offsetDistance *= -1;
                var offsetSurface = DbSurface.CreateOffsetSurface(surface, offsetDistance);

                //if (curves[0] is Region r)
                //{
                //    curves.Clear();
                //    r.Explode(curves);
                //}
                //var plane = offsetSurface.GetPlane();

                var curves = new DBObjectCollection();
                offsetSurface.Explode(curves);
                var railCurves = curves.Cast<Curve>()
                    .OrderByDescending(p => Math.Abs(p.EndPoint.Z - p.StartPoint.Z))
                    .Take(2)
                    .ToList();
                foreach (var curve in railCurves)
                    if (curve.StartPoint.Z < curve.EndPoint.Z ^ IsReverseDirection)
                        curve.ReverseCurve();

                //if (Approach > 0)
                //    points.Add(railCurves.Select(p => p.StartPoint + Vector3d.ZAxis * Approach).ToArray());
                //Processor.GCommand(0, railCurves[0].StartPoint, railCurves[1].StartPoint, IsReverseAngle);

                var stepCurves = railCurves.ConvertAll(p => new
                    { Curve = p, step = (p.EndParam - p.StartParam) / StepCount });
                for (var i = 0; i <= StepCount; i++)
                {
                    var points = stepCurves.ConvertAll(p => p.Curve.GetPointAtParameter(i * p.step.Value));

                  //  Processor.GCommand(1, points[0], points[1]);
                }
            }
        }
    }
}
