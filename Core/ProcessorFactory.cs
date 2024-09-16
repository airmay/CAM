using System;
using CAM.CncWorkCenter;

namespace CAM
{
    public static class ProcessorFactory
    {
        public static ProcessorCnc Create(ProcessingCnc processing)
        {
            IPostProcessor postProcessor;
            switch (processing.Machine)
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

            return new ProcessorCnc(postProcessor)
            {
                Tool = processing.Tool,
                Frequency = processing.Frequency,
                CuttingFeed = processing.CuttingFeed,
                PenetrationFeed = processing.PenetrationFeed,
                ZSafety = processing.ZSafety,
                Origin = processing.Origin
            };
        }
    }
}