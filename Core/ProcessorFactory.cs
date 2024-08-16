using System;
using CAM.Program.Generator;

namespace CAM
{
    public static class ProcessorFactory
    {
        public static Processor Create(Machine machine)
        {
            IPostProcessor postProcessor;
            switch (machine)
            {
                case Machine.ScemaLogic:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case Machine.Donatoni:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case Machine.Krea:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case Machine.CableSawing:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case Machine.Forma:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case Machine.Champion:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new MillingProcessor(postProcessor);
        }
    }
}