﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.RotationProfileBody
{
    [Serializable]
    [MenuItem("Тело вращения по профилю", 7)]
    public class RotationProfileBodyTechProcess : MillingTechProcess
    {
        public int CuttingFeed { get; set; }

        public double Penetration { get; set; }

        public double StartZ { get; set; }

        public double StepZ { get; set; }

        public double RadiusMin { get; set; }

        public double RadiusMax { get; set; }

        public double Delta { get; set; }

        public RotationProfileBodyTechProcess()
        {
            MachineType = CAM.Machine.Donatoni;
            Material = CAM.Material.Marble;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddIndent();
            view.AddAcadObject(nameof(ProcessingArea), "Профиль", "Укажите профиль тела вращения");
            view.AddIndent();
            view.AddTextBox(nameof(Penetration));
            view.AddTextBox(nameof(StartZ), "Z начало");
            view.AddTextBox(nameof(StepZ), "Шаг Z");
            view.AddTextBox(nameof(RadiusMin), "Радиус мин.");
            view.AddTextBox(nameof(RadiusMax), "Радиус макс.");
            view.AddIndent();
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(ZSafety));
        }

        protected override void BuildProcessing(MillingCommandGenerator generator)
        {
            var toolThickness = Tool.Thickness.Value;
            var profile = ProcessingArea.GetCurve();
            if (Delta != 0)
                profile = (Curve)profile.GetOffsetCurves(Delta)[0];

            var zMax = profile.GetStartEndPoints().Max(p => p.Y);
            generator.SetZSafety(ZSafety, zMax);
            var xMax = 0D;

            using (var curve = profile.ToCompositeCurve2d())
            using (var ray = new Ray2d())
            using (var intersector = new CurveCurveIntersector2d())
            {
                var angleC = 360;
                var gCode = 2;

                for (var z = StartZ; z > 0; z -= StepZ)
                {
                    var xMin = RadiusMin;
                    for (int i = 0; i < 3; i++)
                    {
                        ray.Set(new Point2d(0, z + i * toolThickness / 2), Vector2d.XAxis);
                        intersector.Set(curve, ray);
                        if (intersector.NumberOfIntersectionPoints == 1)
                            xMin = Math.Max(xMin, intersector.GetIntersectionPoint(0).X);
                    }
                    if (xMin == 0) throw new Exception("Нет точек пересечения с профилем");

                    var x = Math.Max(xMin, RadiusMax - Penetration);
                    var s = x - xMin;
                    int passCount = (int)Math.Ceiling(s / Penetration);
                    var dx = s > Consts.Epsilon ? s / passCount : 1;

                    if (generator.IsUpperTool)
                        generator.Move(0, -x - ZSafety, angleC: 0, angleA: 90);
                    else
                        generator.Transition(y: -x - ZSafety, feed: CuttingFeed);
                    generator.Transition(z: z);
                    do
                    {
                        var arc = new Arc(new Point3d(0, 0, z), x, Math.PI * 1.5, Math.PI * 1.5 - Consts.Epsilon);
                        generator.Cutting(0, -x, z, PenetrationFeed);
                        generator.GCommand(CommandNames.Cutting, gCode, angleC: angleC, curve: arc, center: arc.Center.To2d(), feed: CuttingFeed);
                        angleC = 360 - angleC;
                        gCode = 5 - gCode;
                        x -= dx;
                    }
                    while (x >= xMin - Consts.Epsilon);

                    xMax = Math.Max(xMin, RadiusMax);
                }
                generator.Transition(y: -xMax - ZSafety, feed: PenetrationFeed);
                generator.Uplifting();
            }
        }
    }
}
