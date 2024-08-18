using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Dreambuild.AutoCAD;

namespace CAM
{
    public class ToolObject
    {
        public Curve[] Curves { get; set; }
        private Machine _machine;
        private Tool _tool;
        private Point3d _position;
        private double _angleC;
        private double _angleA;

        public void Set(Machine? machine, Tool tool, Point3d position, double angleC, double angleA)
        {
            if (Curves != null && (position.IsNull() || machine != _machine || tool != _tool))
                Hide();
            if (tool == null || position.IsNull())
                return;

            _machine = machine.Value;
            _tool = tool;
            if (Curves == null)
                CreateCurves();

            TransformCurves(position, angleC, angleA);
        }

        public void Hide()
        {
            if (Curves == null)
                return;

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Acad.Database.TransactionManager.StartTransaction())
                foreach (var item in Curves)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }

            Curves = null;
        }

        private void CreateCurves()
        {
            Curves = Create();

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (var tr = Acad.Database.TransactionManager.StartTransaction())
            {
                foreach (var item in Curves)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128,
                        new IntegerCollection());
                }

                tr.Commit();
            }

            _position = Point3d.Origin;
            _angleC = 0;
            _angleA = 0;

            return;

            Curve[] Create()
            {
                var thickness = _tool.Thickness.Value;
                switch (_tool.Type)
                {
                    case ToolType.Disk:
                        var frontY = Settings.Machines[_machine].IsFrontPlaneZero ? 0 : -thickness;
                        var radius = _tool.Diameter / 2;
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
                        circle0 = new Circle(line.StartPoint, Vector3d.YAxis, thickness / 2);
                        circle1 = new Circle(line.EndPoint, Vector3d.YAxis, thickness / 2);
                        var circle2 = new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.YAxis,
                            thickness / 2);
                        return new Curve[] { circle0, circle1, circle2, line };

                    default:
                        return new Curve[] { new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };
                }
            }
        }

        private void TransformCurves(Point3d position, double angleC, double angleA)
        {
            var matrix = GetTransformMatrix();

            foreach (var item in Curves)
            {
                item.TransformBy(matrix);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }

            Acad.Editor.UpdateScreen();
            return;

            Matrix3d GetTransformMatrix()
            {
                var mat1 = Matrix3d.Displacement(_position.GetVectorTo(position));
                var mat2 = Matrix3d.Rotation(Graph.ToRad(angleC - _angleC), Vector3d.ZAxis, position);
                var mat3 = Matrix3d.Rotation(Graph.ToRad(angleA - _angleA),
                    Vector3d.XAxis.RotateBy(Graph.ToRad(-angleC), Vector3d.ZAxis), position);

                //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(millPosition.Point));
                //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - millPosition.AngleC), Vector3d.ZAxis, millPosition.Point);
                //var mat3 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-millPosition.AngleC), Vector3d.ZAxis), millPosition.Point);

                return mat3 * mat2 * mat1;
            }
        }
    }
}
