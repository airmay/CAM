using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

namespace CAM
{
    public class ToolObject
    {
        public Curve[] Curves { get; set; }

        public bool HasTool { get; set; }

        public Location Location { get; set; }

        public static ToolObject CreateToolObject(Tool tool, bool isFrontPlaneZero)
        {
            if (tool.Type == ToolType.Cable)
                return CreateToolObjectInt(true, new Line(new Point3d(0, -1000, 0), new Point3d(0, 1000, 0)));

            var circle0 = new Circle(new Point3d(0, isFrontPlaneZero ? 0 : -tool.Thickness.Value, tool.Diameter / 2), Vector3d.YAxis, tool.Diameter / 2);
            var circle1 = new Circle(circle0.Center + Vector3d.YAxis * tool.Thickness.Value, Vector3d.YAxis, tool.Diameter / 2);
            var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * tool.Diameter / 4);

            return CreateToolObjectInt(true, circle0, circle1, axis);
        }

        public static ToolObject CreateToolObject() => 
            CreateToolObjectInt(false, new Circle(Point3d.Origin, Vector3d.ZAxis, 20), new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100));

        private static ToolObject CreateToolObjectInt(bool hasTool, params Curve[] curves)
        {
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Acad.Database.TransactionManager.StartTransaction())
            {
                foreach (var item in curves)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128, new IntegerCollection());
                }
                tr.Commit();
            }
            return new ToolObject
            {
                HasTool = hasTool,
                Curves = curves,
                Location = new Location { Point = Point3d.Origin }
            };
        }
    }
}
