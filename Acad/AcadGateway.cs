using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Windows;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Autodesk.AutoCAD.Windows.SaveFileDialog;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace CAM
{
    /// <summary>
    /// Класс осуществляющий взаимодействие с автокадом
    /// </summary>
    public static class Acad
    {
        public static Document ActiveDocument => AcadApplication.DocumentManager.MdiActiveDocument;

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

        public static void ForEach(this IEnumerable<ObjectId> ids, Action<DBObject> action)
        {
            if (ids.Any())
                App.LockAndExecute(() => ids.QForEach(action));
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

        public static void DeleteObjects(IEnumerable<ObjectId> ids)
        {
            if (ids != null && ids.Any())
            {
                _highlightedObjects = _highlightedObjects.Except(ids).ToArray();
                App.LockAndExecute(() => ids.QForEach(p => p.Erase()));
            }
        }

        public static Curve[] GetSelectedCurves() => OpenForRead(Interaction.GetPickSet());

        public static ObjectId[] CreateOriginObject(Point3d point)
        {
            int length = 100;
            var curves = new Curve[]
            {
                NoDraw.Line(point, point + Vector3d.XAxis * length),
                NoDraw.Line(point, point + Vector3d.YAxis * length),
                NoDraw.Rectang(new Point3d(point.X - length / 10, point.Y - length / 10, 0), new Point3d(point.X + length / 10, point.Y + length / 10, 0))
            };
            var layerId = GetExtraObjectsLayerId();
            App.LockAndExecute(() => curves.Select(p => { p.LayerId = layerId; return p; }).AddToCurrentSpace());
            return Array.ConvertAll(curves, p => p.ObjectId);
        }

        #region ToolObject

        public static ToolObject ToolObject { get; set; }

        public static void ShowToolObject(Tool tool, int index, Location location, bool isFrontPlaneZero)
        {
            if (ToolObject != null && (tool != ToolObject.Tool || index != ToolObject.Index || !location.IsDefined))
                DeleteToolObject();
            if (ToolObject == null && index != 0 && location.IsDefined)
                ToolObject = ToolObject.CreateToolObject(tool, index, isFrontPlaneZero);
            if (ToolObject != null)
                DrawToolObject(location);
        }

        private static void DrawToolObject(Location location)
        {
            var mat1 = Matrix3d.Displacement(ToolObject.Location.Point.GetVectorTo(location.Point));
            var mat2 = Matrix3d.Rotation(Graph.ToRad(ToolObject.Location.AngleC - location.AngleC), Vector3d.ZAxis, location.Point);
            var mat3 = Matrix3d.Rotation(Graph.ToRad(location.AngleA - ToolObject.Location.AngleA), Vector3d.XAxis.RotateBy(Graph.ToRad(-location.AngleC), Vector3d.ZAxis), location.Point);
            var mat = mat3 * mat2 * mat1;
            foreach (var item in ToolObject.Curves)
            {
                item.TransformBy(mat);
                TransientManager.CurrentTransientManager.UpdateTransient(item, new IntegerCollection());
            }
            ToolObject.Location = location;
            Editor.UpdateScreen();
        }

        public static void DeleteToolObject()
        {
            if (ToolObject == null)
                return;
            using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
            using (Transaction tr = Database.TransactionManager.StartTransaction())
                foreach (var item in ToolObject.Curves)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(item, new IntegerCollection());
                    item.Dispose();
                }
            ToolObject = null;
        }

        #endregion

        #region Select methods

        private static ObjectId[] _highlightedObjects = Array.Empty<ObjectId>();

        public static void ClearHighlighted() => _highlightedObjects = Array.Empty<ObjectId>();

        public static void SelectAcadObjects(List<AcadObject> objects) => SelectObjectIds(objects?.Select(p => p.ObjectId).ToArray());

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
            Editor.UpdateScreen();
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
            DeleteToolObject();
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

        public static void DeleteExtraObjects()
        {
            DeleteByLayer(HatchLayerName);
            DeleteByLayer(GashLayerName);
            DeleteToolObject();
        }

        //public static void HideExtraObjects(IEnumerable<Curve> curves)
        //{
        //    curves.ForEach(p => p.Visible = !p.Visible);
        //    DeleteToolObject();
        //    Editor.UpdateScreen();
        //    //Interaction.SetActiveDocFocus();
        //}

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

        /// <summary>
        /// Запил
        /// </summary>
        public static void CreateGash(Curve curve, Point3d point, Side side, double depth, double diam, double thickness, Point3d? pointС = null)
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

            Application.Publisher.PublishSelectedLayouts(false);

            Application.Publisher.AboutToBeginPublishing += new Autodesk.AutoCAD.Publishing.AboutToBeginPublishingEventHandler(Publisher_AboutToBeginPublishing);
            Application.Publisher.AboutToEndPublishing += new Autodesk.AutoCAD.Publishing.AboutToEndPublishingEventHandler(Publisher_AboutToEndPublishing);
            Application.Publisher.EndPublish += new Autodesk.AutoCAD.Publishing.EndPublishEventHandler(Publisher_EndPublish);
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
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginDocument");

        }

        public void PlotManager_BeginPage_Handler(object sender, Autodesk.AutoCAD.PlottingServices.BeginPageEventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginPage");

        }

        public void PlotManager_BeginPlot_Handler(object sender, Autodesk.AutoCAD.PlottingServices.BeginPlotEventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("BeginPlot");

        }

        public void PlotManager_EndDocument_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndDocumentEventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndDocument");

        }

        public void PlotManager_EndPage_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndPageEventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndPage");

        }

        public void PlotManager_EndPlot_Handler(object sender, Autodesk.AutoCAD.PlottingServices.EndPlotEventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("EndPlot");

        }

        public void PlotManager_PageCancelled_Handler(object sender, EventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PageCancelled");

        }

        public void PlotManager_PlotCancelled_Handler(object sender, EventArgs e)
        {
            AcadApplication.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PlotCancelled");

        }
    }
}
