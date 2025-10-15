using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM.CncWorkCenter
{
    public abstract class PostProcessorCnc : PostProcessorBase
    {
        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);
        public virtual string Cycle() => null;

        public bool WithThick { get; set; }

        public string GCommand(int gCode, Point3d? point, double? angleC, double? angleA, int? feed, Point2d? arcCenter)
        {
            var @params = GetParams(gCode, point, angleC, angleA, feed, arcCenter);
            return GetGCommand(@params);
        }

        protected List<CommandParam> GetParams(int gCode, Point3d? point, double? angleC, double? angleA, int? feed, Point2d? arcCenter)
        {
            var commandParams = new List<CommandParam>
            {
                new CommandParam('G', gCode.ToString()),
                new CommandParam('F', feed?.ToString()),
                new CommandParam('A', angleA?.ToStringParam()),
                new CommandParam('C', angleC?.ToStringParam())
            };

            if (point.HasValue)
            {
                commandParams.Add(new CommandParam('X', (point.Value.X - Origin.X).ToStringParam()));
                commandParams.Add(new CommandParam('Y', (point.Value.Y - Origin.Y).ToStringParam()));
                var zParam = point.Value.Z.ToStringParam();
                if (WithThick)
                    zParam = $"({zParam} + THICK)";
                commandParams.Add(new CommandParam('Z', zParam));
            }

            if (arcCenter.HasValue)
            {
                commandParams.Add(new CommandParam('I', (arcCenter.Value.X - Origin.X).ToStringParam()));
                commandParams.Add(new CommandParam('J', (arcCenter.Value.Y - Origin.Y).ToStringParam()));
            }

            return commandParams;
        }
    }
}
