using System.Collections.Generic;

namespace CAM.Domain
{
	public interface ITechOperationFactory
	{
		/// <summary>
		/// Вид технологической операции
		/// </summary>
		TechOperationType TechOperationType { get; }

		/// <summary>
		/// Создает технологичесие операции
		/// </summary>
		/// <param name="techProcess"></param>
		/// <param name="curve">Графическая кривая представляющая область обработки</param>
		/// <returns>Технологическая операцию</returns>
		SawingTechOperation Create(TechProcess techProcess, Curve curve);
	}
}