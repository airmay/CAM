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
        Curve[] GetSelectedEntities();

        /// <summary>
        /// Выделяет примитивы по заданным идентификаторам
        /// </summary>
        /// <param name="idList"></param>
        void SelectEntities(List<ObjectId> idList);

        /// <summary>
        /// Создает примитивы в базе автокада
        /// </summary>
        /// <param name="entities"></param>
        void CreateEntities(List<Curve> entities);

        /// <summary>
        /// Удаляет примитивы из автокада
        /// </summary>
        /// <param name="idList"></param>
        void DeleteEntities(List<ObjectId> idList);

	    void CreateEntities(IEnumerable<Curve> entities);
    }
}