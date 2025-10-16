using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System.Collections.Generic;

namespace CAM.Core
{
    [Serializable]
    public class Origin
    {
        public AcadObject OriginModel { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Point2d Point => new(X, Y);
        public string Description => $"{{{X.Round()}, {Y.Round()}}}";
        public override string ToString() => $"{{{X.Round()}, {Y.Round()}}}";

        public void CreateOriginModel()
        {
            Interaction.SetActiveDocFocus();
            var point = Interaction.GetPoint("\nВыберите точку начала координат");
            if (point.IsNull())
                return;

            X = point.X;
            Y = point.Y;

            Acad.Delete(OriginModel?.ObjectIds);
            point = new Point3d(X, Y, 0);
            const int length = 100;
            const int s = length / 10;
            var v = new Vector3d(s, s, 0);
            var curves = new List<Curve>
            {
                NoDraw.Line(point, point + Vector3d.XAxis * length),
                NoDraw.Line(point, point + Vector3d.YAxis * length),
                NoDraw.Rectang(point - v, point + v)
            };
            OriginModel = AcadObject.Create(curves.AddToDatabase());
        }
    }
}
