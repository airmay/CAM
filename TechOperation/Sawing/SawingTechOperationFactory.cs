using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace CAM.TechOperation.Sawing
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

        private int _techOperationsNumber;

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
        public ITechOperation[] Create(TechProcess techProcess, Curve[] curves)
        {
            List<ITechOperation> techOperations = new List<ITechOperation>();
            foreach (var curve in curves)
            {
                if (curve is Line || curve is Arc)
                {
                    var techOperationParams = curve is Line
                        ? _sawingTechOperationParams.SawingLineTechOperationParams
                        : _sawingTechOperationParams.SawingCurveTechOperationParams;
                    var processingArea = new BorderProcessingArea(curve);
                    techOperations.Add(new SawingTechOperation(techProcess, processingArea, techOperationParams.Clone(), processingArea.ToString() + ++_techOperationsNumber));
                }
                else
                    Acad.Alert($"Операция Распиловка не поддерживается на объектах типа {curve}");
            }
            return techOperations.ToArray();
        }
    }
}
