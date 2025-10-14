using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using Dreambuild.AutoCAD;
using CAM.Utils;
using Autodesk.AutoCAD.Colors;

namespace CAM.Operations.Sawing
{
    [Serializable]
    public class SawingOperation : OperationCnc
    {
        public double Thickness { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }
        public double Departure { get; set; }
        public bool ChangeSide { get; set; }

        public double Depth { get; set; }
        public double? Penetration { get; set; }
        public List<SawingMode> SawingModes { get; set; } = new List<SawingMode>();

        public static void ConfigureParamsView(ParamsControl view)
        {
            var thicknessTextBox = view.AddTextBox(nameof(Thickness), required: true);
            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}", required: true);
            view.AddCheckBox(nameof(ChangeSide), "Сменить сторону", "Поменять обрабатываемою сторону");
            var depthTextBox = view.AddTextBox(nameof(Depth), required: true);
            view.AddTextBox(nameof(Penetration), hint: "Шаг заглубления для прямой и если не заданы Режимы для криволинейных траекторий то для всех кривых", required: true);
            view.AddText("Режимы для криволинейных траекторий", "Режимы применяются для дуги и полилинии");
            view.AddControl(new SawingModesView(), 6, nameof(SawingModesView.DataSource), nameof(SawingModes));

            thicknessTextBox.Validated += (sender, args) =>
            {
                if (view.GetData<SawingOperation>().Depth == 0)
                {
                    var depth = view.GetData<SawingOperation>().Thickness + 2;
                    view.GetData<SawingOperation>().Depth = depth;
                    depthTextBox.Text = depth.ToString();
                }
            };
        }

        public override void Execute()
        {
            var curves = ProcessingArea.GetCurves();
            var (curveSides, points, outerSide) = CalcСurveProcessingInfo(curves);
            Processor.StartOperation(0);
            foreach (var curve in curves)
            {
                if (curveSides.TryGetValue(curve, out var curveSide))
                    ProcessCurve(curve, (Side)curveSide, points[curve.StartPoint], points[curve.EndPoint]);
            }

            Graph.CreateHatch(curveSides.Keys.ToList().ToPolyline(), outerSide, p => Processor.AddEntity(p));
        }

        private (Dictionary<Curve, int>, Dictionary<Point3d, bool>, int) CalcСurveProcessingInfo(Curve[] curves)
        {
            var curveSides = new Dictionary<Curve, int>();
            var points = new Dictionary<Point3d, bool>(Graph.Point3dComparer);

            var pointCurveDict = curves
                .SelectMany(p => p.GetStartEndPoints(), (cv, pt) => (cv, pt))
                .ToLookup(p => p.pt, p => p.cv, Graph.Point3dComparer);
            var corner = pointCurveDict.FirstOrDefault(p => p.Count() == 1);
            var (curve, point) = corner != null
                ? (corner.Single(), corner.Key)
                : (curves.First(), curves.First().StartPoint);
            if (corner != null)
                points[point] = IsExactlyBegin;
            var direction = curve.GetDirection(point);
            var startСurve = curve;
            var center = Graph.GetCenter(pointCurveDict.Select(p => p.Key));
            var outerSide = -curve.GetSide(center) * direction;
            if (outerSide == 0)
                outerSide = 1;
            outerSide *= ChangeSide.GetSign(-1);

            do
            {
                curveSides[curve] = outerSide * direction;
                point = curve.NextPoint(point);
                var endTangent = curve.GetFirstDerivative(point) * direction;

                curve = pointCurveDict[point].SingleOrDefault(p => p != curve);
                if (curve == null)
                {
                    points[point] = IsExactlyEnd;
                    break;
                }

                point = point.IsEqualTo(curve.StartPoint) ? curve.StartPoint : curve.EndPoint;
                direction = curve.GetDirection(point);
                var startTangent = curve.GetFirstDerivative(point) * direction;
                points[point] = endTangent.GetSide(startTangent) * outerSide > 0;
            } 
            while (curve != startСurve);

            return (curveSides, points, outerSide);
        }

        private void ProcessCurve(Curve curve, Side outerSide, bool isExactlyBegin, bool isExactlyEnd)
        {
            var gashLength = GetGashLength(Depth);
            var indent = gashLength + CornerIndentIncrease;
            var sumIndent = indent * (Convert.ToInt32(isExactlyBegin) + Convert.ToInt32(isExactlyEnd));
            if (sumIndent >= curve.Length())
            {
                Scheduling(Processor, curve, isExactlyBegin, isExactlyEnd, indent);
                return;
            }
            if (!isExactlyBegin || !isExactlyEnd)
                AddGash(curve, isExactlyBegin, isExactlyEnd, outerSide, gashLength, indent);

            var engineSide = EngineSideCalculator.Calculate(curve);
            var compensation = 0D;
            var аngleA = 0D;

            if (curve is Arc arc && outerSide == Side.Left) // внутренний рез дуги
            {
                if (!(arc.StartAngle.CosSign() == 1 && arc.EndAngle.CosSign() == -1)) //  дуга не пересекает угол 90 градусов
                {
                    // подворот диска при вн. резе дуги
                    engineSide = Side.Right;
                    var R = arc.Radius;
                    var t = Thickness;
                    var d = ToolDiameter;
                    var comp = (2*R*t*t - Math.Sqrt(-d*d*d*d * t*t + 4 * d*d * R*R * t*t + d*d * t*t*t*t)) / (d*d - 4*R*R);
                    аngleA = -Math.Atan2(comp, Thickness);
                }
                else
                    compensation = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - Thickness * (ToolDiameter - Thickness));
            }

