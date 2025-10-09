// test /b "C:\Catalina\CAM\bin\Debug\netload.scr"

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CAM.Core;
using Dreambuild.AutoCAD;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

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

            Acad.DocumentManager.DocumentActivated += (sender, args) => ActivateDocument(args.Document);
            Acad.DocumentManager.DocumentToBeDeactivated += (sender, args) => DeactivateDocument(args.Document);

            ActivateDocument(Acad.ActiveDocument);
        }

        private void ActivateDocument(Document document)
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

            var camData = (CamDocument)document.UserData[CamDocumentKey];
            _processingView.SetCamData(camData.Processings, camData.Commands, camData.ProcessingIndex);
        }

        private void DeactivateDocument(Document document)
        {
            UpdateCamData(document);
            _processingView.Clear();
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName.IsIn("CLOSE", "QUIT", "QSAVE", "SAVEAS"))
            {
                var camData = UpdateCamData((Document)sender);
                camData.Save();
                _processingView.Program?.SetToolpathVisibility(true);
            }
        }

        private CamDocument UpdateCamData(Document document)
        {
            var camData = (CamDocument)document.UserData[CamDocumentKey];
            camData.Set(_processingView.GetTechProcesses(), _processingView.Program);

            return camData;
        }

        private void DocumentOnImpliedSelectionChanged(object sender, EventArgs e) => _processingView.SelectCommand(Acad.GetSelectedObjectId());

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            ((Document)sender).CommandWillStart -= Document_CommandWillStart;
            ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;
            ((Document)sender).ImpliedSelectionChanged -= DocumentOnImpliedSelectionChanged;

            _processingView.Clear();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
        }
    }
}
