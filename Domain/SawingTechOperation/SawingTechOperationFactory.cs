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
    public class SawingTechOperationFactory //: ITechOperationFactory
    {
        private SawingTechOperationParams _sawingLineTechOperationParamsDefault;
        private SawingTechOperationParams _sawingArcTechOperationParamsDefault;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        /// <param name="sawingLineTechOperationParamsDefault">Параметры по-умолчанию технологической операция "Распиловка по прямой"</param>
        /// <param name="sawingArcTechOperationParamsDefault">Параметры по-умолчанию технологической операция "Распиловка по дуге"</param>
        public SawingTechOperationFactory(SawingTechOperationParams sawingLineTechOperationParamsDefault, SawingTechOperationParams sawingArcTechOperationParamsDefault)
        {
            _sawingLineTechOperationParamsDefault = sawingLineTechOperationParamsDefault;
            _sawingArcTechOperationParamsDefault = sawingArcTechOperationParamsDefault;
        }

        /// <summary>
        /// Создает технологическую операцию "Распиловка"
        /// </summary>
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

            SawingTechOperation techOperation = null;
            if (curve is Line)
                techOperation = new SawingLineTechOperation(techProcess, new LineProcessingArea(curve), _sawingLineTechOperationParamsDefault.Clone());
            if (curve is Arc)
                techOperation = new SawingArcTechOperation(techProcess, new ArcProcessingArea(curve), _sawingArcTechOperationParamsDefault.Clone());
            //Polyline Polyline2d Circle

            if (techOperation == null)
                MessageBox.Show($"Неподдерживаемый тип кривой {curve.GetType()}");

            return techOperation;
        }
    }
}
