using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM
{
    /// <summary>
    /// Фабрика для создания обрабатываемых областей
    /// </summary>
    [Obsolete]
    public class ProcessingAreaFactory
    {
        /// <summary>
        /// Создает обрабатываемую область
        /// </summary>
        /// <param name="entity">Объект автокада представляющий графическую кривую</param>
        /// <returns></returns>
        public ProcessingArea Create(Curve curve)
        {
            //if ((entity.Layer != "0" && ((Entity)dbObject).Layer != "Êàìåíü")
            //{
            //    AutocadUtils.ShowError("Îáúåêò íå â ñëîå \"0\" èëè \"Êàìåíü\"");
            //    continue;
            //}
            // TODO Проверка слоя при добавлении

            ProcessingArea area = null;
            if (curve is Line)
                area = new LineProcessingArea(curve);
            if (curve is Arc)
                area = new ArcProcessingArea(curve);
            //Polyline Polyline2d Circle

            if (area == null)
                throw new ArgumentException($"Неподдерживаемый тип кривой {curve.GetType()}");

            return area;
        }
    }
}
