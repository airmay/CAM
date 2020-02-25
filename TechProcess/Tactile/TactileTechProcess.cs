using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechProcess(TechProcessNames.Tactile)]
    public class TactileTechProcess: TechProcessBase
    {
        public ProcessingArea Objects { get; set; }

        public TactileTechProcessParams TactileTechProcessParams { get; }

        public double? BandWidth { get; set; }

        public double? BandSpacing { get; set; }

        public double? BandStart1 { get; set; }

        public double? BandStart2 { get; set; }

        public int? ProcessingAngle1 { get; set; }

        public int? ProcessingAngle2 { get; set; }

        public string Type { get; set; }

        public TactileTechProcess(string caption, Settings settings) : base(caption, settings)
        {
            TactileTechProcessParams = settings.TactileTechProcessParams.Clone();
            Material = Material.Granite;
        }

        public override void Init(Settings settings)
        {
            base.Init(settings);
            Objects.Refresh();
        }

        public Polyline GetContour()
        {
            var points = ProcessingArea.Curves.SelectMany(p => p.GetStartEndPoints());
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
                const double Delta = 1;
                radius += Delta;
                var vector = circles[1].Center - circles[0].Center;
                var center = (circles[1].Center.X < circles[0].Center.X ? circles[1] : circles[0]).Center;
                BandSpacing = (radius * 2).Round(3);
                BandWidth = (vector.Length - BandSpacing.Value).Round(3);
                if (vector.IsParallelTo(Vector3d.XAxis) || vector.IsPerpendicularTo(Vector3d.XAxis))
                {
                    Type = "Конусы прямые";
                    ProcessingAngle1 = 0;
                    ProcessingAngle2 = 90;
                    BandStart1 = (center.Y + radius - contourPoints[0].Y).Round(3);
                    BandStart2 = (center.X + radius - contourPoints[0].X).Round(3);
                }
                else
                {
                    Type = "Конусы диагональные";
                    var ray = new Ray { BasePoint = center };

                    ProcessingAngle1 = 135;
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle1.Value), Vector3d.ZAxis);
                    BandStart1 = (ray.GetDistToPoint(contourPoints[0], true) + radius).Round(3);

                    ProcessingAngle2 = 45;
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Graph.ToRad(ProcessingAngle2.Value), Vector3d.ZAxis);
                    BandStart2 = (ray.GetDistToPoint(contourPoints[3], true) % vector.Length + radius).Round(3);
                }
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
                    ProcessingAngle1 = 45;
                if (ProcessingAngle1.HasValue)
                {
                    Type = $"Полосы {ProcessingAngle1}";
                    var point = ProcessingAngle1 == 45 ? contourPoints[3] : contourPoints[0];
                    var dist = lines.Select(p => p.GetDistToPoint(point)).OrderBy(p => p).ToArray();
                    var s1 = dist[1] - dist[0];
                    var s2 = dist[2] - dist[1];
                    BandWidth = Math.Max(s1, s2);
                    BandSpacing = Math.Min(s1, s2);
                    BandStart1 = s1 > s2 ? dist[0] : dist[1];
                    return;
                }
            }
            Acad.Alert("Тип плитки не распознан");
        }

        public override List<ITechOperation> CreateTechOperations()
        {
            List<ITechOperation> techOperations = new List<ITechOperation>();
            if (Type.Contains("Конусы"))
            {
                techOperations.Add(new BandsTechOperation(this, "Полосы", ProcessingAngle1, BandStart1));
                techOperations.Add(new BandsTechOperation(this, "Полосы", ProcessingAngle2, BandStart2));
                techOperations.Add(new ChamfersTechOperation(this, "Фаска", 0));
                techOperations.Add(new ChamfersTechOperation(this, "Фаска", 90));
                techOperations.Add(new ConesTechOperation(this, "Конусы"));
            }
            if (Type.Contains("Полосы"))
            {
                techOperations.Add(new BandsTechOperation(this, "Полосы", ProcessingAngle1, BandStart1));
                techOperations.Add(new ChamfersTechOperation(this, "Фаска", ProcessingAngle1.Value));
            }
            return techOperations;
        }

        public override bool Validate()
        {
            if (!base.Validate())
                return false;
            if (Type == null)
            {
                Acad.Alert("Не определен тип плитки");
                return false;
            }
            return true;
        }
    }
}
