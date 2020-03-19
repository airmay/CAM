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

        public override void BuildProcessing(ICommandGenerator generator)
        {
            const int CornerIndentIncrease = 5;
            var techProcess = (SawingTechProcess)TechProcess;
            var curve = ProcessingArea.GetCurve();
            var thickness = techProcess.Thickness.Value;
            var toolDiameter = techProcess.Tool.Diameter;
            var toolThickness = techProcess.Tool.Thickness.Value;
            var compensation = CalcCompensation();
            var sumIndent = CalcIndent(thickness) * (Convert.ToInt32(IsExactlyBegin) + Convert.ToInt32(IsExactlyEnd));
            if (sumIndent < curve.Length())
            {
                var modes = SawingModes.ConvertAll(p => new CuttingMode { Depth = p.Depth, DepthStep = p.DepthStep, Feed = p.Feed });
                var passList = BuilderUtils.GetPassList(modes, techProcess.Thickness.Value, !ProcessingArea.ObjectId.IsLine()).ToList();
                Corner? startCorner = curve.IsUpward() ^ (passList.Count() % 2 == 1) ? Corner.End : Corner.Start;
                foreach (var item in passList)
                {
                    var toolpathCurve = CreateToolpath(item.Key);
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
                var point = Scheduling();
                var line = NoDraw.Line(curve.StartPoint, curve.EndPoint);
                var angle = BuilderUtils.CalcToolAngle(line, curve.StartPoint, Side.Right);
                line.Dispose();
                generator.Cutting(point.X, point.Y, point.Z, angle, techProcess.PenetrationFeed);
            }

            Curve CreateToolpath(double depth)
            {
                var toolpathCurve = curve.GetOffsetCurves(compensation)[0] as Curve;
                toolpathCurve.TransformBy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));

                var indent = CalcIndent(depth);
                switch (toolpathCurve)
                {
                    case Line line:
                        if (IsExactlyBegin)
                            line.StartPoint = line.GetPointAtDist(indent);
                        if (IsExactlyEnd)
                            line.EndPoint = line.GetPointAtDist(line.Length - indent);
                        break;

                    case Arc arc:
                        var indentAngle = indent / ((Arc)curve).Radius;
                        if (IsExactlyBegin)
                            arc.StartAngle = arc.StartAngle + indentAngle;
                        if (IsExactlyEnd)
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

            double CalcIndent(double depth) => Math.Sqrt(depth * (toolDiameter - depth)) + CornerIndentIncrease;

            double CalcCompensation()
            {
                var offset = 0d;
                var isFrontPlaneZero = techProcess.MachineType == MachineType.Donatoni;
                if (curve.IsUpward() ^ OuterSide == Side.Left ^ isFrontPlaneZero)
                    offset = toolThickness;
                if (curve is Arc arc && OuterSide == Side.Left)
                    offset += arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - thickness * (toolDiameter - thickness));
                return OuterSide == Side.Left ^ curve is Arc ? offset : -offset;
            }

            /// <summary>
            /// Намечание
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
    