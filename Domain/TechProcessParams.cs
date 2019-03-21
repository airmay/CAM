using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
	/// <summary>
	/// Параметры технологического процесса обработки
	/// </summary>
	public class TechProcessParams
	{
		public Machine Machine { get; set; }

		public MaterialType Material { get; set; }

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
		public int Frequency { get; set; } = 2500;

		///// <summary>
		///// Инструмент
		///// </summary>
		//public Tool Tool { get; set; } = new Tool();

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

	}

	public enum MaterialType
	{
		Мрамор,

		Гранит
	}

	public enum Machine
	{
		ScemaLogic,

		Krea
	}
}
