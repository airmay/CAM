using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    [Serializable]
    public class MachineSettings
    {
        public MachineType MachineType { get; set; }

        public int MaxFrequency { get; set; }

        public int ZSafety { get; set; }

        public string ProgramFileExtension { get; set; }
    }
}
