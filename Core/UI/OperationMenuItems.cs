using CAM.Operations.Sawing;
using CAM.Operations.Tactile;
using System;
using System.Windows.Forms;

namespace CAM;

public static class OperationMenuItems
{
    public static ToolStripItem[] GetMenuItems(Action<string, Type> onClick)
    {
        return Items.ConvertAll(p => p.GetMenuItem(onClick));
    }

    private static readonly OperationMenuItemBase[] Items =
    [
        new OperationMenuMenuItem("Распиловка", typeof(SawingOperation)),
        new OperationMenuGroupMenuItem("Тактилка", 
        [
            new OperationMenuMenuItem("Полосы", typeof(TactileBandsOperation)),
            new OperationMenuMenuItem("Фаска", typeof(TactileChamfersOperation)),
        ]),
        new OperationMenuGroupMenuItem("Трос", 
        [
            new OperationMenuMenuItem("Распиловка", typeof(WireSawOperation)),
        ]),
    ];
}