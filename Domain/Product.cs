using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Обрабатываемое изделие
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Обрабатываемые участки
        /// </summary>
        public List<Segment> Segments { get; set; }

        public Product()
        {
            Segments = new List<Segment>();
        }

        
    }
}
