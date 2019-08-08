using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    public class ProcessCommand
    {
        public string Number { get; set; }

        public string Name { get; set; }

        public string GCode { get; set; }

        public string MCode { get; set; }

        public string Axis { get; set; }

        public string Feed { get; set; }

        public string X { get; set; }

        public string Y { get; set; }

        public string Param1 { get; set; }

        public string Param2 { get; set; }

        public ProcessCommand(Curve toolpathCurve) => _toolpathCurve = toolpathCurve;

        public Curve _toolpathCurve;

        /// <summary>
        /// Графический примитив автокада представляющего траекторию инструмента
        /// </summary>
        public Curve GetToolpathCurve() => _toolpathCurve;

        public string GetProgrammLine() => $"{Number};{GCode};{MCode};{Axis};{Feed};{X};{Y};{Param1};{Param2};";
    }
}
