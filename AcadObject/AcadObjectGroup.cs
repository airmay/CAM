using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;

namespace CAM
{
    [Serializable]
    public class AcadObjectGroup
    {
        public long[] Handles { get; set; }

        [NonSerialized]
        private ObjectId[] _objectIds;

        public ObjectId[] ObjectIds
        {
            get => _objectIds ?? (_objectIds = Array.ConvertAll(Handles, p => Acad.Database.GetObjectId(false, new Handle(p), 0)));
        }

        public Curve[] GetCurves() => ObjectIds.QOpenForRead<Curve>();

        public AcadObjectGroup(Curve[] curves)
        {
            Handles = Array.ConvertAll(curves, p => p.Handle.Value);
            _objectIds = Array.ConvertAll(curves, p => p.ObjectId);
        }

        public AcadObjectGroup(ObjectId[] ids)
        {
            Handles = Array.ConvertAll(ids, p => p.Handle.Value);
            _objectIds = ids;
        }

        public override string ToString() => ObjectIds.GetDesc();
    }
}
