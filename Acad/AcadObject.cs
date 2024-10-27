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
        [NonSerialized] private ObjectId[] _objectIds;

        private AcadObject(ObjectId[] objectIds)
        {
            _objectIds = objectIds;
            Handles = Array.ConvertAll(objectIds, p => p.Handle.Value);
        }

        public static AcadObject Create(ObjectId id) => new AcadObject(new[] { id });
        public static AcadObject Create(IEnumerable<ObjectId> ids) => new AcadObject(ids.ToArray());

        public ObjectId[] ObjectIds =>
            _objectIds ?? (_objectIds = Handles
                .Select(p => Acad.Database.TryGetObjectId(new Handle(p), out var id) ? id : ObjectId.Null)
                .Where(p => p != ObjectId.Null)
                .ToArray());

        public ObjectId ObjectId => _objectIds[0];
        public Curve GetCurve() => Acad.OpenForRead(ObjectId);
        public Curve[] GetCurves() => Acad.OpenForRead(_objectIds);

        public override string ToString() => ObjectIds.GetDesc();
    }
}
