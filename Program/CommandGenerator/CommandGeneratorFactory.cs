using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public static class CommandGeneratorFactory
    {
        private static readonly Dictionary<Machine, Type> GeneratorTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(CommandGeneratorBase).IsAssignableFrom(p))
                            .ToDictionary(p => ((MachineTypeAttribute)Attribute.GetCustomAttribute(p, typeof(MachineTypeAttribute))).Machine);

        public static T Create<T>(Machine machine)
        {
            return (T)Activator.CreateInstance(GeneratorTypes[machine]);
        }
    }
}
