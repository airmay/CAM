using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace CAM.TechOperations.Sawing
{
    /// <summary>
    /// Фабрика для создания технологических операций "Распиловка"
    /// </summary>
    [Serializable]
    [Processing(ProcessingType.Sawing, MachineType.ScemaLogic, "Распиловка")]
    public class SawingTechOperationFactory : ITechOperationFactory
    {
        public ProcessingType ProcessingType => ProcessingType.Sawing;

        private readonly SawingDefaultParams _sawingTechOperationParams;

        public object GetTechOperationParams() => _sawingTechOperationParams;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        public SawingTechOperationFactory(Settings settings)
        {
            _sawingTechOperationParams = new SawingDefaultParams
            {
                SawingCurveTechOperationParams = settings.SawingCurveTechOperationParams.Clone(),
                SawingLineTechOperationParams = settings.SawingLineTechOperationParams.Clone()
            };
        }

        /// <summary>
        /// Создает технологическую операцию "Распиловка"
        /// </summary>
        /// <param name="techProcess"></param>
        /// <param name="curve">Графическая кривая представляющая область обработки</param>
        /// <returns>Технологическая операцию</returns>
        public TechOperationBase Create(TechProcess techProcess, Curve curve)
        {
            if (!(curve is Line || curve is Arc))
            {
                Acad.Alert("Операция Распиловка выполняется на объектах типа Отрезок и Дуга");
                return null;
            }
            var techOperationParams = curve is Line
                ? _sawingTechOperationParams.SawingLineTechOperationParams
                : _sawingTechOperationParams.SawingCurveTechOperationParams;

            return new SawingTechOperation(techProcess, new BorderProcessingArea(curve), techOperationParams.Clone());
        }
    }
}
