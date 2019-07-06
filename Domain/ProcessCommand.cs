using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public int GCode { get; set; }

        public int? MCode { get; set; }

        public string Axis { get; set; }

        public int? Feed { get; set; }

        public double? X { get; set; }

        public double? Y { get; set; }

        public double? Param1 { get; set; }

        public double? Param2 { get; set; }

        /// <summary>
        /// Идентификатор графического примитива автокада представляющего траекторию инструмента
        /// </summary>
        public ObjectId ToolpathCurveId { get; set; }

        /// <summary>
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public Curve ToolpathCurve { get; set; }

        public string ProgrammLine => $"{Number};{GCode};{MCode};{Axis};{Feed};{X};{Y};{Param1};{Param2};";

        //public ProcessCommand(string name, string gCode, Curve toolpathCurve = null, params string[] @params)
        //{
        //    Name = name;
        //    GCode = gCode;
        //    ToolpathCurve = toolpathCurve;
        //    Param1 = @params.Length > 0 ? @params[0] : null;
        //    Param2 = @params.Length > 1 ? @params[1] : null;
        //    Param3 = @params.Length > 2 ? @params[2] : null;
        //    Param4 = @params.Length > 3 ? @params[3] : null;
        //    Param5 = @params.Length > 4 ? @params[4] : null;
        //}
    }
}
