using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
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

        public static ObjectId GetObjectId(long handle) => Database.GetObjectId(false, new Handle(handle), 0);

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

        public static IEnumerable<Curve> GetSelectedCurves() => OpenForRead(Interaction.GetPickSet());

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

        public static ObjectId GetProcessLayerId() => DbHelper.GetLayerId(ProcessLayerName);

        public const string HatchLayerName = "Штриховка";

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
            });
        }

        public static void DeleteHatch()
        {
            App.LockAndExecute(() =>
            {
                var ids = QuickSelection.SelectAll(FilterList.Create().Layer(HatchLayerName));
                if (ids.Any())
                    ids.QForEach(entity => entity.Erase());
            });
        }

        #endregion
    }
}
