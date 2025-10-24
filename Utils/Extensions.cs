using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Utils;

public static class Extensions
{
    public static bool IsZero(this double value) => Math.Abs(value) < Tolerance.Global.EqualPoint;
    public static bool IsEqual(this double value1, double value2) => IsZero(value1 - value2);
    public static bool InRange(this double value, double v1, double v2) => value >= v1 && value <= v2;
    
    /// <summary>
    ///  (value ? 1 : -1) * sign
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetSign(this bool value, int sign = 1) => (value ? 1 : -1) * sign;

    /// <summary>
    /// value >= 0 ? 1 : -1
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetSign(this double value) => value >= 0 ? 1 : -1;

    public static string ToParam(this double value) => value.ToString("0.####");

    public static string ToParam(this double? value) => value?.ToString("0.####");

    public static double Round(this double value, int digits = 0) => Math.Round(value, digits);

    public static bool IsSimpleType(this Type type)
    {
        return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
    }

    #region Enum
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null) return null;
        var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
        return attribute?.Description ?? value.ToString();
    }

    public static void BindEnum<T>(this ComboBox comboBox, params T[] values) where T : struct
    {
        comboBox.DisplayMember = "Description";
        comboBox.ValueMember = "Value";
        comboBox.DataSource = GetEnumValueDesc(values);
    }

    public static Array GetEnumValueDesc<T>(params T[] values) where T : struct
    {
        return (values.Any() ? values : Enum.GetValues(typeof(T)))
            .Cast<Enum>()
            .Select(value => new
            {
                Description = value.GetDescription(),
                value
            })
            .OrderBy(item => item.value)
            .ToArray();
    }

    #endregion

    /// <summary>
    /// Tries to read value and returns the value if successfully read. Otherwise return default value for value's type.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue TryGetAndReturn<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        dictionary.TryGetValue(key, out var retValue);
        return retValue;
    }

    public static void ForAll<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var feature in enumerable)
        {
            action(feature);
        }
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> collection) where T : struct
    {
        return collection?.Where(p => p.HasValue).Select(p => p.Value);
    }

    /// <summary>
    /// Check if an item is in a list.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <param name="list">List of items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    public static bool IsIn<T>(this T item, params T[] list)
    {
        return list.Contains(item);
    }

    /// <summary>
    /// Check if an item is in the given enumerable.
    /// </summary>
    /// <param name="item">Item to check</param>
    /// <param name="items">Items</param>
    /// <typeparam name="T">Type of the items</typeparam>
    public static bool IsIn<T>(this T item, IEnumerable<T> items)
    {
        return items.Contains(item);
    }

    #region Clone
    public static IList<T> DeepClone<T>(this IEnumerable<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }

    public static object DeepClone(this object obj)
    {
        var formatter = new BinaryFormatter { Binder = new MyBinder() };

        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }
    }

    #endregion

    #region Array
    public static bool IsNotNullOrEmpty<T>(this T[] array) => array is { Length: > 0 };

    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        foreach (var t in array)
        {
            action(t);
        }
    }

    public static void ForEach<T>(this T[] array, Action<T, int> action)
    {
        for (var i = 0; i < array.Length; i++)
        {
            action(array[i], i);
        }
    }

    public static TResult[] ConvertAll<T, TResult>(this T[] array, Func<T, TResult> converter)
    {
        var result = new TResult[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            result[i] = converter(array[i]);
        }
        return result;
    }

    public static TResult[] ConvertAll<T, TResult>(this T[] array, Func<T, int, TResult> converter)
    {
        var result = new TResult[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            result[i] = converter(array[i], i);
        }
        return result;
    }
    #endregion

    #region IList
    public static bool Swap(this IList list, int firstIndex, int secondIndex)
    {
        if (!(firstIndex >= 0 && firstIndex < list.Count && secondIndex >= 0 && secondIndex < list.Count && firstIndex != secondIndex))
            return false;
        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
        return true;
    }

    public static bool SwapNext(this IList list, int index) => list.Swap(index, index + 1);

    public static bool SwapPrev(this IList list, int index) => list.Swap(index, index - 1);

    public static bool SwapNext(this IList list, object item) => SwapNext(list, list.IndexOf(item));

    public static bool SwapPrev(this IList list, object item) => SwapPrev(list, list.IndexOf(item));
    #endregion
}