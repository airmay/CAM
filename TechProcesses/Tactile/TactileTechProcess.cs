﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    [MenuItem("Тактилка", 2)]
    public class TactileTechProcess: MillingTechProcess
    {
        public AcadObject Objects { get; set; }

        public double? BandWidth { get; set; }

        public double? BandSpacing { get; set; }

        public double? BandStart1 { get; set; }

        public double? BandStart2 { get; set; }

        public int? ProcessingAngle1 { get; set; }

        public int? ProcessingAngle2 { get; set; }

        public string Type { get; set; }

        public double Depth { get; set; }

        public double Departure { get; set; }

        public int TransitionFeed { get; set; }

        public double Delta { get; set; }

        public double AC { get; set; }
        public double AC_V { get; set; }

        public TactileTechProcess()
        {
            Material = CAM.Material.Granite;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddIndent();
            view.AddTextBox(nameof(Depth));
            view.AddTextBox(nameof(Departure));
            view.AddComboBox("Режимы", new[] { "Отрезок", "Кривая" }, p => { });
            view.AddTextBox(nameof(TransitionFeed));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddOrigin();
            view.AddIndent();
            view.AddTextBox(nameof(Delta), "Припуск от конуса");
            view.AddAcadObject(nameof(ProcessingArea), "Контур плитки", "Выберите объекты контура плитки", AcadObjectNames.Line);
            view.AddAcadObject(nameof(Objects), "2 элемента плитки", "Выберите 2 элемента плитки",
                afterSelect: ids =>
                {
                    view.GetParams<TactileTechProcess>().CalcType(ids);
                    view.ResetControls();;
                });
            view.AddIndent();
            view.AddTextBox(nameof(Type), "Тип плитки", true);
            view.AddTextBox(nameof(BandWidth), "Ширина полосы");
            view.AddTextBox(nameof(BandSpacing), "Расст.м/у полосами");
            view.AddTextBox(nameof(BandStart1), "Начало полосы 1");
            view.AddTextBox(nameof(BandStart2), "Начало полосы 2");
            view.AddIndent();
            view.AddTextBox(nameof(AC), "a + c");
            view.AddTextBox(nameof(AC_V), "a + c (верт)");
                ;
        }

        protected override void BuildProcessing(MillingCommandGenerator generator)
        {
            //base.BuildProcessing(generator);
            generator.Tool = Tool;
            //generator.AC = AC + Tool.Thickness.GetValueOrDefault();
            //generator.AC_V = AC_V + Tool.Thickness.GetValueOrDefault();
        }

        public Polyline GetContour()
        {
            var points = ProcessingArea.ObjectIds.SelectMany(p => Acad.OpenForRead(p).GetStartEndPoints());
            return NoDraw.Rectang(new Point3d(points.Min(p => p.X), points.Min(p => p.Y), 0), new Point3d(points.Max(p => p.X), points.Max(p => p.Y), 0));
        }

        public void CalcType(ObjectId[] ids)
        {
            Type = null;
            BandSpacing = null;
            BandWidth = null;
            BandStart1 = null;
            BandStart2 = null;
            ProcessingAngle1 = null;
            ProcessingAngle2 = null;

            if (ids.Length < 2)
            {
                Acad.Alert("Выберите 2 элемента");
                return;
            }
            var contourPoints = GetContour().GetPolyPoints().ToArray();
            var curves = ids.QOpenForRead<Curve>();
            var circles = curves.OfType<Circle>().ToArray();
            if (circles.Any())
            {
                var radius = circles.Max(p => p.Radius);
                circles = circles.Where(p => p.Radius == radius).ToArray();
                radius += Delta;
                var vector = circles[1].Center - circles[0].Center;
                var center = (circles[1].Center.X < circles[0].Center.X ? circles[1] : circles[0]).Center;
                BandSpacing = radius * 2;
                BandWidth = vector.Length - BandSpacing.Value;
                if (vector.IsParallelTo(Vector3d.XAxis) || vector.IsPerpendicularTo(Vector3d.XAxis))
                {
                    Type = "Конусы прямые";
                    ProcessingAngle1 = 0;
                    ProcessingAngle2 = 90;
                    BandStart1 = center.Y + radius - contourPoints[0].Y;
                    BandStart2 = center.X + radius - contourPoints[0].X;
                }
                else
                {
                    Type = "Конусы диагональные";
                    var ray = new Ray { BasePoint = center };

                    ProcessingAngle1 = 135;
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle1.Value), Vector3d.ZAxis);
                    BandStart1 = ray.GetDistToPoint(contourPoints[0], true) + radius;

                    ProcessingAngle2 = 45;
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle2.Value), Vector3d.ZAxis);
                    BandStart2 = ray.GetDistToPoint(contourPoints[3], true) % vector.Length + radius;
                }
                RoundParams();
                return;
            }
            var lines = curves.OfType<Line>().ToArray();
            if (lines.Length == 8)
            {
                Type = "Квадраты";
                ProcessingAngle1 = 0;
                ProcessingAngle2 = 90;
                BandSpacing = lines.First().Length;
                var points = lines.SelectMany(p => Graph.GetStartEndPoints(p));
                var point1 = new Point3d(points.Min(p => p.X), points.Min(p => p.Y), 0);
                BandStart1 = point1.Y + BandSpacing - contourPoints[0].Y;
                BandStart2 = point1.X + BandSpacing - contourPoints[0].X;
                var point2 = new Point3d(points.Max(p => p.X), points.Max(p => p.Y), 0);
                var vector = point2 - point1;
                BandWidth = (vector.X > vector.Y ? point2.X - point1.X : point2.Y - point1.Y) - BandSpacing * 2;

                RoundParams();
                return;
            }
            if (lines.Length == 3 || lines.Length == 4)
            {
                var vector = lines[0].Delta;
                if (vector.IsParallelTo(Vector3d.XAxis, Tolerance.Global))
                    ProcessingAngle1 = 0;
                if (vector.IsParallelTo(Vector3d.XAxis.RotateBy(Math.PI/4, Vector3d.ZAxis)))
                    ProcessingAngle1 = 45;
                if (vector.IsParallelTo(Vector3d.YAxis, Tolerance.Global))
                    ProcessingAngle1 = 90;
                if (vector.IsParallelTo(Vector3d.XAxis.RotateBy(Math.PI * 3 / 4, Vector3d.ZAxis)))
                    ProcessingAngle1 = 135;
                if (ProcessingAngle1.HasValue)
                {
                    Type = $"Полосы {ProcessingAngle1}";
                    var point = ProcessingAngle1 == 45 ? contourPoints[3] : contourPoints[0];
                    var dist = lines.Select(p => p.GetDistToPoint(point, true)).OrderBy(p => p).ToArray();
                    var s1 = dist[1] - dist[0];
                    var s2 = dist[2] - dist[1];
                    BandWidth = s2; // Math.Max(s1, s2);
                    BandSpacing = s1; // Math.Min(s1, s2);
                    //BandStart1 = (s1 > s2 ? dist[0] : dist[1]) % (BandWidth + BandSpacing);
                    BandStart1 = dist[1] % (BandWidth + BandSpacing);

                    RoundParams();
                    return;
                }
            }
            Acad.Alert("Тип плитки не распознан");

            void RoundParams()
            {
                BandSpacing = BandSpacing?.Round(3);
                BandWidth = BandWidth?.Round(3);
                BandStart1 = BandStart1?.Round(3);
                BandStart2 = BandStart2?.Round(3);
            }
        }

        public override List<TechOperation> CreateTechOperations()
        {
            var techOperations = new List<TechOperation>();
            techOperations.Add(new BandsTechOperation(this, "Полосы", ProcessingAngle1, BandStart1));
            if (ProcessingAngle2 != null)
                techOperations.Add(new BandsTechOperation(this, "Полосы", ProcessingAngle2, BandStart2));

            if (!Type.Contains("Конусы"))
            {
                techOperations.Add(new ChamfersTechOperation(this, "Фаска", ProcessingAngle1, BandStart1));
                if (ProcessingAngle2 != null)
                    techOperations.Add(new ChamfersTechOperation(this, "Фаска", ProcessingAngle2, BandStart2));
            }
            else
                techOperations.Add(new ConesTechOperation(this, "Конусы"));

            return techOperations;
        }

        public override bool Validate() => Type.CheckNotNull("Тип плитки");
    }
}
