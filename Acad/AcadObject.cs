using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM
{
    [Serializable]
    public class AcadObject
    {
        public long Handle { get; set; }

        [NonSerialized]
        private ObjectId? _objectId;

        private AcadObject(ObjectId id)
        {
            Handle = id.Handle.Value;
            _objectId = id;
        }

        public static AcadObject Create(ObjectId id) => new AcadObject(id);

        public static List<AcadObject> CreateList(IEnumerable<ObjectId> ids) => ids.Select(p => new AcadObject(p)).ToList();

        public static void LoadAcadProps(object @object)
        {
            bool err = false;
            var properties = @object.GetType().GetProperties();
            foreach (var prop in properties.Where(p => p.PropertyType == typeof(AcadObject)))
            {
                var acadObject = (AcadObject)prop.GetValue(@object);
                if (acadObject != null && !acadObject.LoadObject())
                {
                    prop.SetValue(@object, null);
                    err = true;
                }
            }
            foreach (var prop in properties.Where(p => p.PropertyType == typeof(List<AcadObject>)))
            {
                var acadObjects = (List<AcadObject>)prop.GetValue(@object);
                if (acadObjects != null && !acadObjects.All(p => p.LoadObject()))
                {
                    prop.SetValue(@object, null);
                    err = true;
                }
            }
            if (err)
                Acad.Alert("Используемые в техпроцессе объекты чертежа были удалены");
        }

        private bool LoadObject()
        {
            var result = Acad.Database.TryGetObjectId(new Handle(Handle), out var id);
            if (result)
                _objectId = id;
            return result;
        }

        public ObjectId ObjectId => _objectId.Value;

        public Curve GetCurve() => Acad.OpenForRead(ObjectId);

        public string GetDesc() => ObjectId.GetDesc();
    }
}
