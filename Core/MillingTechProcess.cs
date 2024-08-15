using System;
using CAM.Core;

namespace CAM
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    [Serializable]
    public abstract class MillingTechProcess : TechProcess<MillingCommandGenerator>
    {
        public Material? Material { get; set; }

        public double? Thickness { get; set; }

        public int Frequency { get; set; }

        public int PenetrationFeed { get; set; }

        protected virtual void SetTool(MillingCommandGenerator generator) => generator.SetTool(MachineType.Value != CAM.MachineCodes.Donatoni ? Tool.Number : 1, Frequency);

        protected override void BuildProcessing(MillingCommandGenerator generator)
        {
            SetTool(generator);
        }

        public override int GetFrequency() => Frequency;

    }
}