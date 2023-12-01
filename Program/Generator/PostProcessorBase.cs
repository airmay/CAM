using System;
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

        public string GCommand(int gCode, MillToolPosition position, int? feed = null, Point2d? arcCenter = null)
        {
            var newParams = CreateParams(gCode, position, feed, arcCenter);
            var changedParams = GetChangedParams(newParams);
            var command = CreateCommand(changedParams);
            ApplyParams(newParams);

            return command;
        }

        private void ApplyParams(CommandParams newParams)
        {
            ApplyParams(newParams.Params);
        }

        public CommandParams CreateParams(int gCode, MillToolPosition position, int? feed, Point2d? arcCenter = null)
        {
            var commandParams = CreateParams(position);

            commandParams.Set('G', gCode);
            if (feed.HasValue)
                commandParams.Set('F', feed);
            if (arcCenter.HasValue)
            {
                commandParams.Set('I', arcCenter.Value.X);
                commandParams.Set('J', arcCenter.Value.Y);
            }

            return commandParams;
        }

        private CommandParams CreateParams(MillToolPosition position)
        {
            var commandParams = new CommandParams();
            commandParams.Set('X', position.X, Origin.X);
            commandParams.Set('Y', position.Y, Origin.Y);
            if (WithThick)
                commandParams.Set('Z', position.Z, "({0} + THICK)");
            else
                commandParams.Set('Z', position.Z);
            commandParams.Set('C', position.AngleC);
            commandParams.Set('A', position.AngleA);

            return commandParams;
        }

        public IEnumerable<KeyValuePair<char, string>> GetChangedParams(CommandParams newParams)
        {
            return newParams.Params.Where(p => !Params.TryGetValue(p.Key, out var value) || p.Value != value);
        }

        public string CreateCommand(IEnumerable<KeyValuePair<char, string>> par)
        {
            return string.Join(CommandDelimiter, par.Select(p => $"{p.Key}{p.Value}"));
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

        public virtual string Cycle() => null;

        public abstract string[] StartMachine();

        public abstract string[] StopMachine();

        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);

        public abstract string[] StartEngine(int frequency, bool hasTool);

        public abstract string[] StopEngine();

        public abstract string Pause(double duration);
    }
}