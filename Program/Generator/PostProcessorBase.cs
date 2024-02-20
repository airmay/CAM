using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;

namespace CAM.Program.Generator
{
    public abstract class PostProcessorBase : IPostProcessor
    {
        public Dictionary<char, CommandParam> CommandParams { get; }
        protected virtual string ParamCodes => "GXYZCAIJF";
        public Point2d Origin { get; set; } = Point2d.Origin;
        public bool WithThick { get; set; }

        protected PostProcessorBase()
        {
            CommandParams = ParamCodes.ToCharArray().ToDictionary(p => p, p => new CommandParam());
        }

        #region GCommand
        public virtual string GCommand(int gCode, Point3d position, double angleA, double angleC, int? feed, Point2d? arcCenter)
        {
            var @params = GetParams(gCode, position, angleA, angleC, feed, arcCenter);
            Apply(@params);
            return CreateCommand();
        }

        protected virtual IEnumerable<(char, string)> GetParams(int gCode, Point3d position, double angleA, double angleC, int? feed, Point2d? arcCenter)
        {
            return new List<(char, string)>
            {
                ('G', gCode.ToString()),
                ('F', feed?.ToString()),
                ('X', !double.IsNaN(position.X) ? (position.X - Origin.X).ToStringParam() : null),
                ('Y', !double.IsNaN(position.Y) ? (position.Y - Origin.Y).ToStringParam() : null),
                ('Z', WithThick ? $"({position.Z.ToStringParam()} + THICK)" : position.Z.ToStringParam()),
                ('A', angleA.ToStringParam()),
                ('C', angleC.ToStringParam()),
                ('I', arcCenter.HasValue ? (arcCenter.Value.X - Origin.X).ToStringParam() : null),
                ('J', arcCenter.HasValue ? (arcCenter.Value.Y - Origin.Y).ToStringParam() : null),
            };
        }

        protected virtual void Apply(IEnumerable<(char Code, string Value)> @params)
        {
            CommandParams.Values.ToList().ForEach(p => p.IsChanged = false);
            foreach (var (code, value) in @params.Where(item => item.Value != null && item.Value != CommandParams[item.Code].Value))
            {
                CommandParams[code].Value = value;
                CommandParams[code].IsChanged = true;
            }
        }

        protected virtual string CreateCommand()
        {
            return string.Join(" ", 
                CommandParams.Where(p => p.Value.IsChanged).Select(p => $"{p.Key}{p.Value.Value}"));
        } 
        #endregion

        public virtual string Cycle() => null;

        public abstract string[] StartMachine();

        public abstract string[] StopMachine();

        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);

        public abstract string[] StartEngine(int frequency, bool hasTool);

        public abstract string[] StopEngine();

        public abstract string Pause(double duration);
    }
}