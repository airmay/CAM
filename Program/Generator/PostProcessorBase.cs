using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public abstract class PostProcessorBase : IPostProcessor
    {
        public class Param
        {
            public Param(char code, string value)
            {
                Code = code;
                Value = value;
            }
            public char Code { get; set; }
            public string Value { get; set; }
        }

        public Dictionary<char, string> Params { get; } = new Dictionary<char, string>();
        protected virtual string ParamCodes => "GXYZCAIJF";
        public Point2d Origin { get; set; } = Point2d.Origin;
        public bool WithThick { get; set; }

        #region GCommand

        public virtual string GCommand(Param[] @params)
        {
            var changedParams = GetChangedParams(@params);
            var command = CreateCommand(changedParams);
            ApplyParams(@params);
            return command;
        }

        public virtual IEnumerable<Param> GetChangedParams(IEnumerable<Param> @params)
        {
            return @params.Where(p => p.Value != null && (!Params.TryGetValue(p.Code, out var value) || value != p.Value));
        }

        protected virtual string CreateCommand(IEnumerable<Param> @params)
        {
            return string.Join(" ", @params.Select(p => $"{p.Code}{p.Value}"));
        }

        public virtual void ApplyParams(IEnumerable<Param> @params)
        {
            foreach (var param in @params)
                Params[param.Code] = param.Value;
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