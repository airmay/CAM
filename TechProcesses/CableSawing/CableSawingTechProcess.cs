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
        public double ToolThickness { get; set; } = 10;
        public int CuttingFeed { get; set; } = 10;
        public int S { get; set; } = 100;
        public double Departure { get; set; } = 50;
        public double Delta { get; set; } = 0;

        public Point2d Center => new Point2d(OriginX, OriginY);

        public CableSawingTechProcess()
        {
            MachineType = CAM.MachineType.CableSawing;
            Tool = new Tool { Type = ToolType.Cable, Diameter = ToolThickness };
            PenetrationFeed = 10;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddAcadObject(nameof(ProcessingArea), "Объекты", "Выберите обрабатываемые области")
                .AddOrigin()
                .AddIndent()
                .AddParam(nameof(CuttingFeed))
                .AddParam(nameof(S), "Угловая скорость")
                .AddParam(nameof(ZSafety))
                .AddParam(nameof(Departure), "Выезд")
                .AddIndent()
                .AddParam(nameof(ToolThickness), "Толщина троса")
                .AddParam(nameof(Delta));
        }

        protected override void BuildProcessing(CableCommandGenerator generator)
        {
            var extent = ProcessingArea.Select(p => p.ObjectId).GetExtents();
            if (OriginObject == null)
            {
                var center = ProcessingArea.Select(p => p.ObjectId).GetCenter();
                OriginX = center.X.Round(2);
                OriginY = center.Y.Round(2);
                OriginObject = Acad.CreateOriginObject(new Point3d(OriginX, OriginY, 0));
            }
            Tool.Thickness = ToolThickness;

            var origin = new Point3d(OriginX, OriginY, 0);
            generator.CenterPoint = origin;

            var regions = ProcessingArea.Select(p => p.ObjectId.QOpenForRead()).ToList();
            //generator.SetToolPosition(origin, 0, 0, regions[0].Bounds.Value.MaxPoint.Z + ZSafety);
            generator.SetToolPosition(origin, 0, 0, extent.MaxPoint.Z + ZSafety);
            generator.Command($"G92");
            return;


            foreach (Region region in regions)
            {
                var z1 = region.Bounds.Value.MinPoint.Z;
                var z2 = region.Bounds.Value.MaxPoint.Z;

                var collection = new DBObjectCollection();
                region.Explode(collection);
                var ofsset = region.Normal * (ToolThickness / 2 + Delta);
                var lines = collection.Cast<Line>()
                    .Where(p => Math.Abs(p.StartPoint.Z - p.EndPoint.Z) < 1)
                    .Where(p => Math.Abs(z1 - p.StartPoint.Z ) < 1 || Math.Abs(z2 - p.StartPoint.Z) < 1)
                    .OrderByDescending(p => p.Length)
                    .Take(2)
                    .Select(p => p.GetStartEndPoints().Select(pt => pt + ofsset).ToArray())
                    .OrderBy(p => p[0].Z)
                    .ToList();
                //if (lines.Count != 2)
                //    throw new Exception("Задана некорректная область");

                var line1 = new Line(lines[0][0].ToPoint2d().ToPoint3d(), lines[0][1].ToPoint2d().ToPoint3d());
                var pNearest = line1.GetClosestPointTo(origin, true);
                var vector = (pNearest - origin).ToVector2d();
                var u1 = vector.Length;
                var angle = Vector2d.XAxis.Negate().ZeroTo2PiAngleTo(vector).ToDeg(2);
                generator.GCommandAngle(angle, S);

                var line2 = new Line2d(lines[1][0].ToPoint2d(), lines[1][1].ToPoint2d());
                var u2 = line2.GetDistanceTo(origin.ToPoint2d());

                var coeff = (u2 - u1) / (z2 - z1);
                var u3 = u2 + ZSafety * coeff;
                var z3 = z2 + ZSafety;
                generator.GCommand(0, u3, z3);

                generator.Command($"M03", "Включение");

                var u0 = u1 - Departure * coeff;
                var z0 = z1 - Departure;
                generator.GCommand(1, u0, z0, PenetrationFeed);

                generator.Command($"G04 P60", "Задержка");
                generator.Command($"M05", "Выключение");
                generator.Command($"M00", "Пауза");

                generator.GCommand(1, u3, z3, CuttingFeed);
            }
        }
    }
 }
