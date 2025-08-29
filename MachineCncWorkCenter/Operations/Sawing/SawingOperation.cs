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
        public List<CuttingMode> SawingModes { get; set; } = new List<CuttingMode>();

        public static void ConfigureParamsView(ParamsView view)
        {
            var thicknessTextBox = view.AddTextBox(nameof(Thickness));
            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            view.AddCheckBox(nameof(ChangeSide), "Сменить сторону", "Поменять обрабатываемою сторону");
            var depthTextBox = view.AddTextBox(nameof(Depth));
            view.AddTextBox(nameof(Penetration), toolTipText: "Шаг заглубления для прямой и если не заданы Режимы для криволинейных траекторий то для всех кривых");
            view.AddText("Режимы для криволинейных траекторий", "Режимы применяются для дуги и полилинии");
            view.AddControl(new SawingModesView(), 6, nameof(SawingModesView.DataSource), nameof(SawingModes));

            thicknessTextBox.Validated += (sender, args) =>
            {
                if (depthTextBox.Text == "0")
                    depthTextBox.Text = (view.GetParams<SawingOperation>().Thickness + 2).ToString();
            };
        }

        public override void Execute()
        {
            var (curveSides, points, outerSide) = CalcСurveProcessingInfo();
            foreach (var curve in ProcessingArea.GetCurves())
            {
                if (curveSides.TryGetValue(curve, out var curveSide))
                    ProcessCurve(curve, curveSide, points[curve.StartPoint], points[curve.EndPoint]);
            }

            CreateHatch(curveSides, outerSide.Opposite());
        }

        private (Dictionary<Curve, Side>, Dictionary<Point3d, bool>, Side) CalcСurveProcessingInfo()
        {
            var curves = ProcessingArea.GetCurves();
            var curveSides = new Dictionary<Curve, Side>();
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
            var direction = curve.IsStartPoint(point).GetSign();
            var startСurve = curve;

            var center = Graph.GetCenter(pointCurveDict.Select(p => p.Key));
            //var s = center.GetSide(curve.StartPoint, curve.EndPoint) * direction * ChangeSide.GetSign(-1);
            var side = Graph.IsTurnRight(point, curve.NextPoint(point), center) ^ ChangeSide ? Side.Left : Side.Right;
            do
            {
                curveSides[curve] = side.Set(direction);
                point = curve.NextPoint(point);
                var endTangent = curve.GetTangent(point) * direction;

                curve = pointCurveDict[point].SingleOrDefault(p => p != curve);
                if (curve == null)
                {
                    points[point] = IsExactlyEnd;
                    break;
                }

                point = point.IsEqualTo(curve.StartPoint) ? curve.StartPoint : curve.EndPoint;
                direction = curve.IsStartPoint(point).GetSign();
                var startTangent = curve.GetTangent(point) * direction;

                var angle = endTangent.MinusPiToPiAngleTo(startTangent);
                points[point] = side.IsLeft() ^ angle < 0;  // angle < 0 - поворот направо
            } 
            while (curve != startСurve);

            return (curveSides, points, side);
        }

        private void CreateHatch(Dictionary<Curve, Side> curveSides, Side side)
        {
            var poilyline = curveSides.Keys.ToList().ToPolyline();
            Graph.CreateHatch(poilyline, side, p => Processor.AddEntity(p));
            //if (hatchId.HasValue)
            //    SupportGroup = SupportGroup.AppendToGroup(hatchId.Value);
        }

        private void ProcessCurve(Curve curve, Side outerSide, bool isExactlyBegin, bool isExactlyEnd)
        {
            var gashLength = GetGashLength(Depth);
            var indent = gashLength + CornerIndentIncrease;
            AddGash(curve, isExactlyBegin, isExactlyEnd, outerSide, gashLength, indent);

            var sumIndent = indent * (Convert.ToInt32(isExactlyBegin) + Convert.ToInt32(isExactlyEnd));
            if (sumIndent >= curve.Length())
            {
                Scheduling(Processor, curve, isExactlyBegin, isExactlyEnd, indent);
                return;
            }

            var engineSide = EngineSideCalculator.Calculate(curve, Machine);
            var compensation = 0D;
            var аngleA = 0D;

            if (curve is Arc arc && outerSide == Side.Left) // внутренний рез дуги
            {
                if (Machine == Machine.Donatoni && !(arc.StartAngle.CosSign() == 1 && arc.EndAngle.CosSign() == -1)) //  дуга не пересекает угол 90 градусов
                {
                    // подворот диска при вн. резе дуги
                    engineSide = Side.Right;
                    var R = arc.Radius;
                    var t = Thickness;
                    var d = ToolDiameter;
                    var comp = (2*R*t*t - Math.Sqrt(-d*d*d*d * t*t + 4 * d*d * R*R * t*t + d*d * t*t*t*t)) / (d*d - 4*R*R);
                    аngleA = -Math.Atan2(comp, Thickness).ToDeg();
                }
                else
                    compensation = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - Thickness * (ToolDiameter - Thickness));
            }

            var isFrontPlaneZero = Settings.Machines[Machine].IsFrontPlaneZero;
            if (engineSide == outerSide ^ isFrontPlaneZero)
                compensation += ToolThickness;
            var offsetSign = outerSide == Side.Left ^ curve is Line ? -1 : 1;
            var baseCurve = curve.GetOffsetCurves(compensation * offsetSign)[0] as Curve;

            var passList = GetPassList(curve is Arc);
            var tip = engineSide == Side.Right ^ (passList.Count % 2 == 1)
                ? CurveTip.End
                : CurveTip.Start;
            Processor.EngineSide = engineSide;
            Processor.StartOperation();
            foreach (var (depth, feed) in passList)
            {
                indent = isExactlyBegin || isExactlyEnd ? GetGashLength(depth) + CornerIndentIncrease : 0;
                Cutting(depth, feed);
                tip = tip.Swap();
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
                        Processor.Cutting(line, tip, feed);
                        break;

                    case Arc toolpathArc:
                        var indentAngle = indent / toolpathArc.Radius;
                        if (isExactlyBegin) toolpathArc.StartAngle += indentAngle;
                        if (isExactlyEnd) toolpathArc.EndAngle -= indentAngle;
                        Processor.Cutting(toolpathArc, tip, аngleA, feed);
                        break;

                    case Polyline polyline:
                        if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                        if (isExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
                        Processor.Cutting(polyline, tip, feed);
                        break;

                    default: throw new Exception();
                }
            }
        }

        private List<(double, int)> GetPassList(bool isArc)
        {
            var modes = isArc && SawingModes.Any()
                ? SawingModes.OrderByDescending(p => p.Depth.HasValue).ThenBy(p => p.Depth).ToList()
                : new List<CuttingMode> { new CuttingMode { DepthStep = Penetration.Value, Feed = CuttingFeed } };
            var index = 0;
            var mode = modes[index];
            var depth = isArc ? -mode.DepthStep : 0;
            var passList = new List<(double, int)>();
            do
            {
                depth += mode.DepthStep;
                if (depth >= mode.Depth && index < modes.Count - 1)
                    mode = modes[++index];
                if (depth > Depth)
                    depth = Depth;
                passList.Add((depth, mode.Feed));
            } while (depth < Depth);

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
            processor.Penetration(-depth);
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