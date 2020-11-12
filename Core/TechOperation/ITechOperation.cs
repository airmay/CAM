using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace CAM
{
    /// <summary>
    /// Интрефейс технологической операции
    /// </summary>
    public interface ITechOperation
    {
        ITechProcess TechProcess { get; }

        string Caption { get; set; }

        AcadObject ProcessingArea { get; set; }

        ObjectId? ToolpathObjectsGroup { get; set; }

        void BuildProcessing(CommandGeneratorBase generator);

        void PrepareBuild(CommandGeneratorBase generator);

        void Setup(ITechProcess techProcess);

        void Teardown();

        bool Enabled { get; set; }

        bool CanProcess { get; }

        bool Validate();

        int? ProcessCommandIndex { get; set; }
    }
}