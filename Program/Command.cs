using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public class Command
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public ObjectId? Toolpath { get; set; }
        public Operation Operation { get; set; }
        public Point3d Position { get; set; }
        public double AngleC { get; set; }
        public double AngleA { get; set; }
    }
}