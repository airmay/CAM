using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public static class CommandGeneratorFactory
    {
        private static readonly Dictionary<MachineType, Type> GeneratorTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(CommandGeneratorBase).IsAssignableFrom(p))
                            .ToDictionary(p => ((MachineTypeAttribute)Attribute.GetCustomAttribute(p, typeof(MachineTypeAttribute))).MachineType);

        public static T Create<T>(MachineType machineType)
        {
            return (T)Activator.CreateInstance(GeneratorTypes[machineType]);
        }
    }
}
