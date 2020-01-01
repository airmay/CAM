using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CAM.Domain
{
    /// <summary>
    /// Фабрика для создания технологических операций
    /// </summary>
    //[Serializable]
    //public class TechOperationFactory
    //{
    //    /// <summary>
    //    /// Фабрики технологических операций
    //    /// </summary>
    //    private Dictionary<TechOperationType, ITechOperationFactory> _techOperationFactorys = new Dictionary<TechOperationType, ITechOperationFactory>();

    //    /// <summary>
    //    /// Конструктор фабрики
    //    /// </summary>
    //    public TechOperationFactory(Settings settings)
    //    {
    //        var factoryTypes = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsClass && typeof(ITechOperationFactory).IsAssignableFrom(p));
    //        foreach (Type type in factoryTypes)
    //        {
    //            var factory = Activator.CreateInstance(type, new[] { settings }) as ITechOperationFactory;
    //            _techOperationFactorys.Add(factory.TechOperationType, factory);
    //        }
    //    }

    //    /// <summary>
    //    /// Создает технологическую операцию
    //    /// </summary>
    //    /// <param name="curve">Графическая кривая представляющая область обработки</param>
    //    /// <returns>Технологическая операция</returns>
    //    public ITechOperation Create(TechProcess techProcess, TechOperationType techOperationType, Curve curve) => GetFactory(techOperationType).Create(techProcess, curve);

    //    public object GetTechOperationParams(TechOperationType techOperationType) => GetFactory(techOperationType).GetTechOperationParams();

    //    private ITechOperationFactory GetFactory(TechOperationType techOperationType) => 
    //        _techOperationFactorys.ContainsKey(techOperationType) ? _techOperationFactorys[techOperationType] : throw new ArgumentException($"Неподдерживаемый тип операции {techOperationType}");
    //}
}
