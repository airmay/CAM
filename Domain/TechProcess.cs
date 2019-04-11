﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM.Domain
{
    /// <summary>
    /// Технологический процесс обработки
    /// </summary>
    [Serializable]
    public class TechProcess
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        [NonSerialized]
        private CamContainer _container;

        /// <summary>
        /// Параметры технологического процесса
        /// </summary>
        public TechProcessParams TechProcessParams { get; set; }

        private Dictionary<TechOperationType, ITechOperationFactory> TechOperationFactorys = new Dictionary<TechOperationType, ITechOperationFactory>();

        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        public List<TechOperation> TechOperations { get; } = new List<TechOperation>();

	    public IEnumerable<Curve> ToolpathCurves => TechOperations.SelectMany(p => p.ToolpathCurves);

        /// <summary>
        /// Команды
        /// </summary>
        public List<ProcessCommand> ProcessCommands { get; set; } = new List<ProcessCommand>();

        public TechProcess(string name, CamContainer container)
        {
            Name = name;
            _container = container;
            TechProcessParams = container.TechProcessParams.Clone();
        }

        public void SetContainer(CamContainer container) => _container = container;

	    public void BuildProcessing()
	    {
			BorderProcessingArea.SetupBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList());
	        var builder = new ScemaLogicProcessBuilder(TechProcessParams);
            TechOperations.ForEach(p => p.BuildProcessing(builder));
	        ProcessCommands = builder.FinishTechProcess();
	    }

        public ITechOperationFactory GetFactory(TechOperationType techOperationType)
        {
            if (!TechOperationFactorys.TryGetValue(techOperationType, out ITechOperationFactory factory))
            {
                switch (techOperationType)
                {
                    case TechOperationType.Sawing:
                        factory = new SawingTechOperationFactory(_container.SawingLineTechOperationParams.Clone(), _container.SawingCurveTechOperationParams.Clone());
                        break;
                }
                TechOperationFactorys[techOperationType] = factory;
            }
            return factory;
        }

        internal bool SetTool(string text)
        {
            var tool = _container.Tools.SingleOrDefault(p => p.Number.ToString() == text);
            if (tool != null)
            {
                TechProcessParams.ToolDiameter = tool.Diameter;
                TechProcessParams.ToolThickness = tool.Thickness;
                var speed = TechProcessParams.Material == Material.Гранит ? 35 : 50;
                TechProcessParams.Frequency = (int)Math.Round(speed * 1000 / (tool.Diameter * Math.PI) * 60);
            }
            return tool != null;
        }

        public List<SawingTechOperation> CreateTechOperations(TechOperationType techOperationType, List<Curve> curves)
        {
            if (curves == null)
            {
                Application.ShowAlertDialog($"Не выбраны элементы чертежа");
                return null;
            }
            var factory = GetFactory(techOperationType);
            return curves.ConvertAll(p => factory.Create(this, p));
        }
    }
}