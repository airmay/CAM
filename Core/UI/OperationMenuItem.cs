using System;
using System.Windows.Forms;

namespace CAM;

public abstract class OperationMenuItemBase(string caption)
{
    protected readonly string Caption = caption;
    public abstract ToolStripItem GetMenuItem(Action<string, Type> onClick);
} 

public class OperationMenuMenuItem(string caption, Type type) : OperationMenuItemBase(caption)
{
    public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
    {
        return new ToolStripMenuItem(Caption, null, (_, _) => onClick(Caption, type));
    }
}

public class OperationMenuGroupMenuItem(string caption, OperationMenuMenuItem[] items) : OperationMenuItemBase(caption)
{
    public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
    {
        return new ToolStripMenuItem(Caption, null, items.ConvertAll(p => p.GetMenuItem(onClick)));
    }
}