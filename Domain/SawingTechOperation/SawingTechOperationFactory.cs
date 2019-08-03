using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.Domain
{
    /// <summary>
    /// Фабрика для создания технологических операций "Распиловка"
    /// </summary>
    [Serializable]
    public class SawingTechOperationFactory : ITechOperationFactory
    {
	    /// <summary>
	    /// Вид технологической операции
	    /// </summary>
	    public TechOperationType TechOperationType { get; } = TechOperationType.Sawing;

		public SawingTechOperationParams SawingLineTechOperationParams { get; }

	    public SawingTechOperationParams SawingCurveTechOperationParams { get; }

		/// <summary>
		/// Конструктор фабрики
		/// </summary>
		/// <param name="sawingLineTechOperationParams">Параметры по-умолчанию технологической операции "Распиловка по прямой"</param>
		/// <param name="sawingCurveTechOperationParams">Параметры по-умолчанию технологической операции "Распиловка по дуге"</param>
		public SawingTechOperationFactory(SawingTechOperationParams sawingLineTechOperationParams, SawingTechOperationParams sawingCurveTechOperationParams)
        {
            SawingLineTechOperationParams = sawingLineTechOperationParams;
            SawingCurveTechOperationParams = sawingCurveTechOperationParams;
        }

	    /// <summary>
	    /// Создает технологическую операцию "Распиловка"
	    /// </summary>
	    /// <param name="techProcess"></param>
	    /// <param name="curve">Графическая кривая представляющая область обработки</param>
	    /// <returns>Технологическая операцию</returns>
	    public SawingTechOperation Create(TechProcess techProcess, Curve curve)
        {
	        //if ((entity.Layer != "0" && ((Entity)dbObject).Layer != "Êàìåíü")
	        //{
	        //    AutocadUtils.ShowError("Îáúåêò íå â ñëîå \"0\" èëè \"Êàìåíü\"");
	        //    continue;
	        //}
	        // TODO Проверка слоя при добавлении
	        switch (curve)
	        {
		        case Line line:
			        return new SawingTechOperation(techProcess, new BorderProcessingArea(line), SawingLineTechOperationParams.Clone());
		        case Arc arc:
			        return new SawingTechOperation(techProcess, new BorderProcessingArea(arc), SawingCurveTechOperationParams.Clone());
		        default:
                    Acad.Alert($"Неподдерживаемый тип кривой {curve.GetType()}");
                    return null;
	        }
        }
    }
}
