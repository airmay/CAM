using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    [Serializable]
    public class AcadObject
    {
        public long Handle { get; set; }

        [NonSerialized]
        private ObjectId _objectId;

        public ObjectId ObjectId => _objectId == ObjectId.Null ? (_objectId = Acad.Database.GetObjectId(false, new Handle(Handle), 0)) : _objectId;

        public Curve GetCurve() => ObjectId.QOpenForRead<Curve>();

        public AcadObject(ObjectId id)
        {
            Handle = id.Handle.Value;
            _objectId = id;
        }

        public override string ToString() => ObjectId.GetDesc();
    }
}
