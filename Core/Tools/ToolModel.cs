using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CAM.Autocad;
using CAM.Utils;

namespace CAM.Core.Tools;

public static class ToolModel
{
    public const double WireSawLength = 1500;

    private static Curve[] _curves;
    private static Tool _tool;
    private static ToolPosition _position;

    public static void Set(Tool tool, ToolPosition position)
    {
        if (tool != _tool)
            Delete();

        _tool = tool;
        if (tool == null) 
            return;

        Move(position);
        _position = position;
    }

    public static void Delete()
    {
        if (_curves == null || Acad.ActiveDocument == null)
            return;

        using (Acad.LockDocument())
        using (Acad.StartTransaction())
            foreach (var item in _curves)
            {
                TransientManager.CurrentTransientManager.EraseTransient(item, []);
                item.Dispose();
            }

        _curves = null;
    }

    private static void Move(ToolPosition to)
    {
        if (_curves == null)
            Create();

        // знак поворота по углу C = +1 если поворачивается стол, -1 если поворачивается инструмент
        // TODO
        var signAngleC = (_tool.Type == ToolType.WireSaw).GetSign();

        var displacement = Matrix3d.Displacement(_position.Point.GetVectorTo(to.Point));
        var rotationC = Matrix3d.Rotation((to.AngleC - _position.AngleC) * signAngleC, Vector3d.ZAxis, to.Point);
        // todo ToRad()
        var rotationA = Matrix3d.Rotation((to.AngleA - _position.AngleA).ToRad(), Vector3d.XAxis.RotateBy(-to.AngleC, Vector3d.ZAxis), to.Point);
        var matrix = rotationA * rotationC * displacement;

        foreach (var item in _curves)
        {
            item.TransformBy(matrix);
            TransientManager.CurrentTransientManager.UpdateTransient(item, []);
        }

        Acad.Editor.UpdateScreen();
    }

    private static void Create()
    {
        _position = new ToolPosition();
        _curves = CreateCurves();

        using (Acad.ActiveDocument.LockDocument())
        using (var tr = Acad.Database.TransactionManager.StartTransaction())
        {
            var color = Color.FromColorIndex(ColorMethod.ByColor, 131);
            foreach (var item in _curves)
            {
                item.Color = color;
                TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128, []);
            }

            tr.Commit();
        }
    }

    private static Curve[] CreateCurves()
    {
        switch (_tool.Type)
        {
            case ToolType.Disk:
                var radius = _tool.Diameter / 2;
                var circle0 = new Circle(new Point3d(0, 0, radius), Vector3d.YAxis, radius);
                var circle1 = new Circle(circle0.Center + Vector3d.YAxis * _tool.Thickness.Value, Vector3d.YAxis, radius);
                var axis = new Line(circle1.Center, circle1.Center + Vector3d.YAxis * radius / 4);
                return [circle0, circle1, axis];

            case ToolType.Mill:
                return
                [
                    new Circle(Point3d.Origin, Vector3d.ZAxis, 20),
                    new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100)
                ];

            case ToolType.WireSaw:
                var r = _tool.Thickness.Value / 2;
                var line = new Line(new Point3d(-WireSawLength, 0, 0), new Point3d(WireSawLength, 0, 0));
                return 
                [
                    line,
                    new Circle(line.StartPoint, Vector3d.XAxis, r),
                    new Circle(line.EndPoint, Vector3d.XAxis, r),
                    new Circle(line.GetPointAtParameter(line.Length / 2), Vector3d.XAxis, r)
                ];

            default:
                return [new Line(Point3d.Origin, Point3d.Origin + Vector3d.ZAxis * 100)];
        }
    }
}