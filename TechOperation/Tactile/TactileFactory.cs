using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;

namespace CAM.TechOperation.Tactile
{
    /// <summary>
    /// Фабрика для создания технологических операций "Тактилка"
    /// </summary>
    //[Serializable]
    //[Processing(ProcessingType.Tactile, MachineType.ScemaLogic, "Тактилка")]
    //public class TactileFactory : ITechOperationFactory
    //{
    //    public ProcessingType ProcessingType => ProcessingType.Tactile;

    //    private readonly TactileParams _tactileParams;

    //    private int _techOperationsNumber;

    //    public object GetTechOperationParams() => _tactileParams;

    //    /// <summary>
    //    /// Конструктор фабрики
    //    /// </summary>
    //    public TactileFactory(Settings settings)
    //    {
    //        //_tactileParams = settings.TactileParams.Clone();
    //    }

    //    /// <summary>
    //    /// Создает технологическую операцию "Тактилка"
    //    /// </summary>
    //    /// <param name="techProcess"></param>
    //    /// <param name="curve">Графическая кривая представляющая область обработки</param>
    //    /// <returns>Технологическая операцию</returns>
    //    public ITechOperation[] Create(ITechProcess techProcess, Curve[] curves)
    //    {
    //        if (curves.Cast<Line>().Count() != 4)
    //        {
    //            Acad.Alert($"Выделено {curves.Cast<Line>().Count()} отрезков. Необходимо выбрать 4 отрезка контура плитки");
    //            return null;
    //        }
    //        //return new[] { new TactileTechOperanion(techProcess, new ProcessingArea(curves), _tactileParams.Clone(), "Тактилка" + ++_techOperationsNumber)};
    //    }
    //}
}
