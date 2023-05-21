using Autodesk.AutoCAD.DatabaseServices;
using System;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using DbSurface = Autodesk.AutoCAD.DatabaseServices.Surface;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Autodesk.AutoCAD.Windows.ToolPalette;
using System.Drawing;

namespace CAM.TechProcesses.Disk3D
{
    public class PassParams
    {
        public double ToolAngleC { get; set; }
        public double ToolAngleA { get; set; }
        public double PenetrationStep { get; set; }
        public double PenetrationAll { get; set; }
        public double PenetrationStart { get; set; }
        public double? PenetrationEnd { get; set; }
        public double Departure { get; set; }
        public int CuttingFeed { get; set; }
        public double Delta { get; set; }
        public double Step { get; set; }
    }

    public class PassSetParams
    {
        public double Angle { get; set; }
        public double Step { get; set; }
        public double? Start { get; set; }
        public double? End { get; set; }

        public PassSetParams(double angle, double step, double? start, double? end)
        {
            Angle = angle;
            Step = step;
            Start = start;
            End = end;
        }
    }

    public class SurfaceProcessor
    {
        private readonly AcadObject _processingArea;

        public SurfaceProcessor(AcadObject processingArea)
        {
            _processingArea = processingArea;
        }

        public PassSetParams PassSetParams { get; set; }
        public PassParams PassParams { get; set; }
        public Tool Tool { get; set; }

        public void Execute(MillingCommandGenerator generator)
        {
            var surface = _processingArea.ObjectIds.CreateOffsetSurface(PassParams.Delta);

            //var zMax = TechProcess.Thickness.Value + TechProcess.ZSafety;
            //var zMax = offsetSurface.GeometricExtents.MinPoint.Z + TechProcess.Thickness.Value;
            //generator.SetZSafety(TechProcess.ZSafety, zMax);

            var rotation = AcadUtils.GetRotationMatrix(PassSetParams.Angle, PassParams.ToolAngleA);
            surface.TransformBy(rotation);
            
            var targetPassSet = CalcTargetPassSet(surface);
            foreach (var targetPass in targetPassSet)
            {
                var cuttingPassSet = CalcCuttingPassSet(targetPass);
                foreach (var pass in cuttingPassSet)
                {
                    Cutting(generator, pass);
                }
            }
            surface.Dispose();
        }

        private IEnumerable<List<Point3d>> CalcTargetPassSet(DbSurface surface)
        {
            var bounds = surface.GeometricExtents;
            var startY = PassSetParams.Start ?? bounds.MinPoint.Y;
            var endY = PassSetParams.End ?? bounds.MaxPoint.Y;
            Acad.SetLimitProgressor((int)((endY - startY) / PassSetParams.Step));

            for (var y = startY; y < endY; y += PassSetParams.Step)
            {
                Acad.ReportProgressor();
                var points = new Point3dCollection();

                for (var x = bounds.MinPoint.X; x <= bounds.MaxPoint.X; x += PassParams.Step)
                {
                    var z = surface.CalcMaxZ(x, y, y + Tool.Thickness.Value / 2, y + Tool.Thickness.Value);
                    if (z.HasValue)
                        points.Add(new Point3d(x, y, z.Value));
                }

                var offsetPoints = points.CalcOffsetPoints(Tool.Diameter / 2, PassParams.Step).ToList();
                if (offsetPoints.Any())
                    yield return offsetPoints;
            }
        }

        private IEnumerable<List<Point3d>> CalcCuttingPassSet(List<Point3d> targetPass)
        {
            var passZ = PassParams.PenetrationStart;
            var endZ = PassParams.PenetrationEnd ?? double.NegativeInfinity;

            var startIndex = 0;
            var endIndex = targetPass.Count - 1;
            int? nextStartIndex;
            int? nextEndIndex;

            do
            {
                passZ -= PassParams.PenetrationStep;
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

                startIndex = nextStartIndex.GetValueOrDefault();
                endIndex = nextEndIndex.GetValueOrDefault();

                yield return pass;
            }
            while (nextStartIndex.HasValue && passZ > endZ);
        }

        private static void Cutting(MillingCommandGenerator generator, List<Point3d> pass)
        {
            foreach (var point in pass)
            {
                generator.GCommand(CommandNames.Cutting, 1, point: point);
            }
        }
    }

    public class CuttingProcessor
    {
        public List<Point3d> Points { get; set; }
        public double DepartureStart { get; set; }
        public double DepartureEnd { get; set; }

        public void Execute(MillingCommandGenerator generator, List<Point3d> path)
        {
        }
    }
}