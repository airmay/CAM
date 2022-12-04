using System;
using System.Linq;
using System.Collections.Generic;

namespace CAM
{
    public abstract class CommandGeneratorBase : IDisposable
    {
        protected ITechProcess _techProcess;
        protected TechOperation _techOperation;

        public List<ProcessCommand> ProcessCommands { get; } = new List<ProcessCommand>();

        public Dictionary<string, double> Params = new Dictionary<string, double>();
        protected string CommandDelimiter = " ";

        public virtual void Dispose() { }

        public void SetTechOperation(TechOperation techOperation)
        {
            _techOperation = techOperation;
            _techOperation.FirstCommandIndex = null;
        }

        public void AddCommand(ProcessCommand command)
        {
            command.Owner = _techOperation ?? (object)_techProcess;
            command.Number = ProcessCommands.Count + 1;
            ProcessCommands.Add(command);

            if (_techOperation?.FirstCommandIndex.HasValue == false)
                _techOperation.FirstCommandIndex = ProcessCommands.Count - 1;
        }

        public virtual string GetTextParams(Dictionary<string, double?> newParams)
        {
            return string.Join(CommandDelimiter, newParams.ToList().Select(p =>
                {
                    if (p.Value == null)
                        return null;

                    var newValue = p.Value.Value.Round(4);
                    if (Params.TryGetValue(p.Key, out var oldValue) && newValue == oldValue)
                        return null;

                    Params[p.Key] = newValue;
                    return $"{p.Key}{newValue}";
                })
                .Where(p => p != null));
        }

        protected virtual void StartMachineCommands(string caption)
        {
        }

        protected virtual void StopMachineCommands()
        {
        }

        protected virtual void StartEngineCommands()
        {
        }

        protected virtual void StopEngineCommands()
        {
        }

        public virtual void StartTechProcess(ITechProcess techProcess)
        {
        }

        public virtual void FinishTechProcess()
        {
        }
    }
}
