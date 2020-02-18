using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class TechOperationAttribute : Attribute
    {
        public string TechProcessCaption { get; }

        public string TechOperationCaption { get; }

        public TechOperationAttribute(string techProcessCaption, string techOperationCaption)
        {
            this.TechProcessCaption = techProcessCaption;
            this.TechOperationCaption = techOperationCaption;
        }
    }
}
