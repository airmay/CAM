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
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

        protected SawingTechOperation(TechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams techOperationParams)
            : base(techProcess, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил-{ processingArea }";
        }

        public override void BuildProcessing()
        {
            ProcessActions.Clear();
            new ProcessAction(this, "Подвод", "Перемещение", new Point3d(ProcessingArea.StartPoint.X, ProcessingArea.StartPoint.Y, TechProcess.TechProcessParams.ZSafety));
            new ProcessAction(this, "Опускание", "Перемещение", ProcessingArea.StartPoint);
            //int z = 0;
            //do
            //{
            //    z += obj.Depth;
            //    if (z > obj.DepthAll)
            //    {
            //        z = obj.DepthAll;
            //    }
            //    new ProcessAction(this, "Перемещение", "Опускание", ProcessingArea.StartPoint);
            //}
            //while (z < obj.DepthAll);
        }
    }
}
