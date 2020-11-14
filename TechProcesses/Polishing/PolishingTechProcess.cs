using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.Polishing
{
    [Serializable]
    [MenuItem("Полировка", 6)]
    public class PolishingTechProcess : TechProcess
    {
        public int Feed { get; set; }

        public int ZEntry { get; set; }

        public double Angle1 { get; set; }

        public double Angle2 { get; set; }

        public int StepMin { get; set; }

        public int StepMax { get; set; }

        public int AmplitudeMin { get; set; }

        public int AmplitudeMax { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.Krea)
                .AddParam(nameof(Frequency))
                .AddIndent()
                .AddParam(nameof(Feed))
                .AddParam(nameof(ZSafety))
                .AddParam(nameof(ZEntry))
                .AddIndent()
                .AddOrigin()
                .AddAcadObject(nameof(ProcessingArea), "Контур", "Выберите объекты контура")
                .AddIndent()
                .AddParam(nameof(Angle1), "Угол 1")
                .AddParam(nameof(Angle2), "Угол 2")
                .AddIndent()
                .AddParam(nameof(AmplitudeMin), "Амплитуда мин.")
                .AddParam(nameof(AmplitudeMax), "Амплитуда макс.")
                .AddIndent()
                .AddParam(nameof(StepMin), "Шаг мин.")
                .AddParam(nameof(StepMax), "Шаг макс.");
        }

        public override bool Validate()
        {
            if (Angle2 >= 90)
                Acad.Alert("Угол2 должен быть меньше 90");
            return Angle2 < 90;
        }

        protected override void BuildProcessing(CommandGeneratorBase generator)
        {
            generator.ZSafety = ZSafety;
            if (MachineType.Value == CAM.MachineType.Donatoni)
                generator.SetTool(2, Frequency, angleA: 90, hasTool: false);
            else
                generator.SetTool(1, Frequency, angleA: 0, hasTool: false);

            var curves = ProcessingArea.Select(p => p.ObjectId).QOpenForRead<Curve>().ToList();
            var bounds = ProcessingArea.Select(p => p.ObjectId).GetExtents();
            var random = new Random();
            var mainDir = Vector3d.XAxis.RotateBy(Angle1.ToRad(), Vector3d.ZAxis);
            var side = 1;
            var sign = -1;
            var basePoint = bounds.MinPoint + (bounds.GetCenter() - bounds.MinPoint).GetNormal() * 100;
            using (var ray = new Ray())
                while (true)
                {
                    ray.BasePoint = basePoint;
                    ray.UnitDir = mainDir.RotateBy(Angle2.ToRad() * side, Vector3d.ZAxis);
                    var points = ray.Intersect(curves).FindAll(p => p.DistanceTo(ray.BasePoint) > 10);
                    if (!points.Any())
                        break;

                    basePoint = points.First() + ray.UnitDir.Negate();
                    Cutting(ray.BasePoint, basePoint, ray.UnitDir, ray.UnitDir.GetPerpendicularVector() * side * sign);
                    side *= -1;
                }

            void Cutting(Point3d point1, Point3d point2, Vector3d dir, Vector3d pv)
            {
                var z = 0;
                if (generator.IsUpperTool)
                {
                    generator.Move(point1.X, point1.Y);
                    generator.GCommand(CommandNames.Cutting, 1, x: point1.X, y: point1.Y, z: ZEntry, feed: Feed / 5);
                    z = ZEntry;
                }
                var point0 = point1;
                var line = new Line();
                var length = point1.DistanceTo(point2);
                var l = 0D;
                var a = 0D;
                var count = 10D;
                var stepA = Math.PI / count;
                var stepL = 0D;
                var amp = 0;
                var isInner = true;

                while (l < length)
                {                    
                    if (Math.Abs(Math.Sin(a)) < Consts.Epsilon)
                    {
                        stepL = random.Next(StepMin, StepMax) / count;
                        amp = random.Next(AmplitudeMin, AmplitudeMax);
                    }
                    l += stepL;
                    a += stepA;
                    var point = point1 + dir * l + pv * Math.Sin(a) * amp;
                    line.StartPoint = point0;
                    line.EndPoint = point;
                    if (line.Intersect(curves).Any())
                        isInner = !isInner;
                    if (isInner)
                    {
                        if (z > 0)
                            z--;
                        generator.GCommand(CommandNames.Cutting, 1, x: point.X, y: point.Y, z: z, feed: Feed);
                    }
                    point0 = point;
                }
                line.Dispose();
                if (Math.Sin(a - stepA) > 0)
                    sign = -sign;
            }

                //var ray = new Ray
                //{
                //    BasePoint = new Point3d(bounds.MinPoint.X, 0, 0),
                //    UnitDir = Vector3d.YAxis
                //};
                //var uppDir = true;
                //var random = new Random();

                //while (true)
                //{
                //    var points = ray.Intersect(ProcessingArea.Select(p => p.ObjectId).QOpenForRead<Curve>().ToList(), Intersect.ExtendThis);
                //    if (points.Count == 0)
                //        break;
                //    if (points.Count > 1)
                //    {
                //        points = (uppDir ? points.OrderBy(p => p.Y) : points.OrderByDescending(p => p.Y)).ToList();
                //        Cutting(points.First(), points.Last());
                //    }
                //    ray.BasePoint += Vector3d.XAxis * Step1;
                //}

                //void Cutting(Point3d point1, Point3d point2)
                //{
                //    if (generator.IsUpperTool)
                //        generator.Move(point1.X, point1.Y);
                //    generator.GCommand(CommandNames.Cutting, 1, point: point1);

                //    var length = point1.DistanceTo(point2);
                //    var rnd = IsRandomStepCount ? (random.NextDouble() / 2 + 0.75) : 1;
                //    var countCycles = (int)Math.Round(length * rnd / Step2 / 2) * 2;
                //    var coodrs = Enumerable.Range(1, countCycles).Select(p => (double)p);
                //    if (IsRandomStepParams)
                //        coodrs = coodrs.Select(p => p + (p != countCycles ? (random.NextDouble() / 2 - 0.25) : 0));
                //    var coordArray = coodrs.ToArray();
                //    double l = 0;
                //    double a = 0;
                //    var count = 10;
                //    var stepA = Math.PI / count;
                //    for (int i = 0; i < coordArray.Length; i++)
                //    {
                //        var coord0 = i > 0 ? coordArray[i - 1] : 0;
                //        var dl = coordArray[i] - coord0;
                //        var stepL = dl * length / countCycles / count;

                //        foreach (var j in Enumerable.Range(1, count))
                //        {
                //            l += stepL;
                //            a += stepA;
                //            var point = point1 + new Vector3d(Math.Sin(a) * Amplitude * dl, uppDir ? l : -l, 0);
                //            generator.GCommand(CommandNames.Cutting, 1, point: point, feed: Feed);
                //        }
                //    }

                //while(true)
                //{
                //    cnt++;
                //    if (length - l < 2 * Step2)
                //    {

                //    }
                //    var rnd = random.Next(5, 10) / 10D;
                //    foreach (var i in Enumerable.Range(0, 10))
                //    {
                //        var point = point1 + new Vector3d(Math.Sin(a) * Amplitude * rnd, uppDir ? l : -l, 0);
                //        generator.GCommand(CommandNames.Cutting, 1, point: point, feed: Feed);
                //        l += stepL / 10 * rnd;
                //        a += stepA / 10;
                //    }
                //}


                //foreach (var i in Enumerable.Range(0, count))
                //{
                //    var point = point1 + new Vector3d(Math.Sin(a) * Amplitude, uppDir ? l : -l, 0);
                //    generator.GCommand(CommandNames.Cutting, 1, point: point, feed: Feed);
                //    l += stepL;
                //    a += stepA;
                //}
                //    uppDir = !uppDir;
                //}
            }
    }
}
