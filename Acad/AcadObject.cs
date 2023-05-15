using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM
{
    [Serializable]
    public class AcadObject
    {
        public long[] Handles { get; set; }

        [NonSerialized] 
        public ObjectId[] ObjectIds;

        private AcadObject(ObjectId[] objectIds)
        {
            ObjectIds = objectIds;
            Handles = Array.ConvertAll(objectIds, p => p.Handle.Value);
        }

        public static AcadObject Create(ObjectId id) => new AcadObject(new[] { id });

        public static AcadObject Create(IEnumerable<ObjectId> ids) => new AcadObject(ids.ToArray());

        public static void LoadAcadProps(object @object)
        {
            var acadObjects = @object.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(AcadObject))
                .Select(p => (AcadObject)p.GetValue(@object))
                .Where(p => p != null);
            foreach (var acadObject in acadObjects)
            {
                if (!acadObject.LoadObject())
                    Acad.Alert("Используемые в техпроцессе объекты чертежа были удалены");
            }
        }

        private bool LoadObject()
        {
            ObjectIds = Handles
                .Select(p => Acad.Database.TryGetObjectId(new Handle(p), out var id) ? id : ObjectId.Null)
                .Where(p => p != ObjectId.Null)
                .ToArray();
            return Handles.Length == ObjectIds.Length;
        }

        public ObjectId ObjectId => ObjectIds[0];

        public Curve GetCurve() => Acad.OpenForRead(ObjectId);

        public override string ToString() => ObjectIds.GetDesc();
    }
}
