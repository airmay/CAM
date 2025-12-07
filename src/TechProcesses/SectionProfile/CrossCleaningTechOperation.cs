/*using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CAM.TechProcesses.SectionProfile
{
    [Serializable]
    [MenuItem("Поперечная чистка", 3)]
    public class CrossCleaningTechOperation : MillingTechOperation<SectionProfileTechProcess>
    {
        public CrossCleaningTechOperation(SectionProfileTechProcess techProcess, string caption) : base(techProcess, caption)
        {
        }

        public double LongStep { get; set; } = 10;
        public double ProfileStep { get; set; } = 5;
        public double? ProfileBegin { get; set; }
        public double? ProfileEnd { get; set; }
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

        public static void ConfigureParamsView(ParamsView view)
        {
            view.AddTextBox(nameof(IsA90), "A=90");
            view.AddAcadObject(nameof(Profile));
            view.AddTextBox(nameof(CuttingFeed));
            view.AddTextBox(nameof(LongStep), "Шаг продольный");
            view.AddTextBox(nameof(ProfileStep), "Шаг по профилю");
            view.AddTextBox(nameof(ProfileBegin), "Профиль начало");
            view.AddTextBox(nameof(ProfileEnd), "Профиль конец");
            view.AddTextBox(nameof(IsExactlyBegin), "Начало точно");
            view.AddTextBox(nameof(IsExactlyEnd), "Конец точно");
            view.AddIndent();
            view.AddTextBox(nameof(IsOutlet), "Отвод");
            view.AddTextBox(nameof(Departure));
            view.AddTextBox(nameof(Delta));
            view.AddTextBox(nameof(ChangeProcessSide), "Сторона обработки");
            view.AddTextBox(nameof(ChangeEngineSide), "Сторона двигателя");
        }

        public override bool Validate()
        {
            if (LongStep == 0 || ProfileStep == 0)
                Acad.Alert("Не заполнено поле \"Шаг\"");
            return !(LongStep == 0 || ProfileStep == 0);
        }

        public override void BuildProcessing(MillingCommandGenerator generator)
        {
            var railBase = TechProcess.Rail?.GetCurve() ?? new Line(Point3d.Origin, Point3d.Origin + Vector3d.XAxis * TechProcess.Length.Value);
            var profile = (Profile ?? TechProcess.ProcessingArea).GetCurve();
            var processSide = ChangeProcessSide ? 1 : -1;
            CreateProfile3D(profile, railBase, processSide);

            var rail = CreateDepartureRail(railBase, Departure);

            if (Delta != 0)
            {
                var profileOffset = (Curve)profile.GetOffsetCurves(Delta)[0];
                profile = profileOffset.EndPoint.GetAsVector().Length < profile.EndPoint.GetAsVector().Length
                    ? (Curve)profile.GetOffsetCurves(-Delta)[0]
                    : profileOffset;
                CreateProfile3D(profile, railBase, processSide);
            }
            var offsetSign = processSide * (railBase is Line ? 1 : -1);

            if (railBase.IsNewObject)
                railBase.Dispose();

            var side = BuilderUtils.CalcEngineSide(rail.GetFirstDerivative(0).GetAngleTo(Vector3d.XAxis));
            if (ChangeEngineSide)
                side = SideExt.Opposite(side);
            var isMinToolCoord = IsA90
                ? TechProcess.MachineType.Value != Machine.ScemaLogic
                : side == Side.Right ^ TechProcess.MachineType.Value == Machine.ScemaLogic ^ ChangeProcessSide;

            generator.CuttingFeed = CuttingFeed;
            generator.SmallFeed = TechProcess.PenetrationFeed;
            generator.EngineSide = side;

            Curve outletCurve = null;
            if (IsA90 && IsOutlet)
            {
                outletCurve = rail.GetOffsetCurves(TechProcess.ZSafety * offsetSign)[0] as Curve;
                var angleC = BuilderUtils.CalcToolAngle(outletCurve, outletCurve.StartPoint, side);
                generator.Move(outletCurve.StartPoint.X, outletCurve.StartPoint.Y, angleC: angleC, angleA: 90);
            }
            var angleA = IsA90 ? 90 : 0;
            var index = IsA90 ? 1 : 0;

            var profilePoints = BuilderUtils.GetProcessPoints(profile, index, ProfileStep, TechProcess.Tool.Thickness.Value, isMinToolCoord, ProfileBegin, ProfileEnd, IsExactlyBegin, IsExactlyEnd, true);

            var profilePline = new Polyline();
            Enumerable.Range(0, profilePoints.Count).ForEach(i => profilePline.AddVertexAt(i, profilePoints[i], 0, 0, 0));

            var railLength = rail.Length();
            Acad.SetLimitProgressor((int)(railLength / LongStep));

            for (double dist = 0; dist < railLength; dist += LongStep)
            {
                Acad.ReportProgressor();

                var point = rail.GetPointAtDist(dist);
                var angleC = BuilderUtils.CalcToolAngle(rail, point, side);
                var passPline = CalcPassPline(rail, point, profilePline, processSide);

                generator.Cutting(passPline, CuttingFeed, TechProcess.PenetrationFeed, angleC: angleC, angleA: angleA);

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

        private Polyline CalcPassPline(Curve rail, Point3d point, Polyline profilePline, int processSide)
        {
            var angle = Math.PI - rail.GetFirstDerivative(point).ToVector2d().MinusPiToPiAngleTo(processSide * Vector2d.YAxis);
            var matrix = Matrix3d.Displacement(point.GetAsVector()) *
                Matrix3d.Rotation(angle, Vector3d.ZAxis, Point3d.Origin) *
                Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, Point3d.Origin);
            return profilePline.GetTransformedCopy(matrix) as Polyline;
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


        public void BuildProcessingOld(MillingCommandGenerator generator)
        {
            var sectionProfile = (SectionProfileTechProcess)TechProcess;
            var toolThickness = TechProcess.Tool.Thickness.Value;

            var rail = sectionProfile.Rail != null ? sectionProfile.Rail.GetCurve() as Line : new Line(Point3d.Origin, new Point3d(sectionProfile.Length.Value, 0, 0));
            var railVector = rail.Delta.GetNormal();
            var passVector = rail.Delta;
            var crossVector = railVector.RotateBy(-Math.PI / 2, Vector3d.ZAxis);
            var startPass = rail.StartPoint;
            var shift = TechProcess.MachineType == Machine.Donatoni ^ BuilderUtils.CalcEngineSide(rail.Angle) == Side.Left ? toolThickness : 0;
            if (rail.IsNewObject)
                rail.Dispose();

            var profile = sectionProfile.ProcessingArea.GetCurve() as Polyline;
            if (profile == null) throw new Exception("Профиль должен быть полилинией");

            if (Delta != 0)
                profile = (Polyline)profile.GetOffsetCurves(Delta)[0];

            var endX = Math.Max(profile.StartPoint.X, profile.EndPoint.X);
            var profilePoints = profile.GetPolylineFitPoints(ProfileStep).Select(p => new Point2d(endX - p.X, p.Y)).ToList();
            if (profilePoints[0].X > Consts.Epsilon)
                profilePoints.Reverse();
            profilePoints = profilePoints
                .TakeWhile(p => p.X < toolThickness)
                .Select(p => p - Vector2d.XAxis * toolThickness)
                .Concat(profilePoints)
                .ToList();
            var passPoints = profilePoints.Select((p, i) =>
            {
                var y = p.Y;
                for (int j = i + 1; j < profilePoints.Count && profilePoints[j].X - profilePoints[i].X < toolThickness; j++)
                    if (profilePoints[j].Y > y)
                        y = profilePoints[j].Y;
                return new Point2d(p.X, y);
            })
            .ToList();

            if (Departure > 0)
            {
                passPoints.Insert(0, new Point2d(-Departure, passPoints.First().Y));
                passPoints.Add(new Point2d(passPoints.Last().X + Departure, passPoints.Last().Y));
            }

            var sp = startPass + crossVector * (passPoints[0].X + shift) + passPoints[0].Y * Vector3d.ZAxis;
            generator.Move(sp.X, sp.Y, angleC: BuilderUtils.CalcToolAngle(railVector.GetAngleTo(Vector3d.XAxis)));

            var passPointsDirect = new List<Point2d>[2] { passPoints, Enumerable.Reverse(passPoints).ToList() };
            int direct = 0;

            Acad.SetLimitProgressor((int)(passVector.Length / LongStep));
            for (double x = 0; x < passVector.Length; x += LongStep)
            {
                Acad.ReportProgressor();
                passPointsDirect[direct].ForEach(p => generator.GCommand(CommandNames.Cutting, 1, point: startPass + crossVector * (p.X + shift) + p.Y * Vector3d.ZAxis + x * railVector, feed: CuttingFeed));
                direct = 1 - direct;
            }
        }
    }
}*/