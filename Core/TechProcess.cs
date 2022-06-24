using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM
{
    //public abstract class MillingTechProcess : TechProcess<MillingCommandGenerator>
    //{ }

    public interface ITechProcess
    {
        string Caption { get; set; }

        MachineType? MachineType { get; set; }

        Tool Tool { get; set; }

        IEnumerable<TechOperation> TechOperations { get; }

        void AddTechOperation(TechOperation techOperation);
        void RemoveTechOperation(int index);
        bool MoveForwardTechOperation(int index);
        bool MoveBackwardTechOperation(int index);

        List<ProcessCommand> ProcessCommands { get; set; }

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

    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    [Serializable]
    public abstract class MillingTechProcess : TechProcessBase<MillingCommandGenerator>
    {
        public Material? Material { get; set; }

        public double? Thickness { get; set; }

        public int Frequency { get; set; }

        public int PenetrationFeed { get; set; }

        protected virtual void SetTool(MillingCommandGenerator generator) => generator.SetTool(MachineType.Value != CAM.MachineType.Donatoni ? Tool.Number : 1, Frequency);

    }
}