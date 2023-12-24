using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using CAM.Utils;
using System.Drawing;

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
                toolTipText: "Шаг заглубления для прямой и если не заданы Режимы для криволинейных траекторий то для всех кривых");
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
            var curvesSides = new Dictionary<Curve, int>();
            var pointsIsExactly = new Dictionary<Point3d, bool>(Graph.Point3dComparer);
            var curves = ProcessingArea.GetCurves();
            CalcСurveProcessing(curves);
            foreach (var curve in curves)
            {
                ProcessCurve(processor, curve, curvesSides[curve], pointsIsExactly[curve.StartPoint], pointsIsExactly[curve.EndPoint]);
            }

            return;

            void CalcСurveProcessing(IEnumerable<Curve> curvesArray)
            {
                var side = ChangeSide ? -1 : 1;
                var curvesToCalc = new List<Curve>(curvesArray);

                while (curvesToCalc.Any())
                {
                    var chain = CalcChain(curvesToCalc, side);
                    var hatchId = Graph.CreateHatch(chain.ToPolyline(), side);
                    if (hatchId.HasValue)
                        Support = Support.AppendToGroup(hatchId.Value);
                    curvesToCalc.RemoveAll(p => chain.Contains(p));
                }
            }

            List<Curve> CalcChain(List<Curve> curvesToCalc, int side)
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

                    var startTangent = curve.GetTangent(point.IsEqualTo(curve.StartPoint) ? curve.StartPoint : curve.EndPoint);
                    if (point == curve.EndPoint)
                        startTangent *= -1;
                    var angle = endTangent.MinusPiToPiAngleTo(startTangent);
                    pointsIsExactly[point] = side == 1 ^ angle < 0;
                } 
                while (point != startPoint);

                return chain;
            }
        }

        private void ProcessCurve(Processor processor, Curve curve, int side, bool isExactlyBegin, bool isExactlyEnd)
        {
            //    var pt = curve.StartPoint;
            //    foreach (var pass in PassIterator.Create(pt, curve.NextPoint(pt)))
            //    {

            //    }


            //    var point = ToolPosition.IsDefined ? curve.GetClosestPoint(ToolPosition.Point) : curve.StartPoint;
            //    var calcAngleC = angleC == null;

            //    if (IsUpperTool) // && (angleA ?? AngleA).GetValueOrDefault() == 0)
            //    {
            //        if (IsChangeStart)
            //            point = curve.NextPoint(point);
            //        //var upperPoint = new Point3d(point.X, point.Y, ZSafety);
            //        //if (!ToolPosition.Point.IsEqualTo(upperPoint))
            //        //{
            //        if (!angleC.HasValue)
            //            angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

            //        Move(point.X, point.Y, angleC: angleC, angleA: angleA);
            //        //}
            //    }
            //    else if (!(curve is Line) && calcAngleC)
            //        angleC = BuilderUtils.CalcToolAngle(curve, point, engineSide);

            //    if (!point.IsEqual(ToolPosition.Point))
            //        GCommand(point.Z != ToolPosition.Point.Z ? CommandNames.Penetration : CommandNames.Transition, 1, point: point, angleC: angleC, angleA: angleA, feed: smallFeed);

            //    if (curve is Polyline polyline)
            //    {
            //        if (point == polyline.EndPoint)
            //        {
            //            polyline.ReverseCurve();
            //            engineSide = engineSide.Opposite();
            //        }
            //        for (int i = 1; i < polyline.NumberOfVertices; i++)
            //        {
            //            point = polyline.GetPoint3dAt(i);
            //            if (calcAngleC)
            //                angleC = BuilderUtils.CalcToolAngle(polyline, point, engineSide);
            //            if (polyline.GetSegmentType(i - 1) == SegmentType.Arc)
            //            {
            //                var arcSeg = polyline.GetArcSegment2dAt(i - 1);
            //                GCommand(CommandNames.Cutting, arcSeg.IsClockWise ? 2 : 3, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed, center: arcSeg.Center);
            //            }
            //            else
            //                GCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed);
            //        }
            //    }
            //    else
            //    {
            //        var arc = curve as Arc;
            //        if (arc != null && calcAngleC)
            //            angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
            //        var gCode = curve is Line ? 1 : point == curve.StartPoint ? 3 : 2;
            //        GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());
            //    }
            //}

            var engineSide = EngineSideCalculator.Calculate(curve, MachineType);
            double compensation = 0;

            if (curve is Arc arc && OuterSide == Side.Left) // внутренний рез дуги
            {
                if (MachineType == MachineType.Donatoni && 
                    !(Math.Cos(arc.StartAngle.Round(3)) > 0 && Math.Cos(arc.EndAngle.Round(3)) < 0)) //  дуга не пересекает угол 90 градусов
                {
                    // подворот диска при вн. резе дуги
                    engineSide = Side.Right;
                    var R = arc.Radius;
                    var t = Thickness;
                    var d = Tool.Diameter;
                    var comp = (2 * R * t * t - Math.Sqrt(-d * d * d * d * t * t + 4 * d * d * R * R * t * t +
                                                          d * d * t * t * t * t)) / (d * d - 4 * R * R);
                    AngleA = -Math.Atan2(comp, Thickness).ToDeg();
                }
                else
                    compensation = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - Thickness * (Tool.Diameter - Thickness));
            }

            var isFrontPlaneZero = MachineService.Machines[MachineType].IsFrontPlaneZero;
            if (engineSide == OuterSide ^ isFrontPlaneZero)
                compensation += Tool.Thickness.Value;
            var offsetSign = OuterSide == Side.Left ^ curve is Line ? -1 : 1;
            var baseCurve = curve.GetOffsetCurves(compensation * offsetSign)[0] as Curve;

            var sumIndent = CalcIndent(Depth) * (Convert.ToInt32(IsExactlyBegin) + Convert.ToInt32(IsExactlyEnd));
            if (sumIndent >= baseCurve.Length())
            {
                Scheduling(baseCurve);
                return;
            }

            var passList = GetPassList(curve is Arc);
            var tip = engineSide == Side.Right ^ (passList.Count % 2 == 1)
                ? CurveTip.End
                : CurveTip.Start;

            foreach (var (depth, feed) in passList)
            {
                var indent = isExactlyBegin || isExactlyEnd ? CalcIndent(depth) : 0;
                processor.CuttingFeed = feed;
                processor.Cutting(baseCurve, tip, engineSide, depth, isExactlyBegin, isExactlyEnd, indent);
                tip = tip.Swap();
            }
            processor.Uplifting();


            if ((!IsExactlyBegin || !IsExactlyEnd) && Departure == 0)
            {
                var gashCurve = curve.GetOffsetCurves(shift)[0] as Curve;
                var gashList = new List<ObjectId>();
                if (!IsExactlyBegin)
                    gashList.Add(Acad.CreateGash(gashCurve, gashCurve.StartPoint, OuterSide, thickness * depthCoeff,
                        toolDiameter, toolThickness));
                if (!IsExactlyEnd)
                    gashList.Add(Acad.CreateGash(gashCurve, gashCurve.EndPoint, OuterSide, thickness * depthCoeff,
                        toolDiameter, toolThickness));
                if (gashList.Count > 0)
                    Modify.AppendToGroup(base.TechProcess.ExtraObjectsGroup.Value, gashList.ToArray());
                gashCurve.Dispose();
            }

            generator.Uplifting(
                Vector3d.ZAxis.RotateBy(outerSideSign * angleA, toolpathCurve.EndPoint - toolpathCurve.StartPoint) *
                (thickness + generator.ZSafety) * depthCoeff);

            return;


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
            } 
            while (depth < Depth);

            return passList;
        }

        private const int CornerIndentIncrease = 5;
        // запил
        private double CalcGash(double depth) => Math.Sqrt(depth * (Tool.Diameter - depth));
        // отступ
        private double CalcIndent(double depth) => CalcGash(depth) + CornerIndentIncrease;

        private Curve CreateToolpath(Curve curve, double depth)
        {
            var indent = CalcIndent(depth);
            var toolpath = curve.GetTransformedCopy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));

            switch (toolpath)
            {
                case Line line:
                    if (IsExactlyBegin) line.StartPoint = line.GetPointAtDist(indent);
                    if (IsExactlyEnd) line.EndPoint = line.GetPointAtDist(line.Length - indent);
                    return line;

                case Arc arc:
                    var indentAngle = indent / arc.Radius;
                    if (IsExactlyBegin) arc.StartAngle += indentAngle;
                    if (IsExactlyEnd) arc.EndAngle -= indentAngle;
                    return arc;

                case Polyline polyline:
                    if (IsExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                    if (IsExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
                    return polyline;

                default: throw new Exception();
            }
        }

        /// <summary>
        /// Расчет точки намечания
        /// </summary>
        /// <param name="curve"></param>
        Point3d Scheduling(Curve curve)
        {
            var vector = curve.EndPoint - curve.StartPoint;
            var depth = Thickness;
            var point = Point3d.Origin;
            if (IsExactlyBegin && IsExactlyEnd)
            {
                var l = vector.Length - 2 * CornerIndentIncrease;
                depth = (Tool.Diameter - Math.Sqrt(Tool.Diameter * Tool.Diameter - l * l)) / 2;
                point = curve.StartPoint + vector / 2;
            }
            else
            {
                var indentVector = vector.GetNormal() * CalcIndent(depth);
                point = IsExactlyBegin ? curve.StartPoint + indentVector : curve.EndPoint - indentVector;
                Acad.CreateGash(curve, IsExactlyBegin ? curve.EndPoint : curve.StartPoint, OuterSide, depth, Tool.Diameter, ToolThickness, point);
            }

            var angle = BuilderUtils.CalcToolAngle((curve.EndPoint - curve.StartPoint).ToVector2d().Angle, engineSide);
            generator.Move(point.X, point.Y, angleC: angle);
            generator.Cutting(point.X, point.Y, point.Z, TechProcess.PenetrationFeed);
            generator.Uplifting();

            return point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
        }
    }
}