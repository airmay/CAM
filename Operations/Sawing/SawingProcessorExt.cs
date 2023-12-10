using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;

namespace CAM.Operations.Sawing
{
    public static class SawingProcessorExt
    {
        public static void Cutting(this Processor processor, Curve curve, CurveTip tip, double depth,
            bool isExactlyBegin, bool isExactlyEnd, double indent, int feed)
        {
            if (processor.IsUpperTool)
            {
                var toolpath = CreateToolpath(baseCurve, passList[0].Item1);
                var startPoint = toolpath.GetPoint(tip);
                var angleC = BuilderUtils.CalcToolAngle(toolpath, startPoint, engineSide);
                processor.Move(startPoint.X, startPoint.Y, angleC);
                processor.Cycle();

            }
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

                processor.Cutting(line, tip, feed);

            }

            void CuttingArc(Arc arc)
            {
                var indentAngle = indent / arc.Radius;
                if (isExactlyBegin) arc.StartAngle += indentAngle;
                if (isExactlyEnd) arc.EndAngle -= indentAngle;

                var angleC = processor.AngleC + arc.TotalAngle.ToDeg() * (tip == curve.StartPoint ? -1 : 1);
                var gCode = tip == arc.StartPoint ? 3 : 2;
                processor.GCommand(CommandNames.Cutting, gCode, point: curve.NextPoint(tip), angleC: angleC, curve: curve, feed: cuttingFeed, center: arc?.Center.ToPoint2d());

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