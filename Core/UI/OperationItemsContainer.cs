using CAM.Operations.Sawing;
using CAM.Operations.Tactile;
using System;
using System.Linq;
using System.Windows.Forms;
using CAM.Operations.CableSawing;

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
            new OperationGroupItem("Трос", new[]
            {
                new OperationItem("Стелла", typeof(CableSawingOperation)),
            }),

        };

        public static ToolStripMenuItem[] GetMenuItems(Action<string, Type> onClick)
        {
            return OperationItems.Select(p => p.GetMenuItem(onClick)).ToArray();
        }
    }
}