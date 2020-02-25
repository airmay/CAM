using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Интрефейс технологической операции
    /// </summary>
    public interface ITechOperation : IHasProcessCommands
    {
        ITechProcess TechProcess { get; set; }

        string Caption { get; set; }

        IEnumerable<Curve> ToolpathCurves { get; }

        void BuildProcessing(ScemaLogicProcessBuilder builder);

        void SetToolpathVisible(bool visible);
    }
}