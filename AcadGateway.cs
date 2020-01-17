using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public static class Acad
    {
        public static Document ActiveDocument => Application.DocumentManager.MdiActiveDocument;

        public static Database Database => Application.DocumentManager.MdiActiveDocument.Database;

        public static Editor Editor => Application.DocumentManager.MdiActiveDocument.Editor;

        public static void Write(string message, Exception ex = null)
        {
            var text = ex == null ? message : $"{message}: {ex.Message}";
            Interaction.WriteLine($"{text}\n");
            if (ex != null)
                File.WriteAllText($@"\\US-CATALINA3\public\Программы станок\CodeRepository\Logs\error_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log", $"{Acad.ActiveDocument.Name}\n\n{message}\n\n{ex.FullMessage()}");
        }
        public static void Write(Exception ex) => Write("Ошибка", ex);

        public static void Alert(string message, Exception ex = null)
        {
            Write(message, ex);
            Application.ShowAlertDialog(ex == null ? message : $"{message}: {ex.Message}");
        }

        public static void Alert(Exception ex) => Alert("Ошибка", ex);

        public static ObjectId[] GetObjectIds(long[] handles) => Array.ConvertAll(handles, p => Database.GetObjectId(false, new Handle(p), 0));

        public static void SaveCurves(IEnumerable<Curve> entities)
        {
            App.LockAndExecute(() =>
            {
                var layerId = GetProcessLayerId();
                entities.Select(p => { p.LayerId = layerId; return p; }).AddToCurrentSpace();
            });
        }

        public static void DeleteCurves(IEnumerable<Curve> curves)
        {
            if (curves != null && curves.Any())
            {
                var ids = curves.Select(p => p.ObjectId).ToArray();
                _highlightedObjects = _highlightedObjects.Except(ids).ToArray();
                App.LockAndExecute(() => ids.QForEach(p => p.Erase()));
            }
        }

        #region ToolModel
        public static void DrawToolModel(ToolModel toolModel, Point3d? endPoint, double? toolAngle)
        {
            if (endPoint == null)
                return;
            var mat = Matrix3d.Displacement(toolModel.Origin.GetVectorTo(endPoint.Value)) * Matrix3d.Rotation(Graph.ToRad(toolModel.Angle - toolAngle.Value), Vector3d.ZAxis, toolModel.Origin);
            foreach (var item in toolModel.GetCurves())
            {
                item.TransformBy(mat);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }
            toolModel.Origin = endPoint.Value;
            toolModel.Angle = toolAngle.Value;
            Editor.UpdateScreen();
        }

        public static ToolModel CreateToolModel(double diameter, double thickness)
        {
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (Transaction tr = Database.TransactionManager.StartTransaction())
                {
                    var color = Color.FromColorIndex(ColorMethod.ByColor, 131);
                    var center = new Point3d(0, 0, diameter/2);
                    var toolModel = new ToolModel
                    {
                        Circle0 = new Circle(center, Vector3d.YAxis, diameter/2) { Color = color },
                        Circle1 = new Circle(center - Vector3d.YAxis * thickness, Vector3d.YAxis, diameter/2) { Color = color },
                        Axis = new Line(center, center + Vector3d.YAxis * diameter/4),
                        Origin = Point3d.Origin,
                        Angle = 0
                    };
                    foreach (var item in toolModel.GetCurves())
                        TransientManager.CurrentTransientManager.AddTransient(item, TransientDrawingMode.Main, 128, new IntegerCollection());
                    tr.Commit();
                    return toolModel;
                }
            }
        }

        public static void DeleteToolModel(ToolModel toolModel)
        {
            if (toolModel == null)
                return;
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Database.TransactionManager.StartTransaction())
                foreach (var item in toolModel.GetCurves())
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }
        }

        #endregion

        public static Curve[] GetSelectedCurves() => OpenForRead(Interaction.GetPickSet());

        public static Curve[] OpenForRead(IEnumerable<ObjectId> ids)
        {
            return ids.Any() ? ids.QOpenForRead<Curve>() : Array.Empty<Curve>();
        }

        #region Select methods

        private static ObjectId[] _highlightedObjects = Array.Empty<ObjectId>();

        public static void ClearHighlighted() => _highlightedObjects = Array.Empty<ObjectId>();

        public static void SelectCurve(Curve curve)
        {
            if (curve != null)
                SelectObjectIds(curve.ObjectId);
            else
                SelectObjectIds();
        }

        public static void SelectObjectIds(params ObjectId[] objectIds)
        {
            App.LockAndExecute(() =>
            {
                try
                {
                    if (_highlightedObjects.Any())
                        Interaction.UnhighlightObjects(_highlightedObjects);
                    if (objectIds.Any())
                        Interaction.HighlightObjects(objectIds);
                }
                catch { }
                _highlightedObjects = objectIds;
            });
            Editor.UpdateScreen();
        }
        #endregion

        #region Layers
        public const string ProcessLayerName = "Обработка";
        public const string HatchLayerName = "Штриховка";
        public const string GashLayerName = "Запилы";

        public static ObjectId GetProcessLayerId() => DbHelper.GetLayerId(ProcessLayerName);

        public static ObjectId GetHatchLayerId()
        {
            var layerId = DbHelper.GetSymbolTableRecord(HostApplicationServices.WorkingDatabase.LayerTableId, HatchLayerName, ObjectId.Null);
            if (layerId == ObjectId.Null)
            {
                layerId = DbHelper.GetLayerId(HatchLayerName);
                layerId.QOpenForWrite<LayerTableRecord>(layer => layer.Transparency = new Autodesk.AutoCAD.Colors.Transparency(255 * (100 - 70) / 100));
            }
            return layerId;
        }

        public static void DeleteAll()
        {
            App.LockAndExecute(() =>
            {
                ClearHighlighted();
                var layerTable = HostApplicationServices.WorkingDatabase.LayerTableId.QOpenForRead<SymbolTable>();
                if (layerTable.Has(ProcessLayerName))
                {
                    HostApplicationServices.WorkingDatabase.Clayer = layerTable["0"];
                    var ids = QuickSelection.SelectAll(FilterList.Create().Layer(ProcessLayerName));
                    ids.QForEach(entity => entity.Erase());
                    layerTable[ProcessLayerName].Erase();
                }
                if (layerTable.Has(HatchLayerName))
                {
                    HostApplicationServices.WorkingDatabase.Clayer = layerTable["0"];
                    var ids = QuickSelection.SelectAll(FilterList.Create().Layer(HatchLayerName));
                    ids.QForEach(entity => entity.Erase());
                    layerTable[HatchLayerName].Erase();
                }
                if (layerTable.Has(GashLayerName))
                {
                    HostApplicationServices.WorkingDatabase.Clayer = layerTable["0"];
                    var ids = QuickSelection.SelectAll(FilterList.Create().Layer(GashLayerName));
                    ids.QForEach(entity => entity.Erase());
                    layerTable[GashLayerName].Erase();
                }
            });
        }

        public static void DeleteExtraObjects(IEnumerable<Curve> curves, ToolModel toolModel)
        {
            DeleteCurves(curves);
            DeleteByLayer(HatchLayerName);
            DeleteByLayer(GashLayerName);
            DeleteToolModel(toolModel);
        }

        public static void DeleteByLayer(string layerName)
        {
            App.LockAndExecute(() => 
            {
                var ids = QuickSelection.SelectAll(FilterList.Create().Layer(layerName));
                if (ids.Any())
                    ids.QForEach(entity => entity.Erase());
            });
        }
        #endregion

        public static void SaveGash(Curve entity)
        {
            App.LockAndExecute(() =>
            {
                var layerId = DbHelper.GetSymbolTableRecord(HostApplicationServices.WorkingDatabase.LayerTableId, GashLayerName, ObjectId.Null);
                if (layerId == ObjectId.Null)
                {
                    layerId = DbHelper.GetLayerId(GashLayerName);
                    layerId.QOpenForWrite<LayerTableRecord>(layer => layer.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByColor, 210));
                }
                entity.LayerId = layerId;
                entity.AddToCurrentSpace();
            });
        }

    }
}
