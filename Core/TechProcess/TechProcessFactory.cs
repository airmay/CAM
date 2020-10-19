using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public class TechProcessFactory
    {
        private Settings _settings;
        private Dictionary<string, Type> _techProcessNames;
        private Dictionary<Type, Dictionary<string, Type>> _techOperationTypes;

        public TechProcessFactory(Settings settings)
        {
            _settings = settings;
            var techProcessTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechProcess).IsAssignableFrom(p))
                            .Select(p => new { Type = p, Attr = Attribute.GetCustomAttribute(p, typeof(TechProcessAttribute)) as TechProcessAttribute })
                            .ToDictionary(p => p.Attr.TechProcessType, v => v.Type);
            _techProcessNames = techProcessTypes.OrderBy(p => p.Key).ToDictionary(k => k.Key.GetDescription(), v => v.Value);
            _techOperationTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechOperation).IsAssignableFrom(p))
                            .Select(p => new { Type = p, Attr = Attribute.GetCustomAttribute(p, typeof(TechOperationAttribute)) as TechOperationAttribute})
                            .GroupBy(p => p.Attr.TechProcessType)
                            .ToDictionary(p => techProcessTypes[p.Key], p => p.OrderBy(k => k.Attr.Number).ToDictionary(k => k.Attr.TechOperationCaption, v => v.Type));
        }

        public ITechProcess CreateTechProcess(string techProcessCaption)
        {
            var args = _techProcessNames[techProcessCaption].GetConstructors().Single().GetParameters()
                .Select(par => par.ParameterType == typeof(string) 
                    ? techProcessCaption
                    : typeof(Settings).GetProperties().Single(prop => prop.PropertyType == par.ParameterType).GetValue(_settings))
                .ToArray();
            
            return Activator.CreateInstance(_techProcessNames[techProcessCaption], args) as ITechProcess;
        }

        public List<ITechOperation> CreateTechOperations(ITechProcess techProcess, string techOperationName = "Все операции") => 
            !techProcess.Validate() 
                ? new List<ITechOperation>()
                : techOperationName == "Все операции"
                    ? techProcess.CreateTechOperations()
                    : new List<ITechOperation> { Activator.CreateInstance(_techOperationTypes[techProcess.GetType()][techOperationName], new object[] { techProcess, techOperationName }) as ITechOperation };

        public IEnumerable<string> GetTechProcessNames() => _techProcessNames.Keys;

        public ILookup<Type, string> GetTechOperationNames() => _techOperationTypes.SelectMany(p => p.Value.Select(v => new { p.Key, Value = v.Key })).ToLookup(p => p.Key, p => p.Value);
    }
}
