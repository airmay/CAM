using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Обрабатываемая область типа дуга
    /// </summary>
    public class ArcProcessingArea : ProcessingArea
    {
        /// <summary>
        /// Начальный угол дуги
        /// </summary>
        public double StartAngle { get; protected set; }

        /// <summary>
        /// Конечный угол дуги
        /// </summary>
        public double EndAngle { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="curve">Дуга</param>
        public ArcProcessingArea(Arc arc)
            : base(arc)
        {
            Type = ProcessingAreaType.Arc;
            Set(arc);
        }

        /// <summary>
        /// Заполнение параметров обрабатываемой области в соответствии с полученной кривой
        /// </summary>
        /// <param name="curve"></param>
        protected override void Set(Curve curve)
        {
            base.Set(curve);
            var arc = curve as Arc;
            StartAngle = Math.Round(arc.StartAngle, 6);
            EndAngle = Math.Round(arc.EndAngle, 6);
        }
    }
}
