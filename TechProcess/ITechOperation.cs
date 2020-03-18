﻿using Autodesk.AutoCAD.DatabaseServices;
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

        AcadObject ProcessingArea { get; set; }

        IEnumerable<ObjectId> ToolpathObjectIds { get; }

        void BuildProcessing(ICommandGenerator generator);

        void SetToolpathVisible(bool visible);

        void Setup(ITechProcess techProcess);

        void Teardown();
    }
}