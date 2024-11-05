using System;

namespace CAM.Core
{
    [Serializable]
    public abstract class ProcessItem
    {
        public string Caption { get; set; }
        public bool Enabled { get; set; } = true;
        public ProcessItem[] Children { get; set; }
        public abstract MachineType MachineType { get; }
        public abstract void OnSelect();
    }
}