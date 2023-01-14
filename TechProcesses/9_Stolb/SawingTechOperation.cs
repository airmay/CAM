using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.Stolb
{
    [Serializable]
    [MenuItem("Распиловка", 1)]
    public class SawingTechOperation : MillingTechOperation<StolbTechProcess>
    {
        public SawingTechOperation(StolbTechProcess techProcess, string caption) : base(techProcess, caption)
        {
            IsSupressUplifting = true;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(displayName: "Грань пирамиды", allowedTypes: $"{AcadObjectNames.Surface},{AcadObjectNames.Region}");
        }

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            var region = ProcessingArea.ObjectId.QOpenForRead<Region>();
            var normal = region.Normal;
            var curves = new DBObjectCollection();
            region.Explode(curves);
            var line = curves.Cast<Curve>().Select(p => new { Z = p.StartPoint.Z + p.EndPoint.Z, Curve = p }).OrderBy(p => p.Z).First().Curve as Line;
            var maxPoint = curves.Cast<Curve>().SelectMany(p => p.GetStartEndPoints()).OrderBy(x => x.Z).Last();

            generator.Tool = TechProcess.Tool;
            generator.DZ = TechProcess.DZ;
            generator.CuttingFeed = TechProcess.CuttingFeed;
            generator.SmallFeed = TechProcess.PenetrationFeed;

            var sign = line.IsTurnRight(maxPoint) ? -1 : 1;
            generator.EngineSide = line.IsTurnRight(Point3d.Origin) ? Side.Left : Side.Right;
            var angleC = BuilderUtils.CalcToolAngle(line, line.StartPoint, generator.EngineSide);
            var angle = normal.GetAngleTo(Vector3d.ZAxis);
            var angleA = 90 - angle.ToDeg();

            var comp = normal * TechProcess.Tool.Thickness.Value;
            var start = line.StartPoint.GetExtendedPoint(line.EndPoint, TechProcess.Departure) + comp;
            var end = line.EndPoint.GetExtendedPoint(line.StartPoint, TechProcess.Departure) + comp;

            var point = generator.ToolPosition.IsDefined && generator.ToolPosition.Point.DistanceTo(end) < generator.ToolPosition.Point.DistanceTo(start) ? end : start;
            var vector = line.Delta.GetNormal().RotateBy(sign * Math.PI / 2, normal);
            var lSafety = (maxPoint.Z + TechProcess.ZSafety - line.StartPoint.Z) / Math.Sin(angle);
            Move(generator, point + vector * lSafety, angleC, angleA);

            var l = (maxPoint.Z - line.StartPoint.Z) / Math.Sin(angle);
            generator.GCommand(CommandNames.Penetration, 1, point: point + vector * (l - TechProcess.PenetrationStep), feed: TechProcess.PenetrationFeed);

            do
            {
                var feed = TechProcess.CuttingFeed;
                l -= TechProcess.PenetrationStep;
                if (l <= 0 && TechProcess.LastStep > TechProcess.PenetrationStep)
                {
                    l -= TechProcess.LastStep - TechProcess.PenetrationStep;
                    feed = TechProcess.LastFeed;
                }
                line = NoDraw.Line(start + vector * l, end + vector * l);
                generator.Cutting(line, feed, generator.SmallFeed);
            }
            while (l > 0);

            generator.Uplifting(vector * (lSafety - l));
        }

        public void Move(MillingCommandGenerator generator, Point3d point, double angleC, double angleA)
        {
            if (!generator._isEngineStarted)
            {
                generator.GCommand("Наклон", 1, angleA: angleA, feed: 500);
                generator.GCommand("Поворот", 1, angleC: angleC);
                generator.GCommand(CommandNames.InitialMove, 0, x: point.X, y: point.Y);
                generator.GCommand(CommandNames.Descent, 0, z: point.Z);
                generator.StartEngineCommands();
                generator._isEngineStarted = true;
            }
            else
            {
                generator.GCommand("Поворот", 1, angleC: angleC);
                generator.GCommand(CommandNames.Fast, 0, x: point.X, y: point.Y);
            }
        }
    }
}
    