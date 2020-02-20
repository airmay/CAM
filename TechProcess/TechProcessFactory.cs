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
                            .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(TechProcessAttribute)) as TechProcessAttribute).Caption);
            _techOperationTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechOperation).IsAssignableFrom(p))
                            .Select(p => new { Attr = Attribute.GetCustomAttribute(p, typeof(TechOperationAttribute)) as TechOperationAttribute, Type = p })
                            .GroupBy(p => p.Attr.TechProcessCaption)
                            .ToDictionary(p => _techProcessTypes[p.Key], p => p.ToDictionary(k => k.Attr.TechOperationCaption, v => v.Type));
        }

        public ITechProcess CreateTechProcess(string techProcessCaption) => Activator.CreateInstance(_techProcessTypes[techProcessCaption], new object[] { techProcessCaption, _settings }) as ITechProcess;

        public List<ITechOperation> CreateTechOperations(ITechProcess techProcess, string techOperationName) => 
            !techProcess.Validate() 
                ? new List<ITechOperation>()
                : techOperationName == "Все операции"
                    ? techProcess.CreateTechOperations()
                    : new List<ITechOperation> { Activator.CreateInstance(_techOperationTypes[techProcess.GetType()][techOperationName], new object[] { techProcess, techOperationName }) as ITechOperation };

        public IEnumerable<string> GetTechProcessNames() => _techProcessTypes.Keys;

        public ILookup<Type, string> GetTechOperationNames() => _techOperationTypes.SelectMany(p => p.Value.Select(v => new { p.Key, Value = v.Key })).ToLookup(p => p.Key, p => p.Value);
    }
}
