using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    [Serializable]
    public class ProcessCommand
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string GCode { get; set; }

        public string MCode { get; set; }

        public string Param1 { get; set; }

        public string Param2 { get; set; }

        public string Param3 { get; set; }

        public string Param4 { get; set; }

        public string Param5 { get; set; }

        //public string Param6 { get; set; }

        /// <summary>
        /// Идентификатор графического примитива автокада представляющего траекторию инструмента
        /// </summary>
        public readonly ObjectId ToolpathCurveId;

        /// <summary>
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public readonly Curve ToolpathCurve;

        private List<string> par;
        private string v;

        public ProcessCommand(string name, int number, string gCode, string mCode, string param1, string param2, string param3, string param4, string param5, Curve toolpathCurve)
        {
            Name = name;
            Number = number;
            GCode = gCode;
            MCode = mCode;
            ToolpathCurve = toolpathCurve;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
        }

        public override string ToString() => $"{Number};{GCode};{MCode};{Param1};{Param2};{Param3};{Param4};{Param5};";

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
