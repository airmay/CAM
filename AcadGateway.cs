using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.PlottingServices;
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
            if (ids.Any())
                App.LockAndExecute(() => ids.Select(p => p.ObjectId).QForEach(action));
        }

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
                item.Visible = true;
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

        public static void HideExtraObjects(IEnumerable<Curve> curves, ToolModel toolModel)
        {
            curves.ForEach(p => p.Visible = !p.Visible);
            DeleteToolModel(toolModel);
            Editor.UpdateScreen();
            //Interaction.SetActiveDocFocus();
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

        /// <summary>
        /// Запил
        /// </summary>
        public static void CreateGash(Curve curve, Point3d point, Side side, int depth, double diam, double thickness, Point3d? pointС = null)
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

        public static void SaveToPdf()
        {
            if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
            {
                PlotEngine ple = PlotFactory.CreatePublishEngine();
                using (ple)
                {
                    /// Создать диалог Progress для обеспечения информацией и пользователю отменить печать
                    PlotProgressDialog ppd = new PlotProgressDialog(false, 1, true);
                    //try
                    //{
                    using (ppd)
                    {
                        ppd.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Custom Preview Progress");
                        ppd.set_PlotMsgString(PlotMessageIndex.SheetName, ActiveDocument.Name.Substring(ActiveDocument.Name.LastIndexOf("\\") + 1));
                        ppd.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                        ppd.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                        ppd.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                        ppd.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");

                        ppd.LowerPlotProgressRange = 0;
                        ppd.UpperPlotProgressRange = 100;
                        ppd.PlotProgressPos = 0;

                        /// Для определения было ли уведомление начала печати успешным
                        ppd.OnBeginPlot();
                        ppd.IsVisible = true;
                        /// Вызвать эту функцию перед началом печати
                        ple.BeginPlot(ppd, null);

                        string PDFFileName = ActiveDocument.Name.Substring(0, ActiveDocument.Name.Length - 4) + ".pdf";

                        //while (File.Exists(PDFFileName + ".pdf"))
                        //    PDFFileName += "'";
                        /// Печатаем на виртуальный принтер
                        /// 
                        using (var doclock = Application.DocumentManager.MdiActiveDocument.LockDocument())
                        {
                            using (Transaction acTrans = Database.TransactionManager.StartTransaction())
                        {
                            // Reference the Layout Manager
                            LayoutManager acLayoutMgr = LayoutManager.Current;

                            // Get the current layout and output its name in the Command Line window
                            Layout acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForRead) as Layout;
                            PlotInfo pi = new PlotInfo();
                            var lo = acLayout;
                            // We need a PlotSettings object based on the layout settings which we then customize
                            PlotSettings ps = new PlotSettings(lo.ModelType);
                            //LayoutManager.Current.CurrentLayout = lo.LayoutName;
                            pi.Layout = lo.Id;
                            ps.CopyFrom(lo);

                            // The PlotSettingsValidator helps create a valid PlotSettings object
                            PlotSettingsValidator psv = PlotSettingsValidator.Current;
                            //psv.SetPlotWindowArea(ps, window);
                            psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);
                            psv.SetUseStandardScale(ps, true);
                            psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                            psv.SetPlotCentered(ps, true);
                            psv.SetPlotConfigurationName(ps, "DWG To PDF.pc3", "ISO_A4_(297.00_x_210.00_MM)");

                            pi.OverrideSettings = ps;
                            PlotInfoValidator piv = new PlotInfoValidator();
                            piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                            piv.Validate(pi);

                            ple.BeginDocument(pi, ActiveDocument.Name, null, 1, true, PDFFileName);

                            // Который содерит один документ

                            ppd.OnBeginSheet();

                            ppd.LowerSheetProgressRange = 0;
                            ppd.UpperSheetProgressRange = 100;
                            ppd.SheetProgressPos = 0;

                            PlotPageInfo ppi = new PlotPageInfo();
                            ple.BeginPage(ppi, pi, true, null);
                            ple.BeginGenerateGraphics(null);
                            ppd.SheetProgressPos = 50;
                            ple.EndGenerateGraphics(null);

                            ///// Закончить лист

                            ple.EndPage(null);
                            ppd.SheetProgressPos = 100;
                            ppd.OnEndSheet();

                            // Finish the document

                            ple.EndDocument(null);

                            // And finish the plot

                            ppd.PlotProgressPos = 100;
                            ppd.OnEndPlot();
                            ple.EndPlot(null);
                        }
                        }

                        //catch 
                        //{

                        //}
                    }
                }
            }
        }

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
