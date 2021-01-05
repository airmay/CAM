//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using System;
//using System.Linq;

//namespace CAM.TechProcesses.SectionProfile
//{
//    [Serializable]
//    [MenuItem("Продольная чистка", 2)]
//    public class LongCleaningTechOperation : TechOperation<SectionProfileTechProcess>
//    {

//        public double StepYmin { get; set; }

//        public double? StepYmax { get; set; }

//        public double StartY { get; set; }

//        public double Delta { get; set; }

//        public double StepZmax { get; set; }

//        public int CuttingFeed { get; set; }

//        public double Departure { get; set; }

//        public bool IsUplifting { get; set; }

//        public AcadObject Profile { get; set; }

//        public static void ConfigureParamsView(ParamsView view)
//        {
//            view.AddParam(nameof(StepYmin), "Шаг Y мин.")
//                .AddParam(nameof(StepYmax), "Шаг Y макс.")
//                .AddParam(nameof(StartY), "Y начала")
//                .AddParam(nameof(StepZmax), "Шаг Z макс.")
//                .AddIndent()
//                .AddParam(nameof(Departure))
//                .AddParam(nameof(CuttingFeed))
//                .AddParam(nameof(Delta))
//                .AddParam(nameof(IsUplifting))
//                .AddAcadObject(nameof(Profile));
//        }

//        public override void BuildProcessing(CommandGeneratorBase generator)
//        {
//            var sectionProfile = (SectionProfileTechProcess)TechProcess;
//            var toolThickness = TechProcess.Tool.Thickness.Value;

//            var rail = sectionProfile.Rail != null ? sectionProfile.Rail.GetCurve() as Line : new Line(Point3d.Origin, new Point3d(sectionProfile.Length.Value, 0, 0));
//            var railVector = rail.Delta.GetNormal();
//            var passVector = railVector * (rail.Length + 2 * Departure);
//            var crossVector = railVector.RotateBy(-Math.PI / 2, Vector3d.ZAxis);
//            var startPass = rail.StartPoint - railVector * Departure;
//            var shift = TechProcess.MachineType == MachineType.Donatoni ^ BuilderUtils.CalcEngineSide(rail.Angle) == Side.Left ? toolThickness : 0;
//            if (rail.IsNewObject)
//                rail.Dispose();

//            var profile = (Profile ?? sectionProfile.ProcessingArea[0]).GetCurve();
//            var profileX = profile.GetStartEndPoints().OrderBy(p => p.X).Select(p => p.X).ToArray();
//            profileX[1] -= StartY;
//            if (Delta != 0)
//                profile = (Curve)profile.GetOffsetCurves(Delta * (profile.StartPoint.X > profile.EndPoint.X ? 1 : -1))[0];

//            var count = (int)((profileX[1] - profileX[0]) / StepYmin) + 1;
//            Acad.SetLimitProgressor(count);
//            var yArray = new double[count];

//            using (var curve = profile.ToCompositeCurve2d())
//            using (var ray = new Ray2d())
//            using (var intersector = new CurveCurveIntersector2d())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    Acad.ReportProgressor();

//                    var x = profileX[1] - i * StepYmin;
//                    ray.Set(new Point2d(x, -10000), Vector2d.YAxis);
//                    intersector.Set(curve, ray);
//                    if (intersector.NumberOfIntersectionPoints != 1) throw new Exception("Некорректный профиль");
//                    yArray[i] = intersector.GetIntersectionPoint(0).Y;
//                    //Draw.Circle(new CircularArc2d(points[i], 1));
//                }
//            }

//            var thicknessCnt = (int)(toolThickness / StepYmin) + 1;
//            var coeff = (StepYmax ?? toolThickness * 0.8) / StepYmin;
//            var lastY = yArray[0];
//            var lastI = 1 - thicknessCnt;

//            for (int i = -thicknessCnt; i < count; i++)
//            {
//                //if (i * StepYmin < StartY)
//                //    continue;

//                double? y = null;
//                for (int j = 0; j < thicknessCnt && i + j < count; j++)
//                    if (i + j >= 0 && (!y.HasValue || yArray[i + j] > y))
//                        y = yArray[i + j];

//                if (y.HasValue && (Math.Abs(y.Value - lastY) >= StepZmax || i - lastI >= coeff))
//                {
//                    generator.Cutting(startPass + crossVector * (profileX[1] - i * StepYmin - shift) + y.Value * Vector3d.ZAxis, passVector, CuttingFeed, sectionProfile.PenetrationFeed);
//                    if (IsUplifting)
//                        generator.Uplifting();

//                    lastY = y.Value;
//                    lastI = i;
//                }
//            }
//        }
//    }
//}