using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Linq;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [MenuItem("Продольная обработка", 1)]
    public class LongProcessingTechOperation : MillingTechOperation<SectionProfileTechProcess>
    {
        public LongProcessingTechOperation(SectionProfileTechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public double StepPass { get; set; } = 10;
        public bool IsProfileStep { get; set; }
        public double? FirstPass { get; set; }
        public double? LasttPass { get; set; }
        public double? PenetrationStep { get; set; }
        public double? PenetrationBegin { get; set; }
        public double? PenetrationEnd { get; set; }
        public double Delta { get; set; }
        public int CuttingFeed { get; set; } = 5000;
        public double Departure { get; set; }
        public bool IsOutlet { get; set; } = true;
        public AcadObject Profile { get; set; }
        public bool IsA90 { get; set; }
        public bool ChangeProcessSide { get; set; }
        public bool ChangeEngineSide { get; set; }
        public bool IsExactlyBegin { get; set; }
        public bool IsExactlyEnd { get; set; }
        public double DzBillet { get; set; }

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTextBox(nameof(IsA90), "A=90");
            view.AddAcadObject(nameof(Profile));
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(FirstPass), "Начало профиля");
            view.AddTextBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddTextBox(nameof(LasttPass), "Конец профиля");
            view.AddTextBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddTextBox(nameof(StepPass));
            view.AddTextBox(nameof(IsProfileStep), "Шаг по профилю");
            view.AddIndent();
            view.AddTextBox(nameof(PenetrationStep), "Заглубление: шаг");
            view.AddTextBox(nameof(PenetrationBegin), "Заглубление: начало");
            view.AddTextBox(nameof(PenetrationEnd), "Заглубление: конец");
            view.AddIndent();
            view.AddTextBox(nameof(IsOutlet), "Отвод");
            view.AddTextBox(nameof(Departure));
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(ChangeProcessSide), "Сторона обработки");
            view.AddTextBox(nameof(ChangeEngineSide), "Разворот двигателя на 180");
            view.AddTextBox(nameof(DzBillet), "dZ заготовки");
        }

        public override bool Validate()
        {
            if (StepPass == 0)
                Acad.Alert("Не заполнено поле \"Шаг межстрочный\"");
            return StepPass != 0;
        }

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            generator.Tool = TechProcess.Tool;
            generator.CuttingFeed = CuttingFeed;

            var railBase = TechProcess.Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * TechProcess.Length.Value);
            var profile = (Profile ?? TechProcess.ProcessingArea[0]).GetCurve();
            //var processSide = 1;// ChangeEngineSide ? -1 : 1;
            //CreateProfile3D(profile, railBase, 1);
            var processSide = ChangeProcessSide ? -1 : 1;
            CreateProfile3D(profile, railBase, processSide);

            var rail = CreateDepartureRail(railBase, Departure);

            if (Delta != 0)
            {
                var profileOffset = (Curve)profile.GetOffsetCurves(Delta)[0];
                profile = profileOffset.StartPoint.Y < profile.StartPoint.Y
                    ? (Curve)profile.GetOffsetCurves(-Delta)[0]
                    : profileOffset;
                CreateProfile3D(profile, railBase, processSide);
            }
            if (!(railBase is Line))
                processSide *= -1;
            if (railBase.IsNewObject)
                railBase.Dispose();

            var side = BuilderUtils.CalcEngineSide(rail.GetFirstDerivative(0).GetAngleTo(Vector3d.XAxis));
            if (ChangeEngineSide)
                side = SideExt.Opposite(side);
            var isMinToolCoord = IsA90
                ? TechProcess.MachineType.Value != MachineType.ScemaLogic
                : side == Side.Left ^ TechProcess.MachineType.Value == MachineType.ScemaLogic ^ ChangeProcessSide;

            generator.CuttingFeed = CuttingFeed;
            generator.SmallFeed = TechProcess.PenetrationFeed;
            generator.EngineSide = side;

            Curve outletCurve = null;
            if (IsA90 && IsOutlet)
            {
                outletCurve = rail.GetOffsetCurves(TechProcess.ZSafety * processSide)[0] as Curve;
                var angleC = BuilderUtils.CalcToolAngle(outletCurve, outletCurve.StartPoint, side);
                generator.Move(outletCurve.StartPoint.X, outletCurve.StartPoint.Y, angleC: angleC, angleA: 90);
            }
            var angleA = IsA90 ? 90 : 0;
            var index = IsA90 ? 1 : 0;

            var points = BuilderUtils.GetProcessPoints(profile, index, StepPass, TechProcess.Tool.Thickness.Value, isMinToolCoord, FirstPass, LasttPass, IsExactlyBegin, IsExactlyEnd, IsProfileStep);

            foreach (var point in points)
            {
                var end = Math.Max(point[1 - index], PenetrationEnd ?? double.MinValue);
                var count = 1;
                var penetrationStepCalc = 0D;
                if (DzBillet != 0)
                {
                    count = (int)Math.Ceiling(DzBillet / PenetrationStep.Value);
                    penetrationStepCalc = DzBillet / count;
                }
                else if (PenetrationStep.GetValueOrDefault() > 0 && PenetrationBegin.GetValueOrDefault() > end)
                {
                    var allPenetration = PenetrationBegin.Value - end;
                    count = (int)Math.Ceiling(allPenetration / PenetrationStep.Value);
                    penetrationStepCalc = allPenetration / count;
                }
                if (IsA90 && IsOutlet && generator._isEngineStarted)
                    generator.Transition(z: point[index]);

                var coords = Enumerable.Range(1, count).Select(p => end + (count - p) * penetrationStepCalc).ToList();
                if (IsA90)
                    coords.ForEach(p => generator.Cutting(rail, processSide * p, point[index], angleA: angleA));
                else
                    coords.ForEach(p => generator.Cutting(rail, processSide * point[index], p, angleA: angleA));

                if (IsOutlet)
                    if (IsA90)
                    {
                        var pt = outletCurve.GetClosestPoint(generator.ToolPosition.Point);
                        generator.Move(pt.X, pt.Y);
                    }
                    else
                        generator.Uplifting();
            }
            rail.Dispose();
        }        

        private void CreateProfile3D(Curve profile, Curve rail, int processSide)
        {
            Create(rail.StartPoint);
            Create(rail.EndPoint);

            void Create(Point3d point)
            {
                var angle = Math.PI - rail.GetFirstDerivative(point).ToVector2d().MinusPiToPiAngleTo(processSide * Vector2d.YAxis);
                var matrix = Matrix3d.Displacement(point.GetAsVector()) *
                    Matrix3d.Rotation(angle, Vector3d.ZAxis, Point3d.Origin) *
                    Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin);
                var p = profile.GetTransformedCopy(matrix);
                p.Transparency = new Transparency(255 * (100 - 70) / 100);
                p.AddToCurrentSpace();
                TechProcess.ExtraObjectsGroup = TechProcess.ExtraObjectsGroup.AppendToGroup(p.ObjectId);
            }
        }
            
        private Curve CreateDepartureRail(Curve curve, double departure)
        {
            if (curve is Line line)
            {
                var vector = line.Delta.GetNormal() * departure;
                return new Line(line.StartPoint - vector, line.EndPoint + vector);
            }
            var polyline = new Polyline();

            var startPoint = curve.StartPoint - curve.GetFirstDerivative(curve.StartPoint).GetNormal() * departure;
            polyline.AddVertexAt(0, startPoint.ToPoint2d(), 0, 0, 0);

            if (curve is Polyline poly)
                polyline.JoinPolyline(poly);
            else if (curve is Arc arc)
            {
                var bulge = Algorithms.GetArcBulge(arc, curve.StartPoint);
                polyline.AddVertexAt(1, curve.StartPoint.ToPoint2d(), bulge, 0, 0);
                polyline.AddVertexAt(2, curve.EndPoint.ToPoint2d(), 0, 0, 0);
            }
            else
                throw new Exception($"Тип кривой {curve.GetType().Name} не поддерживается");

            var endtPoint = curve.EndPoint + curve.GetFirstDerivative(curve.EndPoint).GetNormal() * departure;
            polyline.AddVertexAt(polyline.GetPolyPoints().Count(), endtPoint.ToPoint2d(), 0, 0, 0);

            return polyline;
        }
    }
}
