using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM
{
    /// <summary>
    /// Генератор команд процесса обработки
    /// </summary>
    public static class ScemaLogicProcessBuilder
    {
        const int CornerIndentIncrease = 5;

        private static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>()
        {
            [CommandNames.Cutting] = Color.FromColor(System.Drawing.Color.Green),
            [CommandNames.Uplifting] = Color.FromColor(System.Drawing.Color.Blue),
            [CommandNames.Penetration] = Color.FromColor(System.Drawing.Color.Yellow),
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson),
            [CommandNames.Transition] = Color.FromColor(System.Drawing.Color.Yellow)
        };

        public static void BuildProcessing(TechProcess techProcess)
        {
            var techProcessGenerator = new ScemaLogicCommandGenerator();
            techProcessGenerator.StartMachine(techProcess.TechProcessParams.ToolNumber);
            var point = Algorithms.NullPoint3d;

            foreach (var techOperation in techProcess.TechOperations)
            {
                var techOperationGenerator = new ScemaLogicCommandGenerator();
                point = techOperation.GetCutting().Aggregate(point, (p, cuttingSet) => Cutting(techOperationGenerator, p, cuttingSet, techProcess.TechProcessParams));

                techProcessGenerator.Commands.AddRange(techOperationGenerator.Commands);
                techOperation.ProcessCommands = techOperationGenerator.Commands;
            }
            techProcessGenerator.StopMachine();
            techProcess.ProcessCommands = techProcessGenerator.Commands;
        }

        public static Point3d Cutting(ScemaLogicCommandGenerator generator, Point3d currentPoint, CuttingSet cuttingSet, TechProcessParams techProcessParams)
        {
            Point3d point;
            double angle = 0;
            var corner = cuttingSet.StartCorner;

            foreach (var cuttingPass in cuttingSet.Cuttings)
            {
                var toolpathCurve = cuttingPass.Toolpath;
                point = toolpathCurve.GetPoint(corner);
                angle = CalcToolAngle(toolpathCurve, corner);

                if (cuttingSet.Cuttings.IndexOf(cuttingPass) == 0)
                {
                    var dest = new Point3d(point.X, point.Y, techProcessParams.ZSafety);
                    if (currentPoint.IsNull())
                        generator.InitialMove(dest, angle, techProcessParams.Frequency);
                    else
                        generator.Fast(CreateToolpath(currentPoint, dest, Colors[CommandNames.Fast]), angle);
                    currentPoint = dest;
                }
                if (currentPoint.Z == point.Z)
                    generator.Transition(CreateToolpath(currentPoint, point, Colors[CommandNames.Transition]), techProcessParams.PenetrationRate, angle);
                else
                    generator.Penetration(CreateToolpath(currentPoint, point, Colors[CommandNames.Penetration]), techProcessParams.PenetrationRate, angle);

                if (toolpathCurve.Length() > 2)
                {
                    corner = corner.Swap();
                    angle = CalcToolAngle(toolpathCurve, corner);
                    currentPoint = toolpathCurve.GetPoint(corner);
                    generator.Cutting(toolpathCurve, cuttingPass.Feed, currentPoint, angle);
                    toolpathCurve.Color = Colors[CommandNames.Cutting];
                }
            }
            point = new Point3d(currentPoint.X, currentPoint.Y, techProcessParams.ZSafety);
            generator.Uplifting(CreateToolpath(currentPoint, point, Colors[CommandNames.Uplifting]), angle);

            return point;
        }

        public static CuttingSet CalcCuttingSet(CuttingParams cuttingParams, TechProcessParams techProcessParams)
        {
            var compensation = CalcCompensation(cuttingParams.Curve, cuttingParams.ToolSide, cuttingParams.DepthAll, techProcessParams.ToolThickness, techProcessParams.ToolDiameter);
            if (cuttingParams.IsExactlyBegin || cuttingParams.IsExactlyEnd)
            {
                var sumIndent = CalcIndent(techProcessParams.ToolDiameter, cuttingParams.DepthAll) * (Convert.ToInt32(cuttingParams.IsExactlyBegin) + Convert.ToInt32(cuttingParams.IsExactlyEnd));
                if (sumIndent >= (cuttingParams.Curve.EndPoint - cuttingParams.Curve.StartPoint).Length)
                    return Scheduling(cuttingParams, techProcessParams, cuttingParams.Curve.StartPoint, cuttingParams.Curve.EndPoint, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd, cuttingParams.DepthAll, compensation, techProcessParams.ToolDiameter);
            }
            if (!cuttingParams.IsExactlyBegin || !cuttingParams.IsExactlyEnd)
                CreateGash(cuttingParams.Curve, cuttingParams.ToolSide, techProcessParams.ToolThickness, cuttingParams.DepthAll, techProcessParams.ToolDiameter, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd);

            var passList = GetPassList(cuttingParams.CuttingModes, cuttingParams.DepthAll, cuttingParams.IsZeroPass);
            return new CuttingSet
            {
                Cuttings = passList.ConvertAll(p => new CuttingPass(
                      CreateToolpath(cuttingParams.Curve, compensation, p.Key, techProcessParams.ToolDiameter, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd), p.Value)),
                StartCorner = cuttingParams.Curve.IsUpward() ^ (passList.Count() % 2 == 1) ? Corner.End : Corner.Start
            };
        }

        /// <summary>
        /// Намечание
        /// </summary>
        private static CuttingSet Scheduling(CuttingParams cuttingParams, TechProcessParams techProcessParams, Point3d startPoint, Point3d endPoint, bool isExactlyBegin, bool isExactlyEnd, int depth, double compensation, double toolDiam)
        {
            double h;
            Point3d pointC = Point3d.Origin;
            var vector = endPoint - startPoint;
            var length = vector.Length;
            if (isExactlyBegin && isExactlyEnd)
            {
                var l = (endPoint - startPoint).Length - 2 * CornerIndentIncrease;
                h = (toolDiam - Math.Sqrt(toolDiam * toolDiam - l * l)) / 2;
                pointC = startPoint + vector / 2 + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * h;
            }
            else
            {
                var indentVector = vector.GetNormal() * CalcIndent(toolDiam, depth);
                var point = isExactlyBegin ? startPoint + indentVector : endPoint - indentVector;
                pointC = point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
                CreateGash(cuttingParams.Curve, cuttingParams.ToolSide, techProcessParams.ToolThickness, cuttingParams.DepthAll, techProcessParams.ToolDiameter, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd, point);
            }
            return new CuttingSet { Cuttings = new List<CuttingPass> { new CuttingPass(new Line(pointC, pointC + vector.GetNormal())) } };
        }

        private static Line CreateToolpath(Point3d point, Point3d dest, Color color) => new Line(point, dest) { Color = color };

        private static Curve CreateToolpath(Curve curve, double offset, double depth, double toolDiam, bool isExactlyBegin, bool isExactlyEnd)
        {
            Curve toolpathCurve = null;
            if (offset != 0)
                toolpathCurve = curve.GetOffsetCurves(offset)[0] as Curve;

            if (depth != 0)
            {
                var matrix = Matrix3d.Displacement(-Vector3d.ZAxis * depth);
                if (toolpathCurve == null)
                    toolpathCurve = curve.GetTransformedCopy(matrix) as Curve;
                else
                    toolpathCurve.TransformBy(matrix);
            }
            if (toolpathCurve == null)
                toolpathCurve = curve.Clone() as Curve;

            var indent = CalcIndent(toolDiam, depth);
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

        private static List<KeyValuePair<double, int>> GetPassList(IEnumerable<CuttingMode> modes, double DepthAll, bool isZeroPass)
        {
            var passList = new List<KeyValuePair<double, int>>();
            var enumerator = modes.OrderBy(p => p.Depth).GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Не заданы режимы обработки");
            var mode = enumerator.Current;
            CuttingMode nextMode = null;
            if (enumerator.MoveNext())
                nextMode = enumerator.Current;
            var depth = isZeroPass ? -mode.DepthStep : 0;
            do
            {
                depth += mode.DepthStep;
                if (nextMode != null && depth >= nextMode.Depth)
                {
                    mode = nextMode;
                    nextMode = enumerator.MoveNext() ? enumerator.Current : null;
                }
                if (depth > DepthAll)
                    depth = DepthAll;
                passList.Add(new KeyValuePair<double, int>(depth, mode.Feed));
            }
            while (depth < DepthAll);

            return passList;
        }

        private static double CalcToolAngle(Curve curve, Corner corner)
        {
            var tangent = curve.GetTangent(corner);
            if (!curve.IsUpward())
                tangent = tangent.Negate();
            var angle = Graph.ToDeg(Math.PI - Graph.Round(tangent.Angle));
            if (curve is Line && angle == 180)
                angle = 0;
            return angle;
        }

        private static double CalcCompensation(Curve curve, Side toolSide, double depth, double toolThickness, double toolDiameter)
        {
            var offset = 0d;
            if (curve.IsUpward() ^ toolSide == Side.Left)
                offset = toolThickness;
            if (curve is Arc arc && toolSide == Side.Left)
                offset += arc.Radius - Math.Sqrt(arc.Radius * arc.Radius - depth * (toolDiameter - depth));
            return toolSide == Side.Left ^ curve is Arc ? offset : -offset;
        }

        private static double CalcIndent(double toolDiam, double depth) => Math.Sqrt(depth * (toolDiam - depth)) + CornerIndentIncrease;

        /// <summary>
        /// Запилы
        /// </summary>
        private static void CreateGash(Curve curve, Side toolSide, double offset, double depth, double toolDiam, bool isExactlyBegin, bool isExactlyEnd, Point3d? point = null)
        {
            var gashLength = Math.Sqrt(depth * (toolDiam - depth));
            //List<Curve> gashCurves = new List<Curve>();
            //if (toolSide == Side.Right ^ curve is Arc)
            //    offset = -offset;
            if (!isExactlyBegin)
            {
                var p1 = curve.StartPoint.ToPoint2d();
                var normal = curve.GetFirstDerivative(curve.StartParam).GetNormal().ToVector2d();
                var p2 = p1 - normal * gashLength;
                if (point.HasValue)
                    p2 += point.Value.ToPoint2d() - p1;
                var offsetVector = normal.GetPerpendicularVector() * offset * (toolSide == Side.Left ? 1 : -1);
                var gashCurve = new Polyline();
                gashCurve.AddVertexAt(0, p1, 0, 0, 0);
                gashCurve.AddVertexAt(0, p2, 0, 0, 0);
                gashCurve.AddVertexAt(0, p2 + offsetVector, 0, 0, 0);
                gashCurve.AddVertexAt(0, p1 + offsetVector, 0, 0, 0);
                Acad.SaveGash(gashCurve);
            }
            if (!isExactlyEnd)
            {
                var p1 = curve.EndPoint.ToPoint2d();
                var normal = curve.GetFirstDerivative(curve.EndPoint).GetNormal().ToVector2d();
                var p2 = p1 + normal * gashLength;
                if (point.HasValue)
                    p2 += point.Value.ToPoint2d() - p1;
                var offsetVector = normal.GetPerpendicularVector() * offset * (toolSide == Side.Left ? 1 : -1);
                var gashCurve = new Polyline();
                gashCurve.AddVertexAt(0, p1, 0, 0, 0);
                gashCurve.AddVertexAt(0, p2, 0, 0, 0);
                gashCurve.AddVertexAt(0, p2 + offsetVector, 0, 0, 0);
                gashCurve.AddVertexAt(0, p1 + offsetVector, 0, 0, 0);
                Acad.SaveGash(gashCurve);
            }
        }
    }
}
