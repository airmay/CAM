using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public class TechProcessFactory
    {
        private Dictionary<string, Type> _techProcessNames;
        private Dictionary<Type, Dictionary<string, Type>> _techOperationTypes;

        public TechProcessFactory()
        {
            _techProcessNames = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(ITechProcess).IsAssignableFrom(p))
                            .Select(p => new { Type = p, Attr = Attribute.GetCustomAttribute(p, typeof(MenuItemAttribute)) as MenuItemAttribute })
                            .OrderBy(p => p.Attr.Position)
                            .ToDictionary(p => p.Attr.Name, p => p.Type);
            _techOperationTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.BaseType.IsGenericType && 
                    (p.BaseType.GetGenericTypeDefinition() == typeof(MillingTechOperation<>) || p.BaseType.GetGenericTypeDefinition() == typeof(WireSawingTechOperation<>)))
                .Select(p => new { tp = p.BaseType.GetGenericArguments()[0], to = p, attr = Attribute.GetCustomAttribute(p, typeof(MenuItemAttribute)) as MenuItemAttribute })
                .GroupBy(p => p.tp)
                .ToDictionary(p => p.Key, p => p.OrderBy(k => k.attr.Position).ToDictionary(k => k.attr.Name, v => v.to));
        }

        public ITechProcess CreateTechProcess(string techProcessCaption)
        {
            var args = _techProcessNames[techProcessCaption].GetConstructors().Single().GetParameters()
                .Select(par => typeof(Settings).GetProperties().Single(prop => prop.PropertyType == par.ParameterType).GetValue(Settings.Instance))
                .ToArray();

            var techProcess = (ITechProcess)Activator.CreateInstance(_techProcessNames[techProcessCaption], args);
            techProcess.Caption = techProcessCaption;

            return techProcess;
        }

        public List<TechOperation> CreateTechOperations(ITechProcess techProcess, string techOperationName = "Все операции")
        {
            if (!techProcess.Validate())
                return new List<TechOperation>();
            if (techOperationName == "Все операции")
                return techProcess.CreateTechOperations();
            var techOperation = (TechOperation)Activator.CreateInstance(_techOperationTypes[techProcess.GetType()][techOperationName]);
            techOperation.Setup(techProcess, techOperationName);
            return new List<TechOperation> { techOperation };
        }

        public IEnumerable<string> GetTechProcessNames() => _techProcessNames.Keys;

        public ILookup<Type, string> GetTechOperationNames() => _techOperationTypes.SelectMany(p => p.Value.Select(v => new { p.Key, Value = v.Key })).ToLookup(p => p.Key, p => p.Value);
    }
}
