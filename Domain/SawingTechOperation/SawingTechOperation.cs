using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    /// <summary>
    /// Технологическая операция "Распиловка"
    /// </summary>
    public class SawingTechOperation : TechOperation
    {
	    const double AngleTolerance = 0.000001;

	    /// <summary>
        /// Вид технологической операции
        /// </summary>
        public override TechOperationType Type { get; } = TechOperationType.Sawing;

	    /// <summary>
        /// Параметры технологической операции
        /// </summary>
        public SawingTechOperationParams TechOperationParams { get; }

	    public Side OuterSide { get; set; }


	    public SawingTechOperation(TechProcess techProcess, ProcessingArea processingArea, SawingTechOperationParams techOperationParams)
            : base(techProcess, processingArea)
        {
            TechOperationParams = techOperationParams;
            Name = $"Распил { processingArea }";

	        CalcExactlyEnd(Corner.Start);
	        CalcExactlyEnd(Corner.End);
        }

	    public void CalcExactlyEnd(Corner corner)
	    {
		    var point = ProcessingArea.Curve.GetPoint(corner);
		    var nextOperation = TechProcess.TechOperations.OfType<SawingTechOperation>().SingleOrDefault(p => p != this && (p.ProcessingArea.Curve.StartPoint == point || p.ProcessingArea.Curve.EndPoint == point));
			if (nextOperation == null)
				return;

		    var nextCorner = nextOperation.ProcessingArea.Curve.GetCorner(point);
            OuterSide = nextOperation.OuterSide != Side.None
                    ? (corner != nextCorner ? nextOperation.OuterSide : nextOperation.OuterSide.Swap())
                    : CalcOuterSide();

            bool isExactly = false;
            switch (ProcessingArea.Curve)
            {
                case Line line:
                    bool isLeftTurn;
                    switch (nextOperation.ProcessingArea.Curve)
                    {
                        case Line nextLine:
                            var angleDiff = nextLine.Angle - line.Angle;
                            if (Math.Abs(angleDiff) > AngleTolerance)
                            {
                                isLeftTurn = Math.Sin(angleDiff) > 0;
                                var isLeftOuterSide = OuterSide == Side.Left;
                                var isNextStartPoint = nextCorner == Corner.Start;
                                isExactly = isLeftTurn ^ isLeftOuterSide ^ isNextStartPoint;
                            }
                            break;
                        case Arc nextArc:
                            var angleTan = nextCorner == Corner.Start ? nextArc.StartAngle + Math.PI / 2 : nextArc.EndAngle - Math.PI / 2;
                            angleDiff = angleTan - line.Angle;
                            isLeftTurn = Math.Abs(angleDiff) > AngleTolerance
                                ? Math.Sin(angleDiff) > 0
                                : nextCorner == Corner.Start;
                            var isRightProcessSide = OuterSide == Side.Right;
                            isExactly = isLeftTurn ^ isRightProcessSide;
                            break;
                    }
                    break;
                case Arc arc:
                    if (nextOperation.ProcessingArea.Curve is Line)
                    {
                        nextOperation.CalcExactlyEnd(nextCorner);
                        return;
                    }
                    break;
            }
            SetExactly(nextCorner, isExactly);
            nextOperation.SetExactly(nextCorner, isExactly);
        }

        private Side CalcOuterSide()
        {
            return Side.Right;
        }

        public void SetExactly(Corner corner, bool isExectly)
        {
            if (corner == Corner.Start)
                TechOperationParams.IsExactlyBegin = isExectly;
            else
                TechOperationParams.IsExactlyEnd = isExectly;
        }

        public override Point3d BuildProcessing(Point3d startPoint, bool isLast)
        {
			var builder = new ScemaLogicProcessBuilder(TechProcess.TechProcessParams, startPoint, ProcessingArea.Curve, CalcStartCorner());

	        var indentSum = builder.CalcIndent(TechOperationParams.IsExactlyBegin, TechOperationParams.IsExactlyEnd, TechProcess.TechProcessParams.BilletThickness);
	        if (indentSum >= ProcessingArea.Curve.Length)
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
				return Point3d.Origin;
			}

	        TechOperationParams.Compensation = builder.CalcCompensation(OuterSide, TechProcess.TechProcessParams.BilletThickness);

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
            var currentPoint = builder.Completion(isLast);
            ProcessCommands = builder.Commands;

	        return currentPoint;
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
