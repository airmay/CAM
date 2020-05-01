using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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

        public static double Round(this double value, int digits = 0) => Math.Round(value, digits);

        public static void BindEnum<T>(this ComboBox comboBox, params T[] values) where T: struct
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
                Acad.Alert($"Не заполнено поле {field}");
            return value != null;
        }
    }
}
