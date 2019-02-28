using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    public class ObjectId
    {
        public string Key { get; set; }

        public ObjectId(string key)
        {
            Key = key;
        }
    }

    public struct Point3d
    {
        public Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3d Origin { get; } = new Point3d(0, 0, 0);
        public double X { get; }
        public double Y { get; }
        public double Z { get; set; }

        public static bool operator ==(Point3d p1, Point3d p2) => p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;

        public static bool operator !=(Point3d p1, Point3d p2) => !(p1 == p2);
    }

    public abstract class Curve
    {
        public ObjectId ObjectId { get; set; }

        public Point3d EndPoint = new Point3d(2, 2, 0);

        public double Length { get; }

        public Point3d StartPoint = new Point3d(1, 1, 0);

        public abstract Curve GetOffsetCurves(double offset = 0, double z = 0);

		public Point3d this[Corner corner]
		{
			get => corner == Corner.Start ? StartPoint : EndPoint;
			set
			{
				if (corner == Corner.Start)
					StartPoint = value;
				else
					EndPoint = value;
			}
		}
    }

	public static class CurveExt
	{
		public static Point3d GetPoint(this Curve curve, Corner corner) => corner == Corner.Start ? curve.StartPoint : curve.EndPoint;

		public static void SetPoint(this Curve curve, Corner corner, Point3d point)
		{
			if (corner == Corner.Start)
				curve.StartPoint = point;
			else
				curve.EndPoint = point;
		}
	}

    public class Line : Curve
    {
        public Line(string key)
        {
            ObjectId = new ObjectId(key);
        }

        public Line(Point3d pointer1, Point3d pointer2)
        {
            StartPoint = pointer1;
            EndPoint = pointer2;
        }
        public double Angle { get; }

        public override Curve GetOffsetCurves(double offset = 0, double dz = 0)
        {
            var line = new Line(StartPoint, EndPoint);
            line.StartPoint.Z += dz;
            line.EndPoint.Z += dz;
            return line;
        }

	    public Point3d GetPointAtDist(double startIndent)
	    {
		    throw new NotImplementedException();
	    }
    }

    public class Arc : Curve
    {
        public Arc(string key)
        {
            ObjectId = new ObjectId(key);
        }

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

        public override Curve GetOffsetCurves(double offset = 0, double dz = 0)
        {
            var arc = new Arc(Center, Radius, StartAngle, EndAngle);
            arc.StartPoint.Z += dz;
            arc.EndPoint.Z += dz;
            return arc;
        }
    }
}
