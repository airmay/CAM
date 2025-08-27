using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CAM.Core;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CAM
{
    public static class ToolObject
    {
        public static double WireSawLength = 1500;

        private static Curve[] _model;
        private static Tool _tool;
        private static ToolPosition _position;
        public static Machine Machine { get; set; }

        public static void Set(Tool tool, ToolPosition position)
        {
            if (tool == null || tool != _tool)
                Delete();

            _tool = tool;
            if (tool == null) 
                return;

            if (_model == null)
                CreateModel();

            TransformModel(_position, position);
            _position = position;
        }

        public static void Delete()
        {
            if (_model == null || Acad.ActiveDocument == null)
                return;

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Acad.Database.TransactionManager.StartTransaction())
                foreach (var item in _model)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }

            _model = null;
        }

        private static void CreateModel()
        {
            _position = new ToolPosition();
            var thickness = _tool.Thickness.Value;

            switch (_tool.Type)
            {
                case ToolType.Disk:
                    var frontY = Settings.Machines[Machine].IsFrontPlaneZero ? 0 : -thickness;
                    var radius = _tool.Diameter / 2;
                    var circle0 = new Circle(new Point3d(0, frontY, radius), Vector3d.YAxis, radius);
                    var circle1 = new Circle(circle0.Center + Vector3d.YAxis * thickness, Vector3d.YAxis, radius);
                    var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * radius / 4);
                    _model = new Curve[] { circle0, circle1, axis };
                    break;

                case ToolType.Mill:
                    _model = new Curve[]
                    {
                        new Circle(Point3d.Origin, Vector3d.ZAxis, 20),
                        new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100)
                    };
                    break;

                case ToolType.WireSaw:
                    var line = new Line(new Point3d(-WireSawLength, 0, 0), new Point3d(WireSawLength, 0, 0));
                    _model = new Curve[]
                    {
                        line,
                        new Circle(line.StartPoint, Vector3d.XAxis, thickness / 2),
                        new Circle(line.EndPoint, Vector3d.XAxis, thickness / 2),
                        new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.XAxis, thickness/ 2)
                    };
                    break;

                default:
                    _model = new Curve[] { new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };
                    break;
            }

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (var tr = Acad.Database.TransactionManager.StartTransaction())
            {
                foreach (var item in _model)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128,
                        new IntegerCollection());
                }

                tr.Commit();
            }
        }

        private static void TransformModel(ToolPosition from, ToolPosition to)
        {
            var displacement = Matrix3d.Displacement(from.Point.GetVectorTo(to.Point));
            var rotationC = Matrix3d.Rotation(from.AngleC - to.AngleC, Vector3d.ZAxis, to.Point);
            // todo ToRad()
            var rotationA = Matrix3d.Rotation((from.AngleA - to.AngleA).ToRad(), Vector3d.XAxis.RotateBy(-to.AngleC.ToRad(), Vector3d.ZAxis), to.Point);
            var matrix = rotationA * rotationC * displacement;

            foreach (var item in _model)
            {
                item.TransformBy(matrix);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }

            Acad.Editor.UpdateScreen();
        }
    }
}
