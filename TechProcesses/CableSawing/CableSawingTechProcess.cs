using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.CableSawing
{
    [Serializable]
    [MenuItem("Распиловка тросом", 8)]
    public class CableSawingTechProcess : CableTechProcess
    {
        public int S { get; set; } = 60;

        public int ToolThickness { get; set; } = 60;

        public int TransitionFeed { get; set; } = 300;

        public double Delta { get; set; }

        public CableSawingTechProcess()
        {
            MachineType = CAM.MachineType.CableSawing;
            Tool = new Tool { Type = ToolType.Cable, Diameter = ToolThickness };
            PenetrationFeed = 10;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области", AcadObjectNames.Region)
                .AddOrigin()
                .AddIndent()
                .AddParam(nameof(PenetrationFeed))
                .AddParam(nameof(TransitionFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddParam(nameof(ZSafety))
                .AddIndent()
                .AddParam(nameof(ToolThickness), "Толщина троса")
                .AddParam(nameof(Delta));
        }

        protected override void BuildProcessing(CableCommandGenerator generator)
        {
            Tool.Thickness = ToolThickness;
            ///generator.ZSafety = ZSafety;
            //generator.SetTool(1, Frequency, hasTool: false);

            var origin = new Point3d(OriginX, OriginY, 0);
            generator.CenterToolPosition = origin;
            generator.Command($"G92");

            var regions = ProcessingArea.Select(p => p.ObjectId.QOpenForRead());

            foreach (Region region in regions)
            {
                var z1 = region.Bounds.Value.MinPoint.Z;
                var z2 = region.Bounds.Value.MaxPoint.Z;

                var collection = new DBObjectCollection();
                region.Explode(collection);
                var ofsset = region.Normal * (ToolThickness / 2 + Delta);
                var lines = collection.Cast<Line>()
                    .Where(p => Math.Abs(p.StartPoint.Z - p.EndPoint.Z) < 1)
                    .Select(p => p.GetStartEndPoints().Select(pt => pt + ofsset).ToArray())
                    .OrderBy(p => p[0].Z)
                    .ToList();
                if (lines.Count != 2)
                    throw new Exception("Задана некорректная область");

                var line1 = new Line(lines[0][0].ToPoint2d().ToPoint3d(), lines[0][1].ToPoint2d().ToPoint3d());
                var pNearest = line1.GetClosestPointTo(origin, true);
                var vector = (pNearest - origin).ToVector2d();
                var u1 = vector.Length;
                var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg(2);

                var line2 = new Line2d(lines[1][0].ToPoint2d(), lines[1][1].ToPoint2d());
                var u2 = line2.GetDistanceTo(origin.ToPoint2d());

                var u3 = u2 + ZSafety * (u2 - u1) / (z2 - z1);
                var z3 = z2 + ZSafety;           
                
                generator.AngleToolPosition= angle;
                generator.Command($"G05 A{angle} S{S}");

                generator.PointToolPosition = new Point3d(OriginX - u3, OriginY, z3);
                generator.Command($"G00 U{u3.Round(4)} V{z3.Round(4)}");

                generator.Command($"M03", "Включение");

                generator.PointToolPosition = new Point3d(OriginX - u1, OriginY, z1);
                generator.Command($"G01 U{u1.Round(4)} V{z1.Round(4)} F{PenetrationFeed}");

                generator.Command($"G04 P60", "Задержка");

                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");

                generator.PointToolPosition = new Point3d(OriginX - u3, OriginY, z3);
                generator.Command($"G01 U{u3.Round(4)} V{z3.Round(4)} F{TransitionFeed}");

            }
        }
    }
 }
