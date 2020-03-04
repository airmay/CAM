using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public interface ITechProcess
    {
        int Frequency { get; set; }
        MachineSettings MachineSettings { get; }
        MachineType MachineType { get; set; }
        Material Material { get; set; }
        string Caption { get; set; }
        double OriginX { get; set; }
        double OriginY { get; set; }
        ObjectId[] OriginObject { get; set; }
        List<ProcessCommand> ProcessCommands { get; set; }
        ProcessingArea ProcessingArea { get; set; } 
        List<ITechOperation> TechOperations { get; }
        Tool Tool { get; set; }
        ToolObject ToolObject { get; set; }
        IEnumerable<Curve> ToolpathCurves { get; }

        void BuildProcessing();
        void DeleteProcessCommands();

        bool TechOperationMoveDown(ITechOperation techOperation);
        bool TechOperationMoveUp(ITechOperation techOperation);

        List<ITechOperation> CreateTechOperations();

        bool Validate();

        void Setup(Settings settings);

        void Teardown();
    }
}