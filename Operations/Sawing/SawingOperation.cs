using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;

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

        public override void Execute(GeneralOperation generalOperation, Processor processor)
        {
            var curves = new List<Curve>(ProcessingArea.GetCurves());
            var curveParams = CalcСurveProcessingParams(curves);
            foreach (var curve in curves)
            {
                ProcessCurve(processor, curve, curveParams[curve]);
            }
        }

        private Dictionary<Curve, СurveProcessingParams> CalcСurveProcessingParams(List<Curve> curves)
        {
            var curveParams = new Dictionary<Curve, СurveProcessingParams>();
            var startObject = ObjectId.Null;
            Point3d? point = null;
            var curvesToCalc = new List<Curve>(curves);

            while (curvesToCalc.Any())
            {
                var curvesChain = new List<Curve>();
                var side = ChangeSide ? -1 : 1;
                var curve = curves[0];
                curvesToCalc.Remove(curve);
                var chainForward = CalcChain(curvesToCalc, curve, curve.EndPoint, side);
                var chainBackward = CalcChain(curvesToCalc, curve, curve.StartPoint, -side);
                
                Graph.CreateHatch(chainBackward.r)



                if (startObject == ObjectId.Null)
                    startObject = curves[0];

                var nextCurve = FindCurve(curves, point.Value);
                side = CalcSide(curve, side, point.Value);
                curve = nextCurve;


                point = curve.NextPoint(point.Value);
                curves.Remove(curve);
            }
        }

        private List<Curve> CalcChain(List<Curve> curvesToCalc, Curve curve, Point3d point, int side)
        {
            
        }

        private Curve FindCurve(List<Curve> curves, Point3d point)
        {

        }

        private void ProcessCurve(Processor processor, Curve curve, СurveProcessingParams @params)
        {

        }
    }
}