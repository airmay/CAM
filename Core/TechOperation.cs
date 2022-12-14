using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    /// <summary>
    /// Базовая технологическая операция
    /// </summary>
    [Serializable]
    public abstract class TechOperation<TGenerator> : TechOperation where TGenerator : CommandGeneratorBase
    {
        public abstract void BuildProcessing(TGenerator generator);

        public virtual void PrepareBuild(TGenerator generator) { }

        public virtual void PostBuild(TGenerator generator) { }
    }

    [Serializable]
    public abstract class TechOperation<TTechProcess, TGenerator> : TechOperation<TGenerator> where TGenerator : CommandGeneratorBase where TTechProcess : TechProcess<TGenerator>
    {
        public TTechProcess TechProcess { set; get; } //=> (TTechProcess)TechProcessBase;

        public TechOperation(TTechProcess techProcess, string caption)
        {
            TechProcess = techProcess;
            TechProcess.AddTechOperation(this);
            Caption = $"{caption}{techProcess.TechOperations.Count()}";

            Init();
        }

        public override bool TryMoveBackward() => TechProcess.MoveBackwardTechOperation(this);

        public override bool TryMoveForward() => TechProcess.MoveForwardTechOperation(this);

        public override void Remove()
        {
            Teardown();
            TechProcess.RemoveTechOperation(this);
        }
    }

    [Serializable]
    public abstract class MillingTechOperation<TTechProcess> : TechOperation<TTechProcess, MillingCommandGenerator> where TTechProcess : MillingTechProcess
    {
        protected bool IsSupressUplifting;

        protected MillingTechOperation(TTechProcess techProcess, string caption) : base(techProcess, caption) { }

        public override void PostBuild(MillingCommandGenerator generator) 
        {
            if (!generator.IsUpperTool && !IsSupressUplifting)
                generator.Uplifting();
        }
    }

    [Serializable]
    public abstract class WireSawingTechOperation<TTechProcess> : TechOperation<TTechProcess, CableCommandGenerator> where TTechProcess : CableTechProcess
    {
        protected WireSawingTechOperation(TTechProcess techProcess, string caption) : base(techProcess, caption) { }
    }

    [Serializable]
    public abstract class TechOperation
    {
        /// <summary>
        /// Технологический процесс обработки
        /// </summary>
        //[NonSerialized]
        //public ITechProcess TechProcessBase;

        [NonSerialized]
        public ObjectId? ToolpathObjectsGroup;

        [NonSerialized]
        public int? FirstCommandIndex;

        /// <summary>
        /// Наименование
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Обрабатываемая область
        /// </summary>
        public AcadObject ProcessingArea { get; set; }

        //public void Setup(ITechProcess techProcess, string caption)
        //{
        //    TechProcessBase = techProcess;
        //    TechProcessBase.AddTechOperation(this);
        //    Caption = $"{caption}{techProcess.TechOperations.Count()}";

        //    Init();
        //}

        public virtual void Init() { }

        public virtual void SerializeInit() { }

        public virtual void Teardown() { }

        public virtual bool CanProcess => true;

        public bool Enabled { get; set; } = true;

        public virtual bool Validate() => true;

        public abstract bool TryMoveBackward();

        public abstract bool TryMoveForward();

        public abstract void Remove();
    }
}
