using System;
using System.Collections.Generic;
using System.Text;

namespace CAM.Domain
{
    /// <summary>
    /// Генератор программы для станка типа ScemaLogic
    /// </summary>
    public class ScemaLogicProgramGenerator
    {
        /// <summary>
        /// Генерирует программу по техпроцессу
        /// </summary>
        /// <param name="techProcess"></param>
        /// <returns></returns>
        public List<ProgramLine> Generate(TechProcess techProcess)
        {
            var program = new List<ProgramLine>();
            void AddLine(string s1, string s2 = null, string s3 = null, string s4 = null, string s5 = null) => program.Add(new ProgramLine(s1, s2, s3, s4, s5));

            AddLine($"; Программа обработки для станка Denver {DateTime.Now}");
            AddLine("98");
            AddLine("97", "2", "1");
            AddLine("17", "XYCZ");
            AddLine("28", "XYCZ");
            AddLine($"97", "6", techProcess.TechProcessParams.Tool.Number.ToString());
            AddLine("");
            AddLine("");

            //Point3d currentPoint = Point3d.Origin;
            //foreach (var action in techProcess.TechOperations.ProcessActions)
            //{

            //}
            return program;
        }
    }
}