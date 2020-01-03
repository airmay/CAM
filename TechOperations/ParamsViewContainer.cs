using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CAM
{
    public static class ParamsViewContainer
    {
        private static Dictionary<ProcessingType, Type> _paramsViewTypes;
        private static Dictionary<ProcessingType, Type> _defaultParamsViewTypes;
        private static Dictionary<ProcessingType, ParamsView> _paramsViews = new Dictionary<ProcessingType, ParamsView>();
        private static Dictionary<ProcessingType, ParamsView> _defaultParamsViews = new Dictionary<ProcessingType, ParamsView>();

        static ParamsViewContainer()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsClass && typeof(ParamsView).IsAssignableFrom(p)).ToList();
            _paramsViewTypes = types.Where(p => Attribute.IsDefined(p, typeof(ParamsViewAttribute)))
                .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(ParamsViewAttribute)) as ParamsViewAttribute).ProcessingType);
            _defaultParamsViewTypes = types.Where(p => Attribute.IsDefined(p, typeof(DefaultParamsViewAttribute)))
               .ToDictionary(p => (Attribute.GetCustomAttribute(p, typeof(DefaultParamsViewAttribute)) as DefaultParamsViewAttribute).ProcessingType);
        }

        public static ParamsView SetParamsView(ProcessingType type, object @params, Control owner) => 
            SetParamsViewInternal(type, @params, owner, _paramsViewTypes, _paramsViews);

        public static ParamsView SetDefaultParamsView(ProcessingType type, object @params, Control owner) => 
            SetParamsViewInternal(type, @params, owner, _defaultParamsViewTypes, _defaultParamsViews);

        private static ParamsView SetParamsViewInternal(ProcessingType processingType, object @params, Control owner, Dictionary<ProcessingType, Type> types, Dictionary<ProcessingType, ParamsView> views)
        {
            if (!views.TryGetValue(processingType, out var view))
            {
                view = types.ContainsKey(processingType)
                    ? Activator.CreateInstance(types[processingType]) as ParamsView
                    : new ParamsView();
                view.Dock = DockStyle.Fill;
                views.Add(processingType, view);
                owner.Controls.Add(view);
            }
            view.SetParams(@params);
            view.Visible = true;
            view.BringToFront();

            return view;
        }
    }
}
