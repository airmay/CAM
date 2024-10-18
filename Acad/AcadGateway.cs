using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using CAM.Core;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using static Autodesk.AutoCAD.Windows.SaveFileDialog;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public static class Acad
    {
        public static Dictionary<Document, CamDocument> Documents = new Dictionary<Document, CamDocument>();

        public static CamDocument CamDocument => ActiveDocument != null ? Documents.TryGetAndReturn(ActiveDocument) : null;

        public static Document ActiveDocument => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;

        public static Database Database => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;

        public static Editor Editor => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

        public static void Write(string message, Exception ex = null)
        {
#if !DEBUG
            var text = ex == null ? message : $"{message}: {ex.Message}";
            Interaction.WriteLine($"{text}\n");
            ex?.WriteToFile(message);
#endif
        }
        public static void CloseAndDiscard()
        {
            //DocumentExtension.CloseAndDiscard(Application.DocumentManager.CurrentDocument);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Quit();
        }

        public static void Write(Exception ex) => Write("Ошибка", ex);

        public static void Alert(string message, Exception ex = null)
        {
            Write(message, ex);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowAlertDialog(ex == null ? message : $"{message}: {ex}");
        }

        public static void Alert(Exception ex) => Alert("Ошибка", ex);

        public static ObjectId Add(this Curve curve) => App.LockAndExecute(() => curve.AddToCurrentSpace());


        //public static void QForEach(this IEnumerable<ObjectId> ids, Action<DBObject> action)
        //{
        //    if (ids.Any())
        //        App.LockAndExecute(() => ids.QForEach(action));
        //}

        public static string GetSize(AcadObject processingArea)
        {
            if (processingArea == null)
                return "";
            var bounds = processingArea.ObjectIds.GetExtents();
            var vector = bounds.MaxPoint - bounds.MinPoint;
            return $"{vector.X.Round()} x {vector.Y.Round()} x {vector.Z.Round()}";
        }

        public static void ForEach<T>(this IEnumerable<ObjectId> ids, Action<T> action) where T : DBObject
        {
            if (ids.Any())
                App.LockAndExecute(() => ids.QForEach(action));
        }

        public static void ForEach(this IEnumerable<Curve> ids, Action<Curve> action)
        {
            if (ids?.Any() == true)
                App.LockAndExecute(() => ids.Select(p => p.ObjectId).QForEach(action));
        }

        public static Curve[] OpenForRead(IEnumerable<ObjectId> ids)
        {
            return ids.Any() ? ids.QOpenForRead<Curve>() : Array.Empty<Curve>();
        }

        public static Curve OpenForRead(ObjectId id)
        {
            if (id.IsErased)
                RecoveryObject(id);
            return id.QOpenForRead<Curve>();
        }

        public static void Show(ObjectId id)
        {
            if (id.IsErased)
                RecoveryObject(id);
            App.LockAndExecute(() => id.QOpenForWrite<Curve>(p => p.Visible = true));
        }

        public static void RecoveryObject(ObjectId id)
        {
            App.LockAndExecute(() =>
            {
                using (var trans = id.Database.TransactionManager.StartTransaction())
                {
                    var ent = (Entity)trans.GetObject(id, OpenMode.ForWrite, true);
                    ent.Erase(false);
                    trans.Commit();
                }
            });
        }

        public static void SetGroupVisibility(this ObjectId groupId, bool value)
        {
            App.LockAndExecute(() => groupId.QOpenForWrite<Group>(p => p.SetVisibility(value)));
        }

        public static void Delete(this ObjectId id) => DeleteObjects(new[] { id });

        public static void Delete(this IEnumerable<ObjectId?> ids) => DeleteObjects(ids.NotNull());

        public static void DeleteObjects(IEnumerable<ObjectId> ids)
        {
            UnhighlightAll();
            if (ids?.Any() == true)
                App.LockAndExecute(() => ids.QForEach(p => p.Erase()));
        }

        public static void DeleteGroup(this ObjectId groupId)
        {
            UnhighlightAll();
            App.LockAndExecute(() => groupId.EraseGroup());
        }

        public static ObjectId? CreateGroup(this IEnumerable<ObjectId?> entityIds)
        {
            // TODO add слой обработка
            var list = entityIds.NotNull().Distinct().ToList();
            return list.Any()
                ? (ObjectId?)App.LockAndExecute(() => list.Group(selectable: false))
                : null;
        }

        public static ObjectId AppendToGroup(this ObjectId? groupId, params ObjectId[] entityIds)
        {
            return App.LockAndExecute(() =>
            {
                if (!groupId.HasValue)
                {
                    var id = entityIds.Group(selectable: false);
                    id.QOpenForWrite<Group>(p =>
                    {
                        p.SetLayer(entityIds[0].QOpenForRead<Entity>().LayerId);
                        p.SetVisibility(true);
                    });

                    return id;
                }
                Modify.AppendToGroup(groupId.Value, entityIds);
                return groupId.Value;
            });
        }

        public static Curve[] GetSelectedCurves() => OpenForRead(Interaction.GetPickSet());

        public static ObjectId CreateOriginObject(Point2d point2d)
        {
            var length = 100;
            var point = point2d.ToPoint3d();
            var curves = new List<Curve>
            {
                NoDraw.Line(point, point + Vector3d.XAxis * length),
                NoDraw.Line(point, point + Vector3d.YAxis * length),
                NoDraw.Rectang(new Point3d(point.X - length / 10, point.Y - length / 10, 0),
                    new Point3d(point.X + length / 10, point.Y + length / 10, 0))
            };
            var layerId = GetExtraObjectsLayerId();
            curves.ForEach(p => p.LayerId = layerId);
            return App.LockAndExecute(() => curves.AddToCurrentSpace().Group(selectable: true));
        }

        public static ObjectId? GetSelectedObjectId()
        {
            return Editor.SelectImplied().Value?.GetObjectIds()[0];
        }
        public static ObjectId[] GetSelectedObjectIds()
        {
            var res = Acad.Editor.SelectImplied();
            return res.Value?.GetObjectIds();
        }

        public static ObjectId? GetToolpathId()
        {
            var res = Acad.Editor.SelectImplied();
            if (res.Status == PromptStatus.OK) // && res.Value[res.Value.Count - 1].ObjectId.QOpenForRead<Entity>().Layer == ProcessLayerName)
                return res.Value[res.Value.Count - 1].ObjectId;
            return null;
        }

        #region Select methods

        private static ObjectId[] _highlightedObjects = Array.Empty<ObjectId>();

        public static void ClearHighlighted() => _highlightedObjects = Array.Empty<ObjectId>();

        public static void UnhighlightAll() => SelectObjectIds();

        public static void SelectObjectIds(params ObjectId[] objectIds)
        {
            if (objectIds == null)
                objectIds = Array.Empty<ObjectId>();
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
            //Editor.Regen();
            Editor.UpdateScreen();
        }

        public static void SubscribeObjects(IEnumerable<ObjectId> objectIds, Action handler)
        {
            //objectIds.First().
            //objectIds.ForEach<Curve>(p => p.)
        }
        #endregion

        #region Layers
        public const string ProcessLayerName = "Обработка";
        public const string HatchLayerName = "Штриховка";
        public const string GashLayerName = "Запилы";
        public const string ExtraObjectsLayerName = "Дополнительные объекты";

        public static ObjectId GetProcessLayerId() => App.LockAndExecute(() => DbHelper.GetLayerId(ProcessLayerName));

        public static ObjectId GetExtraObjectsLayerId() => App.LockAndExecute(() => DbHelper.GetLayerId(ExtraObjectsLayerName));

        public static ObjectId GetHatchLayerId() => GetLayerId(HatchLayerName, layer => layer.Transparency = new Transparency(255 * (100 - 70) / 100));

        public static ObjectId GetGashLayerId() => GetLayerId(GashLayerName, layer => layer.Color = Color.FromColorIndex(ColorMethod.ByColor, 210));

        public static ObjectId GetLayerId(string layerName, Action<LayerTableRecord> setupLayer)
        {
            var layerId = DbHelper.GetSymbolTableRecord(HostApplicationServices.WorkingDatabase.LayerTableId, layerName, ObjectId.Null);
            if (layerId == ObjectId.Null)
                App.LockAndExecute(() =>
                {
                    layerId = DbHelper.GetLayerId(layerName);
                    layerId.QOpenForWrite(setupLayer);
                });
            return layerId;
        }

        public static void DeleteAll()
        {
            //TODO
            //ToolObject.Hide();
            if (Application.DocumentManager.MdiActiveDocument != null)
                App.LockAndExecute(() =>
                {
                    ClearHighlighted();
                    var layerTable = HostApplicationServices.WorkingDatabase.LayerTableId.QOpenForRead<SymbolTable>();
                    HostApplicationServices.WorkingDatabase.Clayer = layerTable["0"];
                    DeleteByLayer(layerTable, ProcessLayerName);
                    DeleteByLayer(layerTable, HatchLayerName);
                    DeleteByLayer(layerTable, GashLayerName);
                    DeleteByLayer(layerTable, ExtraObjectsLayerName);
                });

            void DeleteByLayer(SymbolTable layerTable, string layerName)
            {
                if (layerTable.Has(layerName))
                {
                    var ids = QuickSelection.SelectAll(FilterList.Create().Layer(layerName));
                    if (ids.Any())
                        ids.QForEach(entity => entity.Erase());
                    layerTable[layerName].Erase();
                }
            }
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

        #region Progressor

        public static Progressor _progressor;

        public static void CreateProgressor(string caption) => _progressor = new Progressor(caption);

        public static void SetLimitProgressor(int max) => _progressor.SetLimit(max);

        public static bool ReportProgressor(bool throwException = true) => _progressor.Report(throwException);

        public static void CloseProgressor() => _progressor.Stop();

        #endregion

        /// <summary>
        /// Запил
        /// </summary>
        public static ObjectId CreateGash(Curve curve, Point3d point, Side side, double depth, double diam, double thickness, Point3d? pointС = null)
        {
            var gashLength = Math.Sqrt(depth * (diam - depth));
            var normal = curve.GetFirstDerivative(point).GetNormal();
            var point2 = point + normal * gashLength * (point == curve.StartPoint ? -1 : 1);
            if (pointС.HasValue)
                point2 += pointС.Value - point;
            var offsetVector = normal.GetPerpendicularVector() * thickness * (side == Side.Left ? 1 : -1);
            var gash = NoDraw.Pline(point, point2, point2 + offsetVector, point + offsetVector);
            gash.LayerId = GetGashLayerId();
            App.LockAndExecute(() => gash.AddToCurrentSpace());
            return gash.ObjectId;
        }

        static public string SaveFileDialog(string defaultName, string extension, string dialogName)
        {
            var dialog = new SaveFileDialog("Запись файла с программой", defaultName, extension, dialogName, SaveFileDialogFlags.AllowAnyExtension);
            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK
                ? dialog.Filename
                : null;
        }

        public static ObjectId[] CreateMeasurementPoint(Point3d point) => App.LockAndExecute(() =>
        {
            int radius = 10;
            var curves = new Curve[]
            {
                NoDraw.Line(new Point3d(point.X - radius, point.Y, 0), new Point3d(point.X + radius, point.Y, 0)),
                NoDraw.Line(new Point3d(point.X, point.Y - radius, 0), new Point3d(point.X, point.Y + radius, 0)),
                NoDraw.Circle(point, radius)
            };
            var layerId = GetExtraObjectsLayerId();
            curves.Select(p => { p.LayerId = layerId; return p; }).AddToCurrentSpace();
            return Array.ConvertAll(curves, p => p.ObjectId);
        });

        public static void Register()
        {
            PlotManagerEvents1 mPlotManagerEvents1 = new PlotManagerEvents1();
            mPlotManagerEvents1.Register();
            return;

            Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher.PublishSelectedLayouts(false);

            Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher.AboutToBeginPublishing += new Autodesk.AutoCAD.Publishing.AboutToBeginPublishingEventHandler(Publisher_AboutToBeginPublishing);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher.AboutToEndPublishing += new Autodesk.AutoCAD.Publishing.AboutToEndPublishingEventHandler(Publisher_AboutToEndPublishing);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher.EndPublish += new Autodesk.AutoCAD.Publishing.EndPublishEventHandler(Publisher_EndPublish);
            Application.Publisher.BeginPublishingSheet += new Autodesk.AutoCAD.Publishing.BeginPublishingSheetEventHandler(Publisher_BeginPublishingSheet);

            void Publisher_BeginPublishingSheet(object sender, Autodesk.AutoCAD.Publishing.BeginPublishingSheetEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("\n{0}\n", "BeginPublishingSheet"));
            }

            void Publisher_EndPublish(object sender, Autodesk.AutoCAD.Publishing.PublishEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("\n{0}\n", "EndPublish"));
            }

            void Publisher_AboutToEndPublishing(object sender, Autodesk.AutoCAD.Publishing.PublishEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("\n{0}\n", "AboutToEndPublishing"));
            }
            void Publisher_AboutToBeginPublishing(object sender, Autodesk.AutoCAD.Publishing.AboutToBeginPublishingEventArgs e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("\n{0}\n", "AboutToBeginPublishing"));
            }
        }
    }

    public class PlotManagerEvents1
    {
        public void Register()
        {
            Autodesk.AutoCAD.PlottingServices.PlotReactorManager plotManager = new Autodesk.AutoCAD.PlottingServices.PlotReactorManager();
            plotManager.PlotCancelled += PlotManager_PlotCancelled_Handler;
            plotManager.PageCancelled += PlotManager_PageCancelled_Handler;
            plotManager.EndPlot += PlotManager_EndPlot_Handler;
            plotManager.EndPage += PlotManager_EndPage_Handler;
            plotManager.EndDocument += PlotManager_EndDocument_Handler;
            plotManager.BeginPlot += PlotManager_BeginPlot_Handler;
            plotManager.BeginPage += PlotManager_BeginPage_Handler;
            plotManager.BeginDocument += PlotManager_BeginDocument_Handler;

        }

        public void UnRegister()
        {
            Autodesk.AutoCAD.PlottingServices.PlotReactorManager plotManager = new Autodesk.AutoCAD.PlottingServices.PlotReactorManager();
            plotManager.PlotCancelled -= PlotManager_PlotCancelled_Handler;
            plotManager.PageCancelled -= PlotManager_PageCancelled_Handler;
            plotManager.EndPlot -= PlotManager_EndPlot_Handler;
            plotManager.EndPage -= PlotManager_EndPage_Handler;
            plotManager.EndDocument -= PlotManager_EndDocument_Handler;
            plotManager.BeginPlot -= PlotManager_BeginPlot_Handler;
            plotManager.BeginPage -= PlotManager_BeginPage_Handler;
            plotManager.BeginDocument -= PlotManager_BeginDocument_Handler;

        }

        public void PlotManager_BeginDocument_Handler(object sender, Autodesk.AutoCAD.PlottingServices.BeginDocumentEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginDocument");

        }

        public void PlotManager_BeginPage_Handler(object sender, Autodesk.AutoCAD.PlottingServices.BeginPageEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginPage");

        }

        public void PlotManager_BeginPlot_Handler(object sender, Autodesk.AutoCAD.PlottingServices.BeginPlotEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginPlot");

        }

        public void PlotManager_EndDocument_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndDocumentEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndDocument");

        }

        public void PlotManager_EndPage_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndPageEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndPage");

        }

        public void PlotManager_EndPlot_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndPlotEventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndPlot");

        }

        public void PlotManager_PageCancelled_Handler(object sender, EventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PageCancelled");

        }

        public void PlotManager_PlotCancelled_Handler(object sender, EventArgs e)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PlotCancelled");

        }
    }
}
