using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM.Core;

public static class ParamsValidator
{
    private static readonly Dictionary<Type, List<(PropertyInfo Prop, string Name)>> RequiredProps = [];

    public static void AddRequiredParam(Type type, string paramName, string displayName)
    {
        if (!RequiredProps.ContainsKey(type))
            RequiredProps[type] = [];

        var prop = type.GetProperty(paramName, BindingFlags.Public | BindingFlags.Instance);
        RequiredProps[type].Add((prop, displayName));
    }

    public static bool Validate(string name, object paramsObject)
    {
        var type = paramsObject.GetType();
        if (!RequiredProps.TryGetValue(type, out var props))
            return true;

        var paramNames = props
            .Select(p => new { value = p.Prop.GetValue(paramsObject)?.ToString(), name = p.Name })
            .Where(p => string.IsNullOrEmpty(p.value) || p.value == "0")
            .Select(p => p.name)
            .ToArray();

        if (paramNames.Length > 0)
            Acad.Alert($"Не заполнены поля \"{name}\": \"{string.Join(",", paramNames)}\"");

        return paramNames.Length == 0;
    }
}