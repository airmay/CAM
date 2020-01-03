using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.TechOperation.Profiling
{
    [Processing(ProcessingType.Profiling, MachineType.ScemaLogic, "Профилирование")]
    public class ProfilingFactory : ITechOperationFactory
    {
        public ProcessingType ProcessingType => ProcessingType.Profiling;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        public ProfilingFactory(Settings settings)
        {
        }

        public TechOperationBase Create(TechProcess techProcess, Curve curve)
        {
            return new ProfilingTechOperation(techProcess, new BorderProcessingArea(curve));
        }

        public object GetTechOperationParams()
        {
            return null;
        }
    }
}
