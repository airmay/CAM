using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using CAM.Utils;

namespace CAM.Operations.Sawing
{
    [Serializable]
    public class SawingOperation : Operation
    {
        public Side OuterSide { get; set; }
        public double ToolThickness => Tool.Thickness.Value;
        public double Thickness { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }
        public double AngleA { get; set; }
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
            view.AddTextBox(nameof(AngleA));
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(
                allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            view.AddCheckBox(nameof(ChangeSide), "Сменить сторону", "Поменять обрабатываемою сторону у объектов");
            var depthTextBox = view.AddTextBox(nameof(Depth));
            view.AddTextBox(nameof(Penetration),
                toolTipText:
                "Шаг заглубления для прямой и если не заданы Режимы для криволинейных траекторий то для всех кривых");
            view.AddText("Режимы для криволинейных траекторий", "Режимы применяются для дуги и полилинии");
            view.AddControl(new SawingModesView(), 6, nameof(SawingModesView.DataSource), nameof(SawingModes));

            thicknessTextBox.Validated += (sender, args) =>
            {
                if (depthTextBox.Text == "0")
                    depthTextBox.Text = (view.GetParams<SawingOperation>().Thickness + 2).ToString();
            };
        }

        public override void Execute(Processor processor)
        {
            var (curvesSides, pointsIsExactly) = CalcСurveProcessingInfo();

            foreach (var item in curvesSides)
            {
                ProcessCurve(processor, item.Key, item.Value, pointsIsExactly[item.Key.StartPoint], pointsIsExactly[item.Key.EndPoint]);
            }
        }

        private (Dictionary<Curve, Side> CurvesSides, Dictionary<Point3d, bool> PointsIsExactly) CalcСurveProcessingInfo()
        {
            var curvesSides = new Dictionary<Curve, Side>();
            var pointsIsExactly = new Dictionary<Point3d, bool>(Graph.Point3dComparer);
            var side = ChangeSide ? -1 : 1;
            var curvesToCalc = new List<Curve>(ProcessingArea.GetCurves());

            while (curvesToCalc.Any())
            {
                var chain = CalcChain();
                var hatchId = Graph.CreateHatch(chain.ToPolyline(), side);
                if (hatchId.HasValue)
                    Support = Support.AppendToGroup(hatchId.Value);
                curvesToCalc.RemoveAll(p => chain.Contains(p));
            }

            return (curvesSides, pointsIsExactly);

            List<Curve> CalcChain()
            {
                Tolerance.Global = new Tolerance(0.001, 0.001);

                var chain = new List<Curve>();
                var pointCurveDict = curvesToCalc
                    .SelectMany(p => p.GetStartEndPoints(), (cv, pt) => (cv, pt))
                    .ToLookup(p => p.pt, p => p.cv, Graph.Point3dComparer);
                var corner = pointCurveDict.FirstOrDefault(p => p.Count() == 1);
                var (curve, point) = corner != null
                    ? (corner.Single(), corner.Key)
                    : (curvesToCalc.First(), curvesToCalc.First().StartPoint);
                if (corner != null)
                {
                    pointsIsExactly[point] = IsExactlyBegin;
                }

                var startPoint = point;
                do
                {
                    var sd = point == curve.StartPoint ? side : -side;
                    curvesSides[curve] = sd;
                    chain.Add(curve);

                    point = curve.NextPoint(point);
                    var endTangent = curve.GetTangent(point);
                    if (point == curve.StartPoint)
                        endTangent *= -1;

                    curve = pointCurveDict[point].SingleOrDefault(p => p != curve);
                    if (curve == null)
                    {
                        pointsIsExactly[point] = IsExactlyEnd;
                        break;
                    }

                    var startTangent =
                        curve.GetTangent(point.IsEqualTo(curve.StartPoint) ? curve.StartPoint : curve.EndPoint);
                    if (point == curve.EndPoint)
                        startTangent *= -1;
                    var angle = endTangent.MinusPiToPiAngleTo(startTangent);
                    pointsIsExactly[point] = side == 1 ^ angle < 0;
                } while (point != startPoint);

                return chain;
            }
        }

