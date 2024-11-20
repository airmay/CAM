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
        public AcadObject OriginObject { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Point2d Point => new Point2d(X, Y);

        public override string ToString() => $"{{{X.Round(3)}, {Y.Round(3)}}}";

        public void CreateOriginObject()
        {
            X = Y = 0;
            Acad.DeleteObjects(OriginObject?.ObjectIds);
            Interaction.SetActiveDocFocus();
            var point = Interaction.GetPoint("\nВыберите точку начала координат");
            if (point.IsNull()) 
                return;

            X = point.X;
            Y = point.Y;
            point = point.WithZ(0);
            const int length = 100;
            const int s = length / 10;
            var curves = new List<Curve>
            {
                NoDraw.Line(point, point + Vector3d.XAxis * length),
                NoDraw.Line(point, point + Vector3d.YAxis * length),
                NoDraw.Rectang(point.WithXY(point.X - s, point.Y - s), point.WithXY(point.X + s, point.Y + s))
            };
            OriginObject = AcadObject.Create(curves.Add());
        }
    }
}
