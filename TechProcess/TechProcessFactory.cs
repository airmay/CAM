using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public class TechProcessFactory
    {
        private Settings _settings;
        private Dictionary<string, Type> _techProcessTypes;
        private Dictionary<Type, Dictionary<string, Type>> _techOperationTypes;

        public TechProcessFactory(Settings settings)
        {
            _settings = settings;
            _techProcessTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechProcess).IsAssignableFrom(p))
                            .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(TechProcessAttribute)) as TechProcessAttribute).Name);
            _techOperationTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechOperation).IsAssignableFrom(p))
                            .Select(p => new { Attr = Attribute.GetCustomAttribute(p, typeof(TechOperationAttribute)) as TechOperationAttribute, Type = p })
                            .GroupBy(p => p.Attr.TechProcessName)
                            .ToDictionary(p => _techProcessTypes[p.Key], p => p.ToDictionary(k => k.Attr.TechOperationName, v => v.Type));
        }

        public ITechProcess CreateTechProcess(string techProcessName) => Activator.CreateInstance(_techProcessTypes[techProcessName], new[] { _settings }) as ITechProcess;

        public ITechOperation CreateTechOperation(ITechProcess techProcess, string techOperationName) => 
            Activator.CreateInstance(_techOperationTypes[techProcess.GetType()][techOperationName], new object[] { techProcess, techOperationName }) as ITechOperation;

        public IEnumerable<string> GetTechProcessNames() => _techProcessTypes.Keys;

        public ILookup<Type, string> GetTechOperationNames() => _techOperationTypes.SelectMany(p => p.Value.Select(v => new { p.Key, Value = v.Key })).ToLookup(p => p.Key, p => p.Value);
    }
}
