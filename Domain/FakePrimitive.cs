using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    public class ObjectId {}

    public struct Point3d
    {
        public Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
    }

    public class Curve
    {
        public ObjectId ObjectId { get; }
        
        public Point3d EndPoint { get; set; }

        public double Length { get; }

        public Point3d StartPoint { get; set; }
    }

    public class Line : Curve
    {
        public Line(Point3d pointer1, Point3d pointer2)
        {
            StartPoint = pointer1;
            EndPoint = pointer2;
        }
        public double Angle { get; }
}

    public class Arc : Curve
    {
        public Arc(Point3d center, double radius, double startAngle, double endAngle)
        {
            Center = center;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = EndAngle;
        }

        public Point3d Center { get; set; }

        public double Radius { get; set; }

        public double EndAngle { get; set; }

        public double StartAngle { get; set; }
    }
}
