using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [TechProcess(6, TechProcessNames.SectionProfile)]
    public class SectionProfileTechProcess : TechProcessBase
    {
        public AcadObject Rail { get; set; }
        public int CuttingFeed { get; set; }
        public bool IsNormal { get; set; }
        public double Step { get; set; }
        public double Departure { get; set; }
        public double Delta { get; set; }

        public SectionProfileTechProcess(string caption) : base(caption)
        {
            MachineType = CAM.MachineType.Donatoni;
        }

        protected override void BuildProcessing(ICommandGenerator generator)
        {
            var rail = Rail.GetCurve() as Line;
            var startRail = rail.GetClosestPoint(Point3d.Origin);
            var railVector = (rail.NextPoint(startRail) - startRail).GetNormal();
            var startPass = startRail - railVector * Departure;
            var passVector = railVector * (rail.Length + 2 * Departure);
            var railAngle = railVector.GetAngleTo(Vector3d.XAxis);

            var profile = ProcessingArea[0].GetCurve();
            var profileLength = profile.Length();
            double dist = 0;
            var engineSide = Side.None;
            double angleC;
            do
            {
                var point = profile.GetPointAtDist(dist);
                var dist2 = dist + Tool.Thickness.Value;
                if (dist2 > profileLength)
                    dist2 = profileLength;
                var point2 = profile.GetPointAtDist(dist2);
                var distAvg = (dist2 + dist) / 2;
                var pointAvg = profile.GetPointAtDist(distAvg);

                var profileVector = point - profile.StartPoint;
                var startPoint1 = startPass + railVector.RotateBy(Math.PI / 2, -Vector3d.ZAxis).RotateBy(profileVector.GetAngleTo(Vector3d.XAxis), railVector) * profileVector.Length;

                profileVector = point2 - profile.StartPoint;
                var startPoint2 = startPass + railVector.RotateBy(Math.PI / 2, -Vector3d.ZAxis).RotateBy(profileVector.GetAngleTo(Vector3d.XAxis), railVector) * profileVector.Length;

                var tangent = profile.GetTangent(pointAvg);
                var angleA = Math.Abs(tangent.GetAngleTo(Vector2d.XAxis).ToDeg());

                var side = tangent.Y < 0 ? Side.Left : Side.Right;
                if (engineSide != side)
                {
                    engineSide = side;
                    angleC = BuilderUtils.CalcToolAngle(railAngle, side);
                    if (!generator.IsUpperTool)
                        generator.Uplifting();
                    var sp = engineSide == Side.Left ? startPoint2 : startPoint1;
                    generator.Move(sp.X, sp.Y, angleC: angleC, angleA: angleA);
                }
                var startPoint = engineSide == Side.Left ? startPoint2 : startPoint1;
                generator.Cutting(startPoint, startPoint + passVector, CuttingFeed, PenetrationFeed, angleA: angleA);
                dist += Step;
            }
            while (dist < profileLength);

            generator.Uplifting();

            //Point3d GetStartPoint()
            //{

            //}
        }
    }
}
