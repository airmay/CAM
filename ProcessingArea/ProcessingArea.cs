using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM
{
    /// <summary>
    /// Обрабатываемая область
    /// </summary>
    [Serializable]
    public class ProcessingArea
    {
        public long[] Handles { get; set; }

        /// <summary>
        /// Идентификатор графического примитива автокада
        /// </summary>
        [NonSerialized]
        public ObjectId[] AcadObjectIds;

        [NonSerialized]
        public Curve[] Curves;

        /// <summary>
        /// Тип обрабатываемой области
        /// </summary>
        //public abstract ProcessingAreaType Type { get; }

        /// <summary>
        /// Начальная точка кривой
        /// </summary>
        public Point3d StartPoint { get { return Curves[0].StartPoint; } }

        /// <summary>
        /// Конечная точка кривой
        /// </summary>
        public Point3d EndPoint { get { return Curves[0].EndPoint; } }

        /// <summary>
        /// Длина
        /// </summary>
        public double Length { get { return Curves[0].Length(); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="curve">Графический примитива автокада представляющий область</param>
        public ProcessingArea(Curve[] curves)
        {
            Curves = curves;
            AcadObjectIds = Array.ConvertAll(curves, p => p.ObjectId);
            Handles = Array.ConvertAll(curves, p => p.Handle.Value);
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
		    switch (Curves[0])
		    {
			    case Line _:
				    return $"Прямая L{ Math.Round(Length) }";
				case Arc _:
				    return $"Дуга L{ Math.Round(Length) }";
				default:
					return $"{Curves[0].GetType()} L{ Math.Round(Length) }";
		    }
	    }
	}
}
