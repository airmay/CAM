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
            throw new System.NotImplementedException();
        }

        public void SetOperarion(Operation operation)
        {
            throw new System.NotImplementedException();
        }

        public void Finish()
        {
        }
    }
}