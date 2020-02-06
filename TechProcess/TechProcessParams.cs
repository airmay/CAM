using System;
using System.ComponentModel;

namespace CAM
{
    /// <summary>
    /// Параметры технологического процесса обработки
    /// </summary>
    [Serializable]
    public class TechProcessParams
	{
		public MachineType Machine { get; set; }

		public Material Material { get; set; }

	    /// <summary>
	    /// Толщина заготовки
	    /// </summary>
	    public int BilletThickness { get; set; } = 30;

		/// <summary>
		/// Высота подъема инструмента
		/// </summary>
		public int ZSafety { get; set; } = 20;

		/// <summary>
		/// Скорость заглубления
		/// </summary>
		public int PenetrationRate { get; set; } = 250;

        /// <summary>
        /// Частота вращения шпинделя
        /// </summary>
        public int Frequency { get; set; } = 2000;

		/// <summary>
		/// Номер
		/// </summary>
		public int ToolNumber { get; set; } = 1;

	    /// <summary>
	    /// Диаметр
	    /// </summary>
	    public double ToolDiameter { get; set; } = 100;

	    /// <summary>
	    /// Толщина
	    /// </summary>
	    public double ToolThickness { get; set; } = 5;

        public TechProcessParams Clone() => (TechProcessParams)MemberwiseClone();
    }

	public enum Material
	{
        [Description("Гранит")]
		Granite, 

        [Description("Мрамор")]
        Marble
    }

    public enum MachineType
	{
		ScemaLogic,

        Donatoni,

        Krea
    }
}
