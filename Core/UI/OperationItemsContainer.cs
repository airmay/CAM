using CAM.Operations.Sawing;

namespace CAM
{
    public static class OperationItemsContainer
    {
        public static OperationItem[] OperationItems =
        {
            new OperationItem("Распиловка", typeof(SawingOperation))
        };
    }
}