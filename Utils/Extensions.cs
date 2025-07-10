using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using static CAM.DataLoader;

namespace CAM
{
    public static class Extensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        ///  value ? 1 : -1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetSign(this bool value) => value ? 1 : -1;
        public static int GetSign(this int value) => value >= 0 ? 1 : -1;

        /// <summary>
        /// value >= 0 ? 1 : -1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetSign(this double value) => value >= 0 ? 1 : -1;
        public static string ToStringParam(this double value) => value.ToString("0.####");
        public static string ToParam(this double? value) => value?.ToString("0.####");

        public static double Round(this double value, int digits = 0) => Math.Round(value, digits);

        public static void BindEnum<T>(this ComboBox comboBox, params T[] values) where T : struct
        {
            comboBox.DisplayMember = "Description";
            comboBox.ValueMember = "Value";
            comboBox.DataSource = (values.Any() ? values : Enum.GetValues(typeof(T)))
                .Cast<Enum>()
                .Select(value => new
                {
                    Description = value.GetDescription(),
                    value
                })
                .OrderBy(item => item.value)
                .ToList();
        }

        public static bool CheckNotNull(this object value, string field)
        {
            if (value == null)
                Acad.Alert($"Не заполнено поле \"{field}\"");
            return value != null;
        }

        public static T GetSource<T>(this BindingSource bindingSource) => (T)bindingSource.DataSource;

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> collection) where T : struct
        {
            return collection?.Where(p => p.HasValue).Select(p => p.Value);
        }

        public static T If<T>(this T obj, bool condition, Func<T, T> func)
        {
            if (condition)
            {
                return func(obj);
            }

            return obj;
        }

        public static void WriteToFile(this Exception ex, string message)
        {
            var builder = new StringBuilder();
            while (ex != null)
            {
                builder.AppendFormat("{0}{1}", ex, Environment.NewLine);
                ex = ex.InnerException;
            }
            try
            {
                File.WriteAllText($@"\\US-CATALINA3\public\Программы станок\CodeRepository\Logs\error_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log", $"{Acad.ActiveDocument.Name}\n\n{message}\n\n{builder}");
            }
            catch { }
        }

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

        /// <summary>
        /// Tries to read value and returns the value if successfully read. Otherwise return default value
        /// for value's type.
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

        #region Clone

        public static IList<T> DeepClone<T>(this IEnumerable<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static List<T> Clone<T>(this List<T> source)
        {
            return source.GetRange(0, source.Count);
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

        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
        }
    }
}
