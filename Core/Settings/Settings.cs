using System.Collections.Generic;

namespace CAM
{
    public static class Settings
    {
        public static readonly Dictionary<MachineCodes, Machine> Machines = new Dictionary<MachineCodes, Machine>
        {
            [MachineCodes.ScemaLogic] = new Machine
            {
                MaxFrequency = 3000,
                ProgramFileExtension = "csv",
                ProgramLineNumberFormat = "{0}",
                IsFrontPlaneZero = false
            },
            [MachineCodes.Donatoni] = new Machine
            {
                MaxFrequency = 5000,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}",
                IsFrontPlaneZero = true
            },
            [MachineCodes.Forma] = new Machine
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineCodes.Champion] = new Machine
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pim",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineCodes.Krea] = new Machine
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineCodes.CableSawing] = new Machine
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "{ 0}"
            },
        };
    }
}
