using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CAM.Autocad.AutoCADCommands;
using CAM.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CAM.Autocad;

/// <summary>
/// Класс осуществляющий взаимодействие с автокадом
/// </summary>
public static class Acad
{
    public static DocumentCollection DocumentManager => Application.DocumentManager;
    public static Document ActiveDocument => Application.DocumentManager.MdiActiveDocument;
    public static Database Database => Application.DocumentManager.MdiActiveDocument.Database;
    public static Editor Editor => Application.DocumentManager.MdiActiveDocument.Editor;
    public static DocumentLock LockDocument() => ActiveDocument.LockDocument();
    public static Transaction StartTransaction() => Database.TransactionManager.StartTransaction();

    public static void CloseAndDiscard()
    {
        Application.DocumentManager.CurrentDocument.CloseAndDiscard();
        Application.Quit();
    }

    #region Messages

    public static void Write(string message, Exception ex = null)
    {
        Interaction.WriteLine($"{message}. {ex?.Message}\n");
#if RELEASE
            if (ex != null)
                try
                {
                    // @"\\US-CATALINA3\public\Программы станок\CodeRepository\Logs\
                    File.WriteAllText($@"error_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log", $"{Acad.ActiveDocument.Name}\n{message}\n{ex}");
                }
                catch { }
#endif
    }

    public static void Alert(string message, Exception ex = null)
    {
        Write(message, ex);
        Application.ShowAlertDialog(ex == null ? message : $"{message}: {ex.Message}");
    }

    public static bool Confirm(string text)
    {
        return MessageBox.Show(text, "Подтверждение",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button1) == DialogResult.Yes;
    }

    #endregion

    public static void ForEach<T>(this IEnumerable<ObjectId> ids, Action<T> action) where T : DBObject
    {
        if (ids?.Any() == true)
            App.LockAndExecute(() => ids.QForEach(action));
    }

    public static void Update<T>(this ObjectId id, Action<T> action) where T : DBObject => App.LockAndExecute(() => id.QOpenForWrite(action));

    public static ObjectId[] AddToDatabase(this IEnumerable<Curve> curves) => App.LockAndExecute(() => curves.AddToCurrentSpace());
    
    public static ObjectId AddToDatabase(this Curve curve) => App.LockAndExecute(() => curve.AddToCurrentSpace());

    public static void Delete(this ObjectId id) => Delete(new[] { id });

    public static void Delete(this IEnumerable<ObjectId?> ids) => Delete(ids.NotNull().ToArray());

    public static void Delete(this ObjectId[] ids)
    {
        if (ids.Length > 0)
            App.LockAndExecute(() => ids.QForEach(p => p.Erase()));
    }

    public static void RecoveryObject(ObjectId id)
    {
        App.LockAndExecute(() =>
        {
            using var trans = id.Database.TransactionManager.StartTransaction();
            var ent = (Entity)trans.GetObject(id, OpenMode.ForWrite, true);
            ent.Erase(false);
            trans.Commit();
        });
    }

    public static void SetGroupVisibility(this ObjectId groupId, bool value) => App.LockAndExecute(() => groupId.QOpenForWrite<Group>(p => p.SetVisibility(value)));

    public static ObjectId? GetSelectedObjectId() => Editor.SelectImplied().Value?.GetObjectIds().LastOrDefault();

    #region Highlight

    private static ObjectId[] _highlightedObjects = [];

    public static void ClearHighlighted() => _highlightedObjects = [];

    public static void UnhighlightObjects() => SelectObjectIds();

    public static void SelectObjectIds(params ObjectId[] objectIds)
    {
        objectIds ??= [];
        App.LockAndExecute(() =>
        {
            try
            {
                if (_highlightedObjects.Any())
                    Interaction.UnhighlightObjects(_highlightedObjects);
                if (objectIds.Any())
                    Interaction.HighlightObjects(objectIds);
            }
            catch (Exception ex)
            {
                Acad.Write($"Error Acad.SelectObjectIds: {ex.Message}");
            }
            _highlightedObjects = objectIds;
        });
        Editor.UpdateScreen();
    }

    #endregion

    #region ProcessLayer

    private const string ProcessLayerName = "Обработка";

    public static ObjectId GetProcessLayer() => DbHelper.GetLayerId(ProcessLayerName);

    public static void DeleteProcessObjects()
    {
        var ids = QuickSelection.SelectAll(FilterList.Create().Layer(ProcessLayerName));
        ids.Delete();
    }

    public static void SetProcessLayerVisibility(bool isOff)
    {
        using var _ = LockDocument();
        using var tr = StartTransaction();
        var layerTable = (LayerTable)tr.GetObject(Database.LayerTableId, OpenMode.ForWrite);
        if (!layerTable.Has(ProcessLayerName))
            return;

        var layerTableRecord = (LayerTableRecord)tr.GetObject(layerTable[ProcessLayerName], OpenMode.ForWrite);
        layerTableRecord.IsOff = isOff;
        tr.Commit();
    }

    #endregion

    public static Transparency GetSemitransparent() => new Transparency(255 * (100 - 70) / 100);
}