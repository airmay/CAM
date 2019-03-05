using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Обрабатываемая область
    /// </summary>
    public abstract class ProcessingArea
    {
        /// <summary>
        /// Идентификатор графического примитива автокада
        /// </summary>
        public readonly ObjectId AcadObjectId;

        public Curve Curve { get; set; }

        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        //public abstract ProcessingAreaType Type { get; }

        /// <summary>
        /// Начальная точка кривой
        /// </summary>
        public Point3d StartPoint { get; protected set; }

        /// <summary>
        /// Конечная точка кривой
        /// </summary>
        public Point3d EndPoint { get; protected set; }

        /// <summary>
        /// Длина
        /// </summary>
        public double Length { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="curve">Графический примитива автокада представляющий область</param>
        protected ProcessingArea(Curve curve)
        {
            AcadObjectId = curve.ObjectId;
            Set(curve);
        }

        /// <summary>
        /// Изменение обрабатываемой области
        /// </summary>
        /// <param name="curve">Графический примитива автокада представляющий область</param>
        public void Modify(Curve curve)
        {
            //Contract.
            if (curve.ObjectId != AcadObjectId)
                throw new ArgumentException("Обрабатываемая область не соответствует полученной кривой");

            Set(curve);
        }

        /// <summary>
        /// Заполнение параметров обрабатываемой области в соответствии с полученной кривой
        /// </summary>
        /// <param name="curve"></param>
        protected virtual void Set(Curve curve)
        {
            Curve = curve;
            StartPoint = curve.StartPoint;
            EndPoint = curve.EndPoint;
            Length = curve.Length;
        }

	    public override string ToString()
	    {
		    switch (Curve)
		    {
			    case Line _:
				    return $"Прямая L{ Math.Round(Length) }";
				case Arc _:
				    return $"Дуга L{ Math.Round(Length) }";
				default:
					return $"{Curve.GetType()} L{ Math.Round(Length) }";
		    }
	    }
	}
}
