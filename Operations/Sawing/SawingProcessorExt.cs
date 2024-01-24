using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        public static void Cutting(this Processor processor, Curve curve, CurveTip tip, double depth, bool isExactlyBegin, bool isExactlyEnd, double indent)
        {
            var toolpath = curve.GetTransformedCopy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));
            switch (toolpath)
            {
                case Line line:
                    if (isExactlyBegin) line.StartPoint = line.GetPointAtDist(indent);
                    if (isExactlyEnd) line.EndPoint = line.GetPointAtDist(line.Length - indent);
                    processor.Cutting(line, tip);
                    break;

                case Arc arc:
                    var indentAngle = indent / arc.Radius;
                    if (isExactlyBegin) arc.StartAngle += indentAngle;
                    if (isExactlyEnd) arc.EndAngle -= indentAngle;
                    processor.Cutting(arc, tip);
                    break;

                case Polyline polyline:
                    if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                    if (isExactlyEnd) polyline.SetPointAt(polyline.NumberOfVertices - 1, polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
                    processor.Cutting(polyline, tip);
                    break;

                default: throw new Exception();
            }
        }

        public static void Cutting(this Processor processor, Arc arc, CurveTip tip)
        {
            var point = arc.GetPoint(tip);
            if (processor.IsUpperTool)
            {
                var moveAngleC = BuilderUtils.CalcToolAngle(arc, point, processor.EngineSide);
                processor.Move(point.ToPoint2d(), moveAngleC);
                processor.Cycle();
            }

            processor.Penetration(point);

            point = arc.NextPoint(point);
            var angleC = BuilderUtils.CalcToolAngle(arc, point, processor.EngineSide);
            var gCode = point == arc.StartPoint ? 3 : 2;
            processor.GCommand(CommandNames.Cutting, gCode, arc, point, angleC, feed: processor.CuttingFeed, arcCenter: arc.Center);
        }

        public static void Cutting(this Processor processor, Polyline polyline, CurveTip tip)
        {
            var point = polyline.GetPoint(tip);
            if (point == polyline.EndPoint)
            {
                polyline.ReverseCurve();
                engineSide = engineSide.Opposite();
            }
            for (int i = 1; i < polyline.NumberOfVertices; i++)
            {
                point = polyline.GetPoint3dAt(i);
                if (calcAngleC)
                    angleC = BuilderUtils.CalcToolAngle(polyline, point, engineSide);
                if (polyline.GetSegmentType(i - 1) == SegmentType.Arc)
                {
                    var arcSeg = polyline.GetArcSegment2dAt(i - 1);
                    GCommand(CommandNames.Cutting, arcSeg.IsClockWise ? 2 : 3, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed, center: arcSeg.Center);
                }
                else
                    GCommand(CommandNames.Cutting, 1, point: point, angleC: angleC, curve: polyline, feed: cuttingFeed);
            }
        }
    }
}
