using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;

namespace CAM.Sawing
{
    [Serializable]
    [TechOperation(1, TechProcessNames.Sawing)]
    public class SawingTechOperation : TechOperationBase
    {
        public bool IsExactlyBegin { get; set; }

        public bool IsExactlyEnd { get; set; }

        public Side OuterSide { get; set; }

        public double AngleA { get; set; }

        public List<SawingMode> SawingModes { get; set; }

        public SawingTechOperation(ITechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public SawingTechOperation(ITechProcess techProcess, Border border) : base(techProcess, $"Распиловка{border.ObjectId.GetDesc()}")
        {
            SetFromBorder(border);
        }

        public void SetIsExactly(Corner corner, bool value)
        {
            if (corner == Corner.Start)
                IsExactlyBegin = value;
            else
                IsExactlyEnd = value;
        }

        public void SetFromBorder(Border border)
        {
            ProcessingArea = new AcadObject(border.ObjectId);
            var par = ((SawingTechProcess)TechProcess).SawingTechProcessParams;
            SawingModes = (border.ObjectId.IsLine() ? par.SawingLineModes : par.SawingCurveModes).ConvertAll(x => x.Clone());
            OuterSide = border.OuterSide;
            IsExactlyBegin = border.IsExactlyBegin;
            IsExactlyEnd = border.IsExactlyEnd;
        }

        public override void BuildProcessing(ScemaLogicProcessBuilder builder)
        {
            var techProcess = (SawingTechProcess)TechProcess;
            var curve = ProcessingArea.GetCurve();
            var compensation = builder.CalcCompensation(curve, OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value, techProcess.MachineType == MachineType.Donatoni);
            var sumIndent = builder.CalcIndent(techProcess.Thickness.Value, techProcess.Tool.Diameter) * (Convert.ToInt32(IsExactlyBegin) + Convert.ToInt32(IsExactlyEnd));
            if (sumIndent < curve.Length())
            {
                var modes = SawingModes.ConvertAll(p => new CuttingMode { Depth = p.Depth, DepthStep = p.DepthStep, Feed = p.Feed });
                builder.LineCut(curve, modes, !ProcessingArea.ObjectId.IsLine(), IsExactlyBegin, IsExactlyEnd, OuterSide, techProcess.Thickness.Value, compensation, techProcess.PenetrationFeed, techProcess.Tool.Diameter);
                if (!IsExactlyBegin)
                    Acad.CreateGash(curve, curve.StartPoint, OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value);
                if (!IsExactlyEnd)
                    Acad.CreateGash(curve, curve.EndPoint, OuterSide, techProcess.Thickness.Value, techProcess.Tool.Diameter, techProcess.Tool.Thickness.Value);
            }
            else
            {
                var point = Scheduling(curve, compensation);
                var line = NoDraw.Line(curve.StartPoint, curve.EndPoint);
                var angle = builder.CalcToolAngle(line, curve.StartPoint, Side.Right);
                line.Dispose();
                builder.Cutting(point.X, point.Y, point.Z, angle, techProcess.PenetrationFeed);
            }
            ProcessCommands = builder.FinishTechOperation();
        }

        /// <summary>
        /// Намечание
        /// </summary>
        private Point3d Scheduling(Curve curve, double compensation)
        {
            var techProcess = (SawingTechProcess)TechProcess;
            var diam = techProcess.Tool.Diameter;
            var startPoint = curve.StartPoint;
            var endPoint = curve.EndPoint;
            const int CornerIndentIncrease = 5;
            var vector = endPoint - startPoint;
            var depth = techProcess.Thickness.Value;
            var point = Point3d.Origin;
            if (IsExactlyBegin && IsExactlyEnd)
            {
                var l = vector.Length - 2 * CornerIndentIncrease;
                depth = (diam - Math.Sqrt(diam * diam - l * l)) / 2;
                point = startPoint + vector / 2;
            }
            else
            {
                var indentVector = vector.GetNormal() * (Math.Sqrt(depth * (diam - depth)) + CornerIndentIncrease);
                point = IsExactlyBegin ? startPoint + indentVector : endPoint - indentVector;
                Acad.CreateGash(curve, IsExactlyBegin ? curve.EndPoint : curve.StartPoint, OuterSide, depth, diam, techProcess.Tool.Thickness.Value, point);
            }
            return point + vector.GetPerpendicularVector().GetNormal() * compensation - Vector3d.ZAxis * depth;
        }
    }
}
    