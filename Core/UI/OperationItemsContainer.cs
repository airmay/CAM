using CAM.Operations.Sawing;
using CAM.Operations.Tactile;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public static class OperationItemsContainer
    {
        private static readonly OperationItemBase[] OperationItems =
        {
            new OperationItem("Распиловка", typeof(SawingOperation)),
            new OperationGroupItem("Тактилка", new[]
            {
                new OperationItem("Полосы", typeof(TactileBandsOperation)),
                new OperationItem("Фаска", typeof(TactileChamfersOperation)),
            }),
            new OperationItem("Трос", typeof(WireSawOperation)),
        };

        public static ToolStripMenuItem[] GetMenuItems(Action<string, Type> onClick)
        {
            return OperationItems.Select(p => p.GetMenuItem(onClick)).ToArray();
        }
    }
}