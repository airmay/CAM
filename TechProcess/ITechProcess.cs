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
        int OriginX { get; set; }
        int OriginY { get; set; }
        List<ProcessCommand> ProcessCommands { get; set; }
        ProcessingArea ProcessingArea { get; set; }
        List<ITechOperation> TechOperations { get; }
        Tool Tool { get; set; }
        ToolModel ToolModel { get; set; }
        IEnumerable<Curve> ToolpathCurves { get; }

        void BuildProcessing();
        void DeleteProcessCommands();
        void Init(Settings settings);

//        ITechOperation[] CreateTechOperations(string techOperationName);
//        string[] GetTechOperationNames();
        bool TechOperationMoveDown(ITechOperation techOperation);
        bool TechOperationMoveUp(ITechOperation techOperation);
    }
}