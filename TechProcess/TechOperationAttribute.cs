using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class TechOperationAttribute : Attribute
    {
        public string TechProcessName { get; }

        public string TechOperationName { get; }

        public TechOperationAttribute(string techProcessName, string techOperationName)
        {
            this.TechProcessName = techProcessName;
            this.TechOperationName = techOperationName;
        }
    }
}
