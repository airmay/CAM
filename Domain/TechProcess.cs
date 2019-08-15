using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
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
            TechProcessParams = Settings.Instance.TechProcessParams.Clone();
        }

        public void BuildProcessing(BorderProcessingArea startBorder = null)
        {
            try
            {
                Acad.Write($"Выполняется расчет обработки по техпроцессу {Name} ...");

                Acad.DeleteHatch();
                Acad.DeleteCurves(ToolpathCurves);

                DeleteToolpath();
                TechOperations.ForEach(p => p.ProcessingArea.Curve = p.ProcessingArea.AcadObjectId.QOpenForRead<Curve>());

                ProcessBorders(TechOperations.Select(p => p.ProcessingArea).OfType<BorderProcessingArea>().ToList(), startBorder);

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

        public ITechOperationFactory GetFactory(TechOperationType techOperationType)
        {
            if (!TechOperationFactorys.TryGetValue(techOperationType, out ITechOperationFactory factory))
            {
                switch (techOperationType)
                {
                    case TechOperationType.Sawing:
                        factory = new SawingTechOperationFactory(Settings.Instance.SawingLineTechOperationParams.Clone(), 
                            Settings.Instance.SawingCurveTechOperationParams.Clone());
                        break;
                }
                TechOperationFactorys[techOperationType] = factory;
            }
            return factory;
        }

        public bool SetTool(string text)
        {
            var tool = Settings.Instance.Tools.SingleOrDefault(p => p.Number.ToString() == text);
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

        private void ProcessBorders(List<BorderProcessingArea> borders, BorderProcessingArea fixedSideBorder = null)
        {
            BorderProcessingArea startBorder;
            while ((startBorder = fixedSideBorder ?? borders.Find(p => p.OuterSide != Side.None) ?? borders.FirstOrDefault()) != null)
            {
                fixedSideBorder = null;
                if (startBorder.OuterSide == Side.None)
                    CalcOuterSide(startBorder);
                var sign = startBorder.OuterSide == Side.Left ? 1 : -1;
                var contour = CalcBordersChain(Corner.End);
                if (borders.Contains(startBorder))
                {
                    var contourBack = CalcBordersChain(Corner.Start);
                    contourBack.Reverse();
                    contourBack.Add(startBorder.Curve);
                    contourBack.AddRange(contour);
                    contour = contourBack;
                    borders.Remove(startBorder);
                }
                Graph.CreateHatch(contour, sign);
            }

            List<Curve> CalcBordersChain(Corner corner)
            {
                var border = startBorder;
                var point = border.Curve.GetPoint(corner);
                BorderProcessingArea nextBorder;
                var contour = new List<Curve>();
                while ((nextBorder = borders.SingleOrDefault(p => p != border && p.Curve.HasPoint(point))) != null)
                {
                    contour.Add(nextBorder.Curve);
                    borders.Remove(nextBorder);
                    var nextCorner = nextBorder.Curve.GetCorner(point);
                    nextBorder.OuterSide = nextCorner != corner ? border.OuterSide : border.OuterSide.Swap();

                    var isExactly = !border.IsAutoExactly(corner)
                        ? border.IsExactly(corner)
                        : (!nextBorder.IsAutoExactly(nextCorner)
                            ? nextBorder.IsExactly(nextCorner)
                            : CalcIsExactly(border, corner, nextBorder, nextCorner, point));
                    border.IsExactly(corner) = nextBorder.IsExactly(nextCorner) = isExactly;

                    if (nextBorder == startBorder) // цикл
                        return contour;

                    border = nextBorder;
                    corner = nextCorner.Swap();
                    point = border.Curve.GetPoint(corner);
                }

                // свободный конец цепочки
                if (!borders.Contains(border) && border.IsAutoExactly(corner))
                    border.IsExactly(corner) = false;
                return contour;
            }

            bool CalcIsExactly(BorderProcessingArea border, Corner corner, BorderProcessingArea nextBorder, Corner nextCorner, Point3d point)
            {
                var v1 = border.Curve.GetTangent(point);
                var v2 = nextBorder.Curve.GetTangent(point);
                var isLeftTurn = v1.MinusPiToPiAngleTo(v2) > Consts.Epsilon;
                var isLeftOuterSide = border.OuterSide == Side.Left;
                var isNextStartPoint = nextCorner == Corner.Start;
                return isLeftTurn ^ isLeftOuterSide ^ isNextStartPoint;
            }

            void CalcOuterSide(BorderProcessingArea border)
            {
                border.OuterSide = Side.Right;
                var center = borders.Select(p => p.Curve).GetCenter();
                var v1 = border.EndPoint.ToPoint2d() - border.StartPoint.ToPoint2d();
                var v2 = center.ToPoint2d() - border.StartPoint.ToPoint2d();
                border.OuterSide = v1.MinusPiToPiAngleTo(v2) > 0 ? Side.Right : Side.Left;
            }
        }
    }
}