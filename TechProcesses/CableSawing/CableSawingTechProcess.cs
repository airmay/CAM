using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    [MenuItem("Распиловка тросом", 8)]
    public class CableSawingTechProcess : TechProcess
    {
        public int S { get; set; } = 60;

        public CableSawingTechProcess()
        {
            MachineType = CAM.MachineType.CableSawing;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области", AcadObjectNames.Region)
                .AddIndent()
                .AddOrigin()
                .AddParam(nameof(S), "Угловая скорость")
                .AddParam(nameof(ZSafety));
        }

        protected override void BuildProcessing(CommandGeneratorBase generator)
        {
            ///generator.ZSafety = ZSafety;
            //generator.SetTool(1, Frequency, hasTool: false);

            var regions = ProcessingArea.Select(p => p.ObjectId.QOpenForRead());

            foreach (Region region in regions)
            {
                var z1 = region.Bounds.Value.MinPoint.Z;
                var z2 = region.Bounds.Value.MaxPoint.Z;

                var collection = new DBObjectCollection();
                region.Explode(collection);
                var lines = collection.Cast<Line>().Where(p => Math.Abs(p.StartPoint.Z - p.EndPoint.Z) < 1).OrderBy(p => p.StartPoint.Z).ToList();
                if (lines.Count != 2)
                    throw new Exception("Задана некорректная область");

                var line1 = new Line(lines[0].StartPoint.ToPoint2d().ToPoint3d(), lines[0].EndPoint.ToPoint2d().ToPoint3d());
                var origin = new Point3d(OriginX, OriginY, 0);
                var pNearest = line1.GetClosestPointTo(origin, true);
                var vector = (pNearest - origin).ToVector2d();
                var u1 = vector.Length;
                var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg(2);

                var line2 = new Line2d(lines[1].StartPoint.ToPoint2d(), lines[1].EndPoint.ToPoint2d());
                var u2 = line2.GetDistanceTo(origin.ToPoint2d());

                var u3 = u2 + ZSafety * (u2 - u1) / (z2 - z1);
                var z3 = z2 + ZSafety;

                generator.Command($"G05 A{angle} S{S}");
                generator.Command($"G0 U{u3.Round(4)} V{z3.Round(4)}");
                generator.Command($"G01 U{u1.Round(4)} V{z1.Round(4)}");
                generator.Command($"G0 U{u3.Round(4)} V{z3.Round(4)}");
            }
        }
    }
 }
