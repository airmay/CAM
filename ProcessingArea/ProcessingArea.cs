using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM
{
    /// <summary>
    /// Обрабатываемая область
    /// </summary>
    [Serializable]
    public class ProcessingArea
    {
        public long[] Handles { get; set; }

        [NonSerialized]
        public ObjectId[] AcadObjectIds;

        [NonSerialized]
        public Curve[] Curves;

        public ProcessingArea(Curve[] curves)
        {
            Curves = curves;
            AcadObjectIds = Array.ConvertAll(curves, p => p.ObjectId);
            Handles = Array.ConvertAll(curves, p => p.Handle.Value);
            //Set(curve);
        }

        public ProcessingArea(ObjectId[] ids)
        {
            AcadObjectIds = ids;
            Handles = Array.ConvertAll(ids, p => p.Handle.Value);
            Refresh();
        }

        public void Refresh()
        {
            if (AcadObjectIds == null)
                AcadObjectIds = Array.ConvertAll(Handles, p => Acad.Database.GetObjectId(false, new Handle(p), 0));
            Curves = AcadObjectIds.QOpenForRead<Curve>();
        }

        public override string ToString()
	    {
            var count = Curves.Select(p => p.GetType()).Distinct().Count();
            return $"{(count > 1 ? "Объекты" : Curves[0] is Line ? "Отрезок" : Curves[0] is Arc ? "Дуга" : "Объекты")} ({Curves.Length})";
	    }
	}
}
