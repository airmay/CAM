using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    public interface ITechProcess
    {
        string Caption { get; set; }

        MachineType? MachineType { get; set; }

        Tool Tool { get; set; }

        IEnumerable<TechOperation> TechOperations { get; }

        List<ProcessCommand> ProcessCommands { get; }

        Dictionary<ObjectId, int> GetToolpathObjectIds();

        List<TechOperation> CreateTechOperations();

        bool Validate();

        void DeleteProcessing();

        ObjectId? GetToolpathObjectsGroup();

        void SkipProcessing(ProcessCommand processCommand);

        ObjectId? GetExtraObjectsGroup();

        void BuildProcessing();

        void Teardown();

        void SerializeInit();

        double ZSafety { get; set; }

        double OriginX { get; set; }

        double OriginY { get; set; }
    }
}