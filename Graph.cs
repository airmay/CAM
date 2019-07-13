using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    /// <summary>
    /// Методы для работы с графикой
    /// </summary>
    public static class Graph
    {
        public static double AngleRad(this Line line) => Math.Round(line.Angle, 6);

        public static double AngleDeg(this Line line) => Math.Round(line.Angle * 180 / Math.PI, 6);
    }
}   
