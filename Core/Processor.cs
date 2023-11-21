using System;
using CAM.Program.Generator;
using System.Collections.Generic;

namespace CAM
{
    public class Processor
    {
        private readonly IPostProcessor _postProcessor;
        public List<Command> ProcessCommands { get; } = new List<Command>();

        public Processor(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;

        }

        public void Start()
        {
        }

        public void SetOperarion(Operation operation)
        {
        }

        public void Finish()
        {
        }
    }
}