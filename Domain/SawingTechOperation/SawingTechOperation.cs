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
            builder.StartTechOperation(ProcessingArea.Curve, CalcStartCorner());

            var indentSum = builder.CalcIndent(((BorderProcessingArea)ProcessingArea).IsExactlyBegin, ((BorderProcessingArea)ProcessingArea).IsExactlyEnd, TechProcess.TechProcessParams.BilletThickness);
	        if (indentSum >= ProcessingArea.Curve.Length())
	        {
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

	        TechOperationParams.Compensation = builder.CalcCompensation(((BorderProcessingArea)ProcessingArea).OuterSide, TechProcess.TechProcessParams.BilletThickness);

			var modes = TechOperationParams.Modes.OrderBy(p => p.Depth).GetEnumerator();
            modes.MoveNext();
            var mode = modes.Current;
	        Debug.Assert(mode != null, nameof(mode) + " != null");
	        var z = TechOperationParams.IsFirstPassOnSurface ? -mode.DepthStep : 0;
            var billetThickness = TechProcess.TechProcessParams.BilletThickness;
            do
            {
                z += mode.DepthStep;
                if (z > mode.Depth && modes.MoveNext())
                    mode = modes.Current;
                if (z > billetThickness)
                    z = billetThickness;
                builder.Cutting(mode.Feed, -z);
            }
            while (z < billetThickness);
			modes.Dispose();
            ProcessCommands = builder.FinishTechOperation();
        }

	    private Corner CalcStartCorner()
	    {
		    var modes = TechOperationParams.Modes.OrderBy(p => p.Depth).GetEnumerator();
		    modes.MoveNext();
		    var mode = modes.Current;
		    Debug.Assert(mode != null, nameof(mode) + " != null");
		    var z = TechOperationParams.IsFirstPassOnSurface ? -mode.DepthStep : 0;
		    var passCount = 0;
		    do
		    {
			    z += mode.DepthStep;
			    if (z > mode.Depth && modes.MoveNext())
				    mode = modes.Current;
			    passCount++;
		    }
		    while (z < TechProcess.TechProcessParams.BilletThickness);
		    modes.Dispose();

		    switch (ProcessingArea.Curve)
		    {
			    case Line line:
				    return (line.Angle > 0 && line.Angle <= Math.PI) ^ (passCount % 2 == 1) ? Corner.End : Corner.Start;

			    case Arc arc:
				    return (arc.StartAngle >= 0.5 * Math.PI && arc.StartAngle < 1.5 * Math.PI) ^ (passCount % 2 == 1) ? Corner.Start : Corner.End;

			    default:
				    return Corner.Start;
		    }
	    }
	}
}
