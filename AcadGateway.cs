using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public class AcadGateway
    {
        public Curve[] GetSelectedCurves()
        {
            return new Curve[] { new Line(new Point3d(0, 0, 0), new Point3d(1, 0, 0)), new Arc(new Point3d(0, 0, 0), 1, 0, 1) };
        }
    }
}
