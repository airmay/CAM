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

        public Tool Tool { get; set; }

        public int Index { get; set; }

        public Location Location { get; set; }

        public static ToolObject CreateToolObject(Tool tool, int index, bool isFrontPlaneZero)
        {
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Acad.Database.TransactionManager.StartTransaction())
            {
                var toolModel = new ToolObject
                {
                    Tool = tool,
                    Index = index,
                    Location = new Location { Point = Point3d.Origin }
                };
                if (index == 1)
                {
                    var circle0 = new Circle(new Point3d(0, isFrontPlaneZero ? 0 : -tool.Thickness.Value, tool.Diameter / 2), Vector3d.YAxis, tool.Diameter / 2);
                    var circle1 = new Circle(circle0.Center + Vector3d.YAxis * tool.Thickness.Value, Vector3d.YAxis, tool.Diameter / 2);
                    var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * tool.Diameter / 4);
                    toolModel.Curves = new Curve[] { circle0, circle1, axis };
                }
                if (index == 2)
                    toolModel.Curves = new Curve[] { new Circle(Point3d.Origin, Vector3d.ZAxis, 20), new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };

                foreach (var item in toolModel.Curves)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128, new IntegerCollection());
                }
                tr.Commit();
                return toolModel;
            }
        }
    }
}
