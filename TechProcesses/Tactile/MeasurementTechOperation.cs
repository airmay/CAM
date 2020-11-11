using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CAM.TechProcesses.Tactile
{
    [Serializable]
    [TechOperation(TechProcessType.Tactile, "Измерение", 4)]
    public class MeasurementTechOperation : TechOperationBase
    {
        public List<double> PointsX { get; set; } = new List<double>();

        public List<double> PointsY { get; set; } = new List<double>();

        public double? Thickness { get; set; }

        public enum CalcMethodType
        {
            [Description("Среднее")]
            Average,

            [Description("Наименьшее")]
            Minimum,

            [Description("По 4 углам")]
            Сorners
        };

        public CalcMethodType CalcMethod { get; set; } = CalcMethodType.Average;

        [NonSerialized]
        public ObjectId[] PointObjectIds;

        public MeasurementTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
            Thickness = techProcess.Thickness;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            var selector = view.AddSelector("Точки", "۞", ConfigurePointsSelector)
                .AddParam(nameof(Thickness))
                .AddEnumParam<CalcMethodType>(nameof(CalcMethod), "Метод расчета");
        }

        private static void ConfigurePointsSelector(TextBox textBox, Button button, BindingSource bindingSource)
        {
            textBox.Enter += (s, e) => Acad.SelectObjectIds(bindingSource.GetSource<MeasurementTechOperation>().PointObjectIds);

            button.Click += (s, e) =>
            {
                var operation = bindingSource.GetSource<MeasurementTechOperation>();
                operation.Clear();
                Interaction.SetActiveDocFocus();
                Point3d point;
                while (!(point = Interaction.GetPoint("\nВыберите точку измерения")).IsNull())
                {
                    operation.CreatePoint(point);
                    textBox.Text = operation.PointsX.Count.ToString();
                }
            };

            bindingSource.DataSourceChanged += (s, e) => textBox.Text = bindingSource.GetSource<MeasurementTechOperation>().PointsX.Count.ToString();
        }

        public override void BuildProcessing(CommandGeneratorBase generator)
        {
            switch (CalcMethod)
            {
                case CalcMethodType.Average:
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
                    break;

                case CalcMethodType.Minimum:
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
                    break;

                case CalcMethodType.Сorners:

                    var points = PointsX.Select((p, i) => new Point2d(p, PointsY[i])).OrderBy(p => p.X).ToList();
                    var corners = points.Take(2).OrderBy(p => p.Y).Concat(points.Skip(2).OrderByDescending(p => p.Y)).ToList();

                    for (int i = 0; i < corners.Count; i++)
                    {
                        generator.GCommand(CommandNames.Fast, 0, x: corners[i].X + 230, y: corners[i].Y - 100 - TechProcess.Tool.Thickness.GetValueOrDefault());
                        generator.ToolLocation.Point -= new Vector3d(0, 0, 1);
                        generator.GCommand("", 0, z: TechProcess.Thickness.GetValueOrDefault() + TechProcess.ZSafety);

                        generator.Command("M131");
                        generator.Command($"DBL THICK{i} = (%TastL.ZLastra - %TastL.ZBanco)/1000", "Измерение");
                        generator.Command($"G0 Z(THICK{i} + 100)");
                    }
                    var l1 = corners[3].X - corners[0].X;
                    var l2 = corners[1].Y - corners[0].Y;
                    generator.Command($"DBL KK1=(THICK3 - THICK0) / {l1}");
                    generator.Command($"DBL KK2=(THICK2 - THICK1) / {l1}");
                    generator.Command("DBL THICK");
                    generator.ThickCommand = $"THICK = (({{0}}-{corners[0].X})*KK2+THICK1)*({{1}}-{corners[0].Y})/{l2} + (({{0}}-{corners[0].X})*KK1+THICK0)*(1-({{1}}-{corners[0].Y})/{l2})";
                    break;
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
            PointObjectIds = Array.Empty<ObjectId>();
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
