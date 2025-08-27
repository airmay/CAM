using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CAM.Core;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CAM
{
    public static class ToolObject
    {
        private static Curve[] _model;
        private static Tool _tool;
        private static ToolPosition _location;
        public static Machine Machine { get; set; }

        public static void Set(Tool tool, ToolPosition location)
        {
            if (tool == null || tool != _tool)
                Hide();

            if (tool == null) 
                return;

            if (_model == null)
                CreateModel(tool.GetModel(Machine));

            TransformModel(tool.GetTransformMatrix(_location, location));

            _location = location;
            _tool = tool;
        }

        public static void Hide()
        {
            if (_model == null || Acad.ActiveDocument == null)
                return;

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Acad.Database.TransactionManager.StartTransaction())
                foreach (var item in _model)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }

            _model = null;
        }

        private static void CreateModel(Curve[] curves)
        {
            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (var tr = Acad.Database.TransactionManager.StartTransaction())
            {
                foreach (var item in curves)
                {
                    item.Color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128,
                        new IntegerCollection());
                }

                tr.Commit();
            }
            _model = curves;
            _location = new ToolPosition();
        }

        private static void TransformModel(Matrix3d matrix)
        {
            foreach (var item in _model)
            {
                item.TransformBy(matrix);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }

            Acad.Editor.UpdateScreen();
        }
    }
}
