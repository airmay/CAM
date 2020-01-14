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
            [CommandNames.Fast] = Color.FromColor(System.Drawing.Color.Crimson)
        };

        public static void BuildProcessing(TechProcess techProcess)
        {
            var techProcessParams = techProcess.TechProcessParams;
            var techProcessGenerator = new ScemaLogicCommandGenerator();
            techProcessGenerator.StartMachine(techProcessParams.ToolNumber);
            var point = Algorithms.NullPoint3d;
            var techOperationGenerator = new ScemaLogicCommandGenerator();

            foreach (var techOperation in techProcess.TechOperations)
            {
                foreach (var cuttingParams in techOperation.GetCuttingParams())
                {
                    var compensation = CalcCompensation(cuttingParams.Curve, cuttingParams.ToolSide, cuttingParams.DepthAll, techProcessParams.ToolThickness, techProcessParams.ToolDiameter);

                    var sumIndent = CalcIndent(techProcessParams.ToolDiameter, cuttingParams.DepthAll) * (Convert.ToInt32(cuttingParams.IsExactlyBegin) + Convert.ToInt32(cuttingParams.IsExactlyEnd));
                    if (sumIndent >= cuttingParams.Curve.Length())
                        point = Scheduling(cuttingParams.Curve.StartPoint, cuttingParams.Curve.EndPoint, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd, cuttingParams.DepthAll, compensation);
                    else
                        point = Cutting(techOperationGenerator, point, cuttingParams, compensation, techProcessParams);
                }
                techProcessGenerator.AddCommands(techOperationGenerator);
                techOperation.ProcessCommands = techOperationGenerator.GetCommands();
                techOperationGenerator.ClearCommands();
            }
            techProcessGenerator.StopMachine();
            techProcess.ProcessCommands = techProcessGenerator.GetCommands();
        }

        public static Point3d Cutting(ScemaLogicCommandGenerator generator, Point3d currentPoint, CuttingParams cuttingParams, double compensation, TechProcessParams techProcessParams)
        {
            var toolpathList = cuttingParams.ToolpathList ?? CreateToolpathList(cuttingParams, compensation, techProcessParams);
            var corner = cuttingParams.StartCorner;
            Point3d point;
            double angle = 0;

            foreach (var toolpath in toolpathList)
            {
                var toolpathCurve = toolpath.Key;
                point = toolpathCurve.GetPoint(corner);
                angle = CalcToolAngle(toolpathCurve, corner);

                if (toolpathList.IndexOf(toolpath) == 0)
                {
                    var dest = new Point3d(point.X, point.Y, techProcessParams.ZSafety);
                    if (currentPoint.IsNull())
                        generator.InitialMove(dest, angle, techProcessParams.Frequency);
                    else
                        generator.Fast(CreateToolpath(currentPoint, dest, Colors[CommandNames.Fast]), angle);
                    currentPoint = dest;
                }
                generator.Penetration(CreateToolpath(currentPoint, point, Colors[CommandNames.Penetration]), techProcessParams.PenetrationRate, angle);

                corner = corner.Swap();
                angle = CalcToolAngle(toolpathCurve, corner);
                currentPoint = toolpathCurve.GetPoint(corner);
                generator.Cutting(toolpathCurve, toolpath.Value, currentPoint, angle);
            }
            point = new Point3d(currentPoint.X, currentPoint.Y, techProcessParams.ZSafety);
            generator.Uplifting(CreateToolpath(currentPoint, point, Colors[CommandNames.Uplifting]), angle);

            return point;
        }

        private static List<KeyValuePair<Curve, int>> CreateToolpathList(CuttingParams cuttingParams, double compensation, TechProcessParams techProcessParams)
        {
            var passList = GetPassList(cuttingParams.CuttingModes, cuttingParams.DepthAll, cuttingParams.IsZeroPass);

            bool oddPassCount = passList.Count() % 2 == 1;
            cuttingParams.StartCorner = cuttingParams.Curve.IsUpward() ^ oddPassCount ? Corner.End : Corner.Start;

            return passList.ConvertAll(p => new KeyValuePair<Curve, int>(
                CreateToolpath(cuttingParams.Curve, compensation, p.Key, techProcessParams.ToolDiameter, cuttingParams.IsExactlyBegin, cuttingParams.IsExactlyEnd), p.Value));
        }

        private static Point3d Scheduling(Point3d startPoint, Point3d endPoint, bool isExactlyBegin, bool isExactlyEnd, int depth, double compensation)
        {
            // TODO Намечание
            //if (!(obj is ProcessObjectLine))
            // return;
            //double h;
            //Point3d pointC;
            //if (obj.IsBeginExactly && obj.IsEndExactly)
            //{
            // var l = obj.Length - 2 * ExactlyIncrease;
            // h = (obj.Diameter - Math.Sqrt(obj.Diameter * obj.Diameter - l * l)) / 2;
            // pointC = obj.ProcessCurve.GetPointAtParameter(obj.ProcessCurve.EndParam / 2);
            //}
            //else
            //{
            // h = obj.DepthAll;
            // pointC = obj.ProcessCurve.StartPoint + obj.ProcessCurve.GetFirstDerivative(0).GetNormal() * (obj.IsBeginExactly ? s : obj.Length - s);
            //}
            return Point3d.Origin;
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
            toolpathCurve.Color = Colors[CommandNames.Cutting];
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
            return Graph.ToDeg(Math.PI - Graph.Round(tangent.Angle));
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
    }
}
