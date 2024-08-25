using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Core
{
    public static class OperationFactory
    {
        public static OperationCnc Create(Type operationType, OperationCnc prototype)
        {
            var operation = (OperationCnc)Activator.CreateInstance(operationType);
            prototype?.CopyPropertiesTo(operation);

            if (Acad.GetSelectedObjectIds() is ObjectId[] ids)
                operation.ProcessingArea = AcadObject.Create(ids);

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
