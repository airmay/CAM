using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CAM
{
    public class ToolObject
    {
        public static Curve[] Model { get; set; }
        private static Machine? _machine;
        private static ITool _tool;
        private static IToolLocation _location;

        public static void Set(Machine? machine, ITool tool, IToolLocation location)
        {
            if (machine != _machine || tool != _tool || !location.IsDefined)
            {
                Hide();
                _machine = machine;
                _tool = tool;
            }

            if (location.IsDefined)
            {
                if (Model == null)
                {
                    Model = tool.GetModel(_machine);
                    AddModel(Model);
                    _location = location.Origin;
                }

                var matrix = location.GetTransformMatrixFrom(_location);
                TransformModel(Model, matrix);
                _location = location;
            }
        }

        public static void Hide()
        {
            if (Model == null || Application.DocumentManager.MdiActiveDocument == null)
                return;

            using (Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Acad.Database.TransactionManager.StartTransaction())
                foreach (var item in Model)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }

            Model = null;
        }

        private static void AddModel(Curve[] curves)
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
        }

        private static void TransformModel(Curve[] curves, Matrix3d matrix)
        {
            foreach (var item in curves)
            {
                item.TransformBy(matrix);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }

            Acad.Editor.UpdateScreen();
        }
    }
}
