using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Autocad;

public static class Curve2dExtensions
{
    public static bool IsBetween(this Point2d pt, Point2d p1, Point2d p2)
    {
        return ((pt - p1).GetNormal() == (p2 - pt).GetNormal());
    }

    public static Point2d To2d(this Point3d pt)
    {
        return (new Point2d(pt.X, pt.Y));
    }

    public static Point2d GetPointAtRelativeParam(this Curve c, double rpm)
    {
        double spm = c.StartParam;
        double epm = c.EndParam;
        return c.GetPointAtParameter(spm + (epm - spm) * rpm).To2d();
    }

    public static Curve2d[] ToCurve2dArray(this Arc a)
    {
        Point2d sp = a.GetPointAtRelativeParam(0.0);
        Point2d mp = a.GetPointAtRelativeParam(0.5);
        Point2d ep = a.GetPointAtRelativeParam(1.0);
        return new Curve2d[2]{
            new CircularArc2d(sp, mp, ep),
            new LineSegment2d(ep, sp)
        };
    }

    public static Curve2d[] ToCurve2dArray(this Circle ci)
    {
        Point2d sp = ci.GetPointAtRelativeParam(0.0);
        Point2d q1 = ci.GetPointAtRelativeParam(0.25);
        Point2d ep = ci.GetPointAtRelativeParam(0.5);
        Point2d q3 = ci.GetPointAtRelativeParam(0.75);
        return new Curve2d[2]{
            new CircularArc2d(sp, q1, ep),
            new CircularArc2d(ep, q3, sp)
        };
    }

    public static Curve2d[] ToCurve2dArray(this Curve c)
    {
        if (c.StartParam != 0.0 || (c.EndParam - Math.Truncate(c.EndParam) != 0.0))
            throw (new ArgumentException("Invalid Curve Parameter"));

        int n = (int)c.EndParam;
        Curve2d[] ca = new Curve2d[n];
        Point2d sp0 = (c.GetPointAtParameter(0.0)).To2d();
        Point2d sp = sp0;
        for (int i = 0; i < n; i++)
        {
            Point2d mp = (c.GetPointAtParameter(i + 0.5)).To2d();
            Point2d ep = (c.GetPointAtParameter(i + 1.0)).To2d();
            if (mp.IsBetween(sp, ep))
                ca[i] = new LineSegment2d(sp, ep);
            else
                ca[i] = new CircularArc2d(sp, mp, ep);
            sp = ep;
        }
        return ca;
    }

    public static CompositeCurve2d ToCompositeCurve2d(this Curve c)
    {
        Curve2d[] ca;
        Arc a = c as Arc;
        if (a != null)
            ca = a.ToCurve2dArray();
        else
        {
            Circle ci = c as Circle;
            if (ci != null)
                ca = ci.ToCurve2dArray();
            else if (c is Line)
                ca = new Curve2d[] { new LineSegment2d(c.StartPoint.To2d(), c.EndPoint.To2d()) };
            else
                ca = c.ToCurve2dArray();
        }
        return new CompositeCurve2d(ca);
    }

    public static bool IsPointInside(this Curve c, Point2d pt)
    {
        int n = 0;
        using (CompositeCurve2d c2d = c.ToCompositeCurve2d())
        using (Ray2d r2d = new Ray2d(pt, Vector2d.XAxis))
        using (CurveCurveIntersector2d cci2d = new CurveCurveIntersector2d(c2d, r2d))
        {
            for (int i = 0; i < cci2d.NumberOfIntersectionPoints; i++)
                if (cci2d.IsTransversal(i)) n++;
        }
        return (n % 2 != 0);
    }

    public static Point2d[] IntersectWith(this Curve2d curve, Curve2d intersectCurve)
    {
        using (CurveCurveIntersector2d intersector = new CurveCurveIntersector2d(curve, intersectCurve))
        {
            return Enumerable.Range(0, intersector.NumberOfIntersectionPoints).Select(i => intersector.GetIntersectionPoint(i)).ToArray();
        }
    }
}