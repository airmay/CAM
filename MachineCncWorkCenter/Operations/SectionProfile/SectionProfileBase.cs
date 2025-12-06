using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Processing;
using CAM.UI;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAM.MachineCncWorkCenter.Operations.SectionProfile;

[Serializable]
public abstract class SectionProfileBase : OperationCnc
{
    [NonSerialized] private Curve _rail;
    [NonSerialized] protected Polyline _profile;
    [NonSerialized] private int _outside;
    [NonSerialized] protected int _engineSide;
    [NonSerialized] private Vector2d _offsetVector;
    protected static TextBox AngleATextBox;

    public AcadObject Rail { get; set; }
    public double? Length { get; set; }
    public double Departure { get; set; }
    public double AngleA { get; set; }
    public double? ProfileStart { get; set; }
    public double? ProfileEnd { get; set; }
    public bool IsReverse { get; set; }
    public bool ChangeProcessSide { get; set; }
    public bool ChangeEngineSide { get; set; }

    public static void ConfigureParamsViewBase(ParamsControl view)
    {
        view.AddAcadObject(nameof(Rail), "Направляющая");
        view.AddTextBox(nameof(Length), "Длина направляющей", hint: "Длина направляющей на оси Х");
        view.AddTextBox(nameof(Departure));
        AngleATextBox = view.AddTextBox(nameof(AngleA), "Угол A", hint: "Вертикальный угол");
        view.AddAcadObject(nameof(ProcessingArea), "Профиль");
        view.AddTextBox(nameof(ProfileStart), "Начало профиля", hint: "Расстояние от начала профиля до начала обрабатываемого участка профиля");
        view.AddTextBox(nameof(ProfileEnd), "Конец профиля", hint: "Расстояние от начала профиля до конца обрабатываемого участка профиля");
        view.AddCheckBox(nameof(IsReverse), "Обратно", hint: "Обратное направление обработки профиля");
        view.AddCheckBox(nameof(ChangeProcessSide), "Другая сторона", hint: "Сменить сторону обработки");
        var checkBoxChangeEngineSide = view.AddCheckBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");

        AngleATextBox.Validated += (_, _) => UpdateCheckBoxEnable(checkBoxChangeEngineSide);
    }

    protected static void UpdateCheckBoxEnable(CheckBox checkBox)
    {
        checkBox.Enabled = AngleATextBox.Text == @"0";
        if (!checkBox.Enabled)
        {
            checkBox.Checked = false;
            checkBox.DataBindings[0].WriteValue();
        }
    }

    public override void Execute()
    {
        _outside = ChangeProcessSide.GetSign();
        _engineSide = _outside * ChangeEngineSide.GetSign(); // по-умолчанию двигатель со стороны заготовки, обратно наружней стороне
        _rail = Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * Length.GetValueOrDefault(1000));
        var profileDb = ProcessingArea.Get<Polyline>();
        var railPoint = profileDb.StartPoint.Y <= profileDb.EndPoint.Y ? profileDb.StartPoint : profileDb.EndPoint;
        _profile = profileDb.CreateCopy(Delta * (railPoint == profileDb.StartPoint).GetSign());

        AddProfile3D(profileDb, _rail.StartPoint);
        AddProfile3D(profileDb, _rail.EndPoint);
        if (Delta != 0)
        {
            AddProfile3D(_profile, _rail.StartPoint);
            AddProfile3D(_profile, _rail.EndPoint);
        }

        if (Departure != 0)
            _rail = _rail.ExtendLines(Departure);
        if (_profile.StartPoint.Y > _profile.EndPoint.Y)
            _profile.ReverseCurve();
        _offsetVector = (_profile.StartPoint - railPoint).ToVector2d();
        _profile.TransformBy(Matrix3d.Displacement(Point3d.Origin - _profile.StartPoint));
        if (AngleA > 0)
            _profile.TransformBy(Matrix3d.Rotation(-AngleA.ToRad(), Vector3d.ZAxis, _profile.StartPoint));

        Processor.StartOperation(Math.Abs(profileDb.EndPoint.Y - profileDb.StartPoint.Y));
        return;

        void AddProfile3D(Polyline polyline, Point3d target)
        {
            var angle = _rail.GetTangent(target).Angle - Math.PI / 2 * _outside;
            var matrix = Matrix3d.Displacement(target - railPoint) *
                Matrix3d.Rotation(angle, Vector3d.ZAxis, railPoint) *
                Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, railPoint);
            var p = polyline.GetTransformedCopy(matrix);
            p.Transparency = Acad.GetSemitransparent();
            ProcessingObjectBuilder.AddEntity(p);
        }
    }

    protected IEnumerable<Curve> CreateCurves(IEnumerable<Point2d> points)
    {
        return points.If(AngleA > 0,
                x => x.Select(p => p.TransformBy(Matrix2d.Rotation(AngleA.ToRad(), Point2d.Origin))))
            .Select(p => p + _offsetVector)
            .Select(p => _rail.CreateCopy(-_outside * p.X, p.Y));
    }
}