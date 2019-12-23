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
        private ObjectId[] _objectIds;
        private ObjectId _sideId;
        private ObjectId _guideId;

        public PullingView()
        {
            InitializeComponent();
        }

        private void bSetObjects_Click(object sender, EventArgs e)
        {
            var objectIds = Interaction.GetPickSet();
            if (objectIds.Length == 0)
            {
                Acad.Alert($"Выделите притягиваемые кривые");
                return;
            }
            _objectIds = objectIds;
            lbObjects.Text = $"{_objectIds.Length} объекта";
            bSelectObjects.Visible = true;
            RefreshAcad();
        }

        private void bSelectObjects_Click(object sender, EventArgs e)
        {
            Interaction.SetPickSet(_objectIds);
            Acad.Editor.UpdateScreen();
        }

        private void bSetSide_Click(object sender, EventArgs e)
        {
            var objectIds = Interaction.GetPickSet();
            if (objectIds.Length != 1 || objectIds[0] == _guideId)
            {
                Acad.Alert($"Выделите притягиваемую сторону");
                return;
            }
            var curve = objectIds[0].QOpenForRead<Line>();
            if (curve == null)
            {
                Acad.Alert($"Притягиваемая сторона должна являться отрезком");
                return;
            }
            _sideId = objectIds[0];
            lbSide.Text = $"Отрезок {{{Math.Round(curve.StartPoint.X)},{Math.Round(curve.StartPoint.Y)}}}, {{{Math.Round(curve.EndPoint.X)},{Math.Round(curve.EndPoint.Y)}}}";
            bSelectSide.Visible = true;
            RefreshAcad();

            if (_objectIds != null && !_objectIds.Contains(_sideId))
                _objectIds = _objectIds.Concat(new[] { _sideId }).ToArray();
        }

        private void bSelectSide_Click(object sender, EventArgs e)
        {
            Interaction.SetPickSet(new[] { _sideId });
            Acad.Editor.UpdateScreen();
        }

        private void bSetGuide_Click(object sender, EventArgs e)
        {
            var objectIds = Interaction.GetPickSet();
            if (objectIds.Length != 1 || objectIds[0] == _sideId)
            {
                Acad.Alert($"Выделите направляющиую кривую");
                return;
            }
            var curve = objectIds[0].QOpenForRead<Line>();
            if (curve == null)
            {
                Acad.Alert($"Направляющая кривая должна являться отрезком");
                return;
            }
            _guideId = objectIds[0];
            lbGuide.Text = $"Отрезок {{{Math.Round(curve.StartPoint.X)},{Math.Round(curve.StartPoint.Y)}}}, {{{Math.Round(curve.EndPoint.X)},{Math.Round(curve.EndPoint.Y)}}}";
            bSelectGuide.Visible = true;
            RefreshAcad();
        }

        private void bSelectGuide_Click(object sender, EventArgs e)
        {
            Interaction.SetPickSet(new[] { _guideId });
            Acad.Editor.UpdateScreen();
        }

        private void RefreshAcad()
        {
            CheckPullingPanelVisible();
            Acad.Editor.SetImpliedSelection(new ObjectId[] { });
            Acad.Editor.UpdateScreen();
            Interaction.SetActiveDocFocus();
        }
        private void CheckPullingPanelVisible() => gbPulling.Visible = _objectIds != null && !_sideId.IsNull && !_guideId.IsNull;

        private void bPulling_Click(object sender, EventArgs e)
        {
            var guide = _guideId.QOpenForRead<Line>();
            var side = _sideId.QOpenForRead<Line>();
            if (!Double.TryParse(tbDist.Text, out double dest))
            {
                Acad.Alert($"Укажите дистанцию до направляющей");
                return;
            }
            var guideVector = guide.GetVector2d();
            var sideVector = side.GetVector2d();

            var objCenter = _objectIds.GetCenter().ToPoint2d();
            var objVector = objCenter - side.StartPoint.ToPoint2d();
            var isObjLeft = sideVector.MinusPiToPiAngleTo(objVector) > 0;

            var normalVector = guide.Normal.GetPerpendicularVector() * dest;

            Interaction.SetActiveDocFocus();

            using (Acad.ActiveDocument.LockDocument())
            {
                Interaction.HighlightObjects(new[] { _guideId });
                    Acad.Editor.SetImpliedSelection(_objectIds);
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
                    _objectIds.QForEach<Entity>(dbobject => dbobject.TransformBy(matrix));
                }
                Acad.Editor.SetImpliedSelection(new ObjectId[] { });
            }

            Matrix3d CalcMatrix(Point3d point)
            {
                var ptVector = point.ToPoint2d() - guide.StartPoint.ToPoint2d();
                var isPointLeft = guideVector.MinusPiToPiAngleTo(ptVector) > 0;

                var rotAngle = sideVector.ZeroTo2PiAngleTo((isObjLeft ^ isPointLeft) ? guideVector.Negate() : guideVector);
                var rotCenter = (isObjLeft ^ isPointLeft) ? side.EndPoint : side.StartPoint;
                var rotationMatrix = Matrix3d.Rotation(rotAngle, Vector3d.ZAxis, rotCenter);

                point = guide.GetClosestPointTo(point, true);
                point += isPointLeft ? -normalVector : normalVector;
                var displacementMatrix = Matrix3d.Displacement(rotCenter.GetVectorTo(point));

                return displacementMatrix * rotationMatrix;
            }
        }
    }
}
