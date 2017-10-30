using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Фабрика для создания обрабатываемых областей
    /// </summary>
    public class ProcessingAreaFactory
    {
        private Dictionary<Type, Type> _processingAreaTypes;

        public ProcessingAreaFactory()
        {
            var types = Assembly.GetAssembly(typeof(ProcessingArea)).GetTypes().Where(type => type.IsSubclassOf(typeof(ProcessingArea))).ToList();
            var typesNoAttr = types.FindAll(p => !p.IsDefined(typeof(CurveTypeAttribute)));
            if (typesNoAttr.Any())
                throw new Exception($"Найдены наследники типа ProcessingArea не помеченные атрибутом CurveTypeAttribute: {string.Join(", ", typesNoAttr)}");
            _processingAreaTypes = types.ToDictionary(t => t.GetCustomAttribute<CurveTypeAttribute>().Type);
        }

        /// <summary>
        /// Создает обрабатываемую область
        /// </summary>
        /// <param name="entity">Объект автокада представляющий графическую кривую</param>
        /// <returns></returns>
        public ProcessingArea Create(Curve curve)
        {
            //if ((entity.Layer != "0" && ((Entity)dbObject).Layer != "Êàìåíü")
            //{
            //    AutocadUtils.ShowError("Îáúåêò íå â ñëîå \"0\" èëè \"Êàìåíü\"");
            //    continue;
            //}
            // TODO Проверка слоя при добавлении

            Type processingAreaType;
            if (!_processingAreaTypes.TryGetValue(curve.GetType(), out processingAreaType))
                throw new ArgumentException($"Неподдерживаемый тип кривой {curve.GetType()}");

            ProcessingArea area = processingAreaType.GetConstructor(new Type[] { typeof(Curve) }).Invoke(new object[] { curve }) as ProcessingArea;

            return area;
        }
    }
}
