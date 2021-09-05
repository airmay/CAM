using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    /// <summary>
    /// Инструмент
    /// </summary>
    [Serializable]
    public class Tool
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public ToolType Type { get; set; }

        /// <summary>
        /// Диаметр
        /// </summary>
        public double Diameter { get; set; }

        /// <summary>
        /// Толщина
        /// </summary>
        public double? Thickness { get; set; }

        [NonSerialized]
        public bool IsFrontPlaneZero;

        public override string ToString() => $"№{Number} {Type.GetDescription()} Ø{Diameter}{(Thickness.HasValue ? " × " + Thickness.ToString() : null)} {Name}";

        public Curve[] GetModelCurves()
        {
            switch (Type)
            {
                case ToolType.Disk:
                    var circle0 = new Circle(new Point3d(0, IsFrontPlaneZero ? 0 : -Thickness.Value, Diameter / 2), Vector3d.YAxis, Diameter / 2);
                    var circle1 = new Circle(circle0.Center + Vector3d.YAxis * Thickness.Value, Vector3d.YAxis, Diameter / 2);
                    var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * Diameter / 4);
                    return new Curve[] { circle0, circle1, axis };

                case ToolType.Mill:
                    return new Curve[] { new Circle(Point3d.Origin, Vector3d.ZAxis, 20), new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };

                case ToolType.Cable:
                    var line = new Line(new Point3d(0, -1000, 0), new Point3d(0, 1000, 0));
                    circle0 = new Circle(line.StartPoint, Vector3d.YAxis, Thickness.Value / 2);
                    circle1 = new Circle(line.EndPoint, Vector3d.YAxis, Thickness.Value / 2);
                    var circle2 = new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.YAxis, Thickness.Value / 2);
                    return new Curve[] { circle0, circle1, circle2, line };

                default:
                    return new Curve[] { new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };
            }
        }
    }
}
