using System.Collections.Generic;

namespace CAM
{
    public static class Settings
    {
        public static readonly Dictionary<Machine, MachineSettings> Machines = new Dictionary<Machine, MachineSettings>
        {
            [Machine.Donatoni] = new MachineSettings
            {
                MaxFrequency = 5000,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}",
                IsFrontPlaneZero = true
            },
            [Machine.Forma] = new MachineSettings
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pgm",
                ProgramLineNumberFormat = "N{0}"
            },
            [Machine.Champion] = new MachineSettings
            {
                MaxFrequency = 1200,
                ProgramFileExtension = "pim",
                ProgramLineNumberFormat = "N{0}"
            },
            [Machine.Krea] = new MachineSettings
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "N{0}"
            },
            [Machine.WireSawMachine] = new MachineSettings
            {
                MaxFrequency = 10000,
                ProgramFileExtension = "txt",
                ProgramLineNumberFormat = "{ 0}"
            },
        };
    }
}
