using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Технология обработки изделия отрезным диском
    /// </summary>
    public class CuttingDiscTechnonogy : ITechnonogy
    {
        /// <summary>
        /// Создает технологический процесс 
        /// </summary>
        /// <returns></returns>
        public TechProcess CreateTechProcess()
        {
            throw new NotImplementedException();
        }
    }
}
