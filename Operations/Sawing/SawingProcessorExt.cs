using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        public static void Cutting(this MillingProcessor processor, Curve curve, Point3d point, double depth,
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

                processor.Cutting(line, point);

            }

            void CuttingArc(Arc arc)
            {
                var indentAngle = indent / arc.Radius;
                if (isExactlyBegin) arc.StartAngle += indentAngle;
                if (isExactlyEnd) arc.EndAngle -= indentAngle;

                var angleC = processor.AngleC + arc.TotalAngle.ToDeg() * (point == curve.StartPoint ? -1 : 1);
                var gCode = point == arc.StartPoint ? 3 : 2;
                processor.GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(point), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());

            }

            void CuttingPolyline(Polyline polyline)
            {
                if (isExactlyBegin) polyline.SetPointAt(0, polyline.GetPointAtDist(indent).ToPoint2d());
                if (isExactlyEnd)
                    polyline.SetPointAt(polyline.NumberOfVertices - 1,
                        polyline.GetPointAtDist(polyline.Length - indent).ToPoint2d());

            }
        }
    }
}