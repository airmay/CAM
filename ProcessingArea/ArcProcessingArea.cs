using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    /// <summary>
    /// Обрабатываемая область типа дуга
    /// </summary>
    public class ArcProcessingArea : ProcessingArea
    {
        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        //public override ProcessingAreaType Type { get; } = ProcessingAreaType.Arc;

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
        public ArcProcessingArea(Curve curve) : base(curve)
        {
            //Set(curve);
        }

        /// <summary>
        /// Заполнение параметров обрабатываемой области в соответствии с полученной кривой
        /// </summary>
        /// <param name="curve"></param>
        //protected override void Set(Curve curve)
        //{
        //    base.Set(curve);
        //    var arc = curve as Arc;
        //    StartAngle = Math.Round(arc.StartAngle, 6);
        //    EndAngle = Math.Round(arc.EndAngle, 6);
        //}

        public override string ToString()
        {
            return $"Дуга L{ Math.Round(Length) }";
        }
    }
}
