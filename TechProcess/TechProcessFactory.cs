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
                            .Select(p => new { Type = p, Attr = Attribute.GetCustomAttribute(p, typeof(TechProcessAttribute)) as TechProcessAttribute })
                            .OrderBy(p => p.Attr.Number)
                            .ToDictionary(p => p.Attr.Caption, v => v.Type);
            _techOperationTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechOperation).IsAssignableFrom(p))
                            .Select(p => new { Type = p, Attr = Attribute.GetCustomAttribute(p, typeof(TechOperationAttribute)) as TechOperationAttribute})
                            .GroupBy(p => p.Attr.TechProcessCaption)
                            .ToDictionary(p => _techProcessTypes[p.Key], p => p.OrderBy(k => k.Attr.Number).ToDictionary(k => k.Attr.TechOperationCaption, v => v.Type));
        }

        public ITechProcess CreateTechProcess(string techProcessCaption)
        {
            var args = _techProcessTypes[techProcessCaption].GetConstructors().Single().GetParameters()
                .Select(par => par.ParameterType == typeof(string) 
                    ? techProcessCaption
                    : typeof(Settings).GetProperties().Single(prop => prop.PropertyType == par.ParameterType).GetValue(_settings))
                .ToArray();
            
            return Activator.CreateInstance(_techProcessTypes[techProcessCaption], args) as ITechProcess;
        }

        public List<ITechOperation> CreateTechOperations(ITechProcess techProcess, string techOperationName = "Все операции") => 
            !techProcess.Validate() 
                ? new List<ITechOperation>()
                : techOperationName == "Все операции"
                    ? techProcess.CreateTechOperations()
                    : new List<ITechOperation> { Activator.CreateInstance(_techOperationTypes[techProcess.GetType()][techOperationName], new object[] { techProcess, techOperationName }) as ITechOperation };

        public IEnumerable<string> GetTechProcessNames() => _techProcessTypes.Keys;

        public ILookup<Type, string> GetTechOperationNames() => _techOperationTypes.SelectMany(p => p.Value.Select(v => new { p.Key, Value = v.Key })).ToLookup(p => p.Key, p => p.Value);
    }
}
