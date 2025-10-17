using System;
using System.Collections.Generic;
using CAM.TechProcesses.Common;
using CAM.UI;

namespace CAM.MachineCncWorkCenter.Operations.Tactile
{
    [Serializable]
    public class TactileBandsOperation : OperationCnc
    {
        public int ProcessingAngle { get; set; }

        public int CuttingFeed { get; set; }

        public int FeedFinishing { get; set; }

        public double BandWidth { get; set; }

        public double BandSpacing { get; set; }

        public double BandStart { get; set; }

        public double Depth { get; set; }

        public double MaxCrestWidth { get; set; }

        public List<Pass> PassList { get; set; } = new List<Pass>();

        public bool IsEdgeProcessing { get; set; }

        //public TactileBandsOperation(TactileTechProcess techProcess, string caption) : base(techProcess, caption) { }

        //public TactileBandsOperation(TactileTechProcess techProcess, string caption, int? processingAngle, double? bandStart) : base(techProcess, caption)
        //{
        //    BandStart = bandStart ?? BandStart;
        //    ProcessingAngle = processingAngle ?? ProcessingAngle;
        //}

        //public override void Init()
        //{
        //    BandWidth = TechProcess.BandWidth.Value;
        //    BandSpacing = TechProcess.BandSpacing.Value;
        //    BandStart = TechProcess.BandStart1.Value;
        //    ProcessingAngle = TechProcess.ProcessingAngle1.Value;
        //    Depth = TechProcess.Depth;
        //    MaxCrestWidth = (TechProcess.Tool?.Thickness).GetValueOrDefault();
        //    IsEdgeProcessing = true;
        //}

        public static void ConfigureParamsView(ParamsControl view)
        {
            view.AddTextBox(nameof(ProcessingAngle), "Угол полосы");
            view.AddTextBox(nameof(CuttingFeed), "Подача гребенка");
            view.AddTextBox(nameof(FeedFinishing), "Подача чистовая");
            view.AddIndent();
            view.AddTextBox(nameof(BandWidth), "Ширина полосы");
            view.AddTextBox(nameof(BandSpacing), "Расст.м/у полосами");
            view.AddTextBox(nameof(BandStart), "Начало полосы");
            view.AddTextBox(nameof(Depth), "Глубина");
            view.AddIndent();
            view.AddTextBox(nameof(MaxCrestWidth), "Макс.шир.гребня");
            view.AddTextBox(nameof(IsEdgeProcessing), "Обработка краев");
            //view.AddControl(new PassListControl(view.BindingSource, true), 10);
        }

        //public void CalcPassList()
        //{
        //    if (TechProcess.Tool.Thickness == null)
        //    {
        //        Acad.Alert("Не указана толщина инструмента");
        //        return;
        //    }
        //    var toolThickness = TechProcess.Tool.Thickness.Value;
        //    if (MaxCrestWidth == 0 || MaxCrestWidth > toolThickness)
        //        MaxCrestWidth = toolThickness;
        //    var periodAll = BandWidth - toolThickness;
        //    var periodWidth = toolThickness + MaxCrestWidth;
        //    var count = Math.Ceiling(periodAll / periodWidth);
        //    periodWidth = periodAll / count;
        //    var x = (toolThickness - (periodWidth - toolThickness)) / 2;
        //    var shift = TechProcess.Machine == Machine.ScemaLogic ^ ProcessingAngle == 45 ? toolThickness : 0;
        //    PassList.Clear();
        //    PassList.Add(new Pass(shift, CuttingType.Roughing));
        //    for (int i = 1; i <= count; i++)
        //    {
        //        PassList.Add(new Pass(i * periodWidth + shift, CuttingType.Roughing));
        //        PassList.Add(new Pass(i * periodWidth + x + shift - toolThickness, CuttingType.Finishing));
        //    }
        //}

        //public override bool Validate() => ToolService.Validate(TechProcess.Tool, ToolType.Disk);

        //public override void BuildProcessing(MillingCommandGenerator generator)
        //{
        //    if (PassList?.Any() != true)
        //        CalcPassList();
        //    var tactileTechProcess = (TactileTechProcess)TechProcess;
        //    var thickness = TechProcess.Tool.Thickness.Value;
        //    var contour = tactileTechProcess.GetContour();
        //    var contourPoints = contour.GetPolyPoints().ToArray();
        //    var basePoint = ProcessingAngle == 45 ? contourPoints[3] : contourPoints[0];
        //    var ray = new Ray
        //    {
        //        BasePoint = basePoint,
        //        UnitDir = Vector3d.XAxis.RotateBy(ProcessingAngle.ToRad(), Vector3d.ZAxis)
        //    };
        //    var passDir = ray.UnitDir.GetPerpendicularVector();
        //    if (ProcessingAngle >= 90)
        //        passDir = passDir.Negate();
        //    double offset = BandStart - BandSpacing - BandWidth;
        //    var size = (contourPoints[ProcessingAngle == 0 ? 1 : ProcessingAngle == 90 ? 3 : 2] - contourPoints[0]).Length;

        //    if (IsEdgeProcessing)
        //    {
        //        if (ProcessingAngle == 45 ^ (TechProcess.Machine == Machine.Donatoni || TechProcess.Machine == Machine.Champion))
        //            Cutting(0.8 * thickness, CuttingFeed, -thickness);

        //        if (offset > -0.5 * thickness)
        //        {
        //            var count = offset > 0 ? (int)Math.Ceiling(offset / (0.8 * thickness)) : 1;
        //            Algorithms.Range(-0.8 * thickness * count, -0.1, 0.8 * thickness).ForEach(p => Cutting(offset + PassList[0].Pos + p, CuttingFeed));
        //        }
        //    }
        //    do
        //    {
        //        foreach (var pass in PassList)
        //            Cutting(offset + pass.Pos, pass.CuttingType == CuttingType.Roughing ? CuttingFeed : FeedFinishing);

        //        offset += BandWidth + BandSpacing;
        //    }
        //    while (offset < size);

        //    if (IsEdgeProcessing)
        //    {
        //        if (offset - BandSpacing < size)
        //            Algorithms.Range(offset - BandSpacing, size, 0.8 * thickness).ForEach(p => Cutting(p, CuttingFeed));

        //        if (ProcessingAngle == 45 ^ TechProcess.Machine == Machine.ScemaLogic)
        //            Cutting(size - 0.8 * thickness, CuttingFeed, thickness);
        //    }
        //    ray.Dispose();
        //    contour.Dispose();

        //    void Cutting(double pos, int feed, double s = 0)
        //    {
        //        if (pos < 0 || pos > size)
        //            return;
        //        ray.BasePoint = basePoint + passDir * pos;
        //        var points = new Point3dCollection();
        //        ray.IntersectWith(contour, Intersect.ExtendThis, new Plane(), points, IntPtr.Zero, IntPtr.Zero);
        //        if (points.Count == 2)
        //        {
        //            var vector = (points[1] - points[0]).GetNormal() * tactileTechProcess.Departure;
        //            var startPoint = points[0] + passDir * s - vector - Vector3d.ZAxis * Depth;
        //            var endPoint = points[1] + passDir * s + vector - Vector3d.ZAxis * Depth;
        //            if (generator.IsUpperTool)
        //                generator.Move(startPoint.X, startPoint.Y, angleC: BuilderUtils.CalcToolAngle(ProcessingAngle.ToRad()));
        //            generator.Cutting(startPoint, endPoint, feed, tactileTechProcess.TransitionFeed);
        //        }
        //    }
        //}

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
