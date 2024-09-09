using CAM.CncWorkCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM.Core
{
    public static class ProcessItemFactory
    {
        public static IProcessItem CreateProcessing(MachineType machineType)
        {
            switch (machineType)
            {
                case MachineType.CncWorkCenter:
                    return new ProcessingCnc();
                case MachineType.WireSawMachine:
                    return new ProcessingCnc();
                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
            }
        }

        public static IProcessItem CreateOperation(Type operationType, object prototype)
        {
            var operation = (IOperation)Activator.CreateInstance(operationType);
            prototype?.CopyPropertiesTo(operation);

            return operation;
        }

        private static void CopyPropertiesTo(this object source, object dest)
        {
            var sourceProps = source.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType.IsSimpleType() || (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                .ToList();
            var destProps = dest.GetType()
                .GetProperties()
                .Where(x => x.CanWrite)
                .ToList();

            foreach (var (sp, dp) in sourceProps.Join(destProps, p => p.Name, p => p.Name, (sp, dp) => (sp, dp)))
            {
                var value = sp.GetValue(source, null);
                if (sp.PropertyType.IsGenericType && sp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    value = value is List<ICloneable> cloneables
                        ? cloneables.DeepClone()
                        : value.DeepClone();
                }

                dp.SetValue(dest, value, null);
            }
        }
    }
}
