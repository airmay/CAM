﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    /// <summary>
    /// Обрабатываемая область
    /// </summary>
    [Serializable]
    public abstract class ProcessingArea
    {
        public long Handle { get; set; }

        /// <summary>
        /// Идентификатор графического примитива автокада
        /// </summary>
        [NonSerialized]
        public ObjectId AcadObjectId;

        [NonSerialized]
        public Curve Curve;

        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        //public abstract ProcessingAreaType Type { get; }

        /// <summary>
        /// Начальная точка кривой
        /// </summary>
        public Point3d StartPoint { get { return Curve.StartPoint; } }

        /// <summary>
        /// Конечная точка кривой
        /// </summary>
        public Point3d EndPoint { get { return Curve.EndPoint; } }

        /// <summary>
        /// Длина
        /// </summary>
        public double Length { get { return Curve.Length(); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="curve">Графический примитива автокада представляющий область</param>
        protected ProcessingArea(Curve curve)
        {
            Curve = curve;
            AcadObjectId = curve.ObjectId;
            Handle = curve.Handle.Value;
            //Set(curve);
        }

        /// <summary>
        /// Изменение обрабатываемой области
        /// </summary>
        /// <param name="curve">Графический примитива автокада представляющий область</param>
        //public void Modify(Curve curve)
        //{
        //    //Contract.
        //    if (curve.ObjectId != AcadObjectId)
        //        throw new ArgumentException("Обрабатываемая область не соответствует полученной кривой");

        //    //Set(curve);
        //    var h = curve.Handle;
        //}

        /// <summary>
        /// Заполнение параметров обрабатываемой области в соответствии с полученной кривой
        /// </summary>
        /// <param name="curve"></param>
        //protected virtual void Set(Curve curve)
        //{
        //    Curve = curve;
        //    StartPoint = curve.StartPoint;
        //    EndPoint = curve.EndPoint;
        //    Length = curve.Length();
        //}

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