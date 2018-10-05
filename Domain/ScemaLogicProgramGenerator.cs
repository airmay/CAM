using System;
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
        public string Generate(TechProcess techProcess)
        {
            var program = new StringBuilder();
            //program.AppendLine($"; Программа обработки для станка Denver {DateTime.Now}");
            program.AppendLine("98");
            program.AppendLine("97 2 1");
            program.AppendLine("17 XYCZ");
            program.AppendLine("28 XYCZ");
            program.AppendLine($"97 6 {techProcess.TechProcessParams.Tool.Number}");
            program.AppendLine("");
            program.AppendLine("");

            return program.ToString();
        }
    }
}