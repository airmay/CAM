using System.Collections.Generic;
using System.Linq;

namespace CAM.Program.Generator
{
    public class CommandParams
    {
        public Dictionary<char, string> Params { get; set; }

        public CommandParams(Dictionary<char, string> @params) => Params = @params;

        public CommandParams() : this(new Dictionary<char, string>()) { }

        public void Set(char code, double? value, double? origin = null)
        {
            if (value != null)
                Params[code] = (value.Value - origin.GetValueOrDefault()).ToStringParam();
        }

        public void Set(char code, double? value, string format)
        {
            if (value != null)
                Params[code] = string.Format(format, value.Value.ToStringParam());
        }

        public CommandParams CreateChangedParams(CommandParams commandParams)
        {
            var changed = commandParams.Params
                .Where(p => Params.TryGetValue(p.Key, out var value) && p.Value != value)
                .ToDictionary(p => p.Key, p => p.Value);
            return new CommandParams(changed);
        }
    }
}
