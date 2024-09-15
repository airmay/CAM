using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.Geometry;
using Dreambuild.AutoCAD;
using Autodesk.AutoCAD.DatabaseServices;

namespace CAM
{
    public class Startup : IExtensionApplication
    {
        public const string CamDocumentKey = "CamDocument";
        private ProcessingView _processingView = new ProcessingView();

        public void Initialize()
        {
            Tolerance.Global = new Tolerance( Consts.Epsilon, Consts.Epsilon);

            Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

            var camPaletteSet = new CamPaletteSet();

            //var manager = new CamManager();

            //PaletteSet paletteSet = new PaletteSet("Технология")
            //{
            //    Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
            //    MinimumSize = new Size(300, 200),
            //    KeepFocus = true,
            //    Visible = true
            //};
            //paletteSet.Add("Объекты", new TechProcessView(manager));
            //var programView = new ProgramView(manager);
            //var programmPalette = paletteSet.Add("Программа", programView);
            //paletteSet.PaletteActivated += (sender, args) =>
            //{
            //    if (args.Activated.Name == "Программа")
            //        programView.SetProgram(manager.GetProgramm());
            //};

            Application.DocumentManager.DocumentActivated += (sender, args) => SetActiveDocument(args.Document);

            SetActiveDocument(Acad.ActiveDocument);

            //PaletteSet focus use Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            //AutocadUtils.AddPaletteSet("Настройки", SettingForm);
            //machine.ChangeActionsCount += (sender, args) => ObjectForm.ShowProgress(String.Format("Генерация обработки... {0} строк", args.Data));

        }

        private void SetActiveDocument(Document document)
        {
            //if (document == null)
            //{
            //    CamManager.RemoveDocument();
            //    return;
            //}

            if (!document.UserData.ContainsKey(CamDocumentKey))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                document.ImpliedSelectionChanged += DocumentOnImpliedSelectionChanged;

                document.UserData[CamDocumentKey] = CamDocument.Create();
            }
            if (CamDocument.Current != null)
                CamDocument.Current.ProcessItems = _processingView.GetProcessItems();
            CamDocument.Current = (CamDocument)document.UserData[CamDocumentKey];

            _processingView.Reset(CamDocument.Current.ProcessItems);
            //ToolObject.Hide();
            Acad.ClearHighlighted();
        }

        private void DocumentOnImpliedSelectionChanged(object sender, EventArgs e) => _processingView.SelectCommand(Acad.GetSelectedObjectId());

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT" || e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVEAS")
            {
                // TODO сохранять все
                CamDocument.Current.Save(_processingView.GetProcessItems());
                _processingView.ClearCommandsView();
                Acad.DeleteAll();
            }
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            ((Document)sender).CommandWillStart -= Document_CommandWillStart;
            ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;

            CamDocument.Current = null;
            _processingView.ClearView();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
        }
    }
}
