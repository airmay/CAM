using Autodesk.AutoCAD.DatabaseServices;
using System;

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

        [NonSerialized]
        private readonly Settings _settings;

        private SawingDefaultParams _params;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        public SawingTechOperationFactory(Settings settings)
        {
            _settings = settings;
        }

	    /// <summary>
	    /// Создает технологическую операцию "Распиловка"
	    /// </summary>
	    /// <param name="techProcess"></param>
	    /// <param name="curve">Графическая кривая представляющая область обработки</param>
	    /// <returns>Технологическая операцию</returns>
	    public ITechOperation Create(TechProcess techProcess, Curve curve)
        {
            var techOperationParams = curve is Line
                ? _params?.SawingLineTechOperationParams ?? _settings.SawingLineTechOperationParams
                : _params?.SawingCurveTechOperationParams ?? _settings.SawingCurveTechOperationParams;

            return new SawingTechOperation(techProcess, new BorderProcessingArea(curve), techOperationParams.Clone());
        }

        public object GetTechOperationParams() => _params ?? (
                _params = new SawingDefaultParams
                {
                    SawingLineTechOperationParams = _settings.SawingLineTechOperationParams.Clone(),
                    SawingCurveTechOperationParams = _settings.SawingCurveTechOperationParams.Clone()
                });
    }
}
