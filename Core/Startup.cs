// test /b "C:\Catalina\CAM\bin\Debug\netload.scr"

using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using Autodesk.AutoCAD.Windows;
using System.Drawing;

namespace CAM
{
    public class Startup : IExtensionApplication
    {
        private const string CamDocumentKey = "CamDocument";
        private readonly ProcessingView _processingView = new ProcessingView();

        public void Initialize()
        {
            Tolerance.Global = new Tolerance( Consts.Epsilon, Consts.Epsilon);

            // Assembly.GetExecutingAssembly().GetProductVersion()
            Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

            var paletteSet = new PaletteSet("Технология")
            {
                Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                MinimumSize = new Size(300, 200),
                KeepFocus = true,
                Visible = true,
                Dock = DockSides.Left
            };
            paletteSet.Add("Обработка", _processingView);
            paletteSet.Add("Инструменты", new UtilsView());

            Acad.DocumentManager.DocumentToBeDeactivated += (sender, args) =>
            {
                _processingView.UpdateCamDocument();
                _processingView.Clear();
            };
            Acad.DocumentManager.DocumentBecameCurrent += (sender, args) => SetActiveDocument(args.Document);

            SetActiveDocument(Acad.ActiveDocument);
        }

        private void SetActiveDocument(Document document)
        {
            if (document == null || !document.IsActive)
                return;

            if (!document.UserData.ContainsKey(CamDocumentKey))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                document.ImpliedSelectionChanged += DocumentOnImpliedSelectionChanged;

                document.UserData[CamDocumentKey] = CamDocument.Create();
            }

            _processingView.SetCamDocument((CamDocument)document.UserData[CamDocumentKey]);
        }

        private void DocumentOnImpliedSelectionChanged(object sender, EventArgs e) => _processingView.SelectCommand(Acad.GetSelectedObjectId());

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName.IsIn("CLOSE", "QUIT", "QSAVE", "SAVEAS"))
            {
                _processingView.SaveCamDocument();
            }
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            ((Document)sender).CommandWillStart -= Document_CommandWillStart;
            ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;

            _processingView.Clear();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
        }
    }
}
