using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public class ExtensionApplication : IExtensionApplication
    {
        private TechProcessFactory _techProcessFactory;
        private CamPaletteSet _camPaletteSet;

        public void Initialize()
        {
            Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

            _techProcessFactory = new TechProcessFactory();
            _camPaletteSet = new CamPaletteSet();

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

        public void SetActiveDocument(Document document)
        {
            if (document != null && !Acad.Documents.ContainsKey(document))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                document.ImpliedSelectionChanged += ImpliedSelectionChanged;
                Acad.Documents[document] = new CamDocument(_techProcessFactory);
                // TechProcessLoader.LoadTechProsess(Acad.Documents[document]);
            }
            Acad.ProcessingView.RefreshView();
            Acad.ClearHighlighted();
        }

        private void ImpliedSelectionChanged(object sender, EventArgs e)
        {
            if (Acad.GetToolpathObjectId() is ObjectId id)
                _camPaletteSet.SelectProcessCommand(id);
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT" || e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVEAS")
            {
                Acad.Documents[sender as Document].TechProcessList.ForEach(p => p.DeleteProcessing());
                TechProcessLoader.SaveTechProsess(Acad.Documents[sender as Document]);
                Acad.ProcessingView.ClearCommandsView();
                Acad.DeleteAll();
            }
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            var document = sender as Document;
            document.CommandWillStart -= Document_CommandWillStart;
            document.BeginDocumentClose -= Document_BeginDocumentClose;
            Acad.Documents.Remove(document);

            if (!Acad.Documents.Any())
                Acad.ProcessingView.RefreshView();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
        }
    }
}
