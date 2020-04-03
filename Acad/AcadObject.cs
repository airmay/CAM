using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
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

        public ObjectId ObjectId => (_objectId ?? (_objectId = Acad.Database.GetObjectId(false, new Handle(Handle), 0))).Value;

        public Curve GetCurve() => ObjectId.QOpenForRead<Curve>();

        public string GetDesc() => ObjectId.GetDesc();
    }
}
