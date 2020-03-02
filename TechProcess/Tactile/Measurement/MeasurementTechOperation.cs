using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(TechProcessNames.Tactile, "Измерение")]
    public class MeasurementTechOperation : TechOperationBase
    {
        public double[] PointsX { get; set; }

        public double[] PointsY { get; set; }

        public MeasurementTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
        }
    }
}
