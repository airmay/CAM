using System;
using CAM.Program.Generator;
using System.Collections.Generic;

namespace CAM
{
    public class Processor
    {
        private readonly IPostProcessor _postProcessor;
        public List<Command> ProcessCommands { get; } = new List<Command>();
        private Operation _operation;

        public Processor(IPostProcessor postProcessor)
        {
            _postProcessor = postProcessor;

        }

        public void Start(Tool tool)
        {
            _postProcessor.StartMachine();
            _postProcessor.SetTool(tool.Number, 0, 0, 0);
        }

        public void SetOperarion(Operation operation)
        {
            _operation = operation;
            _operation.FirstCommandIndex = ProcessCommands.Count;
        }

        public void Finish()
        {
        }
    }
}