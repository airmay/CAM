using System.Collections.Generic;
using CAM.Domain;

namespace CAM
{
    public interface IAcadGateway
    {
        /// <summary>
        /// Получает список выбранных примитивов автокада
        /// </summary>
        /// <returns></returns>
        Curve[] GetSelectedCurves();

        /// <summary>
        /// Выделяет примитивы по заданным идентификаторам
        /// </summary>
        /// <param name="list"></param>
        void SelectCurvies(List<ObjectId> list);
    }
}