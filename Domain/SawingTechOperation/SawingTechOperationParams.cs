using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры технологической операция "Распиловка"
    /// </summary>
    public class SawingTechOperationParams
    {
        /// <summary>
        /// Скорость заглубления
        /// </summary>
        //public int PenetrationRate { get; set; } = 250;

        /// <summary>
        /// Режимы распиловки
        /// </summary>
        public List<SawingMode> Modes { get; set; } = new List<SawingMode>();

        /// <summary>
        /// Компенсация
        /// </summary>
        public double Compensation { get; set; }

        /// <summary>
        ///  Первый проход по поверхности
        /// </summary>
        public bool IsFirstPassOnSurface { get; internal set; }

        public SawingTechOperationParams Clone()
        {
	        var sawingTechOperationParams = new SawingTechOperationParams()
	        {
		        Modes = Modes.ConvertAll(p => p.Clone())
	        };
            return sawingTechOperationParams;
        }
    }
}
