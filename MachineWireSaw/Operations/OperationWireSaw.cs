using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Dreambuild.AutoCAD;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;

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
            var z0 = ProcessingArea.ObjectIds.GetExtents().MaxPoint.Z + Processing.ZSafety;
            Processor.SetToolPosition(0, 0, z0);
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
                Processor.GCommand(0, railCurves[0].StartPoint, railCurves[1].StartPoint, IsReverseAngle);

                var stepCurves = railCurves.ConvertAll(p => new
                    { Curve = p, step = (p.EndParam - p.StartParam) / StepCount });
                for (var i = 0; i <= StepCount; i++)
                {
                    var points = stepCurves.ConvertAll(p => p.Curve.GetPointAtParameter(i * p.step));

                    Processor.GCommand(1, points[0], points[1]);
                }
            }
        }
    }
}
