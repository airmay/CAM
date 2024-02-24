using System.Collections.Generic;

namespace CAM
{
    public static class Settings
    {
        public static readonly Dictionary<MachineType, MachineSettings> Machines = new Dictionary<MachineType, MachineSettings>
        {
            [MachineType.ScemaLogic] = new MachineSettings
            {
                MaxFrequency = 3000,
                ProgramFileExtension = "csv",
                ProgramLineNumberFormat = "{0}",
                IsFrontPlaneZero = false
            },
            [MachineType.Donatoni] = new MachineSettings
            {
                MaxFrequency = 5000,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}",
                IsFrontPlaneZero = true
            },
            [MachineType.Forma] = new MachineSettings
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.Champion] = new MachineSettings
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pim",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.Krea] = new MachineSettings
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "N{0}"
            },
            [MachineType.CableSawing] = new MachineSettings
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "{ 0}"
            },
        };
    }
}
