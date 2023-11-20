using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace CAM.Operations.Sawing
{
    [Serializable]
    public class SawingOperation : Operation
    {
        public Side OuterSide { get; set; }

        public double Thickness { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }
        public double AngleA { get; set; }
        public double Departure { get; set; }
        public bool ChangeSide { get; set; }

        public double Depth { get; set; }
        public double? Penetration { get; set; }
        public List<CuttingMode> SawingModes { get; set; } = new List<CuttingMode>();

        public static void ConfigureParamsView(ParamsView view)
        {
            var thicknessTextBox = view.AddTextBox(nameof(Thickness));
            view.AddCheckBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddCheckBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(AngleA));
            view.AddTextBox(nameof(Departure));
            view.AddIndent();
            view.AddAcadObject(
                allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}");
            view.AddCheckBox(nameof(ChangeSide), "Сменить сторону", "Поменять обрабатываемою сторону у объектов");
            var depthTextBox = view.AddTextBox(nameof(Depth));
            view.AddTextBox(nameof(Penetration),
                toolTipText: "Шаг заглубления для прямой и если не заданы Режимы для криволинейных траекторий то для всех кривых");
            view.AddText("Режимы для криволинейных траекторий", "Режимы применяются для дуги и полилинии");
            view.AddControl(new SawingModesView(), 6, nameof(SawingModesView.DataSource), nameof(SawingModes));

            thicknessTextBox.Validated += (sender, args) =>
            {
                if (depthTextBox.Text == "0")
                    depthTextBox.Text = (view.GetParams<SawingOperation>().Thickness + 2).ToString();
            };
        }

        private class СurveProcessingParams
        {
            public int Side { get; set; }
            public bool IsExactlyBegin { get; set; }
            public bool IsExactlyEnd { get; set; }
        }

        private class ProcessingСurve
        {
            public Curve Curve { get; set; }
            public int Side { get; set; }
            public bool IsExactlyBegin { get; set; }
            public bool IsExactlyEnd { get; set; }
        }

        public override void Execute(GeneralOperation generalOperation, Processor processor)
        {
            var curves = ProcessingArea.GetCurves();
            var curveParams = CalcСurveProcessingParams(curves);
            foreach (var curve in curves)
            {
                ProcessCurve(processor, curve, curveParams[curve]);
            }
        }

        private Dictionary<Curve, СurveProcessingParams> CalcСurveProcessingParams(IEnumerable<Curve> curves)
        {
            var curvesToCalc = new List<Curve>(curves);
            var curvesParams = new Dictionary<Curve, СurveProcessingParams>();
            var side = ChangeSide ? -1 : 1;

            while (curvesToCalc.Any())
            {
                var curveParamsChain = CalcChain(curvesToCalc, side);
                curveParamsChain.ForEach(p => curvesParams.Add(p.Item1, p.Item2));
                Graph.CreateHatch(curveParamsChain.ConvertAll(p => p.Item1), side);
            }

            return curvesParams;
        }

        private List<(Curve, СurveProcessingParams)> CalcChain(List<Curve> curvesToCalc, int side)
        {
            var queue = new Queue<ProcessingСurve>();
            var group = curvesToCalc
                .SelectMany(p => p.GetStartEndPoints(), (cv, pt) => (cv, pt))
                .GroupBy(p => p.pt)
                .FirstOrDefault(p => p.Count() == 1);
            var (curve, point) = group != null
                ? (group.Single().cv, group.Key) 
                : (curvesToCalc[0], curvesToCalc[0].StartPoint);
            curvesToCalc.Remove(curve);
            var processingСurve = new ProcessingСurve
            {
                Curve = curve
            };
            queue.Enqueue(processingСurve);
            while (curvesToCalc.Any())
            {
                var nextPoint = curve.NextPoint(point);
                var nextCurve = curvesToCalc.SelectMany(p => p.GetStartEndPoints(), (cv, pt) => (cv, pt))
                    .SingleOrDefault(p => p.pt == nextPoint);
                p = CalcParams(curve, side, nextPoint);
                curve = nextCurve;


                point = curve.NextPoint(point.Value);
                curves.Remove(curve);
            }

        }

        private void ProcessCurve(Processor processor, Curve curve, СurveProcessingParams @params)
        {

        }
    }
}