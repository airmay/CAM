using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;
using CAM.TechProcesses.CableSawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM
{
    [Serializable]
    public class OperationWireSaw : OperationWireSawBase
    {
        public List<AcadObject> AcadObjects { get; set; }
        public double Delay { get; set; } = 60;
        public bool IsExtraMove { get; set; }
        public bool IsExtraRotate { get; set; }
        public bool IsReverseDirection { get; set; }
        public bool IsReverseAngle { get; set; }
        public bool IsReverseOffset { get; set; }
        public virtual int StepCount { get; set; } = 100;
        public double DU { get; set; }

        public bool Across { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject();
                view.AddTextBox(nameof(Across), "Поперек");
                view.AddTextBox(nameof(IsReverseDirection), "Обратное напр.");
                view.AddTextBox(nameof(IsReverseAngle), "Обратный угол");
                view.AddTextBox(nameof(IsReverseOffset), "Обратный Offset");
                view.AddIndent();
                view.AddTextBox(nameof(Delay), "Задержка");
                view.AddTextBox(nameof(IsExtraMove), "Возврат");
                view.AddTextBox(nameof(IsExtraRotate), "Поворот");
                view.AddTextBox(nameof(StepCount), "Количество шагов");
                view.AddTextBox(nameof(DU), "dU");
        }

        public override void Execute()
        {
            var z0 = ProcessingArea.ObjectIds.GetExtents().MaxPoint.Z + ZSafety;
            Processor.Tool
            generator.IsExtraRotate = IsExtraRotate;

            generator.S = S;
            generator.Feed = CuttingFeed;
            var z00 = ProcessingArea.Select(p => p.ObjectId).GetExtents().MaxPoint.Z + ZSafety;
            generator.SetToolPosition(new Point3d(OriginX, OriginY, 0), 0, 0, z00);
            generator.Command($"G92");

            var offsetDistance = Processing.ToolThickness / 2 + Delta;
            var dbObject = AcadObjects.First().ObjectId.QOpenForRead();

            if (AcadObjects.Count == 1 && dbObject is DbSurface surface1) // ----- Стелла ---------------------------
            {
                if (o is Region region1)
                {
                    var planeSurface = new PlaneSurface();
                    planeSurface.CreateFromRegion(region1);
                    surface1 = planeSurface;
                }

                if (IsReverseOffset)
                    offsetDistance *= -1;
                var offsetSurface1 = DbSurface.CreateOffsetSurface(surface1, offsetDistance);

                //if (curves[0] is Region r)
                //{
                //    curves.Clear();
                //    r.Explode(curves);
                //}
                //var plane = offsetSurface.GetPlane();

                var curves1 = new DBObjectCollection();
                offsetSurface1.Explode(curves1);
                var railCurves = curves1.Cast<Curve>().OrderByDescending(p => Math.Abs(p.EndPoint.Z - p.StartPoint.Z))
                    .Take(2).ToList();
                foreach (var curve in railCurves)
                    if (curve.StartPoint.Z < curve.EndPoint.Z ^ IsReverseDirection)
                        curve.ReverseCurve();

                //if (Approach > 0)
                //    points.Add(railCurves.Select(p => p.StartPoint + Vector3d.ZAxis * Approach).ToArray());
                generator.DU = (DU / StepCount).Round(4);
                generator.GCommand(0, railCurves[0].StartPoint, railCurves[1].StartPoint, IsReverseAngle);

                var stepCurves = railCurves.ConvertAll(p => new
                    { Curve = p, step = (p.EndParam - p.StartParam) / StepCount });
                for (var i = 0; i <= StepCount; i++)
                {
                    var points = stepCurves.ConvertAll(p => p.Curve.GetPointAtParameter(i * p.step));

                    generator.GCommand(1, points[0], points[1]);
                }
            }
        }
    }
}
