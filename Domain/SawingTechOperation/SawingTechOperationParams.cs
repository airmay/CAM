using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CAM.Domain
{
    /// <summary>
    /// Параметры технологической операция "Распиловка"
    /// </summary>
    [Serializable]
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
        [XmlIgnore]
        public double Compensation { get; set; }

        /// <summary>
        ///  Первый проход по поверхности
        /// </summary>
        public bool IsFirstPassOnSurface { get; set; }

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
