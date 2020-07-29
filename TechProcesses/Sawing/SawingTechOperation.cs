using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Sawing
{
    [Serializable]
    [TechOperation(1, TechProcessNames.Sawing)]
    public class SawingTechOperation : TechOperationBase
    {
        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public Side OuterSide { get; set; }

        public double AngleA { get; set; }

        public List<SawingMode> SawingModes { get; set; }

        public SawingTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public SawingTechOperation(ITechProcess techProcess, Border border) : base(techProcess, $"Распиловка{border.ObjectId.GetDesc()}")
        {
            SetFromBorder(border);
        }

        public void SetIsExactly(Corner corner, bool value)
        {
            if (corner == Corner.Start)
                IsExactlyBegin = value;
            else
                IsExactlyEnd = value;
        }

        public void SetFromBorder(Border border)
        {
            ProcessingArea = AcadObject.Create(border.ObjectId);
            var par = ((SawingTechProcess)TechProcess).SawingTechProcessParams;
            if (SawingModes == null)
                SawingModes = (border.ObjectId.IsLine() ? par.SawingLineModes : par.SawingCurveModes).ConvertAll(x => x.Clone());
            OuterSide = border.OuterSide;
            IsExactlyBegin = border.IsExactlyBegin;
            IsExactlyEnd = border.IsExactlyEnd;
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            const int CornerIndentIncrease = 5;
            var techProcess = (SawingTechProcess)TechProcess;
            var curve = ProcessingArea.GetCurve();
            var thickness = techProcess.Thickness.Value + 2; // запил на 2мм ниже нижнего края 
            var toolDiameter = techProcess.Tool.Diameter;
            var engineSide = Side.None;
            double offsetArc = 0;
            double angleA = 0;

            if (techProcess.MachineType == MachineType.ScemaLogic)
                AngleA = 0;

            switch (curve)
            {
                case Arc arc:
                    CalcArc(arc);
                    break;
                case Line line:
                    CalcLine(line);
                    break;
                case Polyline polyline:
                    CalcPolyline(polyline);
                    break;
                default:
                    throw new InvalidOperationException($"Кривая типа {curve.GetType()} не может быть обработана.");
            }

            var outerSideSign = OuterSide == Side.Left ^ curve is Line ? -1 : 1;
            var offsetCoeff = Math.Tan(angleA) * outerSideSign;
            var depthCoeff = 1 / Math.Cos(angleA);
            var toolThickness = techProcess.Tool.Thickness.Value * depthCoeff;
            var compensation = (offsetArc + (engineSide == OuterSide ^ techProcess.MachineType == MachineType.Donatoni ? toolThickness : 0)) * outerSideSign;
            var shift = angleA > 0 ? -thickness * offsetCoeff : 0;

            var sumIndent = CalcIndent(thickness) * (Convert.ToInt32(IsExactlyBegin) + Convert.ToInt32(IsExactlyEnd));
            if (sumIndent >= curve.Length())
            {
                if (AngleA != 0)
                    throw new InvalidOperationException("Расчет намечания выполняется только при нулевом вертикальном угле.");
                var point = Scheduling();
                var angle = BuilderUtils.CalcToolAngle((curve.EndPoint - curve.StartPoint).ToVector2d().Angle, engineSide);
                generator.Move(point.X, point.Y, angleC: angle);
                generator.Cutting(point.X, point.Y, point.Z, techProcess.PenetrationFeed);
                return;
            }

            var modes = SawingModes.ConvertAll(p => new CuttingMode { Depth = p.Depth, DepthStep = p.DepthStep, Feed = p.Feed });
            var passList = BuilderUtils.GetPassList(modes, thickness, !ProcessingArea.ObjectId.IsLine()).ToList();

            Curve toolpathCurve = null;
            foreach (var item in passList)
            {
                CreateToolpath(item.Key, compensation + shift + item.Key * offsetCoeff);
                if (generator.IsUpperTool)
                {
                    var point = engineSide == Side.Right ^ (passList.Count() % 2 == 1) ? toolpathCurve.EndPoint : toolpathCurve.StartPoint;
                    var vector = Vector3d.ZAxis * (item.Key + generator.ZSafety);
                    if (angleA != 0)
                        vector = vector.RotateBy(outerSideSign * angleA, ((Line)toolpathCurve).Delta) * depthCoeff;
                    var p0 = point + vector;
                    var angleC = BuilderUtils.CalcToolAngle(toolpathCurve, point, engineSide);
                    generator.Move(p0.X, p0.Y, angleC: angleC, angleA: Math.Abs(AngleA));
                    if (techProcess.MachineType == MachineType.ScemaLogic)
                        generator.Command("28;;XYCZ;;;;;;", "Цикл");
                }
                generator.Cutting(toolpathCurve, item.Value, techProcess.PenetrationFeed, engineSide);
            }
            generator.Uplifting(Vector3d.ZAxis.RotateBy(outerSideSign * angleA, toolpathCurve.EndPoint - toolpathCurve.StartPoint) * (thickness + generator.ZSafety) * depthCoeff);

            if (!IsExactlyBegin || !IsExactlyEnd)
            {
                var gashCurve = curve.GetOffsetCurves(shift)[0] as Curve;
                if (!IsExactlyBegin)
                    Acad.CreateGash(gashCurve, gashCurve.StartPoint, OuterSide, thickness * depthCoeff, toolDiameter, toolThickness);
                if (!IsExactlyEnd)
                    Acad.CreateGash(gashCurve, gashCurve.EndPoint, OuterSide, thickness * depthCoeff, toolDiameter, toolThickness);
                gashCurve.Dispose();
            }

            // Local func ------------------------

            void CalcArc(Arc arc)
            {
                AngleA = 0;
                var startSide = Math.Sign(Math.Cos(arc.StartAngle.Round(3)));
                var endSide = Math.Sign(Math.Cos(arc.EndAngle.Round(3)));
                var cornersOneSide = Math.Sign(startSide * endSide);

                if (arc.TotalAngle.Round(3) > Math.PI && cornersOneSide > 0)
                    throw new InvalidOperationException("Обработка дуги невозможна - дуга пересекает углы 90 и 270 градусов.");

                if (cornersOneSide < 0) //  дуга пересекает углы 90 или 270 градусов
                {
                    if (techProcess.MachineType == MachineType.ScemaLogic)
                        throw new InvalidOperationException("Обработка дуги невозможна - дуга пересекает угол 90 или 270 градусов.");

                    engineSide = startSide > 0 ? Side.Left : Side.Right;
                }
                if (OuterSide == Side.Left) // внутренний рез дуги
                {
                    if (techProcess.MachineType == MachineType.Donatoni && engineSide != Side.Left) // подворот диска при вн. резе дуги
                    {
                        engineSide = Side.Right;
                        //var comp = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - thickness * (toolDiameter - thickness));
                        //AngleA = Math.Atan2(comp, thickness).ToDeg();

                        var R = arc.Radius;
                        var t = thickness;
                        var d = toolDiameter;
                        var comp = (2 * R * t * t - Math.Sqrt(-d * d * d * d * t * t + 4 * d * d * R * R * t * t + d * d * t * t * t * t)) / (d * d - 4 * R * R);
                        AngleA = -Math.Atan2(comp, thickness).ToDeg();
                    }
                    else
                        offsetArc = arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - thickness * (toolDiameter - thickness));
                }
                if (engineSide == Side.None)
                    engineSide = (startSide + endSide) > 0 ? Side.Right : Side.Left;
            }

            void CalcLine(Line line)
            {
                angleA = AngleA.ToRad();
                engineSide = AngleA == 0 ? BuilderUtils.CalcEngineSide(line.Angle) : AngleA > 0 ? OuterSide : OuterSide.Opposite();
            }

            void CalcPolyline(Polyline polyline)
            {
                int sign = 0;
                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    var point = polyline.GetPoint3dAt(i);
                    var s = Math.Sign(Math.Sin(polyline.GetTangent(point).Angle.Round(6)));
                    if (s == 0)
                        continue;
                    if (sign == 0)
                    {
                        sign = s;
                        continue;
                    }
                    var bulge = polyline.GetBulgeAt(i - 1);
                    if (bulge == 0)
                        bulge = polyline.GetBulgeAt(i);

                    if (s != sign)
                    {
                        if (techProcess.MachineType == MachineType.ScemaLogic)
                            throw new InvalidOperationException("Обработка полилинии невозможна - кривая пересекает углы 90 или 270 градусов.");
                        var side = sign > 0 ^ bulge < 0 ? Side.Left : Side.Right;
                        if (engineSide != Side.None)
                        {
                            if (engineSide != side)
                                throw new InvalidOperationException("Обработка полилинии невозможна.");
                        }
                        else
                            engineSide = side;
                        sign = s;
                    }
                    else
                        if (Math.Abs(bulge) > 1)
                            throw new InvalidOperationException("Обработка невозможна - дуга полилинии пересекает углы 90 и 270 градусов.");
                }
                if (engineSide == Side.None)
                    engineSide = BuilderUtils.CalcEngineSide(polyline.GetTangent(polyline.StartPoint).Angle);
            }

            void CreateToolpath(double depth, double offset)
            {
                toolpathCurve = curve.GetOffsetCurves(offset)[0] as Curve;
                toolpathCurve.TransformBy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));
                if (!IsExactlyBegin && !IsExactlyEnd)
                    return;
                var indent = CalcIndent(depth * depthCoeff);
                
                switch (toolpathCurve)
                {
                    case Line l:
                        if (IsExactlyBegin)
                            l.StartPoint = l.GetPointAtDist(indent);
                        if (IsExactlyEnd)
                            l.EndPoint = l.GetPointAtDist(l.Length - indent);
                        break;

                    case Arc a:
                        var indentAngle = indent / ((Arc)curve).Radius;
                        if (IsExactlyBegin)
                            a.StartAngle = a.StartAngle + indentAngle;
                        if (IsExactlyEnd)
                            a.EndAngle = a.EndAngle - indentAngle;
                        break;

                    case Polyline p:
                        if (IsExactlyBegin)
                            p.SetPointAt(0, p.GetPointAtDist(indent).ToPoint2d());
                        //p.StartPoint = p.GetPointAtDist(indent);
                        if (IsExactlyEnd)
                            //p.EndPoint = p.GetPointAtDist(p.Length - indent);
                            p.SetPointAt(p.NumberOfVertices - 1, p.GetPointAtDist(p.Length - indent).ToPoint2d());
                        break;
                };
            }

            double CalcIndent(double depth) => Math.Sqrt(depth * (toolDiameter - depth)) + CornerIndentIncrease;

            /// <summary>
            /// Расчет точки намечания
            /// </summary>
            Point3d Scheduling()
            {
                var vector = curve.EndPoint - curve.StartPoint;
                var depth = thickness;
                var point = Point3d.Origin;
                if (IsExactlyBegin && IsExactlyEnd)
                {
                    var l = vector.Length - 2 * CornerIndentIncrease;
                    depth = (toolDiameter - Math.Sqrt(toolDiameter * toolDiameter - l * l)) / 2;
                    point = curve.StartPoint + vector / 2;
                }
                else
                {
                    var indentVector = vector.GetNormal() * (Math.Sqrt(depth * (toolDiameter - depth)) + CornerIndentIncrease);
                    point = IsExactlyBegin ? curve.StartPoint + indentVector : curve.EndPoint - indentVector;
                    Acad.CreateGash(curve, IsExactlyBegin ? curve.EndPoint : curve.StartPoint, OuterSide, depth, toolDiameter, toolThickness, point);
                }
                return point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
            }
        }
    }
}
    