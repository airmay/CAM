using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [TechProcess(TechProcessType.SectionProfile)]
    public class SectionProfileTechProcess : TechProcess
    {
        public AcadObject Rail { get; set; }
        public double? Length { get; set; }
        public int CuttingFeed { get; set; }
        public bool IsNormal { get; set; }
        public double Step { get; set; }
        public double Departure { get; set; }
        public double Delta { get; set; }

        public SectionProfileTechProcess(string caption) : base(caption)
        {
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.ScemaLogic)
                .AddMaterial()
                .AddTool()
                .AddParam(nameof(Frequency))
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(PenetrationFeed))
                .AddIndent()
                .AddAcadObject(nameof(ProcessingArea), "Профиль")
                .AddAcadObject(nameof(Rail), "Направляющая")
                .AddParam(nameof(Length), "Длина направляющей")
                .AddIndent()
                .AddParam(nameof(ZSafety));
        }

        protected override void BuildProcessing(CommandGeneratorBase generator)
        {
            return;

            var rail = Rail != null ? Rail.GetCurve() as Line : new Line(Point3d.Origin, new Point3d(Length.Value, 0, 0));
            var startRail = rail.StartPoint;
            var railVector = rail.Delta.GetNormal();
            if (rail.Angle >= Math.PI)
            {
                startRail = rail.EndPoint;
                railVector = railVector.Negate();
            }
            var startPass = startRail - railVector * Departure;
            var passVector = railVector * (rail.Length + 2 * Departure);
            var railAngle = railVector.GetAngleTo(Vector3d.XAxis);
            if (Rail == null)
                rail.Dispose();

            var profile = ProcessingArea[0].GetCurve();
            var profileLength = profile.Length();
            startPass = new Point3d(startPass.X, startPass.Y - profile.StartPoint.X, profile.StartPoint.Y);
            generator.ZSafety += profile.StartPoint.Y;

            double dist = 0;
            var engineSide = Side.None;
            double angleC;
            do
            {
                var point = profile.GetPointAtDist(dist);

                var distAvg = dist + Tool.Thickness.Value / 2;
                if (distAvg > profileLength)
                    distAvg = profileLength;
                var pointAvg = profile.GetPointAtDist(distAvg);

                var tangent = profile.GetFirstDerivative(pointAvg);
                var angleA = Math.Abs(tangent.GetAngleTo(Vector3d.XAxis).ToDeg());
                var side = tangent.Y < 0 ? Side.Left : Side.Right;
                if (engineSide != side)
                {
                    engineSide = side;
                    angleC = BuilderUtils.CalcToolAngle(railAngle, side);
                    if (!generator.IsUpperTool)
                        generator.Uplifting();
                    var sp = GetStartPoint(point, tangent);
                    generator.Move(sp.X, sp.Y, angleC: angleC, angleA: angleA);
                }
                var startPoint = GetStartPoint(point, tangent);
                generator.Cutting(startPoint, startPoint + passVector, CuttingFeed, PenetrationFeed, angleA: angleA);
                dist += Step;
            }
            while (dist < profileLength);

            generator.Uplifting();

            Point3d GetStartPoint(Point3d point, Vector3d tangent)
            {
                if (engineSide == Side.Left)
                    point += tangent.GetNormal() * Tool.Thickness.Value;
                var profileVector = point - profile.StartPoint;
                return startPass + railVector.RotateBy(Math.PI / 2, -Vector3d.ZAxis).RotateBy(profileVector.GetAngleTo(Vector3d.XAxis), railVector) * profileVector.Length;
            }
        }
    }
}
