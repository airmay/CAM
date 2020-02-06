using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CAM
{
    public static class ObjectViewsContainer
    {
        private static readonly Dictionary<Type, Type> _objectViewTypes;
        private static readonly Dictionary<Type, UserControl> _objectViews = new Dictionary<Type, UserControl>();

        static ObjectViewsContainer() => _objectViewTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(p => p.IsClass && typeof(IObjectView).IsAssignableFrom(p))
                .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(ObjectViewAttribute)) as ObjectViewAttribute).ObjectType);

        public static IObjectView SetObjectView(object @object, Control owner)
        {
            var objectType = @object.GetType();
            if (!_objectViewTypes.ContainsKey(objectType))
                throw new Exception($"Не найдено представление для объектов типа {objectType}");
            if (!_objectViews.TryGetValue(objectType, out var view))
            {
                view = Activator.CreateInstance(_objectViewTypes[objectType]) as UserControl;
                view.Dock = DockStyle.Fill;
                _objectViews.Add(objectType, view);
                owner.Controls.Add(view);
            }
            ((IObjectView)view).SetObject(@object);
            view.Show();
            view.BringToFront();

            return (IObjectView)view;
        }
    }
}
