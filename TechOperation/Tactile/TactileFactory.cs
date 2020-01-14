using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;

namespace CAM.TechOperation.Tactile
{
    /// <summary>
    /// Фабрика для создания технологических операций "Тактилка"
    /// </summary>
    [Serializable]
    [Processing(ProcessingType.Tactile, MachineType.ScemaLogic, "Тактилка")]
    public class TactileFactory : ITechOperationFactory
    {
        public ProcessingType ProcessingType => ProcessingType.Tactile;

        private readonly TactileParams _tactileParams;

        public object GetTechOperationParams() => _tactileParams;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        public TactileFactory(Settings settings)
        {
            _tactileParams = settings.TactileParams.Clone();
        }

        /// <summary>
        /// Создает технологическую операцию "Тактилка"
        /// </summary>
        /// <param name="techProcess"></param>
        /// <param name="curve">Графическая кривая представляющая область обработки</param>
        /// <returns>Технологическая операцию</returns>
        public ITechOperation[] Create(TechProcess techProcess, Curve[] curves)
        {
            if (curves.Length != 4 || !curves.All(p => p is Line))
            {
                Acad.Alert("Операция Тактилка выполняется на 4 объектах типа Отрезок");
                return null;
            }
            return new[] { new TactileTechOperanion(techProcess, new ProcessingArea(curves), _tactileParams.Clone()) };
        }
    }
}
