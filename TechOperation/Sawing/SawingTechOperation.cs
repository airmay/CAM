//using System;
//using Autodesk.AutoCAD.Geometry;
//using Dreambuild.AutoCAD;

//namespace CAM.TechOperation.Sawing
//{
//    /// <summary>
//    /// Технологическая операция "Распиловка"
//    /// </summary>
//    [Serializable]
//    public class SawingTechOperation : TechOperationBase
//    {
//        /// <summary>
//        /// Вид технологической операции
//        /// </summary>
//        public override ProcessingType Type => ProcessingType.Sawing;

//        /// <summary>
//        /// Параметры технологической операции
//        /// </summary>
//        public SawingTechOperationParams SawingParams { get; }

//        public override object Params => SawingParams;

//        public SawingTechOperation(ITechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams sawingParams, string name)
//            : base(techProcess, processingArea, name)
//        {
//            SawingParams = sawingParams;
//        }

//        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
//        {
//            builder.StartTechOperation();
//            var processParams = null;//TechProcess.TechProcessParams;
//            var border = ((BorderProcessingArea)ProcessingArea);           
//            var compensation = builder.CalcCompensation(border.Curve, border.OuterSide);
//            var sumIndent = builder.CalcIndent(processParams.BilletThickness) * (Convert.ToInt32(border.IsExactlyBegin) + Convert.ToInt32(border.IsExactlyEnd));
//            if (sumIndent < border.Curve.Length())
//            {
//                var modes = SawingParams.Modes.ConvertAll(p => new CuttingMode { Depth = p.Depth, DepthStep = p.DepthStep, Feed = p.Feed });
//                builder.LineCut(border.Curve, modes, SawingParams.IsFirstPassOnSurface, border.IsExactlyBegin, border.IsExactlyEnd, border.OuterSide, processParams.BilletThickness, compensation);
//                if (!border.IsExactlyBegin)
//                    Acad.CreateGash(border.Curve, border.StartPoint, border.OuterSide, processParams.BilletThickness, processParams.ToolDiameter, processParams.ToolThickness);
//                if (!border.IsExactlyEnd)
//                    Acad.CreateGash(border.Curve, border.EndPoint, border.OuterSide, processParams.BilletThickness, processParams.ToolDiameter, processParams.ToolThickness);
//            }
//            else
//            {
//                var point = Scheduling(border, compensation);
//                var angle = builder.CalcToolAngle(NoDraw.Line(border.Curve.StartPoint, border.Curve.EndPoint), border.Curve.StartPoint);
//                builder.Cutting(point, angle, TechProcess.TechProcessParams.PenetrationRate);
//            }
//            ProcessCommands = builder.FinishTechOperation();
//        }

//        /// <summary>
//        /// Намечание
//        /// </summary>
//        private Point3d Scheduling(BorderProcessingArea border, double compensation)
//        {
//            var diam = TechProcess.TechProcessParams.ToolDiameter;
//            var startPoint = border.Curve.StartPoint;
//            var endPoint = border.Curve.EndPoint;
//            const int CornerIndentIncrease = 5;
//            var vector = endPoint - startPoint;
//            var depth = (double)TechProcess.TechProcessParams.BilletThickness;
//            var point = Point3d.Origin;
//            if (border.IsExactlyBegin && border.IsExactlyEnd)
//            {
//                var l = vector.Length - 2 * CornerIndentIncrease;
//                depth = (diam - Math.Sqrt(diam * diam - l * l)) / 2;
//                point = startPoint + vector / 2;
//            }
//            else
//            {
//                var indentVector = vector.GetNormal() * (Math.Sqrt(depth * (diam - depth)) + CornerIndentIncrease);
//                point = border.IsExactlyBegin ? startPoint + indentVector : endPoint - indentVector;
//                Acad.CreateGash(border.Curve, border.IsExactlyBegin ? border.EndPoint : border.StartPoint, border.OuterSide, (int)depth, diam, TechProcess.TechProcessParams.ToolThickness, point);
//            }
//            return point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
//        }
//    }
//}
