using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.TechOperation.Profiling
{
    //[Processing(ProcessingType.Profiling, MachineType.ScemaLogic, "Профилирование")]
    //public class ProfilingFactory : ITechOperationFactory
    //{
    //    public ProcessingType ProcessingType => ProcessingType.Profiling;

    //    /// <summary>
    //    /// Конструктор фабрики
    //    /// </summary>
    //    public ProfilingFactory(Settings settings)
    //    {
    //    }

    //    public ITechOperation[] Create(ITechProcess techProcess, Curve[] curves)
    //    {
    //        return new[] { new ProfilingTechOperation(techProcess, new BorderProcessingArea(curves[0]), "Профилирование") };
    //    }

    //    public object GetTechOperationParams()
    //    {
    //        return null;
    //    }
    //}
}
