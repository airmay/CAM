using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.Polishing
{
    [Serializable]
    [TechProcess(5, TechProcessNames.Polishing)]
    public class PolishingTechProcess : TechProcessBase
    {
        public int Feed { get; set; }

        public double Angle { get; set; }

        public double Step1 { get; set; }

        public double Step2 { get; set; }

        public double Amplitude { get; set; }

        public bool IsRandomStepCount { get; set; }

        public bool IsRandomStepParams { get; set; }

        public PolishingTechProcess(string caption) : base(caption)
        {
        }

        protected override void BuildProcessing(ICommandGenerator generator)
        {
            generator.ZSafety = ZSafety;
            if (MachineType.Value == CAM.MachineType.Donatoni)
                generator.SetTool(2, Frequency, angleA: 90, hasTool: false);
            else
                generator.SetTool(1, Frequency, angleA: 0, hasTool: false);

            var bounds = ProcessingArea.Select(p => p.ObjectId).GetExtents();
            var ray = new Ray
            {
                BasePoint = new Point3d(bounds.MinPoint.X, 0, 0),
                UnitDir = Vector3d.YAxis
            };
            var uppDir = true;
            var random = new Random();

            while (true)
            {
                var points = ray.Intersect(ProcessingArea.Select(p => p.ObjectId).QOpenForRead<Curve>().ToList(), Intersect.ExtendThis);
                if (points.Count == 0)
                    break;
                if (points.Count > 1)
                {
                    points = (uppDir ? points.OrderBy(p => p.Y) : points.OrderByDescending(p => p.Y)).ToList();
                    Cutting(points.First(), points.Last());
                }
                ray.BasePoint += Vector3d.XAxis * Step1;
            }

            void Cutting(Point3d point1, Point3d point2)
            {
                if (generator.IsUpperTool)
                    generator.Move(point1.X, point1.Y);
                generator.GCommand(CommandNames.Cutting, 1, point: point1);

                var length = point1.DistanceTo(point2);
                var rnd = IsRandomStepCount ? (random.NextDouble() / 2 + 0.75) : 1;
                var countCycles = (int)Math.Round(length * rnd / Step2 / 2) * 2;
                var coodrs = Enumerable.Range(1, countCycles).Select(p => (double)p);
                if (IsRandomStepParams)
                    coodrs = coodrs.Select(p => p + (p != countCycles ? (random.NextDouble() / 2 - 0.25) : 0));
                var coordArray = coodrs.ToArray();
                double l = 0;
                double a = 0;
                var count = 10;
                var stepA = Math.PI / count;
                for (int i = 0; i < coordArray.Length; i++)
                {
                    var coord0 = i > 0 ? coordArray[i - 1] : 0;
                    var dl = coordArray[i] - coord0;
                    var stepL = dl * length / countCycles / count;
                    
                    foreach (var j in Enumerable.Range(1, count))
                    {
                        l += stepL;
                        a += stepA;
                        var point = point1 + new Vector3d(Math.Sin(a) * Amplitude * dl, uppDir ? l : -l, 0);
                        generator.GCommand(CommandNames.Cutting, 1, point: point, feed: Feed);
                    }
                }

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
                uppDir = !uppDir;
            }
        }
    }
}
