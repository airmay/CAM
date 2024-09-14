using System;

namespace CAM.Core
{
    [Serializable]
    public abstract class ProcessItem
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public ProcessItem[] Children { get; set; }
        public int CommandIndex { get; set; }
        public virtual void OnDelete() {}
        public virtual void OnSelect() {}
    }
}