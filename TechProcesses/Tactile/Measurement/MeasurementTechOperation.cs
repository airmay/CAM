using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.Tactile
{
    [Serializable]
    [TechOperation(4, TechProcessNames.Tactile, "Измерение")]
    public class MeasurementTechOperation : TechOperationBase
    {
        public List<double> PointsX { get; set; } = new List<double>();

        public List<double> PointsY { get; set; } = new List<double>();

        public enum CalcMethodType
        {
            Average,
            Minimum
        };

        public CalcMethodType CalcMethod { get; set; } = CalcMethodType.Average;

        [NonSerialized]
        public ObjectId[] PointObjectIds;

        public MeasurementTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public override void BuildProcessing(ICommandGenerator generator)
        {
            if (CalcMethod == CalcMethodType.Average)
            {
                for (int i = 0; i < PointsX.Count; i++)
                {
                    generator.GCommand(CommandNames.Fast, 0, x: PointsX[i] + 230, y: PointsY[i] - 100 - TechProcess.Tool.Thickness.GetValueOrDefault());
                    generator.ToolLocation.Point -= new Vector3d(0, 0, 1);
                    generator.GCommand("", 0, z: TechProcess.Thickness.GetValueOrDefault() + TechProcess.ZSafety);

                    generator.Command("M131");
                    generator.Command($"DBL THICK{i} = %TastL.ZLastra - %TastL.ZBanco", "Измерение");
                    generator.Command($"G0 Z(THICK{i}/1000 + 100)");
                }
                var s = String.Join(" + ", Enumerable.Range(0, PointsX.Count).Select(p => $"THICK{p}"));
                generator.Command($"DBL THICK = ({s})/{PointsX.Count}/1000");
            }
            else
            {
                for (int i = 0; i < PointsX.Count; i++)
                {
                    generator.GCommand(CommandNames.Fast, 0, x: PointsX[i] + 230, y: PointsY[i] - 100 - TechProcess.Tool.Thickness.GetValueOrDefault());
                    generator.ToolLocation.Point -= new Vector3d(0, 0, 1);
                    generator.GCommand("", 0, z: TechProcess.Thickness.GetValueOrDefault() + TechProcess.ZSafety);

                    generator.Command("M131");
                    generator.Command($"DBL THICK{i} = %TastL.ZLastra - %TastL.ZBanco", "Измерение");
                    generator.Command($"G0 Z(THICK{i}/1000 + 100)");

                    if (i != 0)
                    {
                        generator.Command($"IF (THICK{i} < THICK0) THEN");
                        generator.Command($" THICK0 = THICK{i}");
                        generator.Command("ENDIF");
                    }
                }
                generator.Command("DBL THICK = THICK0/1000");
            }
            generator.WithThick = true;
        }

        public override void Setup(ITechProcess techProcess)
        {
            base.Setup(techProcess);
            PointObjectIds = PointsX.SelectMany((p, i) => Acad.CreateMeasurementPoint(new Point3d(PointsX[i], PointsY[i], 0))).ToArray();
        }

        public void Clear()
        {
            PointsX.Clear();
            PointsY.Clear();
            if (PointObjectIds.Any())
                Acad.DeleteObjects(PointObjectIds);
        }

        public void CreatePoint(Point3d point)
        {
            PointsX.Add(point.X);
            PointsY.Add(point.Y);
            PointObjectIds = PointObjectIds.Concat(Acad.CreateMeasurementPoint(point)).ToArray();
        }

        public override void Teardown()
        {
            Acad.DeleteObjects(PointObjectIds);
            base.Teardown();
        }
    }

}
