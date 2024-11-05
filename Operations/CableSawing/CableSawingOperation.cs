using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using CAM.TechProcesses.CableSawing;

namespace CAM.Operations.CableSawing
{
    [Serializable]
    public class CableSawingOperation : OperationBase
    {
        [NonSerialized] protected ProcessingCnc Processing;
        public override ProcessingBase ProcessingBase
        {
            get => Processing;
            set => Processing = (ProcessingCnc)value;
        }

        public double ToolThickness { get; set; } = 10;
        public int CuttingFeed { get; set; } = 10;

        public int S { get; set; } = 100;
        public double Approach { get; set; }
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 0;
        public double Delay { get; set; } = 60;
        public bool IsExtraRotate { get; set; }
        public int StepCount { get; set; } = 100;


        //public Point2d Center => new Point2d(OriginX, OriginY);

        //public CableSawingTechProcess()
        //{
        //    Machine = CAM.Machine.CableSawing;
        //}

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области");
            view.AddIndent();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(S), "Угловая скорость");
            view.AddText("Z безопасности отсчитывается от верха выбранных объектов техпроцесса");
            view.AddIndent();
            view.AddTextBox(nameof(Approach), "Заезд");
            view.AddTextBox(nameof(Departure), "Выезд");
            view.AddIndent();
            view.AddTextBox(nameof(ToolThickness), "Толщина троса");
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(Delay), "Задержка");
            view.AddTextBox(nameof(IsExtraRotate), "Поворот+возврат"); ;

            //view.AddTextBox(nameof(IsRevereseDirection), "Обратное напр.");
            //view.AddTextBox(nameof(IsRevereseOffset), "Обратный Offset");
            view.AddTextBox(nameof(StepCount), "Количество шагов");
        }

        //public override void SetProcessing(Processor processor)
        //{
        //    var tool = new Tool { Type = ToolType.Cable, Diameter = ToolThickness, Thickness = ToolThickness };

        //    var offsetDistance = ToolThickness / 2 + Delta;
        //    var curves = ProcessingArea.GetCurves();
        //    foreach (var curve in curves)
        //    {
        //        if (curve.StartPoint.Z < curve.EndPoint.Z)
        //            curve.ReverseCurve();
        //    }

        //    processor.AddCommand("G92");

        //    var pt0 = new Point3d[StepCount + 3];
        //    var pt1 = new Point3d[StepCount + 3];
        //    for (int i = 0; i <= StepCount; i++)
        //    {
        //        pt0[i + 1] = curves[0].GetPointAtDist(curves[0].Length() / StepCount);
        //        pt1[i + 1] = curves[1].GetPointAtDist(curves[1].Length() / StepCount);
        //    }
        //    pt0[0] = pt0[1].GetExtendedPoint(pt0[2], Departure);
        //    pt1[0] = pt1[1].GetExtendedPoint(pt1[2], Departure);
        //    pt0[StepCount + 2] = pt0[StepCount + 1].GetExtendedPoint(pt0[StepCount], Departure);
        //    pt1[StepCount + 2] = pt1[StepCount + 1].GetExtendedPoint(pt1[StepCount], Departure);

        //    for (int i = 0; i < pt0.Length; i++)
        //    {
        //        var line = new Line2d(pt0[i].ToPoint2d(), pt1[i].ToPoint2d());
        //        var pNearest = line.GetClosestPointTo(Origin).Point;
        //        var vector = pNearest - Origin;
        //        var u = vector.Length;
        //        var z = (pt0[i] + (pt1[i] - pt0[i]) / 2).Z;
        //        var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg();

                //if (i == 0)
                //{
                //    processor.GCommand1(0, u, 0);
                //    processor.GCommandAngle1(vector, S);
                //    processor.GCommand1(0, 0, z);
                //    processor.Command1($"M03", "Включение");
                //}
                //else
                //{
                //    processor.GCommand1(1, u, z, CuttingFeed);
                //    processor.GCommandAngle1(angle, S);
                //}
            // }
            //processor.Command($"M05", "Выключение");

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
            public override Machine Machine { get; }
            public override Tool Tool { get; }


            public override void Execute()
            {
                throw new NotImplementedException();
            }

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

        public override MachineType MachineType { get; }
    }
}
