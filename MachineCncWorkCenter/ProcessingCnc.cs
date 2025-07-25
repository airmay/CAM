﻿using System;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CAM.Core;

namespace CAM.CncWorkCenter
{
    [Serializable]
    public class ProcessingCnc : ProcessingBase
    {
        public override MachineType MachineType => MachineType.CncWorkCenter;
        [NonSerialized] public ProcessorCnc Processor;

        public Material? Material { get; set; }
        public Tool Tool { get; set; }
        public int Frequency { get; set; }
        public int CuttingFeed { get; set; }
        public int PenetrationFeed { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddMachine(CAM.Machine.Donatoni, CAM.Machine.ScemaLogic, CAM.Machine.Forma);
            view.AddMaterial();
            view.AddIndent();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddIndent();
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddIndent();
            view.AddOrigin();
            view.AddTextBox(nameof(ZSafety));
        }

        public ProcessingCnc()
        {
            Caption = "Обработка ЧПУ";
        }

        protected override IProcessor CreateProcessor()
        {
            PostProcessorCnc postProcessor;
            switch (Machine.Value)
            {
                case CAM.Machine.ScemaLogic:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case CAM.Machine.Donatoni:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case CAM.Machine.Krea:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case CAM.Machine.CableSawing:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case CAM.Machine.Forma:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                case CAM.Machine.Champion:
                    postProcessor = new DonatoniPostProcessor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Processor = new ProcessorCnc(this, postProcessor);
            return Processor;
        }

        protected override bool Validate()
        {
            return Machine.CheckNotNull("Станок") && Tool.CheckNotNull("Инструмент");
        }

        //public void RemoveAcadObjects()
        //{
        //    foreach (var operation in Operations)
        //        operation.RemoveAcadObjects();
        //}
    }
}