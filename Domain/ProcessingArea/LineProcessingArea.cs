using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Обрабатываемая область типа отрезок прямой
    /// </summary>
    [CurveType(typeof(Line))]
    public class LineProcessingArea : ProcessingArea
    {
       // public override Type CurveType { get; } = typeof(Line);

        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        public override ProcessingAreaType Type { get; } = ProcessingAreaType.Line;


        public LineProcessingArea(Curve curve) : base(curve)
        {
        }

        public override string ToString()
        {
            return $"Прямая[{ Math.Round(Length) }]";
        }
    }
}
