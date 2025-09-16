using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.CncWorkCenter;
using Dreambuild.AutoCAD;
using System;

namespace CAM.MachineWireSaw
{
    public class ProcessorWireSaw : ProcessorBase<ProcessingWireSaw, ProcessorWireSaw>
    {
        private readonly PostProcessorWireSaw _postProcessor;
        protected override PostProcessorBase PostProcessor => _postProcessor;

        private Vector2d _uAxis;
        private double _u, _v;
        private Point2d Center => Processing.Origin.Point;

        public override void StartOperation(double? zMax = null)
        {
            base.StartOperation(zMax);
            _v = UpperZ;
        }

        public ProcessorWireSaw()
        {
            _postProcessor = new PostProcessorWireSaw();
            AngleC = Math.PI / 2;
            _uAxis = -Vector2d.XAxis;
            _u = 0;
        }

        #region public

        public void Move(Point3d point, Vector3d direction, bool isReverseAngle = false, bool isReverseU = false)
        {
            Move(point, point + direction, isReverseAngle, isReverseU);
        }

        public void Move(Point3d point1, Point3d point2, bool isReverseAngle = false, bool isReverseU = false)
        {
            if (!IsUpperTool)
                return;

            var z = (point1.Z + point2.Z) / 2;
            var line = new Line2d(point1.ToPoint2d(), point2.ToPoint2d());

            if (UpperZ - z > 0.001)
            {
                GCommands(0, line, UpperZ, isReverseAngle, isReverseU);
                isReverseAngle = false;
                isReverseU = false;
            }

            GCommands(0, line, z, isReverseAngle, isReverseU);
        }

        public void Cutting(Point3d point, Vector3d direction) => Cutting(point, point + direction);
        
        public void Cutting(Line3d line) => Cutting(line.StartPoint, line.EndPoint);

        public void Cutting(Point3d point1, Point3d point2)
        {
            if (!IsEngineStarted)
            {
                AddCommands(_postProcessor.StartEngine());
                IsEngineStarted = true;
            }
            GCommands(1, point1, point2, false, false);
        }

        #endregion

        private void GCommands(int gCode, Point3d point1, Point3d point2, bool isReverseAngle, bool isReverseU)
        {
            GCommands(gCode, new Line2d(point1.ToPoint2d(), point2.ToPoint2d()), (point1.Z + point2.Z) / 2, isReverseAngle, isReverseU);
        }

        private void GCommands(int gCode, Line2d line, double z, bool isReverseAngle, bool isReverseU)
        {
            var point = line.GetClosestPointTo(Center).Point;
            var centerVector = point - Center;
            var centerDist = centerVector.Length.Round(3);
            if (centerDist == 0)
                centerVector = line.Direction.GetPerpendicularVector();
            var uSign = centerVector.DotProduct(_uAxis).GetSign();
            uSign *= isReverseU.GetSign(-1);

            GCommandA(centerVector * uSign, isReverseAngle, line.Direction.Angle);

            GCommandUV(gCode, centerDist * uSign, z, point);
        }

        private void GCommandA(Vector2d vector, bool isReverseAngle, double newToolAngle)
        {
            // + это поворот стола По часовой
            var da = _uAxis.MinusPiToPiAngleTo(vector).ToRoundDeg();
            if (da == 0) 
                return;

            if (isReverseAngle)
                da -= 360 * da.GetSign();

            var daRad = da.ToRad();
            var toolVector = Vector3d.XAxis.RotateBy(AngleC, Vector3d.ZAxis) * ToolObject.WireSawLength;
            var duration = Math.Abs(da) / Processing.S * 60;
            AddCommand($"G05 A{da} S{Processing.S}", angleC: newToolAngle, duration: duration, toolpath1: CreateToolpath(toolVector), toolpath2: CreateToolpath(-toolVector));

            _uAxis = _uAxis.RotateBy(daRad);

            return;

            ObjectId? CreateToolpath(Vector3d vector1)
            {
                return ToolpathBuilder.AddToolpath(NoDraw.ArcSCA(ToolPoint + vector1, Center.WithZ(ToolPoint.Z), daRad), 1);
            }
        }

        private void GCommandUV(int gCode, double u, double v, Point2d point)
        {
            var du = (u - _u).Round(3);
            var dv = (v - _v).Round(3);
            if (du == 0 && dv == 0)
                return;

            _u += du;
            _v += dv;

            var commandText = $"G0{gCode} U{du} V{dv}";
            if (gCode == 1)
                commandText += $" F{Processing.CuttingFeed}";

            var toolVector = Vector3d.XAxis.RotateBy(AngleC, Vector3d.ZAxis) * ToolObject.WireSawLength;
            var newToolPoint = point.WithZ(v);
            var toolpath1 = CreateToolpath(toolVector);
            var toolpath2 = CreateToolpath(-toolVector);

            var duration = Math.Sqrt(du * du + dv * dv) / (gCode == 0 ? 500 : Processing.CuttingFeed) * 60;
            AddCommand(commandText, point: newToolPoint, duration: duration, toolpath1: toolpath1, toolpath2: toolpath2);
            
            return;

            ObjectId? CreateToolpath(Vector3d vector)
            {
                return ToolpathBuilder.AddToolpath(NoDraw.Line(ToolPoint + vector, newToolPoint + vector), gCode);
            }
        }
    }
}