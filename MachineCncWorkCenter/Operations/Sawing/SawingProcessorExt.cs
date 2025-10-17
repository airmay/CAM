using Autodesk.AutoCAD.DatabaseServices;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Enums;

namespace CAM.MachineCncWorkCenter.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        private static void Approach(this ProcessorCnc processor, Curve curve, bool fromStart, Side engineSide, double angleA = 0)
        {
            var point = curve.GetPoint(fromStart);
            if (processor.IsUpperTool)
            {
                var angleC = curve.GetToolAngle(point, engineSide);
                processor.Move(point, angleC, angleA);
                //processor.Cycle();
            }

            processor.Penetration(point);
        }

        public static void Cutting(this ProcessorCnc processor, Line line, bool fromStart, int feed, Side engineSide)
        {
            var point = line.GetPoint(fromStart);
            if (processor.IsUpperTool)
            {
                var angleC = line.Angle.GetToolAngle(engineSide);
                processor.Move(point, angleC);
                //Cycle();
            }
            processor.Penetration(point);
            processor.GCommand(1, feed, line, line.GetPoint(!fromStart));
        }

        public static void Cutting(this ProcessorCnc processor, Arc arc, bool fromStart, double angleA, int feed, Side engineSide)
        {
            processor.Approach(arc, fromStart, engineSide, angleA);

            var point = arc.GetPoint(!fromStart);
            var angleC = arc.GetToolAngle(point, engineSide);
            var gCode = point == arc.StartPoint ? 3 : 2;
            processor.GCommand(gCode, feed, arc, point, angleC: angleC, arcCenter: arc.Center.ToPoint2d());
        }

        public static void Cutting(this ProcessorCnc processor, Polyline polyline, bool fromStart, int feed, Side engineSide)
        {
            processor.Approach(polyline, fromStart, engineSide);

            var isReverse = !fromStart;
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
                    var angleC = polyline.GetToolAngle(point, engineSide);
                    processor.GCommand(gCode, feed, polyline, point, angleC: angleC, arcCenter: arcSeg.Center);
                }
                else
                    processor.GCommand(1, feed, polyline, point);
            }
        }
    }
}
