using System.Collections.Generic;
using System.Linq;

namespace CAM
{
    public abstract class PostProcessorBase
    {
        #region GCommand
        public Dictionary<char, string> Params { get; } = new Dictionary<char, string>();

        protected virtual string GCommand(Dictionary<char, string> @params)
        {
            var changedParams = GetChangedParams(@params);
            var command = CreateCommand(changedParams);
            ApplyParams(@params);
            return command;
        }

        protected virtual IEnumerable<KeyValuePair<char, string>> GetChangedParams(Dictionary<char, string> @params)
        {
            return @params
                .Where(p => p.Value != null && (!Params.TryGetValue(p.Key, out var value) || value != p.Value));
        }

        protected virtual string CreateCommand(IEnumerable<KeyValuePair<char, string>> @params)
        {
            return string.Join(" ", @params.Select(p => $"{p.Key}{p.Value}"));
        }

        protected virtual void ApplyParams(Dictionary<char, string> @params)
        {
            foreach (var param in @params)
                Params[param.Key] = param.Value;
        }
        #endregion

        public virtual string Cycle() => null;
        public abstract string[] StartMachine();
        public abstract string[] StopMachine();
        public abstract string[] StartEngine(int frequency, bool hasTool);
        public abstract string[] StopEngine();
        public abstract string Pause(double duration);
    }
}