using System;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace CAM.UI
{
    public partial class PullingView : UserControl
    {
        public PullingView()
        {
            InitializeComponent();
        }

        private void bPulling_Click(object sender, EventArgs e)
        {
            double.TryParse(tbDist.Text, out double dist);

            var objectIds = Interaction.GetPickSet();
            if (objectIds.Length == 0)
            {
                Acad.Alert($"Выделите притягиваемые кривые");
                return;
            }
            Acad.Editor.SetImpliedSelection(Array.Empty<ObjectId>());
            App.LockAndExecute(() => Interaction.HighlightObjects(objectIds));
            Acad.Write($"Выполняется притягивание {objectIds.Length} объектов");
            Interaction.SetActiveDocFocus();
            var sideId = Interaction.GetEntity("\nВыберите притягиваемую сторону", typeof(Line));
            if (sideId == ObjectId.Null)
            {
                App.LockAndExecute(() => Interaction.UnhighlightObjects(objectIds));
                return;
            }
            if (!objectIds.Contains(sideId))
            {
                Acad.Write($"Притягиваемая сторона должна быть из числа притягиваемых кривых. Операция прервана.");
                App.LockAndExecute(() => Interaction.UnhighlightObjects(objectIds));
                return;
            }
            Acad.Editor.SetImpliedSelection(new ObjectId[] { sideId });
            var guideId = Interaction.GetEntity("\nВыберите отрезок к которому выполнить притягивание", typeof(Line));
            if (guideId == ObjectId.Null)
            {
                App.LockAndExecute(() => Interaction.UnhighlightObjects(objectIds));
                return;
            }
            Line guide = guideId.QOpenForRead<Line>();
            Line side = sideId.QOpenForRead<Line>();

            var isObjLeft = !side.IsTurnRight(objectIds.GetCenter());
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
                                                    mtx = CalcMatrix(pt);
                                                    return SamplerStatus.OK;
                                                });
                    if (ppr.Status == PromptStatus.OK)
                    {
                        var matrix = CalcMatrix(ppr.Value);
                        objectIds.QForEach<Entity>(dbobject => dbobject.TransformBy(matrix));
                    }
                    App.LockAndExecute(() => Interaction.UnhighlightObjects(new ObjectId[] { guideId }));
                }
            }
            else
            {
                var mt = CalcMatrix(side.StartPoint);
                App.LockAndExecute(() => objectIds.QForEach<Curve>(p => p.TransformBy(mt)));
                Acad.Editor.SetImpliedSelection(objectIds);
            }
            Acad.Write($"Притягивание завершено");

            Matrix3d CalcMatrix(Point3d point)
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
