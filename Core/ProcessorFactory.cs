using System;
using CAM.CncWorkCenter;

namespace CAM
{
    public static class ProcessorFactory
    {
        public static ProcessorCnc Create(Machine machine)
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
            return new ProcessorCnc(postProcessor);
        }
    }
}