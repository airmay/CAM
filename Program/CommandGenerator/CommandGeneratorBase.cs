using System;
using System.Collections.Generic;

namespace CAM
{
    public abstract class CommandGeneratorBase : IDisposable
    {
        protected object _techProcess;
        protected TechOperation _techOperation;

        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public virtual void Dispose() { }

        public void AddCommand(ProcessCommand command)
        {
            command.Owner = _techOperation ?? (object)_techProcess;
            command.Number = ProcessCommands.Count + 1;
            ProcessCommands.Add(command);

            if (_techOperation != null && !_techOperation.ProcessCommandIndex.HasValue)
                _techOperation.ProcessCommandIndex = ProcessCommands.Count - 1;
        }

        protected virtual void StartMachineCommands(string caption)
        {
        }

        protected virtual void StopMachineCommands()
        {
        }

        protected virtual void SetToolCommands(int toolNo, double angleA)
        {
        }

        protected virtual void StartEngineCommands()
        {
        }

        protected virtual void StopEngineCommands()
        {
        }
    }
}
