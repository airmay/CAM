using CAM.Operations.Sawing;
using CAM.Operations.Tactile;
using System;
using System.Windows.Forms;

namespace CAM;

public static class OperationItems
{
    public static ToolStripItem[] GetMenuItems(Action<string, Type> onClick)
    {
        return Items.ConvertAll(p => p.GetMenuItem(onClick));
    }

    private static readonly OperationItemBase[] Items =
    [
        new OperationItem("Распиловка", typeof(SawingOperation)),
        new OperationGroupItem("Тактилка", 
        [
            new OperationItem("Полосы", typeof(TactileBandsOperation)),
            new OperationItem("Фаска", typeof(TactileChamfersOperation)),
        ]),
        new OperationGroupItem("Трос", 
        [
            new OperationItem("Распиловка", typeof(WireSawOperation)),
        ]),
    ];
}