using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class MillingTechOperation : TechOperation
    {
        public abstract void BuildProcessing(MillingCommandGenerator generator);

        public virtual void PrepareBuild(MillingCommandGenerator generator) { }
    }

    [Serializable]
    public abstract class MillingTechOperation<T> : MillingTechOperation where T : ITechProcess
    {
        public T TechProcess => (T)TechProcessBase;
    }

    [Serializable]
    public abstract class WireSawingTechOperation : TechOperation
    {
        public int? CuttingFeed { get; set; }
        public int? S { get; set; }
        public double? Departure { get; set; }
        public bool IsRevereseDirection { get; set; }
        public bool IsRevereseOffset { get; set; }

        public abstract Point3d[][] GetProcessPoints();
        public abstract void BuildProcessing(CableCommandGenerator generator);
    }

    [Serializable]
    public abstract class WireSawingTechOperation<T> : WireSawingTechOperation where T : ITechProcess
    {
        public T TechProcess => (T)TechProcessBase;
    }

    [Serializable]
    public abstract class TechOperation
    {
        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        [NonSerialized]
        public ITechProcess TechProcessBase;

        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;

        [NonSerialized]
        public int? ProcessCommandIndex;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        public void Setup(ITechProcess techProcess, string caption)
        {
            TechProcessBase = techProcess;
            TechProcessBase.TechOperations.Add(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";

            Init();
        }

        public virtual void Init() { }

        public virtual void SerializeInit() { }

        public virtual void Teardown() { }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

    }
}