            if (engineSide != outerSide)
                compensation += ToolThickness;
            var offsetSign = outerSide == Side.Left ^ curve is Line ? -1 : 1;
            var baseCurve = curve.GetOffsetCurves(compensation * offsetSign)[0] as Curve;
            
            var passList = GetPassList(curve is Arc);
            var fromStart = engineSide == Side.Left ^ (passList.Count % 2 == 1);

            foreach (var (depth, feed) in passList)
            {
                indent = isExactlyBegin || isExactlyEnd ? GetGashLength(depth) + CornerIndentIncrease : 0;
                Cutting(depth, feed);
                fromStart = !fromStart;
            }
            Processor.Uplifting();

            return;

            void Cutting(double depth, int feed)
            {
                var toolpath = baseCurve.GetTransformedCopy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));
                switch (toolpath)
                {
                    case Line line:
                        if (isExactlyBegin) line.StartPoint = line.GetPointAtDist(indent);
                        if (isExactlyEnd) line.EndPoint = line.GetPointAtDist(line.Length - indent);
                        Processor.Cutting(line, fromStart, feed, engineSide);
                        break;

                    case Arc toolpathArc:
                        var indentAngle = indent / toolpathArc.Radius;
                        if (isExactlyBegin) toolpathArc.StartAngle += indentAngle;
                        if (isExactlyEnd) toolpathArc.EndAngle -= indentAngle;
                        Processor.Cutting(toolpathArc, fromStart, аngleA, feed, engineSide);
                        break;

                    case Polyline polyline:
                        if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                        if (isExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
                        Processor.Cutting(polyline, fromStart, feed, engineSide);
                        break;

                    default: throw new Exception();
                }
            }
        }

        private List<(double, int)> GetPassList(bool isArc)
        {
            var modes = isArc && SawingModes.Any()
                ? SawingModes.OrderByDescending(p => p.Depth.HasValue).ThenBy(p => p.Depth).ToList()
                : new List<SawingMode> { new SawingMode { DepthStep = Penetration.Value, Feed = Processing.CuttingFeed } };
            var index = 0;
            var mode = modes[0];
            var depth = isArc ? -mode.DepthStep : 0;
            var passList = new List<(double, int)>();
            do
            {
                depth += mode.DepthStep;
                if (index < modes.Count - 1 && depth >= mode.Depth)
                    mode = modes[++index];
                if (depth > Depth)
                    depth = Depth;
                passList.Add((depth, mode.Feed));
            } 
            while (depth < Depth);

            return passList;
        }

        private const int CornerIndentIncrease = 5;

        private double GetGashLength(double depth) => Math.Sqrt(depth * (ToolDiameter - depth));

        private void AddGash(Curve curve, bool isExactlyBegin, bool isExactlyEnd, Side side, double gashLength, double indent)
        {
            var vector = curve.EndPoint - curve.StartPoint;
            if (isExactlyBegin ^ isExactlyEnd && indent > vector.Length)
                gashLength += indent - vector.Length;
            if (!isExactlyBegin)
                CreateCurve(curve.StartPoint, -gashLength);
            if (!isExactlyEnd)
                CreateCurve(curve.EndPoint, gashLength);
            return;

            void CreateCurve(Point3d point, double length)
            {
                var normal = curve.GetFirstDerivative(point).GetNormal();
                var point2 = point + normal * length;
                var offsetVector = normal.GetPerpendicularVector() * ToolThickness * (int)side;
                var gash = NoDraw.Pline(point, point2, point2 + offsetVector, point + offsetVector);
                gash.Color = Color.FromColorIndex(ColorMethod.ByColor, 210);
                Processor.AddEntity(gash);
            }
        }

        private void Scheduling(ProcessorCnc processor, Curve curve, bool isExactlyBegin, bool isExactlyEnd, double indent)
        {
            var vector = curve.EndPoint - curve.StartPoint;
            var (point, depth) = CalcSchedulingPoint();
            var angle = BuilderUtils.CalcToolAngle(vector.ToVector2d().Angle);
            processor.Move(point, angle);
            processor.Penetration(point.WithZ(-depth));
            processor.Uplifting();
            return;

            (Point3d, double) CalcSchedulingPoint()
            {
                if (isExactlyBegin && isExactlyEnd)
                {
                    var l = vector.Length - 2 * CornerIndentIncrease;
                    var d = (ToolDiameter - Math.Sqrt(ToolDiameter * ToolDiameter - l * l)) / 2;
                    return (curve.StartPoint + vector / 2, d);
                }

                var indentVector = vector.GetNormal() * indent;
                var pt = isExactlyBegin
                    ? curve.StartPoint + indentVector
                    : curve.EndPoint - indentVector;

                return (pt, Depth);
            }
        }
    }
}