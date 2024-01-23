using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        public static void Cutting(this Processor processor, Curve curve, CurveTip tip, double depth,
            bool isExactlyBegin, bool isExactlyEnd, double indent)
        {
            var toolpath = curve.GetTransformedCopy(Matrix3d.Displacement(-Vector3d.ZAxis * depth));
            switch (toolpath)
            {
                case Line line:
                    CuttingLine(line);
                    break;

                case Arc arc:
                    CuttingArc(arc);
                    break;

                case Polyline polyline:
                    CuttingPolyline(polyline);
                    break;

                default: throw new Exception();
            }

            return;

            void CuttingLine(Line line)
            {
                if (isExactlyBegin) line.StartPoint = line.GetPointAtDist(indent);
                if (isExactlyEnd) line.EndPoint = line.GetPointAtDist(line.Length - indent);

                processor.Cutting(line, tip);
            }

            void CuttingArc(Arc arc)
            {
                var indentAngle = indent / arc.Radius;
                if (isExactlyBegin) arc.StartAngle += indentAngle;
                if (isExactlyEnd) arc.EndAngle -= indentAngle;

                processor.Cutting(arc, tip);
            }

            }

            void CuttingPolyline(Polyline polyline)
            {
                if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                if (isExactlyEnd)
                    polyline.SetPointAt(polyline.NumberOfVertices - 1,
                        polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());
            }
        }

        private static void Cutting(Arc arc, Point3d point)
        {
            if (arc != null && calcAngleC)
                angleC += arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
            var gCode = curve is Line ? 1 : point == curve.StartPoint ? 3 : 2;
            GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());

        }
    }
}