using System;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public abstract class OperationItemBase
    {
        protected readonly string Caption;
        protected OperationItemBase(string caption) => Caption = caption;
        public abstract ToolStripMenuItem GetMenuItem(Action<string, Type> onClick);
    } 

    public class OperationItem : OperationItemBase
    {
        private readonly Type _type;
        public OperationItem(string caption, Type type) : base(caption) => _type = type;
        public override ToolStripMenuItem GetMenuItem(Action<string, Type> onClick)
        {
            return new ToolStripMenuItem(Caption, null, (o, args) => onClick(Caption, _type));
        }
    }

    public class OperationGroupItem : OperationItemBase
    {
        private readonly OperationItem[] _items;
        public OperationGroupItem(string caption, OperationItem[] items) : base(caption) => _items = items;
        public override ToolStripMenuItem GetMenuItem(Action<string, Type> onClick)
        {
            return new ToolStripMenuItem(Caption, null, _items.Select(p => p.GetMenuItem(onClick)).ToArray());
        }
    }
}