﻿using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public interface ITechProcess
    {
        int Frequency { get; set; }
        int PenetrationFeed { get; set; }
        MachineType? MachineType { get; set; }
        Material? Material { get; set; }
        double? Thickness { get; set; }
        double ZSafety { get; set; }
        string Caption { get; set; }
        double OriginX { get; set; }
        double OriginY { get; set; }
        ObjectId[] OriginObject { get; set; }
        List<ProcessCommand> ProcessCommands { get; set; }
        List<AcadObject> ProcessingArea { get; set; } 
        List<ITechOperation> TechOperations { get; }
        Tool Tool { get; set; }

        Dictionary<ObjectId, int> ToolpathObjectIds { get; set; }

        void BuildProcessing();

        bool TechOperationMoveDown(ITechOperation techOperation);
        bool TechOperationMoveUp(ITechOperation techOperation);

        List<ITechOperation> CreateTechOperations();

        bool Validate();

        void Setup();

        void Teardown();

        void SkipProcessing(ProcessCommand processCommand);

        ObjectId? ToolpathObjectsGroup { get; set; }

        ObjectId? ExtraObjectsGroup { get; set; }
    }
}