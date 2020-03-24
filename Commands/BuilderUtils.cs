using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Dreambuild.AutoCAD;

namespace CAM
{
    public static class BuilderUtils
    {
        public static IEnumerable<KeyValuePair<double, int>> GetPassList(IEnumerable<CuttingMode> modes, double DepthAll, bool isZeroPass)
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
                yield return new KeyValuePair<double, int>(depth, mode.Feed);
            }
            while (depth < DepthAll);
        }

        public static double CalcToolAngle(Curve curve, Point3d point, Side engineSide) => CalcToolAngle(curve.GetFirstDerivative(point).ToVector2d().Angle, engineSide);

        public static double CalcToolAngle(double angle, Side engineSide)
        {
            return ((engineSide == Side.Right ? 180 : 360) + 360 - angle.ToDeg().Round(4)) % 360;
        }
    }
}
