/*
using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class OperationProcessor
    {
        public Point3d[] InitialPoints { get; set; }
        public Curve InitialCurve { get; set; }
        public double? InitialCoordinate { get; set; }

        public Point3d[] TargetPoints { get; set; }
        public Curve TargetCurve { get; set; }
        public double? TargetCoordinate { get; set; }
        public double LimitCoordinate { get; set; }
        
        public Vector3d PenetrationDirection { get; set; }
        public double PenetrationStep { get; set; }
        public double? PenetrationAll { get; set; }

        public void Execute(MillingCommandGenerator generator)
        {
            Initialization();

            var maxdepth = InitialPoints.Select((p, i) => p.DistanceTo(TargetPoints[i])).Max();
            var passCount = Math.Ceiling(maxdepth / PenetrationStep);

            var startPoint = InitialPoints.First().DistanceTo(generator.ToolPosition.Point) <= InitialPoints.Last().DistanceTo(generator.ToolPosition.Point) 
                ? InitialPoints.First() 
                : InitialPoints.Last();

            for (int i = 0; i < passCount; i++)
            {
                
            }

        }

        private void Initialization()
        {
            // TODO Arc, Polyline
            if (InitialCurve != null && InitialCurve is Line)
            {
                InitialPoints = InitialCurve.GetStartEndPoints().ToArray();
            }

            if (TargetCurve != null && TargetCurve is Line)
            {
                TargetPoints = TargetCurve.GetStartEndPoints().ToArray();
            }

            if (TargetPoints != null)
            {
                if (InitialCoordinate.HasValue)
                    InitialPoints = TargetPoints.Select(p => new Point3d(p.X, p.Y, InitialCoordinate.Value)).ToArray();
                else if (PenetrationAll.HasValue)
                    InitialPoints = TargetPoints.Select(p => p - PenetrationDirection * PenetrationAll.Value).ToArray();

                if (TargetCoordinate.HasValue)
                    TargetPoints = TargetPoints.Select(p => new Point3d(p.X, p.Y, p.Z > TargetCoordinate.Value ? p.Z : TargetCoordinate.Value)).ToArray();
            }
            else if (InitialPoints != null && PenetrationAll.HasValue)
            {
                TargetPoints = InitialPoints.Select(p => p + PenetrationDirection * PenetrationAll.Value).ToArray();
            }
            else
            {
                throw new Exception("Не заданы параметры обработки");
            }
        }
    }
}
*/
