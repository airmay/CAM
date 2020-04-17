using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;

namespace CAM.Disk3D
{
    [Serializable]
    [TechOperation(1, TechProcessNames.Disk3D, "Гребенка")]
    public class CombTechOperation : TechOperationBase
    {
        public double StepPass { get; set; }

        public double StartPass { get; set; }

        public double Penetration { get; set; }

        public double Delta { get; set; }

        public double StepLong { get; set; }

        public int CuttingFeed { get; set; }

        public CombTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            var boundsModel = new Extents3d();
            var surfaces = new List<DbSurface>();
            DbSurface unionSurface = null;
            foreach (var dBObject in TechProcess.ProcessingArea.Select(p => p.ObjectId.QOpenForRead()))
            {
                var surface = dBObject as DbSurface;
                if (dBObject is Region region)
                {
                    var ex = region.GeometricExtents;
                    if (ex.MaxPoint.Z < Consts.Epsilon)
                        continue;

                    surface = new PlaneSurface();
                    ((PlaneSurface)surface).CreateFromRegion(region);
                }
                if (surface == null)
                    throw new Exception($"Объект типа {dBObject.GetType()} не может быть обработан (1)");

                var us = unionSurface == null ? surface : unionSurface.BooleanUnion(surface);
                if (us != null)
                    unionSurface = us;
                //               var s = DbSurface.CreateOffsetSurface(surface, 2, true);
                //s.AddToCurrentSpace();
                boundsModel.AddExtents(surface.GeometricExtents);
                surfaces.Add(surface);
            }
            var s = DbSurface.CreateOffsetSurface(unionSurface, 2) as DbSurface;

            var ray = new Ray { UnitDir = Vector3d.XAxis };
            var y = StartPass;
            do
            {
                ray.BasePoint = new Point3d(0, y, 0);
                var curves = s.ProjectOnToSurface(ray, Vector3d.ZAxis);
                curves[0].ColorIndex = 2;
                curves[0].AddToCurrentSpace();
                y += StepPass;
            }
            while (y < boundsModel.MaxPoint.Y);

        }


