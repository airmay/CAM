using Autodesk.AutoCAD.DatabaseServices;
using CAM.CncWorkCenter;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        private static void Approach(this ProcessorCnc processor, Curve curve, CurveTip tip, double angleA = 0)
        {
            var point = curve.GetPoint(tip);
            if (processor.IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle(curve, point, processor.EngineSide);
                processor.Move(point, angleC, angleA);
                processor.Cycle();
            }

            processor.Penetration(point);
        }

        public static void Cutting(this ProcessorCnc processor, Line line, CurveTip tip, int? feed = null)
        {
            var point = line.GetPoint(tip);
            if (processor.IsUpperTool)
            {
                var angleC = BuilderUtils.CalcToolAngle(line.Angle, processor.EngineSide);
                processor.Move(point, angleC);
                //Cycle();
            }
            processor.Penetration(point);
            processor.GCommand(1, feed, line, line.NextPoint(point), feed);
        }

        public static void Cutting(this ProcessorCnc processor, Arc arc, CurveTip tip, double angleA, int? feed = null)
        {
            processor.Approach(arc, tip, angleA);

            var point = arc.GetPoint(tip.Swap());
            var angleC = BuilderUtils.CalcToolAngle(arc, point, processor.EngineSide);
            var gCode = point == arc.StartPoint ? 3 : 2;
            processor.GCommand(gCode, feed, arc, point, angleC: angleC, arcCenter: arc.Center.ToPoint2d());
        }

        public static void Cutting(this ProcessorCnc processor, Polyline polyline, CurveTip tip, int? feed = null)
        {
            processor.Approach(polyline, tip);

            var isReverse = tip == CurveTip.End;
            var count = polyline.NumberOfVertices - 1;
            var firstIndex = isReverse ? count - 1 : 0;
            var sign = isReverse ? -1 : 1;
            var pointIndexShift = isReverse ? 0 : 1;
            for (var i = 0; i < count; i++)
            {
                var index = firstIndex + i * sign;
                var point = polyline.GetPoint3dAt(index + pointIndexShift);
                if (polyline.GetSegmentType(i) == SegmentType.Arc)
                {
                    var arcSeg = polyline.GetArcSegment2dAt(i);
                    var gCode = arcSeg.IsClockWise ? 2 : 3;
                    var angleC = BuilderUtils.CalcToolAngle(polyline, point, processor.EngineSide);
                    processor.GCommand(gCode, feed, polyline, point, angleC: angleC, arcCenter: arcSeg.Center);
                }
                else
                    processor.GCommand(1, feed, polyline, point);
            }
        }
    }
}
