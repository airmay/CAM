using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    [MenuItem("Паз", 2)]
    public class PazTechOperation : MillingTechOperation<StolbTechProcess>
    {
        public string ZList { get; set; }

        public PazTechOperation(StolbTechProcess techProcess, string caption) : base(techProcess, caption) { }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(displayName: "Верхняя грань паза")
                .AddParam(nameof(ZList), "Координаты Z проходов");
        }

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            generator.AC = TechProcess.AC;
            generator.DiskRadius = TechProcess.Tool.Diameter / 2;
            generator.CuttingFeed = TechProcess.CuttingFeed;
            generator.SmallFeed = TechProcess.PenetrationFeed;

            var line = (Line)ProcessingArea.GetCurve();
            var sign = line.IsTurnRight(Point3d.Origin) ? -1 : 1;
            generator.EngineSide = line.IsTurnRight(Point3d.Origin) ? Side.Right : Side.Left;

            var start = line.StartPoint.GetExtendedPoint(line.EndPoint, TechProcess.Departure);
            var end = line.EndPoint.GetExtendedPoint(line.StartPoint, TechProcess.Departure);
            line = NoDraw.Line(start, end);

            var list = ZList.Split(new char[] { ',', ';' }).Select(p => double.Parse(p));
            foreach (var z in list)
            {
                var s = 0D;
                do
                {
                    s += TechProcess.PenetrationStep.Value;
                    if (s > TechProcess.Depth)
                        s = TechProcess.Depth;
                    generator.Cutting(line, s * sign, -z, angleA: 90);

                }
                while (s < TechProcess.Depth);
            }
        }
    }
}