        private void ProcessCurve(Processor processor, Curve curve, Side side, bool isExactlyBegin, bool isExactlyEnd)
        {
            var gashLength = GetGashLength(Depth);
            var indent = gashLength + CornerIndentIncrease;
            AddGash(curve, isExactlyBegin, isExactlyEnd, side, gashLength, indent);

            var sumIndent = indent * (Convert.ToInt32(isExactlyBegin) + Convert.ToInt32(isExactlyEnd));
            if (sumIndent >= curve.Length())
            {
                Scheduling(processor, curve, isExactlyBegin, isExactlyEnd, indent);
                return;
            }

            var engineSide = EngineSideCalculator.Calculate(curve, MachineType);
            var compensation = 0D;

            if (curve is Arc arc && side == Side.Left) // внутренний рез дуги
            {
                if (MachineType == MachineType.Donatoni && !(arc.StartAngle.CosSign() == 1 && arc.EndAngle.CosSign() == -1)) //  дуга не пересекает угол 90 градусов
                {
                    // подворот диска при вн. резе дуги
                    engineSide = Side.Right;
                    var R = arc.Radius;
                    var t = Thickness;
                    var d = Tool.Diameter;
                    var comp = (2 * R * t * t - Math.Sqrt(-d * d * d * d * t * t + 4 * d * d * R * R * t * t + d * d * t * t * t * t)) / (d * d - 4 * R * R);
                    AngleA = -Math.Atan2(comp, Thickness).ToDeg();
                }
                else
                    compensation = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - Thickness * (Tool.Diameter - Thickness));
            }

            var isFrontPlaneZero = MachineService.Machines[MachineType].IsFrontPlaneZero;
            if (engineSide == side ^ isFrontPlaneZero)
                compensation += ToolThickness;
            var offsetSign = side == Side.Left ^ curve is Line ? -1 : 1;
            var baseCurve = curve.GetOffsetCurves(compensation * offsetSign)[0] as Curve;

            var passList = GetPassList(curve is Arc);
            var tip = engineSide == Side.Right ^ (passList.Count % 2 == 1)
                ? CurveTip.End
                : CurveTip.Start;
            processor.EngineSide = engineSide;
            foreach (var (depth, feed) in passList)
            {
                indent = isExactlyBegin || isExactlyEnd ? GetGashLength(depth) + CornerIndentIncrease : 0;
                processor.CuttingFeed = feed;
                Cutting(depth);
                tip = tip.Swap();
            }
            processor.Uplifting();

            return;

            void Cutting(double depth)
            {
                var toolpath = baseCurve.GetTransformedCopy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));
                switch (toolpath)
                {
                    case Line line:
                        if (isExactlyBegin) line.StartPoint = line.GetPointAtDist(indent);
                        if (isExactlyEnd) line.EndPoint = line.GetPointAtDist(line.Length - indent);
                        processor.Cutting(line, tip);
                        break;

                    case Arc toolpathArc:
                        var indentAngle = indent / toolpathArc.Radius;
                        if (isExactlyBegin) toolpathArc.StartAngle += indentAngle;
                        if (isExactlyEnd) toolpathArc.EndAngle -= indentAngle;
                        processor.Cutting(toolpathArc, tip);
                        break;

                    case Polyline polyline:
                        if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                        if (isExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
                        processor.Cutting(polyline, tip);
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
        private double GetGashLength(double depth) => Math.Sqrt(depth * (Tool.Diameter - depth));
        private double CalcIndent(double depth) => GetGashLength(depth) + CornerIndentIncrease;

        public void AddGash(Curve curve, bool isExactlyBegin, bool isExactlyEnd, Side side, double gashLength, double indent)
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
                var offsetVector = normal.GetPerpendicularVector() * ToolThickness * (side == Side.Left ? 1 : -1);
                var gash = NoDraw.Pline(point, point2, point2 + offsetVector, point + offsetVector);
                gash.LayerId = Acad.GetGashLayerId();
                Support.AppendToGroup(gash.Add());
            }
        }

        private void Scheduling(Processor processor, Curve curve, bool isExactlyBegin, bool isExactlyEnd, double indent)
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
                    var d = (Tool.Diameter - Math.Sqrt(Tool.Diameter * Tool.Diameter - l * l)) / 2;
                    return (curve.StartPoint + vector / 2, d);
                }

                var indentVector = vector.GetNormal() * indent;
                var pt = isExactlyBegin
                    ? curve.StartPoint + indentVector
                    : curve.EndPoint - indentVector;

                return (pt, Thickness);
            }
        }
    }
}