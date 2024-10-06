using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    public interface ITool
    {
        Curve[] GetModel(Machine? machine);
    }

    /// <summary>
    /// Инструмент
    /// </summary>
    [Serializable]
    public class Tool: ITool
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

        public override string ToString() => $"№{Number} {Type.GetDescription()} Ø{Diameter}{(Thickness.HasValue ? " × " + Thickness.ToString() : null)} {Name}";

        public Curve[] GetModel(Machine? machine)
        {
            var thickness = Thickness.Value;
            switch (Type)
            {
                case ToolType.Disk:
                    var frontY = Settings.Machines[machine.Value].IsFrontPlaneZero ? 0 : -thickness;
                    var radius = Diameter / 2;
                    var circle0 = new Circle(new Point3d(0, frontY, radius), Vector3d.YAxis, radius);
                    var circle1 = new Circle(circle0.Center + Vector3d.YAxis * thickness, Vector3d.YAxis, radius);
                    var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * radius / 4);
                    return new Curve[] { circle0, circle1, axis };

                case ToolType.Mill:
                    return new Curve[]
                    {
                        new Circle(Point3d.Origin, Vector3d.ZAxis, 20),
                        new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100)
                    };

                case ToolType.Cable:
                    var line = new Line(new Point3d(0, -1500, 0), new Point3d(0, 1500, 0));
                    return new Curve[]
                    {
                        new Circle(line.StartPoint, Vector3d.YAxis, thickness / 2),
                        new Circle(line.EndPoint, Vector3d.YAxis, thickness / 2),
                        new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.YAxis,
                            thickness / 2)
                    };

                default:
                    return new Curve[] { new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };
            }
        }
    }
}
