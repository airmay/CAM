using System;
using CAM.Program.Generator;
using System.Collections.Generic;

namespace CAM.Core
{
    public class Processor
    {
        private readonly Processing _processing;
        private readonly IPostProcessor _postProcessor;
        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public Processor(Processing processing)
        {
            _processing = processing;
            switch (_processing.MachineType)
            {
                case MachineType.ScemaLogic:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Donatoni:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Krea:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.CableSawing:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Forma:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case MachineType.Champion:
                    _postProcessor = new DonatoniPostProcessor();
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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