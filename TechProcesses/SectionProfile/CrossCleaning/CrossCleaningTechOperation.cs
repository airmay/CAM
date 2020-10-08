using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [TechOperation(3, TechProcessNames.SectionProfile, "Поперечная чистка")]
    public class CrossCleaningTechOperation : TechOperationBase
    {

        public double StepX { get; set; }

        public double StepY { get; set; }

        public double Departure { get; set; }

        public int CuttingFeed { get; set; }

        public double Delta { get; set; }

        public CrossCleaningTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            var sectionProfile = (SectionProfileTechProcess)TechProcess;
            var toolThickness = TechProcess.Tool.Thickness.Value;

            var rail = sectionProfile.Rail != null ? sectionProfile.Rail.GetCurve() as Line : new Line(Point3d.Origin, new Point3d(sectionProfile.Length.Value, 0, 0));
            var railVector = rail.Delta.GetNormal();
            var passVector = rail.Delta;
            var crossVector = railVector.RotateBy(Math.PI / 2, Vector3d.ZAxis);
            var startPass = rail.StartPoint;
            var shift = TechProcess.MachineType == MachineType.Donatoni ^ BuilderUtils.CalcEngineSide(rail.Angle) == Side.Left ? toolThickness : 0;
            if (rail.IsNewObject)
                rail.Dispose();

            var profile = sectionProfile.ProcessingArea[0].GetCurve() as Polyline;
            if (profile == null) throw new Exception("Профиль должен быть полилинией");

            if (Delta != 0)
                profile = (Polyline)profile.GetOffsetCurves(Delta)[0];

            var endX = Math.Max(profile.StartPoint.X, profile.EndPoint.X);
            var profilePoints = profile.GetPolylineFitPoints(StepY).Select(p => new Point2d(endX - p.X, p.Y)).ToList();
            if (profilePoints[0].X > Consts.Epsilon)
                profilePoints.Reverse();
            profilePoints = profilePoints
                .TakeWhile(p => p.X < toolThickness)
                .Select(p => p - Vector2d.XAxis * toolThickness)
                .Concat(profilePoints)
                .ToList();
            var passPoints = profilePoints.Select((p, i) =>
            {
                var y = p.Y;
                for (int j = i + 1; j < profilePoints.Count && profilePoints[j].X - profilePoints[i].X < toolThickness; j++)
                    if (profilePoints[j].Y > y)
                        y = profilePoints[j].Y;
                return new Point2d(p.X, y);
            })
            .ToList();

            if (Departure > 0)
            {
                passPoints.Insert(0, new Point2d(-Departure, passPoints.First().Y));
                passPoints.Add(new Point2d(passPoints.Last().X + Departure, passPoints.Last().Y));
            }

            var sp = startPass + crossVector * (passPoints[0].X + shift) + passPoints[0].Y * Vector3d.ZAxis;
            generator.Move(sp.X, sp.Y, angleC: BuilderUtils.CalcToolAngle(railVector.GetAngleTo(Vector3d.XAxis)));
            
            var passPointsDirect = new List<Point2d>[2] { passPoints, Enumerable.Reverse(passPoints).ToList() };
            int direct = 0;

            Acad.SetLimitProgressor((int)(passVector.Length / StepX));
            for (double x = 0; x < passVector.Length; x += StepX)
            {
                Acad.ReportProgressor();
                passPointsDirect[direct].ForEach(p => generator.GCommand(CommandNames.Cutting, 1, point: startPass + crossVector * (p.X + shift) + p.Y * Vector3d.ZAxis + x * railVector, feed: CuttingFeed));
                direct = 1 - direct;
            }
        }
    }
}