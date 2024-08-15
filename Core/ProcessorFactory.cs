using System;
using CAM.Program.Generator;

namespace CAM
{
    public static class ProcessorFactory
    {
        public static Processor Create(MachineCodes machineCodes)
        {
            IPostProcessor postProcessor;
            switch (machineCodes)
            {
                case MachineCodes.ScemaLogic:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineCodes.Donatoni:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineCodes.Krea:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineCodes.CableSawing:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineCodes.Forma:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineCodes.Champion:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new MillingProcessor(postProcessor);
        }
    }
}