using System;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;

namespace CAM.UtilViews
{
    public partial class PullingView : UserControl
    {
        public PullingView()
        {
            InitializeComponent();
        }

        private bool _isPullingActive = false;

        private void bPulling_Click(object sender, EventArgs e)
        {
            if (_isPullingActive)
                return;

            var objectIds = Interaction.GetPickSet();
            if (objectIds.Length == 0)
            {
                Acad.Alert($"Выделите притягиваемые кривые");
                return;
            }
            lPulling.Text = $"Включен режим притягивания {objectIds.Length} объектов";

            Acad.Editor.SetImpliedSelection(Array.Empty<ObjectId>());
            App.LockAndExecute(() => Interaction.HighlightObjects(objectIds));
            Acad.Write($"Выполняется притягивание {objectIds.Length} объектов");
            _isPullingActive = true;

            while (true)
            {
                Interaction.SetActiveDocFocus();
                var sideId = Interaction.GetEntity("\nВыберите притягиваемую сторону", typeof(Line));
                if (sideId == ObjectId.Null)
                    break;

                if (!objectIds.Contains(sideId))
                {
                    Acad.Write($"Притягиваемая сторона должна быть из числа притягиваемых кривых");
                    continue;
                }
                Acad.Editor.SetImpliedSelection(new ObjectId[] { sideId });
                var guideId = Interaction.GetEntity("\nВыберите отрезок к которому выполнить притягивание", typeof(Line));
                if (guideId == ObjectId.Null)
                    break;

                Line guide = guideId.QOpenForRead<Line>();
                Line side = sideId.QOpenForRead<Line>();

                var isObjLeft = !side.IsTurnRight(objectIds.GetCenter());
                double.TryParse(tbDist.Text, out double dist);
                var normalVector = guide.Delta.GetNormal().GetPerpendicularVector() * dist;

                if (cbMove.Checked)
                {
                    Interaction.SetActiveDocFocus();

                    using (Acad.ActiveDocument.LockDocument())
                    {
                        Interaction.HighlightObjects(new[] { guideId });
                        Acad.Editor.SetImpliedSelection(objectIds);
                        PromptSelectionResult psr = Acad.Editor.SelectImplied();
                        PromptPointResult ppr = Acad.Editor.Drag(psr.Value, "\nУкажите точку вставки объектов: ",
                                                    (Point3d pt, ref Matrix3d mtx) =>
                                                    {
                                                        mtx = CalcMatrix(pt, guide, side, normalVector, isObjLeft);
                                                        return SamplerStatus.OK;
                                                    });
                        if (ppr.Status == PromptStatus.OK)
                        {
                            var matrix = CalcMatrix(ppr.Value, guide, side, normalVector, isObjLeft);
                            objectIds.QForEach<Entity>(dbobject => dbobject.TransformBy(matrix));
                        }
                        App.LockAndExecute(() => Interaction.UnhighlightObjects(new ObjectId[] { guideId }));
                    }
                }
                else
                {
                    var mt = CalcMatrix(side.StartPoint, guide, side, normalVector, isObjLeft);
                    App.LockAndExecute(() => objectIds.QForEach<Curve>(p => p.TransformBy(mt)));
                }
                Acad.Editor.SetImpliedSelection(Array.Empty<ObjectId>());
                App.LockAndExecute(() => Interaction.HighlightObjects(objectIds));
            }
            Acad.Editor.SetImpliedSelection(Array.Empty<ObjectId>());
            App.LockAndExecute(() => Interaction.UnhighlightObjects(objectIds));
            lPulling.Text = "";
            Acad.Write($"Притягивание завершено");
            _isPullingActive = false;

            Matrix3d CalcMatrix(Point3d point, Line guide, Line side, Vector3d normalVector, bool isObjLeft)
            {
                var isPointLeft = !guide.IsTurnRight(point);
                var rotAngle = side.GetVector2d().ZeroTo2PiAngleTo((isObjLeft ^ isPointLeft) ? guide.GetVector2d().Negate() : guide.GetVector2d());
                var rotCenter = side.StartPoint;
                var rotationMatrix = Matrix3d.Rotation(rotAngle, Vector3d.ZAxis, rotCenter);

                point = guide.GetClosestPointTo(point, true);
                point += isPointLeft ? normalVector : -normalVector;
                var displacementMatrix = Matrix3d.Displacement(rotCenter.GetVectorTo(point));

                return displacementMatrix * rotationMatrix;
            }
        }
    }
}
