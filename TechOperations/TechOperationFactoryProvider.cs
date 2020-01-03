using CAM.TechOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace CAM
{
    public static class TechOperationFactoryProvider
    {
        private static Dictionary<ProcessingType, Type> _types;

        public static ITechOperationFactory CreateFactory(ProcessingType processingType, Settings settings)
        {
            if (_types == null)
                _types = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(p => p.IsClass && typeof(ITechOperationFactory).IsAssignableFrom(p) && Attribute.IsDefined(p, typeof(ProcessingAttribute)))
                    .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(ProcessingAttribute)) as ProcessingAttribute).Type);
            if (!_types.ContainsKey(processingType))
                throw new ArgumentException($"Неподдерживаемый вид обработки {processingType}");
            return Activator.CreateInstance(_types[processingType], new[] { settings }) as ITechOperationFactory;             
        }
    }
}
