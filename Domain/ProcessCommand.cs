using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM.Domain
{
    public class ProcessCommand
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Param1 { get; set; }

        public string Param2 { get; set; }

        public string Param3 { get; set; }

        public string Param4 { get; set; }

        public string Param5 { get; set; }

        public string Param6 { get; set; }

        /// <summary>
        /// Идентификатор графического примитива автокада представляющего траекторию инструмента
        /// </summary>
        public readonly ObjectId ToolpathAcadObjectId;

        /// <summary>
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public readonly Curve ToolpathAcadObject;
        private List<string> par;
        private string v;

        public ProcessCommand(string name, string code, Curve toolpathAcadObject = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null)
        {
            Name = name;
            Code = code;
            ToolpathAcadObject = toolpathAcadObject;
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
        }

        //public ProcessCommand(string name, string code, Curve toolpathAcadObject = null, params string[] @params)
        //{
        //    Name = name;
        //    Code = code;
        //    ToolpathAcadObject = toolpathAcadObject;
        //    Param1 = @params.Length > 0 ? @params[0] : null;
        //    Param2 = @params.Length > 1 ? @params[1] : null;
        //    Param3 = @params.Length > 2 ? @params[2] : null;
        //    Param4 = @params.Length > 3 ? @params[3] : null;
        //    Param5 = @params.Length > 4 ? @params[4] : null;
        //}
    }
}
