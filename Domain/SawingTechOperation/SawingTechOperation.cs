using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Технологическая операция "Распиловка"
    /// </summary>
    public abstract class SawingTechOperation : TechOperation
    {
        /// <summary>
        /// Вид технологической операции
        /// </summary>
        public override TechOperationType Type { get; } = TechOperationType.Sawing;

        /// <summary>
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

        protected SawingTechOperation(TechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams techOperationParams)
            : base(techProcess, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил { processingArea }";
        }

        public override void BuildProcessing(ProcessBuilder builder)
        {
            ProcessActions.Clear();
            builder.ProcessActions = ProcessActions;

            builder.SetGroup(ProcessActionNames.Approach);
            builder.Move(ProcessingArea.StartPoint.X, ProcessingArea.StartPoint.Y);
            builder.Descent(0);
            var modes = TechOperationParams.Modes.OrderBy(p => p.Depth).GetEnumerator();
            modes.MoveNext();
            var mode = modes.Current;
            var pass = 1;
            int z = ProcessingArea.Type == ProcessingAreaType.Arc ? -mode.DepthStep : 0;
            while (true)
            {
                z += mode.DepthStep;
                if (z > mode.Depth)
                {
                    if (modes.MoveNext())
                        mode = modes.Current;
                    else
                        break;
                }
                builder.SetGroup($"{pass++} {ProcessActionNames.Pass} Z{-z}");
                builder.Penetration(-z);
                builder.Cutting(ProcessingArea.Curve, dz: -z, feed: mode.Feed);
            }
            builder.SetGroup(ProcessActionNames.Departure);
            builder.Uplifting();
        }
    }
}
