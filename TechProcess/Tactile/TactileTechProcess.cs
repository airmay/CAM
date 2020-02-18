using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Tactile
{
    [Serializable]
    [TechProcess(TechProcessNames.Tactile)]
    public class TactileTechProcess: TechProcessBase
    {
        public TactileTechProcessParams TactileTechProcessParams { get; }

        public RectangleF Contour { get; set; }

        public double? BandWidth { get; set; }

        public double? BandSpacing { get; set; }

        public double? BandStart1 { get; set; }

        public double? BandStart2 { get; set; }

        public string Type { get; set; }

        public TactileTechProcess(string caption, Settings settings) : base(caption, settings)
        {
            TactileTechProcessParams = settings.TactileTechProcessParams.Clone();
            Material = Material.Granite;
        }

        public void CalcContour(ObjectId[] ids)
        {
            var points = ids.QOpenForRead<Curve>().SelectMany(p => p.GetPoints());
            Contour = RectangleF.FromLTRB((float)points.Min(p => p.X), (float)points.Max(p => p.Y), (float)points.Max(p => p.X), (float)points.Min(p => p.Y));
        }

        public void CalcType(ObjectId[] ids)
        {
            Type = null;
            BandSpacing = null;
            BandWidth = null;
            BandStart1 = null;
            BandStart2 = null;

            if (ids.Length < 2)
            {
                Acad.Alert("Выберите 2 элемента");
                return;
            }
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
                    BandStart1 = (center.Y + radius - Contour.Bottom).Round(3);
                    BandStart2 = (center.X + radius - Contour.Left).Round(3);
                }
                else
                {
                    Type = "Конусы диагональные";
                    var ray = new Ray { BasePoint = center };
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Math.PI * 3 / 4, Vector3d.ZAxis);
                    BandStart1 = (ray.GetDistToPoint(new Point3d(Contour.Left, Contour.Bottom, 0), true) + radius).Round(3);
                    ray.UnitDir = Vector3d.XAxis.RotateBy(Math.PI / 4, Vector3d.ZAxis);
                    BandStart2 = (ray.GetDistToPoint(new Point3d(Contour.Right, Contour.Bottom, 0), true) % vector.Length + radius).Round(3);
                }
                return;
            }
            var lines = curves.OfType<Line>().ToArray();
            if (lines.Length == 8)
            {
                Type = "Квадраты";
                BandSpacing = lines.First().Length;
                var points = lines.SelectMany(p => Graph.GetPoints(p));
                var point1 = new Point3d(points.Min(p => p.X), points.Min(p => p.Y), 0);
                BandStart1 = point1.Y + BandSpacing - Contour.Bottom;
                BandStart2 = point1.X + BandSpacing - Contour.Left;
                var point2 = new Point3d(points.Max(p => p.X), points.Max(p => p.Y), 0);
                var vector = point2 - point1;
                BandWidth = (vector.X > vector.Y ? point2.X - point1.X : point2.Y - point1.Y) - BandSpacing * 2;
                return;
            }
            if (lines.Length == 3 || lines.Length == 4)
            {
                var vector = lines[0].Delta;
                double? angle = null;
                if (vector.IsParallelTo(Vector3d.XAxis, Tolerance.Global))
                    angle = 0;
                if (vector.IsParallelTo(Vector3d.XAxis.RotateBy(Math.PI/4, Vector3d.ZAxis)))
                    angle = 45;
                if (vector.IsParallelTo(Vector3d.YAxis, Tolerance.Global))
                    angle = 90;
                if (vector.IsParallelTo(Vector3d.XAxis.RotateBy(Math.PI * 3 / 4, Vector3d.ZAxis)))
                    angle = 45;
                if (angle.HasValue)
                {
                    Type = $"Полосы {angle}";
                    var point = angle == 45 ? new Point3d(Contour.Right, Contour.Bottom, 0) : new Point3d(Contour.Left, Contour.Bottom, 0);
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
            if (Type == null)
            {
                Acad.Alert("Не определен тип плитки");
                return techOperations;
            }
            if (Type.Contains("Конусы"))
            {
                var angle1 = Type.Contains("прямые") ? 0 : 135;
                var angle2 = Type.Contains("прямые") ? 90 : 45;
                techOperations.Add(new BandsTechOperation(this, "Конусы1") { ProcessingAngle = angle1 });
                techOperations.Add(new BandsTechOperation(this, "Конусы2") { ProcessingAngle = angle2 });
                techOperations.Add(new ChamfersTechOperation(this, "Фаска3") { ProcessingAngle = angle1 });
                techOperations.Add(new ChamfersTechOperation(this, "Фаска4") { ProcessingAngle = angle2 });
                techOperations.Add(new ConesTechOperation(this, "Конусы5") { ProcessingAngle = angle1 });
            }
            return techOperations;
        }
    }
}
