using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [TechOperation(1, TechProcessNames.SectionProfile, "Гребенка")]
    public class CombTechOperation : TechOperationBase
    {
        public double StepPass { get; set; }

        public double StartPass { get; set; }

        public double Penetration { get; set; }

        public double Delta { get; set; }

        public int CuttingFeed { get; set; }

        public double Departure { get; set; }

        public double ZMax { get; set; }

        public CombTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            var sectionProfile = (SectionProfileTechProcess)TechProcess;
            var toolThickness = TechProcess.Tool.Thickness.Value;
            generator.SetZSafety(TechProcess.ZSafety, ZMax);

            var rail = sectionProfile.Rail != null ? sectionProfile.Rail.GetCurve() as Line : new Line(Point3d.Origin, new Point3d(sectionProfile.Length.Value, 0, 0));
            var railVector = rail.Delta.GetNormal();
            var passVector = railVector * (rail.Length + 2 * Departure);
            var crossVector = railVector.RotateBy(Math.PI / 2, Vector3d.ZAxis);
            var startPass = rail.StartPoint - railVector * Departure;
            var shift = TechProcess.MachineType == MachineType.Donatoni ^ BuilderUtils.CalcEngineSide(rail.Angle) == Side.Left ? toolThickness : 0;
            if (rail.IsNewObject)
                rail.Dispose();

            var profile = sectionProfile.ProcessingArea[0].GetCurve();
            var profileX = profile.GetStartEndPoints().OrderBy(p => p.X).Select(p => p.X).ToArray();

            using (var curve = profile.ToCompositeCurve2d())
            using (var ray = new Ray2d())
            using (var intersector = new CurveCurveIntersector2d())
            {
                Acad.SetLimitProgressor((int)((profile.EndPoint.X - profile.StartPoint.X) / StepPass));

                for (var x = profileX[1] - StartPass; x > profileX[0] + toolThickness; x -= StepPass)
                {
                    Acad.ReportProgressor();

                    var y = double.NegativeInfinity;
                    for (int i = 0; i < 3; i++)
                    {
                        ray.Set(new Point2d(x - i * toolThickness / 2, ZMax), -Vector2d.YAxis);
                        intersector.Set(curve, ray);
                        if (intersector.NumberOfIntersectionPoints != 1) throw new Exception("Некорректный профиль");
                        y = Math.Max(y, intersector.GetIntersectionPoint(0).Y);

                        //Draw.Circle(new CircularArc2d(new Point2d(x - i * toolThickness / 2, intersector.GetIntersectionPoint(0).Y), 1));
                    }
                    if (y > ZMax) throw new Exception("Высота профиля больше толщины заготовки");
                    //Draw.Line(new Point3d(x, y, 0), new Point3d(x- toolThickness, y, 0));

                    int passCount = (int)Math.Ceiling((ZMax - y) / Penetration);
                    var penetrationCalc = (ZMax - y) / passCount;
                    var zArray = Enumerable.Range(1, passCount).Select(p => ZMax - p * penetrationCalc).ToArray();

                    generator.Cutting(startPass + crossVector * (profileX[1] - x - shift), passVector, zArray, CuttingFeed, sectionProfile.PenetrationFeed);
                    generator.Uplifting();
                }
            }
        }



            //    using (var Ray = new Ray { UnitDir = -Vector3d.YAxis })
            //{
            //    Acad.SetLimitProgressor((int)((profile.EndPoint.X - profile.StartPoint.X) / StepPass));

            //    for (var x = profileX[1] - StartPass; x > profileX[0] + toolThickness; x -= StepPass)
            //    {
            //        Acad.ReportProgressor();

            //        var points = new Point3dCollection();
            //        for (int i = 0; i < 3; i++)
            //        {
            //            Ray.BasePoint = new Point3d(x - i * toolThickness / 2, ZMax, 0);
            //            Ray.IntersectWith(profile, default, points, IntPtr.Zero, IntPtr.Zero);
            //        }
            //        var y = points.Cast<Point3d>().Select(p => p.Y).Max();

            //        if (y > ZMax) throw new Exception("Высота профиля больше толщины заготовки");

            //        int passCount = (int)Math.Ceiling((ZMax - y) / Penetration);
            //        var step = (ZMax - y) / passCount;
            //        var zArray = Enumerable.Range(1, passCount).Select(p => ZMax - p * step).ToArray();

            //        generator.Cutting(startPass + crossVector * (x + shift), passVector, zArray, CuttingFeed, sectionProfile.PenetrationFeed);
            //        generator.Uplifting();
            //    }
            //}
        //}
    }
}
