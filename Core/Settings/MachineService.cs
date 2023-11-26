using System.Collections.Generic;

namespace CAM
{
    public class Machine
    {
        public int MaxFrequency { get; set; }
        public string ProgramFileExtension { get; set; }
        public string ProgramLineNumberFormat { get; set; }
        public bool IsFrontPlaneZero { get; set; }
    }

    public static class MachineService
    {
        public static readonly Dictionary<MachineType, Machine> Machines = new Dictionary<MachineType, Machine>
        {
            [MachineType.ScemaLogic] = new Machine
            {
                MaxFrequency = 3000,
                ProgramFileExtension = "csv",
                ProgramLineNumberFormat = "{0}",
                IsFrontPlaneZero = false
            },
            [MachineType.Donatoni] = new Machine
            {
                MaxFrequency = 5000,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}",
                IsFrontPlaneZero = true
            },
            [MachineType.Forma] = new Machine
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.Champion] = new Machine
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pim",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.Krea] = new Machine
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.CableSawing] = new Machine
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "{ 0}"
            },
        };
    }
}
