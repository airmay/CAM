using System;

namespace CAM
{
    public class TechOperationAttribute : Attribute
    {
        public int Number { get; }

        public TechProcessType TechProcessType { get; }

        public string TechOperationCaption { get; }

        public TechOperationAttribute(TechProcessType techProcessType, string techOperationCaption, int number)
        {
            this.TechProcessType = techProcessType;
            this.TechOperationCaption = techOperationCaption ;
            this.Number = number;
        }
    }
}
