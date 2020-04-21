using System;

namespace CAM
{
    public class TechOperationAttribute : Attribute
    {
        public int Number { get; }

        public string TechProcessCaption { get; }

        public string TechOperationCaption { get; }

        public TechOperationAttribute(int number, string techProcessCaption, string techOperationCaption = null)
        {
            this.Number = number;
            this.TechProcessCaption = techProcessCaption;
            this.TechOperationCaption = techOperationCaption ?? techProcessCaption;
        }
    }
}
