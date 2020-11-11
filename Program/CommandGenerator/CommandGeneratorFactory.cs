using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public static class CommandGeneratorFactory
    {
        private static Dictionary<MachineType, Type> _generatorTypes;

        static CommandGeneratorFactory() => _generatorTypes = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(p => p.IsClass && !p.IsAbstract && typeof(CommandGeneratorBase).IsAssignableFrom(p))
                            .ToDictionary(p => ((MachineTypeAttribute)Attribute.GetCustomAttribute(p, typeof(MachineTypeAttribute))).MachineType);

        public static CommandGeneratorBase Create(MachineType machineType) => (CommandGeneratorBase)Activator.CreateInstance(_generatorTypes[machineType]);
    }
}
