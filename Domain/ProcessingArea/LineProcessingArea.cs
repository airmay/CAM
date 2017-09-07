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
    public class LineProcessingArea : ProcessingArea
    {
        public LineProcessingArea(Line line) 
            : base(line)
        {
            Type = ProcessingAreaType.Line;
            Set(line);
        }
    }
}
