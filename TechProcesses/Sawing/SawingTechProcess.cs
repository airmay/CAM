using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.TechProcesses.Sawing
{
    [Serializable]
    [MenuItem("Распиловка", 1)]
    public class SawingTechProcess : MillingTechProcess
    {
        public SawingTechProcessParams SawingTechProcessParams { get; }

        [NonSerialized]
        private List<Border> _borders;

        public SawingTechProcess(SawingTechProcessParams @params)
        {
            SawingTechProcessParams = @params.Clone();
            PenetrationFeed = SawingTechProcessParams.PenetrationFeed;
        }

        public static void ConfigureParamsView(ParamsView view)
        {
            var sawingModesView = new SawingModesView();

            view.AddMachine(CAM.MachineType.Donatoni, CAM.MachineType.ScemaLogic, CAM.MachineType.Forma);
            view.AddMaterial();
            view.AddTextBox(nameof(Thickness));
            view.AddIndent();
            view.AddTool();
            view.AddTextBox(nameof(Frequency));
            view.AddTextBox(nameof(PenetrationFeed));
            view.AddTextBox(nameof(ZSafety));
            view.AddIndent();
            view.AddAcadObject(message: "Выберите объекты распиловки",
                    allowedTypes: $"{AcadObjectNames.Line},{AcadObjectNames.Arc},{AcadObjectNames.Lwpolyline}",
                    afterSelect: ids => view.GetParams<SawingTechProcess>().CreateExtraObjects(ids)
                );
            view.AddIndent();
            view.AddComboBox("Режимы", new[] { "Отрезок", "Кривая" }, SetSawingModes);
            view.AddControl(sawingModesView, 6);

            void SetSawingModes(int index) => 
                sawingModesView.sawingModesBindingSource.DataSource = index == 0 
                    ? view.GetParams<SawingTechProcess>().SawingTechProcessParams.SawingLineModes 
                    : view.GetParams<SawingTechProcess>().SawingTechProcessParams.SawingCurveModes;
        }

        public override void SerializeInit()
        {
            base.SerializeInit();
            CreateExtraObjects();
        }

        public List<Border> CreateExtraObjects(params ObjectId[] ids)
        {
            ExtraObjectsGroup?.DeleteGroup();
            ExtraObjectsGroup = null;

            var techOperations = TechOperations.Where(p => p.ProcessingArea != null).ToList();
            techOperations.FindAll(p => ids.Contains(p.ProcessingArea.ObjectId)).ForEach(p => ((SawingTechOperation)p).OuterSide = Side.None);
            _borders = ids.Except(techOperations.Select(p => p.ProcessingArea.ObjectId)).Select(p => new Border(p)).ToList();
            var borders = _borders.Concat(techOperations.Select(p => new Border((SawingTechOperation)p))).ToList();
            borders.ForEach(p => p.Curve = Acad.OpenForRead(p.ObjectId));
            ProcessingArea = AcadObject.CreateList(borders.Select(p => p.ObjectId));

            while ((borders.Find(p => p.OuterSide != Side.None) ?? borders.FirstOrDefault()) is Border startBorder)
            {
                if (startBorder.OuterSide == Side.None)
                    startBorder.OuterSide = GetOuterSide(startBorder.Curve);
                var contour = CalcBordersChain(startBorder, Corner.End);
                if (borders.Contains(startBorder))
                {
                    var contourBack = CalcBordersChain(startBorder, Corner.Start);
                    contourBack.Reverse();
                    contourBack.Add(startBorder.Curve);
                    contourBack.AddRange(contour);
                    contour = contourBack;
                    borders.Remove(startBorder);
                }
                var sign = startBorder.OuterSide == Side.Left ? 1 : -1;

                var hatchId = Graph.CreateHatch(contour, sign);
                if (hatchId.HasValue)
                    ExtraObjectsGroup = ExtraObjectsGroup.AppendToGroup(hatchId.Value);
            }
            return _borders;

            List<Curve> CalcBordersChain(Border border, Corner corner)
            {
                var point = border.Curve.GetPoint(corner);
                var contour = new List<Curve>();
                while (borders.SingleOrDefault(p => p != border && p.Curve.HasPoint(point)) is Border nextBorder)
                {
                    contour.Add(nextBorder.Curve);
                    borders.Remove(nextBorder);
                    var nextCorner = nextBorder.Curve.GetCorner(point);
                    nextBorder.OuterSide = nextCorner != corner ? border.OuterSide : border.OuterSide.Opposite();

                    if (border.MustCalc || nextBorder.MustCalc)
                    {
                        var isExactly = CalcIsExactly(border, corner, nextBorder, nextCorner, point);
                        border.SetIsExactly(corner, isExactly);
                        nextBorder.SetIsExactly(nextCorner, isExactly);
                    }
                    border = nextBorder;
                    corner = nextCorner.Swap();
                    point = border.Curve.GetPoint(corner);
                }
                return contour;
            }

            bool CalcIsExactly(Border border, Corner corner, Border nextBorder, Corner nextCorner, Point3d point)
            {
                var v1 = border.Curve.GetTangent(corner);
                var v2 = nextBorder.Curve.GetTangent(nextCorner);
                var isLeftTurn = v1.MinusPiToPiAngleTo(v2) > Consts.Epsilon;
                var isLeftOuterSide = border.OuterSide == Side.Left;
                var isNextStartPoint = nextCorner == Corner.Start;
                return isLeftTurn ^ isLeftOuterSide ^ isNextStartPoint;
            }

            Side GetOuterSide(Curve curve)
            {
                var startPoint = curve.GetPointAtParameter((curve.EndParam + curve.StartParam) / 2);
                var point = Interaction.GetLineEndPoint("Выберите направление внешней нормали к объекту", startPoint);
                var vector = curve.GetTangent(startPoint);
                return vector.IsTurnRight((point - startPoint).ToVector2d()) ? Side.Right : Side.Left;
            }
        }

        public override List<TechOperation> CreateTechOperations() => _borders?.ConvertAll(p => new SawingTechOperation(this, p) as TechOperation);

        public override bool Validate() => ToolService.Validate(Tool, ToolType.Disk) && Thickness.CheckNotNull("Толщина");

        public override void BuildProcessing()
        {
            CreateExtraObjects();
            base.BuildProcessing();
        }
    }
}
