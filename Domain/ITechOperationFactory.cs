using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
	public interface ITechOperationFactory
	{
		/// <summary>
		/// Вид технологической операции
		/// </summary>
		TechOperationType TechOperationType { get; }

        object GetTechOperationParams();

        /// <summary>
        /// Создает технологическую операцию
        /// </summary>
        /// <param name="techProcess"></param>
        /// <param name="curve">Графическая кривая представляющая область обработки</param>
        /// <returns>Технологическая операцию</returns>
        ITechOperation Create(TechProcess techProcess, Curve curve);
	}
}