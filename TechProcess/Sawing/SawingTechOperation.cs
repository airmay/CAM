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
            ProcessingArea = new AcadObject(border.ObjectId);
            var par = ((SawingTechProcess)TechProcess).SawingTechProcessParams;
            SawingModes = (border.ObjectId.IsLine() ? par.SawingLineModes : par.SawingCurveModes).ConvertAll(x => x.Clone());
            OuterSide = border.OuterSide;
            IsExactlyBegin = border.IsExactlyBegin;
            IsExactlyEnd = border.IsExactlyEnd;
        }

        const int CornerIndentIncrease = 5;

        public override void BuildProcessing(ICommandGenerator generator)
        {
            var techProcess = (SawingTechProcess)TechProcess;
            var curve = ProcessingArea.GetCurve();
            var compensation = CalcCompensation(OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value, techProcess.MachineType == MachineType.Donatoni);
            var sumIndent = CalcIndent(techProcess.Thickness.Value, techProcess.Tool.Diameter) * (Convert.ToInt32(IsExactlyBegin) + Convert.ToInt32(IsExactlyEnd));
            if (sumIndent < curve.Length())
            {
                var modes = SawingModes.ConvertAll(p => new CuttingMode { Depth = p.Depth, DepthStep = p.DepthStep, Feed = p.Feed });
                var passList = BuilderUtils.GetPassList(modes, techProcess.Thickness.Value, !ProcessingArea.ObjectId.IsLine()).ToList();
                Corner? startCorner = curve.IsUpward() ^ (passList.Count() % 2 == 1) ? Corner.End : Corner.Start;
                foreach (var item in passList)
                {
                    var toolpathCurve = CreateToolpath(item.Key, IsExactlyBegin, IsExactlyEnd, techProcess.Tool.Diameter);
                    generator.Cutting(toolpathCurve, item.Value, techProcess.PenetrationFeed, corner: startCorner);
                    startCorner = null;
                }
                if (!IsExactlyBegin)
                    Acad.CreateGash(curve, curve.StartPoint, OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value);
                if (!IsExactlyEnd)
                    Acad.CreateGash(curve, curve.EndPoint, OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value);
            }
            else
            {
                var point = Scheduling(curve, compensation);
                var line = NoDraw.Line(curve.StartPoint, curve.EndPoint);
                var angle = BuilderUtils.CalcToolAngle(line, curve.StartPoint, Side.Right);
                line.Dispose();
                generator.Cutting(point.X, point.Y, point.Z, angle, techProcess.PenetrationFeed);
            }

            Curve CreateToolpath(double depth, bool isExactlyBegin, bool isExactlyEnd, double toolDiameter)
            {
                var toolpathCurve = curve.GetOffsetCurves(compensation)[0] as Curve;
                toolpathCurve.TransformBy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));

                var indent = CalcIndent(depth, toolDiameter);
                switch (toolpathCurve)
                {
                    case Line line:
                        if (isExactlyBegin)
                            line.StartPoint = line.GetPointAtDist(indent);
                        if (isExactlyEnd)
                            line.EndPoint = line.GetPointAtDist(line.Length - indent);
                        break;

                    case Arc arc:
                        var indentAngle = indent / ((Arc)curve).Radius;
                        if (isExactlyBegin)
                            arc.StartAngle = arc.StartAngle + indentAngle;
                        if (isExactlyEnd)
                            arc.EndAngle = arc.EndAngle - indentAngle;
                        var deltaStart = arc.StartPoint.X - arc.Center.X;
                        var deltaEnd = arc.EndPoint.X - arc.Center.X;
                        // if ((arc.StartAngle >= 0.5 * Math.PI && arc.StartAngle < 1.5 * Math.PI) ^ (arc.EndAngle > 0.5 * Math.PI && arc.EndAngle <= 1.5 * Math.PI))
                        if ((Math.Abs(deltaStart) > Consts.Epsilon && Math.Abs(deltaEnd) > Consts.Epsilon && (deltaStart > 0 ^ deltaEnd > 0)) || (arc.TotalAngle > Math.PI + Consts.Epsilon))
                            throw new InvalidOperationException(
                                $"Обработка дуги невозможна - дуга пересекает угол 90 или 270 градусов. Текущие углы: начальный {Graph.ToDeg(arc.StartAngle)}, конечный {Graph.ToDeg(arc.EndAngle)}");
                        break;
                }
                return toolpathCurve;
            }

            double CalcIndent(double depth, double toolDiameter) => Math.Sqrt(depth * (toolDiameter - depth)) + CornerIndentIncrease;

            double CalcCompensation(Side toolSide, double depth, double toolDiameter, double toolThickness, bool isFrontPlaneZero)
            {
                var offset = 0d;
                if (curve.IsUpward() ^ toolSide == Side.Left ^ isFrontPlaneZero)
                    offset = toolThickness;
                if (curve is Arc arc && toolSide == Side.Left)
                    offset += arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - depth * (toolDiameter - depth));
                return toolSide == Side.Left ^ curve is Arc ? offset : -offset;
            }
        }

        /// <summary>
        /// Намечание
        /// </summary>
        private Point3d Scheduling(Curve curve, double compensation)
        {
            var techProcess = (SawingTechProcess)TechProcess;
            var diam = techProcess.Tool.Diameter;
            var startPoint = curve.StartPoint;
            var endPoint = curve.EndPoint;
            const int CornerIndentIncrease = 5;
            var vector = endPoint - startPoint;
            var depth = techProcess.Thickness.Value;
            var point = Point3d.Origin;
            if (IsExactlyBegin && IsExactlyEnd)
            {
                var l = vector.Length - 2 * CornerIndentIncrease;
                depth = (diam - Math.Sqrt(diam * diam - l * l)) / 2;
                point = startPoint + vector / 2;
            }
            else
            {
                var indentVector = vector.GetNormal() * (Math.Sqrt(depth * (diam - depth)) + CornerIndentIncrease);
                point = IsExactlyBegin ? startPoint + indentVector : endPoint - indentVector;
                Acad.CreateGash(curve, IsExactlyBegin ? curve.EndPoint : curve.StartPoint, OuterSide, depth, diam, techProcess.Tool.Thickness.Value, point);
            }
            return point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
        }
    }
}
    