using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;

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

        /// <summary>
        /// Параметры технологического процесса
        /// </summary>
        public TechProcessParams TechProcessParams { get; set; }

        private Dictionary<TechOperationType, ITechOperationFactory> TechOperationFactorys = new Dictionary<TechOperationType, ITechOperationFactory>();

        /// <summary>
        /// Список технологических операций процесса
        /// </summary>
        public List<TechOperation> TechOperations { get; } = new List<TechOperation>();

        public IEnumerable<Curve> ToolpathCurves => TechOperations.Where(p => p.ToolpathCurves != null).SelectMany(p => p.ToolpathCurves);

        public void DeleteToolpath()
        {
            TechOperations.ForEach(p => p.DeleteToolpath());
            ProcessCommands = null;
        }

        /// <summary>
        /// Команды
        /// </summary>
        [NonSerialized]
        public List<ProcessCommand> ProcessCommands;

        public TechProcess(string name)
        {
            Name = name ?? throw new ArgumentNullException("TechProcessName");
            TechProcessParams = CamContainer.Instance.TechProcessParams.Clone();
        }

	    public void BuildProcessing()
	    {
            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {Name} ...");

                Acad.DeleteCurves(ToolpathCurves);
                DeleteToolpath();
                TechOperations.ForEach(p => p.ProcessingArea.Curve = p.ProcessingArea.AcadObjectId.QOpenForRead<Curve>());
                BorderProcessingArea.ProcessBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList());

                var builder = new ScemaLogicProcessBuilder(TechProcessParams);
                TechOperations.ForEach(p => p.BuildProcessing(builder));
                ProcessCommands = builder.FinishTechProcess();
                Acad.SaveCurves(ToolpathCurves);

                Acad.Write("Расчет обработки завершен");
            }
            catch (Exception ex)
            {
                DeleteToolpath();
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
        }

        public void ProcessBorders(BorderProcessingArea startBorder = null)
        {
            TechOperations.ForEach(p => p.ProcessingArea.Curve = p.ProcessingArea.AcadObjectId.QOpenForRead<Curve>());
            BorderProcessingArea.ProcessBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList(), startBorder);
        }

        public ITechOperationFactory GetFactory(TechOperationType techOperationType)
        {
            if (!TechOperationFactorys.TryGetValue(techOperationType, out ITechOperationFactory factory))
            {
                switch (techOperationType)
                {
                    case TechOperationType.Sawing:
                        factory = new SawingTechOperationFactory(CamContainer.Instance.SawingLineTechOperationParams.Clone(), 
                            CamContainer.Instance.SawingCurveTechOperationParams.Clone());
                        break;
                }
                TechOperationFactorys[techOperationType] = factory;
            }
            return factory;
        }

        public bool SetTool(string text)
        {
            var tool = CamContainer.Instance.Tools.SingleOrDefault(p => p.Number.ToString() == text);
            if (tool != null)
            {
                TechProcessParams.ToolDiameter = tool.Diameter;
                TechProcessParams.ToolThickness = tool.Thickness;
                var speed = TechProcessParams.Material == Material.Гранит ? 35 : 50;
                TechProcessParams.Frequency = (int)Math.Round(speed * 1000 / (tool.Diameter * Math.PI) * 60);
            }
            return tool != null;
        }

        public SawingTechOperation[] CreateTechOperations(TechOperationType techOperationType, IEnumerable<Curve> curves) => 
            curves.Select(p => GetFactory(techOperationType).Create(this, p)).Where(p => p != null).ToArray();

    }
}