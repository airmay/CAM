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
    [MenuItem("Распиловка по дуге", 2)]
    public class ArcSawingTechOperation : CableSawingTechOperation
    {
        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject("AcadObjects")
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddIndent()
                .AddParam(nameof(Approach), "Заезд")
                .AddParam(nameof(Departure), "Выезд")
                .AddIndent()
                .AddParam(nameof(IsRevereseDirection), "Обратное напр.")
                .AddParam(nameof(IsRevereseOffset), "Обратный Offset")
                .AddIndent()
                .AddParam(nameof(Delta))
                .AddParam(nameof(Delay), "Задержка")
                .AddParam(nameof(StepCount), "Количество шагов");
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