                    /*
                    var surfaceDict = new Dictionary<DbSurface, Curve>();
                    var projectCurves = new List<Curve>();
                    var vertLines = new List<Line>();

                    var ray = new Ray
                    {
                        UnitDir = Vector3d.YAxis,
                        BasePoint = new Point3d(boundsModel.MinPoint.X + (boundsModel.MaxPoint.X - boundsModel.MinPoint.X) / 2, 0, 0)
                    };
                    foreach (var surface in surfaces)
                    { 
                        var curves = surface.ProjectOnToSurface(ray, Vector3d.ZAxis);
                        if (curves.Length > 1)
                            throw new Exception($"Объект не может быть обработан (2)");
                        if (curves.Length == 0)
                            continue;
                        if (curves[0] is Line l && l.StartPoint.Z.Round(6) == 0 && l.EndPoint.Z.Round(6) == 0)
                            continue;
                        if (curves[0] is Line line && line.StartPoint.Y.Round(6) == line.EndPoint.Y.Round(6))
                            vertLines.Add(line);
                        else
                            projectCurves.Add((Curve)curves[0]);
                    }
                    var plines = new List<Polyline>();
                    while (projectCurves.Any())
                    {
                        var point = projectCurves.SelectMany(p => p.GetStartEndPoints()).OrderBy(p => p.Y).First();
                        var ptc = new Point3dCollection();

                        //var pline = new Polyline();
                        //var i = 0;
                        while (projectCurves.SingleOrDefault(p => p.HasPoint(point)) is Curve curve)
                        {
                            if (curve is Arc arc)
                            {
                                int divs = (int)(arc.Length / 5 + 10);
                                for (int i = 0; i < divs; i++)
                                    ptc.Add(arc.GetPointAtParameter(arc.StartParam + arc.TotalAngle * (point == arc.StartPoint ? i : divs - i) / divs));
                                    //ptc.Add(arc.GetPointAtParameter(arc.StartParam + arc.TotalAngle * (point == arc.StartPoint ? i : divs - i) / divs));
                            }
                            else
                                ptc.Add(point);

                            //var bulge = curve is Arc arc? Algorithms.GetArcBulge(arc, point) : 0;
                            //pline.AddVertexAt(i++, new Point2d(point.Y, point.Z), bulge, 0, 0);
                            point = curve.NextPoint(point);
                            projectCurves.Remove(curve);
                        }
                        ptc.Add(point);

                        for (int i = 1; i < ptc.Count; i++)
                        {
                            var ll = NoDraw.Line(ptc[i - 1], ptc[i]);
                            ll.ColorIndex = 6;
                            ll.AddToCurrentSpace();
                            //Draw.Line(new Point3d(ptc[i - 1].Y, ptc[i - 1].Z, 0), new Point3d(ptc[i].Y, ptc[i].Z, 0));
                        }
                        //return;

                        //pline.AddVertexAt(i, new Point2d(point.Y, point.Z), 0, 0, 0);
                        //pline.ColorIndex = 6;
                        //pline.AddToCurrentSpace();

                        //var pline1 = (Curve)pline.GetOffsetCurvesGivenPlaneNormal(Vector3d.ZAxis, -2)[0];
                        //pline1.ColorIndex = 0;

                        //var seg = pline.GetArcSegmentAt(0);
                        //var pts = pline.GetPolylineFitPoints().ToArray();
                        //var ptc = new Point3dCollection(pts);
                        var pl = new PolylineCurve3d(ptc);
                        //pl.SetControlPointAt(2, pl.EndPoint + Vector3d.XAxis * 5);


                        var cc = pl.GetTrimmedOffset(2, Vector3d.XAxis, OffsetCurveExtensionType.Fillet);
                        if (cc[0] is CompositeCurve3d cc3d)
                            cc = cc3d.GetCurves();

                            var pline1 = new Polyline();
                            //int i = 0;
                            foreach (var item in cc)
                            {
                                Curve curve = null;
                                if (item is LineSegment3d ls)
                                    curve = NoDraw.Line(ls.StartPoint, ls.EndPoint);
                                if (item is CircularArc3d ca)
                                {
                                    //System.Diagnostics.Debug.WriteLine(ca.Center.Z + " : " + ca.StartPoint.Z);
                                    var angle = (ca.Center - ca.StartPoint).GetAngleTo(Vector3d.YAxis);
                                    curve = new Arc(ca.Center, ca.Normal, ca.Radius, angle, angle + ca.EndAngle);
                                    //arc.StartAngle = (arc.Center - arc.StartPoint).GetAngleTo(Vector3d.YAxis);
                                    //arc.EndAngle = arc.StartAngle + ca.EndAngle;
                                    //curve = arc;
                                }
                                //var bulge = curve is Arc arc ? Algorithms.GetArcBulge(arc, item.StartPoint) : 0;
                                //pline1.AddVertexAt(i++, item.StartPoint.ToPoint2d(), bulge, 0, 0);
                                curve.ColorIndex = item is LineSegment3d ? 3 : 6;
                                curve.AddToCurrentSpace();
                            }
                            //pline1.AddVertexAt(i, new Point2d(cc.Last().EndPoint.Y, cc.Last().EndPoint.Z), 0, 0, 0);
                            //pline1.ColorIndex = 3;
                            //pline1.AddToCurrentSpace();

                            //foreach (var item in cc)
                            //{
                            //    if (item is LineSegment3d ls)
                            //        Draw.Line(ls.StartPoint, ls.EndPoint);
                            //    if (item is CircularArc3d ca)
                            //        Draw.ArcFromGeometry(ca);
                            //}
                            //cc.ForEach(p => Draw.Line(p.StartPoint, p.EndPoint));

                            //var ln = NoDraw.Line(new Point3d(3.5, 0, 0), new Point3d(3.5, 1000, 0));

                            //ptc = new Point3dCollection();
                            //ln.IntersectWith(pline1, Intersect.OnBothOperands, ptc, IntPtr.Zero, IntPtr.Zero);
                            //if (ptc.Count > 0)
                            //{
                            //    Draw.Circle(ptc[0], 1);
                            //    ln.AddToCurrentSpace();
                            //}
                        }
                        //DBObjectCollection col = new DBObjectCollection();
                        //((Polyline)pline1).Explode(col);
                        //var ar = col[0] as Arc;
                        //pline1.AddToCurrentSpace();

                        //PolylineCurve2d pp;
                        //pp.GetTrimmedOffset

                    var posY = StartPass - StepPass;
                    surface = null;
                    ray.UnitDir = Vector3d.XAxis;
                    ray.BasePoint = new Point3d(StartPass, 0, 0);
                    double thick = TechProcess.Tool.Thickness.Value;
                    double? changeFlagPosition = null;
                    Curve profile = null;
                    double endProfile = 0;
                    while ((posY += StepPass) < boundsModel.MaxPoint.Y)
                    {
                        ray.BasePoint = new Point3d(0, posY, 0);
                        if (surface == null)
                        {
                            GetSurface();
                            if (surface == null)
                                continue;
                        }
                        var line = GetProjectCurve();
                        if (line == null)
                            continue;

                       // var z = line.StartPoint.Z + Delta / Math.Cos();
                        if (profile is Arc arc && arc.EndPoint.Y > posY && arc.Center.Y > posY && arc.Center.Y < posY + thick)
                        {
                            ray.BasePoint = new Point3d(0, arc.Center.Y, 0);
                            //z = GetProjectCurve().StartPoint.Z;
                        }
                        if (posY + thick < endProfile)
                        {
                            ray.BasePoint = new Point3d(0, posY + thick, 0);
                            var z2 = GetProjectCurve().StartPoint.Z;
                        }
                        else
                        {
                            ray.BasePoint = new Point3d(0, endProfile, 0);
                            var z3 = GetProjectCurve().StartPoint.Z;
                            ray.BasePoint = new Point3d(0, posY + thick, 0);
                            GetSurface();
                            if (surface != null)
                            {
                                //var z3 = GetProjectCurve().StartPoint.Z;

                            }
                        }
                            if (generator.IsUpperTool)
                            generator.Move(line.StartPoint.X, line.StartPoint.Y, angleC: 0);

                        generator.Cutting(line, CuttingFeed, 200);
                    }
                    ray.Dispose();

                    void GetSurface()
                    {
                        surface = surfaceDict.Keys.FirstOrDefault(p => p.ProjectOnToSurface(ray, Vector3d.ZAxis).Length == 1);
                        if (surface != null)
                        {
                            profile = surfaceDict[surface];
                            endProfile = profile.GetStartEndPoints().Max(p => p.Y);
                        }
                    }

                    Line GetProjectCurve()
                    {
                        var curves = surface.ProjectOnToSurface(ray, Vector3d.ZAxis);
                        if (curves.Length == 0)
                        {
                            GetSurface();
                            if (surface == null)
                                return null;
                            curves = surface.ProjectOnToSurface(ray, Vector3d.ZAxis);
                        }
                        var line = curves[0] as Line;
                        if (line == null)
                            throw new Exception($"Объект не может быть обработан (5)");
                        return line;
                    }

                    double GetZ(double y1, double y2)
                    {
                        if (profile is Line line)
                        {
                            var dy = line.Delta.Z < 0 ? y1 : y2 - line.StartPoint.Y;
                            return (dy * line.Delta.Z + Delta * line.Length) / line.Delta.Y;
                        }
                        if (profile is Arc arc)
                        {
                            if (arc.EndPoint.Y > arc.StartPoint.Y && arc.Center.Y >= y1 && arc.Center.Y <= y2)
                                return arc.Center.Z + arc.Radius + Delta;
                            var y0 = arc.StartPoint.Y < arc.Center.Y ^ arc.StartPoint.Z < arc.Center.Z ? y2 : y1;
                            var z = Math.Sqrt(arc.Radius * arc.Radius - (arc.Center.Y - y0) * (arc.Center.Y - y0));
                            return arc.Center.Z + (arc.EndPoint.Y > arc.StartPoint.Y ? 1 : -1) * z + Delta * arc.Radius / z;
                        }
                        throw new Exception($"Объект не может быть обработан (6)");
                    }
                }
                    //curve.AddToCurrentSpace();

                    //var l1 = (Curve)line.GetOffsetCurves(100)[0];
                    //l1.Extend(-2);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(200)[0];
                    //l1.Extend(-1);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(300)[0];
                    //l1.Extend(-0.5);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(400)[0];
                    //l1.Extend(0);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(500)[0];
                    //l1.Extend(0.5);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(600)[0];
                    //l1.Extend(1);
                    //l1.AddToCurrentSpace();

                    //l1 = (Curve)line.GetOffsetCurves(700)[0];
                    //l1.Extend(2);
                    //l1.AddToCurrentSpace();
                }
            */
    }
}
