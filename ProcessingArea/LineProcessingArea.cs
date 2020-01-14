using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    /// <summary>
    /// Обрабатываемая область типа отрезок прямой
    /// </summary>
    public class LineProcessingArea : ProcessingArea
    {
        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        //public override ProcessingAreaType Type { get; } = ProcessingAreaType.Line;

        public LineProcessingArea(Curve curve) : base(new Curve[] { curve })
        {
        }

        public override string ToString()
        {
            return $"Прямая L{ Math.Round(Length) }";
        }
    }
}
