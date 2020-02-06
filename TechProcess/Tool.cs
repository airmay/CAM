using System;
using System.ComponentModel;

namespace CAM
{
    /// <summary>
    /// Инструмент
    /// </summary>
    [Serializable]
    public class Tool
    {
        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public ToolType Type { get; set; }

        /// <summary>
        /// Диаметр
        /// </summary>
        public double Diameter { get; set; }

        /// <summary>
        /// Толщина
        /// </summary>
        public double? Thickness { get; set; }

        public override string ToString() => $"№{Number} {Type.GetDescription()} Ø{Diameter}{(Thickness.HasValue ? " × " + Thickness.ToString() : null)} {Name}";

        public int CalcFrequency(Material material)
        {
            var speed = material == Material.Granite ? 35 : 50;
            return (int)Math.Round(speed * 1000 / (Diameter * Math.PI) * 60);
        }
    }

    public enum ToolType
    {
        [Description("Диск")]
        Disk,

        [Description("Фреза")]
        Mill
    }
}
