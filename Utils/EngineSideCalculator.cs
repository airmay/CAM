using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM.Utils
{
    public static class EngineSideCalculator
    {
        public static Side Calculate(Curve curve, MachineType machineType)
        {
            switch (curve)
            {
                case Arc arc:
                    return CalcArc(arc, machineType);
                case Line line:
                    return CalcLine(line);
                case Polyline polyline:
                    return CalcPolyline(polyline, machineType);
                default:
                    throw new InvalidOperationException($"Кривая типа {curve.GetType()} не может быть обработана.");
            }
        }

        private static Side CalcArc(Arc arc, MachineType machineType)
        {
            var startSide = Math.Sign(Math.Cos(arc.StartAngle.Round(3)));
            var endSide = Math.Sign(Math.Cos(arc.EndAngle.Round(3)));
            var cornersOneSide = Math.Sign(startSide * endSide);

            if (arc.TotalAngle.Round(3) > Math.PI && cornersOneSide > 0)
                throw new InvalidOperationException("Обработка дуги невозможна - дуга пересекает углы 90 и 270 градусов.");

            if (cornersOneSide < 0) //  дуга пересекает углы 90 или 270 градусов
            {
                if (machineType == MachineType.ScemaLogic)
                    throw new InvalidOperationException("Обработка дуги на ScemaLogic невозможна - дуга пересекает угол 90 или 270 градусов.");

                return startSide > 0 ? Side.Left : Side.Right;
            }

            return (startSide + endSide) > 0 ? Side.Right : Side.Left;
        }

        private static Side CalcLine(Line line)
        {
            return BuilderUtils.CalcEngineSide(line.Angle);
        }

        private static Side CalcPolyline(Polyline polyline, MachineType machineType)
        {
            var sign = 0;
            var engineSide = Side.None;

            for (var i = 0; i < polyline.NumberOfVertices; i++)
            {
                var point = polyline.GetPoint3dAt(i);
                var s = Math.Sign(Math.Sin(polyline.GetTangent(point).Angle.Round(6)));
                if (s == 0)
                    continue;
                if (sign == 0)
                {
                    sign = s;
                    continue;
                }

                var bulge = polyline.GetBulgeAt(i - 1);
                if (bulge == 0)
                    bulge = polyline.GetBulgeAt(i);

                if (s != sign)
                {
                    if (machineType == MachineType.ScemaLogic)
                        throw new InvalidOperationException("Обработка полилинии на ScemaLogic невозможна - кривая пересекает углы 90 или 270 градусов.");

                    var sd = sign > 0 ^ bulge < 0 ? Side.Left : Side.Right;
                    if (engineSide != Side.None)
                    {
                        if (engineSide != sd)
                            throw new InvalidOperationException("Обработка полилинии невозможна.");
                    }
                    else
                        engineSide = sd;

                    sign = s;
                }
                else if (Math.Abs(bulge) > 1)
                    throw new InvalidOperationException("Обработка невозможна - дуга полилинии пересекает углы 90 и 270 градусов.");
            }

            return engineSide != Side.None
                ? engineSide
                : BuilderUtils.CalcEngineSide(polyline.GetTangent(polyline.StartPoint).Angle);
        }
    }
}
