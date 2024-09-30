using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;

namespace CAM
{
    public class ToolObjectCnc: ToolObject
    {
        private readonly Machine _machine;
        private readonly Tool _tool;
        private readonly ToolLocationCnc _location;

        private ToolObjectCnc(Machine machine, Tool tool)
        {
            _machine = machine;
            _tool = tool;
            _location = new ToolLocationCnc
            {
                Point = Point3d.Origin
            };
        }

        public static void Show(Machine machine, Tool tool, ToolLocationCnc location)
        {
            if (!(ToolObjectInstance is ToolObjectCnc toolObjectCnc) 
                || toolObjectCnc._machine != machine || toolObjectCnc._tool != tool)
                ToolObjectInstance = new ToolObjectCnc(machine, tool);
            _machine = machine;
            _tool = tool;
            _location = location;
        }
        protected override Curve[] CreateCurves()
        {
            switch (_tool.Type)
            {
                case ToolType.Disk:
                    var thickness = _tool.Thickness.Value;
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

                default:
                    return new Curve[] { new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100) };
            }
        }

        protected override Matrix3d GetTransformMatrix()
        {
            var mat1 = Matrix3d.Displacement(_position.GetVectorTo(position));
            var mat2 = Matrix3d.Rotation(Graph.ToRad(_angleC - angleC), Vector3d.ZAxis, position);
            var mat3 = Matrix3d.Rotation(Graph.ToRad(angleA - _angleA),
                Vector3d.XAxis.RotateBy(Graph.ToRad(-angleC), Vector3d.ZAxis), position);

            //var mat1 = Matrix3d.Displacement(Point.GetVectorTo(millPosition.Point));
            //var mat2 = Matrix3d.Rotation(Graph.ToRad(AngleC - millPosition.AngleC), Vector3d.ZAxis, millPosition.Point);
            //var mat3 = Matrix3d.Rotation(Graph.ToRad(millPosition.AngleA - AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-millPosition.AngleC), Vector3d.ZAxis), millPosition.Point);

            return mat3 * mat2 * mat1;
        }

    }
}