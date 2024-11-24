using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace CAM.CncWorkCenter
{
    public abstract class PostProcessorCnc : PostProcessorBase
    {
        public Point2d Origin { get; set; }
        public bool WithThick { get; set; }

        public virtual string GCommand(int gCode, ToolLocationCnc location, int? feed, Point2d? arcCenter)
        {
            var @params = GetParams(gCode, location, feed, arcCenter);
            return GCommand(@params);
        }

        public virtual Dictionary<char, string> GetParams(int gCode, ToolLocationCnc location, int? feed, Point2d? arcCenter)
        {
            return new Dictionary<char, string>
            {
                ['G'] = gCode.ToString(),
                ['F'] = feed?.ToString(),
                ['X'] = !double.IsNaN(location.X) ? (location.X - Origin.X).ToStringParam() : null,
                ['Y'] = !double.IsNaN(location.Y) ? (location.Y - Origin.Y).ToStringParam() : null,
                ['Z'] = WithThick ? $"({location.Z.ToStringParam()} + THICK)" : location.Z.ToStringParam(),
                ['A'] = location.AngleA.ToStringParam(),
                ['C'] = location.AngleC.ToStringParam(),
                ['I'] = arcCenter.HasValue ? (arcCenter.Value.X - Origin.X).ToStringParam() : null,
                ['J'] = arcCenter.HasValue ? (arcCenter.Value.Y - Origin.Y).ToStringParam() : null,
            };
        }

        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);

    }
}
