using System.Collections.Generic;
using System.Linq;

namespace CAM.Program.Generator
{
    public class CommandParam
    {
        public char Code { get; set; }
        public string Value { get; set; }
        public bool IsChanged { get; set; }

        public CommandParam(char code, string value)
        {
            Code = code;
            Value = value;
        }
    }

    public class CommandParams
    {
        public Dictionary<char, CommandParam> Params { get; set; }

        public CommandParams(IEnumerable<CommandParam> @params) => Params = @params.ToDictionary(p => p.Code);

        public CommandParams(string codes)
        {
            Params = codes.ToCharArray().ToDictionary(p => p, p => new CommandParam(p, string.Empty));
        }

        public void Set(char code, double? value)
        {
            if (value != null)
                Params[code] = new CommandParam(code, value.Value.ToStringParam());
        }

        public void Set(char code, double? value, string format)
        {
            if (value != null)
                Params[code] = new CommandParam(code, string.Format(format, value.Value.ToStringParam()));
        }

        public List<CommandParam> Apply(Dictionary<char, string> @params)
        {
            foreach (var commandParam in Params)
            {
                if (@params.TryGetValue(commandParam.Key, out var value) && value != commandParam.Value.Value)
                {
                    commandParam.Value = 
                }
            }


            var changed = @params
                .Where(p => p.Value != null && p.Value != Params[p.Key].Value)
                .Select(p => new CommandParam(p.Key, p.Value))
                .ToList();
            changed.ForEach(p => Params[p.Code].Value = p.Value);
            return changed;
        }
    }
}
