using System;
using System.Collections.Generic;

namespace CAM
{
    public class OperationItem
    {
        public string Caption;
        public Type Type;
        public List<OperationItem> Items;

        public OperationItem(string caption) => Caption = caption;
        public OperationItem(string caption, Type type) : this(caption) => Type = type;
        public OperationItem(string caption, List<OperationItem> items) : this(caption) => Items = items;
    }
}