using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public static class CommandGeneratorFactory
    {
        private static readonly Dictionary<MachineCodes, Type> GeneratorTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(CommandGeneratorBase).IsAssignableFrom(p))
                            .ToDictionary(p => ((MachineTypeAttribute)Attribute.GetCustomAttribute(p, typeof(MachineTypeAttribute))).MachineCodes);

        public static T Create<T>(MachineCodes machineCodes)
        {
            return (T)Activator.CreateInstance(GeneratorTypes[machineCodes]);
        }
    }
}
