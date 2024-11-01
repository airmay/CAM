using System;

namespace CAM.Core
{
    [Serializable]
    public abstract class ProcessItem
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; } = true;
        public ProcessItem[] Children { get; set; }
        public virtual MachineType MachineType { get; }
        public virtual int GetCommandIndex() => 0;
        public virtual void OnDelete() {}
        public virtual void OnSelect() {}
    }
}