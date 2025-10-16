/*
using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using Autodesk.AutoCAD.Geometry;
using System.Security.Cryptography;

namespace CAM.TechProcesses.Disk3D
{
    [Serializable]
    [MenuItem("Гребенка", 1)]
    public class CombTechOperation : MillingTechOperation<Disk3DTechProcess>
    {
        public CombTechOperation(Disk3DTechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public double StepPass { get; set; }

        public double? StartPass { get; set; }

        public double? EndPass { get; set; }

        public bool IsReverse { get; set; }

        public double PenetrationStep { get; set; }
        public double PenetrationBegin { get; set; }
        public double? PenetrationEnd { get; set; }

        public double Delta { get; set; }

        public double StepLong { get; set; }

        public int CuttingFeed { get; set; }

        public double Departure { get; set; }

        public bool IsDepartureOnBorderSection { get; set; }

        public double PenetrationAll { get; set; }

        public bool IsUplifting { get; set; }
        public double? VzMax { get; set; }


        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTextBox(nameof(StepPass));
            view.AddTextBox(nameof(StartPass));
            view.AddTextBox(nameof(EndPass));
            view.AddCheckBox(nameof(IsReverse), "Обратно");
            view.AddIndent();
            view.AddTextBox(nameof(StepLong));
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddTextBox(nameof(PenetrationStep), "Заглубление: шаг");
            view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало");
            view.AddTextBox(nameof(PenetrationEnd), "Заглубление: конец");
            view.AddTextBox(nameof(CuttingFeed));
            view.AddIndent();
            view.AddTextBox(nameof(Delta));
            view.AddCheckBox(nameof(IsDepartureOnBorderSection), "Выезд по границе сечения");
            view.AddTextBox(nameof(PenetrationAll), "Заглубление всего");
            view.AddTextBox(nameof(VzMax), "Vz максимальная");
            view.AddCheckBox(nameof(IsUplifting));
        }

        private void SetTool(MillingCommandGenerator generator, double angleA, double angleC) 
            => generator.SetTool(
                TechProcess.MachineType.Value != Machine.Donatoni ? TechProcess.Tool.Number : 1, 
                TechProcess.Frequency, 
                angleA: angleA,
                angleC: angleC, 
                originCellNumber: TechProcess.OriginCellNumber);

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            generator.Tool = TechProcess.Tool;
            var zMax = TechProcess.Thickness.Value;
            //var zMax = offsetSurface.GeometricExtents.MinPoint.Z + TechProcess.Thickness.Value;
            generator.SetZSafety(TechProcess.ZSafety, zMax);

            var surface = TechProcess.ProcessingArea.ObjectIds.CreateOffsetSurface(Delta);
            var rotation = AcadUtils.GetRotationMatrix(TechProcess.Angle, TechProcess.AngleA);
            surface.TransformBy(rotation);
            var matrix = rotation.Inverse();

            var targetPassSet = CalcTargetPassSet(surface);
            foreach (var targetPass in targetPassSet)
            {
                var cuttingPassSet = CalcCuttingPassSet(targetPass);

                Generate(generator, cuttingPassSet, matrix);
            }
            surface.Dispose();
        }

        private void Generate(MillingCommandGenerator generator, IEnumerable<List<Point3d>> cuttingPassSet, Matrix3d matrix)
        {
            generator.Matrix = matrix;
            foreach (var pass in cuttingPassSet)
            {
                //var pass = cuttingPass;//.ConvertAll(p => p.TransformBy(matrix));

                if (Departure > 0)
                {
                    pass[0] = pass[0].GetExtendedPoint(pass[1], Departure);
                    pass[pass.Count - 1] = pass[pass.Count - 1].GetExtendedPoint(pass[pass.Count - 2], Departure);
                }
                var tp = generator.ToolPosition.IsDefined ? generator.ToolPosition.Point : Point3d.Origin;
                if (pass.First().DistanceTo(tp) > pass.Last().DistanceTo(tp))
                    pass.Reverse();

                if (generator.IsUpperTool)
                {
                    generator.Move(pass[0].X, pass[0].Y);
                    generator.Cycle();
                }
                if (pass[0] != generator.ToolPosition.Point)
                    generator.GCommand(CommandNames.Penetration, 1, point: pass[0], feed: TechProcess.PenetrationFeed);

                var prevPoint = pass[1];
                var prevVector = pass[0].GetVectorTo(prevPoint);
                foreach (var point in pass.Skip(2))
                {
                    var vector = prevPoint.GetVectorTo(point);
                    if (!vector.IsCodirectionalTo(prevVector, Tolerance.Global))
                    {
                        var feed = CuttingFeed;
                        if (VzMax.HasValue) // ограничение вертикальной составляющей
                        {
                            var cos = Math.Cos(prevVector.GetAngleTo(Vector3d.ZAxis.Negate()));
                            if (cos > 0)
                            {
                                var maxFeed = VzMax.Value / cos;
                                if (feed > maxFeed)
                                    feed = (int)maxFeed;
                            }
                        }
                        generator.GCommand(CommandNames.Cutting, 1, point: prevPoint, feed: feed);
                        prevVector = vector;
                    }
                    prevPoint = point;
                }
                generator.GCommand(CommandNames.Cutting, 1, point: pass.Last(), feed: CuttingFeed);
            }

            //generator.Move();
            generator.Matrix = null;
            if (IsUplifting)
                generator.Uplifting();
        }

        private IEnumerable<List<Point3d>> CalcTargetPassSet(DbSurface surface)
        {
            var bounds = surface.GeometricExtents;
            var startY = StartPass ?? bounds.MinPoint.Y;
            var endY = EndPass ?? bounds.MaxPoint.Y;
            Acad.SetLimitProgressor((int)((endY - startY) / StepPass));

            for (var y = startY; y < endY; y += StepPass)
            {
                Acad.ReportProgressor();
                var points = new Point3dCollection();

                for (var x = bounds.MinPoint.X; x <= bounds.MaxPoint.X; x += StepLong)
                {
                    var z = surface.CalcMaxZ(x, y, y + TechProcess.Tool.Thickness.Value / 2, y + TechProcess.Tool.Thickness.Value);
                    if (z.HasValue)
                        points.Add(new Point3d(x, y, z.Value));
                }

                if (points.Count > 1)
                {
                    var offsetPoints = points.CalcOffsetPoints(TechProcess.Tool.Diameter / 2, StepLong).ToList();
                    if (offsetPoints.Any())
                    {
                        if (Departure < 0)
                            offsetPoints = offsetPoints.Trim(-Departure, -Departure);
                        yield return offsetPoints;
                    }
                }
            }
        }

        private IEnumerable<List<Point3d>> CalcCuttingPassSet(List<Point3d> targetPass)
        {
            var passZ = PenetrationBegin;
            var endZ = PenetrationEnd ?? double.NegativeInfinity;

            var startIndex = 0;
            var endIndex = targetPass.Count - 1;
            int? nextStartIndex;
            int? nextEndIndex;

            do
            {
                passZ -= PenetrationStep;
                if (passZ < endZ)
                    passZ = endZ;
                var pass = new List<Point3d>();
                nextStartIndex = null;
                nextEndIndex = null;

                for (var index = startIndex; index <= endIndex; index++)
                {
                    var z = passZ;
                    if (z < targetPass[index].Z + Consts.Epsilon) // последний проход 
                    {
                        z = targetPass[index].Z;
                    }
                    else
                    {
                        if (!nextStartIndex.HasValue)
                            nextStartIndex = index;
                        nextEndIndex = index;
                    }

                    pass.Add(new Point3d(targetPass[index].X, targetPass[index].Y, z));
                }

                //startIndex = nextStartIndex.GetValueOrDefault();
                //endIndex = nextEndIndex.GetValueOrDefault();

                yield return pass;
            }
            while (nextStartIndex.HasValue && passZ > endZ);
        }
    }
}
*/
