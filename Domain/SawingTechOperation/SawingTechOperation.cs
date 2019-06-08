using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    /// <summary>
    /// Технологическая операция "Распиловка"
    /// </summary>
    [Serializable]
    public class SawingTechOperation : TechOperation
    {
		/// <summary>
        /// Вид технологической операции
        /// </summary>
        public override TechOperationType Type { get; } = TechOperationType.Sawing;

	    /// <summary>
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

	    public SawingTechOperation(TechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams techOperationParams)
            : base(techProcess, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил { processingArea }";
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            int thickness = TechProcess.TechProcessParams.BilletThickness;
            BorderProcessingArea border = ((BorderProcessingArea)ProcessingArea);
            var indentSum = builder.CalcIndent(border.IsExactlyBegin, border.IsExactlyEnd, thickness);
	        if (indentSum >= ProcessingArea.Curve.Length())
	        {
                throw new InvalidOperationException("Обработка невозможна");
                // TODO Намечание
                //if (!(obj is ProcessObjectLine))
                // return;
                //double h;
                //Point3d pointC;
                //if (obj.IsBeginExactly && obj.IsEndExactly)
                //{
                // var l = obj.Length - 2 * ExactlyIncrease;
                // h = (obj.Diameter - Math.Sqrt(obj.Diameter * obj.Diameter - l * l)) / 2;
                // pointC = obj.ProcessCurve.GetPointAtParameter(obj.ProcessCurve.EndParam / 2);
                //}
                //else
                //{
                // h = obj.DepthAll;
                // pointC = obj.ProcessCurve.StartPoint + obj.ProcessCurve.GetFirstDerivative(0).GetNormal() * (obj.IsBeginExactly ? s : obj.Length - s);
                //}
            }

            bool oddPassCount = false;
            Calculate(false);
            Corner startCorner = Corner.Start;
            switch (ProcessingArea.Curve)
            {
                case Line line:
                    startCorner = (line.Angle > 0 && line.Angle <= Math.PI) ^ oddPassCount ? Corner.End : Corner.Start;
                    break;

                case Arc arc:
                    startCorner = (arc.StartAngle >= 0.5 * Math.PI && arc.StartAngle < 1.5 * Math.PI) ^ oddPassCount ? Corner.Start : Corner.End;
                    break;
            }
            builder.StartTechOperation(ProcessingArea.Curve, startCorner);
            TechOperationParams.Compensation = builder.CalcCompensation(border.OuterSide, thickness);
            Calculate(true);

            ProcessCommands = builder.FinishTechOperation();

            void Calculate(bool buildMode)
            {
                var modes = TechOperationParams.Modes.OrderBy(p => p.Depth).GetEnumerator();
                if (!modes.MoveNext())
                    throw new InvalidOperationException("Не заданы режимы обработки");
                var mode = modes.Current;
                SawingMode nextMode = null;
                if (modes.MoveNext())
                    nextMode = modes.Current;
                var z = TechOperationParams.IsFirstPassOnSurface ? -mode.DepthStep : 0;
                do
                {
                    z += mode.DepthStep;
                    if (nextMode != null && z >= nextMode.Depth)
                    {
                        mode = nextMode;
                        nextMode = modes.MoveNext() ?  modes.Current : null;
                    }
                    if (z > thickness)
                        z = thickness;
                    if (buildMode)
                        builder.Cutting(mode.Feed, -z);

                    oddPassCount = !oddPassCount;
                }
                while (z < thickness);
                modes.Dispose();
            }
        }
	}
}
