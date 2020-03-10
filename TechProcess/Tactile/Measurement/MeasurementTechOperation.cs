using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(4, TechProcessNames.Tactile, "Измерение")]
    public class MeasurementTechOperation : TechOperationBase
    {
        public List<double> PointsX { get; set; } = new List<double>();

        public List<double> PointsY { get; set; } = new List<double>();

        [NonSerialized]
        public ObjectId[] PointObjectIds;

        public MeasurementTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            builder.Measure(PointsX, PointsY);
        }

        public override void Setup(ITechProcess techProcess)
        {
            base.Setup(techProcess);
            PointObjectIds = PointsX.SelectMany((p, i) => Acad.CreateMeasurementPoint(new Point3d(PointsX[i], PointsY[i], 0))).ToArray();
        }

        public void Clear()
        {
            PointsX.Clear();
            PointsY.Clear();
            if (PointObjectIds.Any())
                Acad.DeleteObjects(PointObjectIds);
        }

        public void CreatePoint(Point3d point)
        {
            PointsX.Add(point.X);
            PointsY.Add(point.Y);
            PointObjectIds = PointObjectIds.Concat(Acad.CreateMeasurementPoint(point)).ToArray();
        }

        public override void Teardown()
        {
            Acad.DeleteObjects(PointObjectIds);
            base.Teardown();
        }
    }
}
