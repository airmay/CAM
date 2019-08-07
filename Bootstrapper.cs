using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CAM.Domain;
using CAM.UI;
using Dreambuild.AutoCAD;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace CAM
{
    public class Bootstrapper : IExtensionApplication
    {
        public void Initialize()
        {
            Acad.WriteMessage($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

             var manager = new CamManager();

            PaletteSet paletteSet = CreatePaletteSet();

            //var techProcessView = new TechProcessView();
            paletteSet.Add("Объекты", new TechProcessView(manager));
            paletteSet.Add("Программа", new ProgramView());

            Application.DocumentManager.DocumentActivated += (sender, args) => manager.SetActiveDocument(args.Document);

            manager.SetActiveDocument(Acad.Document);

            //PrepareTest();

            //PaletteSet focus use Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            //PaletteSet.PaletteActivated

            //Application.DocumentManager.DocumentActivated += (sender, args) => SetActiveDocument(args.Document);

            //ProcessingForm.CurrentChanged += (sender, args) => AutocadUtils.SelectObjects(args.Data.ObjectId, args.Data.ToolObjectId);
            //ProcessingForm.DeleteProcessing += (sender, args) => DeleteProcessing();
            //ProcessingForm.Calculate += (sender, args) => Calculate();
            //AutocadUtils.AddPaletteSet("Обработка", ProcessingForm);

            //AutocadUtils.AddPaletteSet("Программа", ProgramForm);
            //AutocadUtils.AddPaletteSet("Настройки", SettingForm);

            //AutocadUtils.Selected += (sender, args) => ProcessingForm.SelectObjects(args.Data);
            //AutocadUtils.Focused += (sender, args) => ProcessingForm.SetFocus(args.Data);
            //AutocadUtils.Focused += (sender, args) => ObjectForm.SetFocus(args.Data);

            //CalcUtils.Init(ProcessObjects, ProcessCurves);
            //ProcessObjectFactory.Init(ProcessObjects, ProcessCurves);

            //var machine = new Machine();
            //machine.ChangeActionsCount += (sender, args) => ObjectForm.ShowProgress(String.Format("Генерация обработки... {0} строк", args.Data));
            //ActionGenerator.SetMachine(machine);

            //AutocadUtils.CreateTest();
            //RunTest();

        }

        private void PrepareTest()
        {
            var rect = NoDraw.Rectang(new Point3d(3000, 2000, 0), new Point3d(5000, 3000, 0));
            DBObjectCollection entitySet = new DBObjectCollection();
            rect.SetBulgeAt(2, 0.5);
            rect.Explode(entitySet);
            foreach (Entity ent in entitySet)
                ent.AddToCurrentSpace();
        }

        //private void DocumentManagerOnDocumentLockModeChanged(object sender, DocumentLockModeChangedEventArgs documentLockModeChangedEventArgs)
        //{
        //    if (documentLockModeChangedEventArgs.GlobalCommandName.ToUpper() == "QUIT")
        //    {
        //        Acad.DeleteProcessLayer();
        //        //_manager.SaveTechProsess();
        //        //documentLockModeChangedEventArgs.Veto();
        //        Application.DocumentManager.DocumentLockModeChanged -= DocumentManagerOnDocumentLockModeChanged;
        //    }
        //}

        public void Terminate()
        {
            CamContainer.SaveData();
            //SettingForm.RefreshSettings();
            //ProcessingParams.SaveDefault();
            //Settings.Save();
        }

        private PaletteSet CreatePaletteSet() => 
            new PaletteSet("Технология")
        {
            Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
            MinimumSize = new Size(300, 200),
            KeepFocus = true,
            Visible = true
        };

        //[CommandMethod("show")]
        //public void ShowPaletteSet()
        //{
        //    //AutocadUtils.ShowPaletteSet();
        //}

        //[Conditional("DEBUG")]
        //private void RunTest()
        //{
        //    //var selectedObjects = AutocadUtils.GetAllCurves();
        //    //if (selectedObjects == null || !Tools.Any())
        //    //    return;
        //    //ProcessObjectFactory.Create(selectedObjects.FindAll(p => p.GetLength() > 100), Tools.Where(p => p.No == 2));
        //    ///*
        //    //SectionCurves.AddRange(selectedObjects.FindAll(p => p.GetLength() < 100).Cast<Curve>().ToList());
        //    //var points = SectionCurves.Select(p => p.StartPoint.Y).Concat(SectionCurves.Select(p => p.EndPoint.Y));
        //    //Settings.GetInstance().HeightMax = points.Max();
        //    //Settings.GetInstance().HeightMin = points.Min();
        //    //SettingForm.RefreshForm();
        //    // * */
        //    //Calculate();
        //    //ObjectForm.RefreshList();
        //}
    }
}
