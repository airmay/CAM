using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using Graph = CAM.Autocad.Graph;

namespace CAM.UtilViews
{
    public partial class TrimmingView : UserControl
    {
        class Item
        {
            public Point3d Point;
            public Vector3d Vector;
            public ObjectId ImageId;
            public ObjectId ClippedImageId;
        }
        private Dictionary<ObjectId, Item> _items = new Dictionary<ObjectId, Item>();

        public TrimmingView()
        {
            InitializeComponent();
        }
       
        private void bObjects_Click(object sender, EventArgs e)
        {
            Interaction.SetActiveDocFocus();
            var ids = Interaction.GetSelection("\nВыберите контур");
            if (ids.Length > 0)
            {
                tbObjects.Text = ids.GetDesc();
                CreatePline(ids);
                tbItemsCount.Text = _items.Count.ToString();
            }
        }

        private void CreatePline(ObjectId[] ids)
        {
            var curves = App.LockAndExecute(() => ids.QOpenForRead<Curve>())
                .SelectMany(p =>
                {
                    if (p is Polyline)
                    {
                        var results = new DBObjectCollection();
                        p.Explode(results);
                        return results.Cast<Curve>();
                    }
                    return new List<Curve>{ p };
                })
                .ToArray();
            var curve = curves[0];
            var item = new Item
            {
                Point = curve.StartPoint,
                Vector = curve.GetFirstDerivative(0),
                ImageId = ObjectId.Null,
                ClippedImageId = ObjectId.Null
            };
            var point = curve.EndPoint;
            var points = new List<Point3d>(ids.Length) { item.Point, point };
            while (point != item.Point)
            {
                curve = curves.FirstOrDefault(p => p != curve && p.HasPoint(point));
                if (curve == null)
                    throw new Exception($"Не найден соседний отрезок в точке {point.X},{point.Y}");
                point = curve.NextPoint(point);
                points.Add(point);
            }
            var pline = NoDraw.Pline(points);
            pline.Move(Vector3d.XAxis.RotateBy(Graph.ToRad(-45), Vector3d.ZAxis) * 500);
            App.LockAndExecute(() =>
            {
                var plineId = pline.AddToCurrentSpace();
                plineId.QOpenForWrite<Polyline>(p => p.Modified += Pline_Modified);
                _items.Add(plineId, item);
                Interaction.SetPickSet(new[] { plineId });
            });
            Interaction.SetActiveDocFocus();
            Acad.Editor.UpdateScreen();
        }

        private bool ImageContain(ObjectId imageId, Point3d point)
        {
            var vertices = imageId.QOpenForRead<RasterImage>().GetVertices();
            return point.X > vertices[0].X && point.X < vertices[2].X && point.Y > vertices[0].Y && point.Y < vertices[2].Y;
        }

        private void Pline_Modified(object sender, EventArgs e)
        {
            var pline = sender as Polyline;
            var item = _items[pline.Id];
            if (item.ImageId != ObjectId.Null)
            {
                if (ImageContain(item.ImageId, pline.StartPoint))
                {
                    ClipImage(item, pline);
                    return;
                }
                else
                {
                    item.ClippedImageId.Erase();
                    item.ClippedImageId = ObjectId.Null;
                    item.ImageId = ObjectId.Null;
                }
            }
            var imagesResult = Acad.Editor.SelectAll(new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, "IMAGE") }));
            if (imagesResult.Status == PromptStatus.OK)
            {
                var imageIds = imagesResult.Value.GetObjectIds();
                foreach (var imageId in imageIds)
                {
                    if (ImageContain(imageId, pline.StartPoint))
                    {
                        item.ImageId = imageId;
                        App.LockAndExecute(() => imageId.QOpenForWrite<RasterImage>(image => item.ClippedImageId = ((RasterImage)image.Clone()).AddToCurrentSpace()));
                        ClipImage(item, pline);
                        return;
                    }
                }
            }
        }

        private void ClipImage(Item item, Polyline pline)
        {
            item.ClippedImageId.QOpenForWrite<RasterImage>(clippedImage =>
            {
                var image = item.ImageId.QOpenForRead<RasterImage>();

                clippedImage.TransformBy(Matrix3d.Rotation(-clippedImage.Rotation, Vector3d.ZAxis, item.Point));
                var matrixD = Matrix3d.Displacement(clippedImage.Position.GetVectorTo(item.Point + pline.StartPoint.GetVectorTo(image.Position)));
                var matrixR = Matrix3d.Rotation(pline.GetFirstDerivative(0).GetAngleTo(item.Vector, Vector3d.ZAxis), Vector3d.ZAxis, item.Point);
                clippedImage.TransformBy(matrixR * matrixD);

                var imageMatrix = image.PixelToModelTransform.Inverse();
                var points = pline.GetPoints().Select(p => p.TransformBy(imageMatrix).Convert2d(new Plane())).ToArray();

                clippedImage.SetClipBoundary(ClipBoundaryType.Poly, new Point2dCollection(points));
                clippedImage.IsClipped = true;
            });
        }
    }
}
