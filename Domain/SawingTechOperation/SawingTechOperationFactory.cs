using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Фабрика для создания технологических операций "Распиловка"
    /// </summary>
    public class SawingTechOperationFactory //: ITechOperationFactory
    {
        private TechProcessParams _techProcessParams;
        private SawingTechOperationParams _sawingLineTechOperationParamsDefault;
        private SawingTechOperationParams _sawingArcTechOperationParamsDefault;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        /// <param name="techProcessParams">Параметры технологического процесса обработки</param>
        /// <param name="sawingLineTechOperationParamsDefault">Параметры по-умолчанию технологической операция "Распиловка по прямой"</param>
        /// <param name="sawingArcTechOperationParamsDefault">Параметры по-умолчанию технологической операция "Распиловка по дуге"</param>
        public SawingTechOperationFactory(TechProcessParams techProcessParams, 
            SawingTechOperationParams sawingLineTechOperationParamsDefault, SawingTechOperationParams sawingArcTechOperationParamsDefault)
        {
            _techProcessParams = techProcessParams;
            _sawingLineTechOperationParamsDefault = sawingLineTechOperationParamsDefault;
            _sawingArcTechOperationParamsDefault = sawingArcTechOperationParamsDefault;
        }

        /// <summary>
        /// Создает технологическую операцию "Распиловка"
        /// </summary>
        /// <param name="curve">Графическая кривая представляющая область обработки</param>
        /// <returns>Технологическая операцию</returns>
        public SawingTechOperation Create(Curve curve)
        {
            //if ((entity.Layer != "0" && ((Entity)dbObject).Layer != "Êàìåíü")
            //{
            //    AutocadUtils.ShowError("Îáúåêò íå â ñëîå \"0\" èëè \"Êàìåíü\"");
            //    continue;
            //}
            // TODO Проверка слоя при добавлении

            SawingTechOperation techOperation = null;
            if (curve is Line)
                techOperation = new SawingLineTechOperation(_techProcessParams, _sawingLineTechOperationParamsDefault.Clone(), curve as Line);
            if (curve is Arc)
                techOperation = new SawingArcTechOperation(_techProcessParams, _sawingArcTechOperationParamsDefault.Clone(), curve as Arc);
            //Polyline Polyline2d Circle

            if (techOperation == null)
                throw new ArgumentException($"Неподдерживаемый тип кривой {curve.GetType()}");

            return techOperation;
        }
    }
}
