using System;

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

    }
}
