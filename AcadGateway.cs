using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public static class Acad
    {
        public const string ProcessLayerName = "Обработка";

        public static Document Document => Application.DocumentManager.MdiActiveDocument;

        public static Database Database => Application.DocumentManager.MdiActiveDocument.Database;

        public static Editor Editor => Application.DocumentManager.MdiActiveDocument.Editor;

        public static void WriteMessage(string message) => Interaction.WriteLine($"{message}\n");

        public static void Alert(string message)
        {
            WriteMessage(message);
            Application.ShowAlertDialog(message);
        }

        public static ObjectId GetObjectId(long handle) => Database.GetObjectId(false, new Handle(handle), 0);

        public static void SaveCurves(IEnumerable<Curve> entities)
        {
            App.LockAndExecute(() =>
            {                
                var layerId = DbHelper.GetLayerId(ProcessLayerName);
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
                if (_highlightedObjects.Any())
                    Interaction.UnhighlightObjects(_highlightedObjects);
                if (objectIds.Any())
                    Interaction.HighlightObjects(objectIds);
                _highlightedObjects = objectIds;
            });
            Editor.UpdateScreen();
        } 
        #endregion

        public static void DeleteProcessLayer()
        {
            App.LockAndExecute(() =>
            {
                var layerTable = HostApplicationServices.WorkingDatabase.LayerTableId.QOpenForRead<SymbolTable>();
                if (layerTable.Has(ProcessLayerName))
                {
                    HostApplicationServices.WorkingDatabase.Clayer = layerTable["0"];
                    var ids = QuickSelection.SelectAll(FilterList.Create().Layer(ProcessLayerName));
                    ids.QForEach(entity => entity.Erase());
                    layerTable[ProcessLayerName].Erase();
                }
            });
        }
    }
}
