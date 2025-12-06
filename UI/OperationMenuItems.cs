using CAM.MachineCncWorkCenter.Operations.Sawing;
using CAM.MachineCncWorkCenter.Operations.Tactile;
using CAM.MachineWireSaw.Operations;
using CAM.Utils;
using System;
using System.Windows.Forms;

namespace CAM.UI;

public static class OperationMenuItems
{
    public static ToolStripItem[] GetMenuItems(Action<string, Type> onClick)
    {
        return Items.ConvertAll(p => p.GetMenuItem(onClick));
    }

    private static readonly OperationMenuItemBase[] Items =
    [
        new OperationMenuMenuItem("Распиловка", typeof(SawingOperation)),
        new OperationMenuGroupMenuItem("Профиль по сечению",
        [
            new OperationMenuMenuItem("Профиль по сечению - Гребенка", typeof(MachineCncWorkCenter.Operations.SectionProfile.LongProcessing)),
            new OperationMenuMenuItem("Профиль по сечению - Чистка", typeof(MachineCncWorkCenter.Operations.SectionProfile.LongCleaning)),
        ]),
        new OperationMenuGroupMenuItem("Тактилка", 
        [
            new OperationMenuMenuItem("Тактилка - Полосы", typeof(TactileBandsOperation)),
            new OperationMenuMenuItem("Тактилка - Фаска", typeof(TactileChamfersOperation)),
        ]),
        new OperationMenuGroupMenuItem("Трос", 
        [
            new OperationMenuMenuItem("Трос - Распиловка", typeof(WireSawOperation)),
        ]),
    ];

    private abstract class OperationMenuItemBase(string caption)
    {
        protected readonly string Caption = caption;
        public abstract ToolStripItem GetMenuItem(Action<string, Type> onClick);
    }

    private class OperationMenuMenuItem(string caption, Type type) : OperationMenuItemBase(caption)
    {
        public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
        {
            return new ToolStripMenuItem(Caption, null, (_, _) => onClick(Caption, type));
        }
    }

    private class OperationMenuGroupMenuItem(string caption, OperationMenuMenuItem[] items) : OperationMenuItemBase(caption)
    {
        public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
        {
            return new ToolStripMenuItem(Caption, null, items.ConvertAll(p => p.GetMenuItem(onClick)));
        }
    }
}