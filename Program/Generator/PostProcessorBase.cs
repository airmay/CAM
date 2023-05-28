using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;

namespace CAM.Program.Generator
{
    public abstract class PostProcessorBase : IPostProcessor
    {
        protected string CommandDelimiter = " ";
        protected string ParamTypes = "GXYZCAIJF";

        public Point2d Origin { get; set; }
        public Dictionary<char, string> Params { get; set; }
        public bool WithThick { get; set; }

        protected PostProcessorBase()
        {
            Params = ParamTypes.ToCharArray().ToDictionary(p => p, p => string.Empty);
        }

        public string GCommand(int gCode, MillToolPosition position, int feed, Point2d? arcCenter = null)
        {
            var newParams = CreateParams(gCode, position, feed, arcCenter);
            var changedParams = GetChangedParams(newParams);
            var command = CreateCommand(changedParams);
            ApplyParams(newParams);

            return command;
        }

        public string CreateCommand(IEnumerable<KeyValuePair<char, string>> par)
        {
            return string.Join(CommandDelimiter, par.Select(p => $"{p.Key}{p.Value}"));
        }

        public IEnumerable<KeyValuePair<char, string>> GetChangedParams(Dictionary<char, string> newParams)
        {
            return newParams.Where(p => Params.TryGetValue(p.Key, out var value) && p.Value != value);
        }

        public Dictionary<char, string> CreateParams(int gCode, MillToolPosition position, int feed, Point2d? arcCenter = null)
        {
            var @params = CreateParams(position);

            @params['G'] = gCode.ToString();
            @params['F'] = feed.ToString();
            if (arcCenter.HasValue)
            {
                @params['I'] = arcCenter.Value.X.ToStringParam();
                @params['J'] = arcCenter.Value.X.ToStringParam();
            }

            return @params;
        }

        private Dictionary<char, string> CreateParams(MillToolPosition position)
        {
            var @params = new Dictionary<char, string>();

            if (position.X.HasValue)
                @params['X'] = (position.X.Value - Origin.X).ToStringParam();
            if (position.Y.HasValue)
                @params['Y'] = (position.Y.Value - Origin.Y).ToStringParam();
            if (position.Z.HasValue)
                @params['Z'] = WithThick
                    ? position.Z.Value.ToStringParam()
                    : $"({position.Z.Value.ToStringParam()} + THICK)";
            @params['C'] = position.AngleC.ToStringParam();
            @params['A'] = position.AngleA.ToStringParam();

            return @params;
        }

        private void ApplyParams(Dictionary<char, string> newParams)
        {
            newParams.ForEach(p => Params[p.Key] = p.Value);
        }

        public void SetParams(MillToolPosition position)
        {
            var @params = CreateParams(position);
            ApplyParams(@params);
        }

        public abstract string[] StartMachine();

        public abstract string[] StopMachine();

        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);

        public abstract string[] StartEngine(int frequency, bool hasTool);

        public abstract string[] StopEngine();

        public abstract string Pause(double duration);
    }
}