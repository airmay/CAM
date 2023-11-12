using System;
using CAM.Program.Generator;

namespace CAM
{
    public static class ProcessorFactory
    {
        public static Processor Create(MachineType machineType)
        {
            IPostProcessor postProcessor;
            switch (machineType)
            {
                case MachineType.ScemaLogic:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Donatoni:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Krea:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.CableSawing:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Forma:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Champion:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new MillingProcessor(postProcessor);
        }
    }
}