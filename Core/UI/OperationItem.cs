using System;
using System.Windows.Forms;

namespace CAM;

public abstract class OperationItemBase(string caption)
{
    protected readonly string Caption = caption;
    public abstract ToolStripItem GetMenuItem(Action<string, Type> onClick);
} 

public class OperationItem(string caption, Type type) : OperationItemBase(caption)
{
    public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
    {
        return new ToolStripMenuItem(Caption, null, (_, _) => onClick(Caption, type));
    }
}

public class OperationGroupItem(string caption, OperationItem[] items) : OperationItemBase(caption)
{
    public override ToolStripItem GetMenuItem(Action<string, Type> onClick)
    {
        return new ToolStripMenuItem(Caption, null, items.ConvertAll(p => p.GetMenuItem(onClick)));
    }
}