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
        protected virtual string ParamCodes => "GXYZCAIJF";

        public Point2d Origin { get; set; }
        public CommandParams CommandParams { get; set; }
        public bool WithThick { get; set; }

        protected PostProcessorBase()
        {
            CommandParams = new CommandParams(ParamCodes);
        }

        public string GCommand(int gCode, Point3d position, double angleA, double angleC, int feed, Point2d? arcCenter)
        {
            var @params = GetParams(gCode, position, angleA, angleC, feed, arcCenter);
            var changedParams = CommandParams.Apply(@params);
            return CreateCommand(changedParams);
        }

        //public string GCommand(int gCode, MillToolPosition position, int? feed, Point2d? arcCenter = null)
        //{
        //    var newParams = CreateParams(gCode, position, feed, arcCenter);
        //    var changedParams = GetChangedParams(newParams);
        //    var command = CreateCommand(changedParams);
        //    ApplyParams(newParams);

        //    return command;
        //}

        //private void ApplyParams(CommandParams newParams)
        //{
        //    ApplyParams(newParams.Params);
        //}

        //public string CreateParams(int gCode, MillToolPosition position, int feed, Point2d? arcCenter)
        //{
        //    return null;
        //}

        public Dictionary<char, string> GetParams(int gCode, Point3d position, double angleA, double angleC, int feed, Point2d? arcCenter)
        {
            var @params = new Dictionary<char, double?>
                {
                    ['G'] = gCode,
                    ['F'] = feed,
                    ['X'] = position.X - Origin.X,
                    ['Y'] = position.Y - Origin.Y,
                    ['Z'] = position.Z,
                    ['A'] = angleA,
                    ['C'] = angleC,
                    ['I'] = arcCenter?.X,
                    ['J'] = arcCenter?.Y
                }
                .ToDictionary(p => p.Key, p => p.Value.ToParam());
            if (WithThick)
                @params['Z'] = $"({@params['Z']} + THICK)";

            return @params;
        }

        //private CommandParams CreateParams(MillToolPosition position)
        //{
        //    var commandParams = new CommandParams();
        //    commandParams.Set('X', position.X, Origin.X);
        //    commandParams.Set('Y', position.Y, Origin.Y);
        //    if (WithThick)
        //        commandParams.Set('Z', position.Z, "({0} + THICK)");
        //    else
        //        commandParams.Set('Z', position.Z);
        //    commandParams.Set('C', position.AngleC);
        //    commandParams.Set('A', position.AngleA);

        //    return commandParams;
        //}

        //public IEnumerable<KeyValuePair<char, string>> GetChangedParams(CommandParams newParams)
        //{
        //    return newParams.Params.Where(p => !Params.TryGetValue(p.Key, out var value) || p.Value != value);
        //}

        public string CreateCommand(List<CommandParam> @params)
        {
            return string.Join(CommandDelimiter, @params.Select(p => $"{p.Code}{p.Value}"));
        }

        //public string CreateCommand(IEnumerable<KeyValuePair<char, string>> par)
        //{
        //    return string.Join(CommandDelimiter, par.Select(p => $"{p.Key}{p.Value}"));
        //}

        //private void ApplyParams(Dictionary<char, string> newParams)
        //{
        //    newParams.ForEach(p => Params[p.Key] = p.Value);
        //}

        //public void SetParams(MillToolPosition position)
        //{
        //    var @params = CreateParams(position);
        //    ApplyParams(@params);
        //}

        public virtual string Cycle() => null;

        public abstract string[] Finish();

        public abstract string[] StartMachine();

        public abstract string[] StopMachine();

        public abstract string[] SetTool(int toolNo, double angleA, double angleC, int originCellNumber);

        public abstract string[] StartEngine(int frequency, bool hasTool);

        public abstract string[] StopEngine();

        public abstract string Pause(double duration);
    }
}