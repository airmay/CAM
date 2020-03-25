using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Sawing
{
    public class Border
    {
        public SawingTechOperation TechOperation { get; set; }
        public ObjectId ObjectId { get; set; }
        public Curve Curve { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }

        private Side _outerSide = Side.None;
        public Side OuterSide
        {
            get => TechOperation?.OuterSide ?? _outerSide;
            set
            {
                if (TechOperation != null)
                    TechOperation.OuterSide = value;
                else
                    _outerSide = value;
            }
        }

        public Border(ObjectId objectId)
        {
            ObjectId = objectId;
        }

        public Border(SawingTechOperation techOperation)
        {
            ObjectId = techOperation.ProcessingArea.ObjectId;
            TechOperation = techOperation;
        }

        public void SetIsExactly(Corner corner, bool value)
        {
            if (corner == Corner.Start)
                IsExactlyBegin = value;
            else
                IsExactlyEnd = value;

            TechOperation?.SetIsExactly(corner, value);
        }
    }
}
