using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

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
            var curvesSides = new Dictionary<Curve, int>();
            var pointsIsExactly = new Dictionary<Point3d, bool>();

            var curves = ProcessingArea.GetCurves();
            CalcСurveProcessing(curves);
            foreach (var curve in curves)
            {
                ProcessCurve(processor, curve, curvesSides[curve], pointsIsExactly[curve.StartPoint], pointsIsExactly[curve.EndPoint]);
            }

            return;

            void CalcСurveProcessing(IEnumerable<Curve> curvesArray)
            {
                var side = ChangeSide ? -1 : 1;
                var curvesToCalc = new List<Curve>(curvesArray);

                while (curvesToCalc.Any())
                {
                    var chain = CalcChain(curvesToCalc, side);
                    var hatchId = Graph.CreateHatch(chain, side);
                    if (hatchId.HasValue)
                        ExtraObjectsGroup = ExtraObjectsGroup.AppendToGroup(hatchId.Value);
                    curvesToCalc.RemoveAll(p => chain.Contains(p));
                }
            }

            List<Curve> CalcChain(List<Curve> curvesToCalc, int side)
            {
                var chain = new List<Curve>();
                var pointCurveDict = curvesToCalc
                    .SelectMany(p => p.GetStartEndPoints(), (cv, pt) => (cv, pt))
                    .ToLookup(p => p.pt, p => p.cv);
                var corner = pointCurveDict.FirstOrDefault(p => p.Count() == 1);
                var (curve, point) = corner != null
                    ? (corner.Single(), corner.Key)
                    : (curvesToCalc.First(), curvesToCalc.First().StartPoint);
                if (corner != null)
                {
                    pointsIsExactly[point] = IsExactlyBegin;
                }

                var startPoint = point;
                do
                {
                    var sd = point == curve.StartPoint ? side : -side;
                    curvesSides[curve] = sd;
                    chain.Add(curve);

                    point = curve.NextPoint(point);
                    var endTangent = curve.GetTangent(point);
                    if (point == curve.StartPoint)
                        endTangent *= -1;

                        curve = pointCurveDict[point].SingleOrDefault(p => p != curve);
                    if (curve == null)
                    {
                        pointsIsExactly[point] = IsExactlyEnd;
                        break;
                    }

                    var startTangent = curve.GetTangent(point);
                    if (point == curve.EndPoint)
                        startTangent *= -1;
                    var angle = endTangent.MinusPiToPiAngleTo(startTangent);
                    pointsIsExactly[point] = side == 1 ^ angle < 0;
                } 
                while (point != startPoint);

                return chain;
            }
        }

        private void ProcessCurve(Processor processor, Curve curve, int side, bool isExactlyBegin, bool isExactlyEnd)
        {

        }
    }
}