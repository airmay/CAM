using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using CAM.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CAM
{
    public class CamManager : IExtensionApplication
    {
        private Dictionary<Document, CamDocument> _documents = new Dictionary<Document, CamDocument>();
        private Settings _settings;
        private CamPaletteSet _camPaletteSet;

        public void Initialize()
        {
            Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

            _settings = Settings.Load();

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
            if (!_documents.ContainsKey(document))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                _documents[document] = new CamDocument(document, _settings);
                TechProcessLoader.LoadTechProsess(_documents[document], _settings);
            }
            _camPaletteSet.SetCamDocument(_documents[document]);
            Acad.ClearHighlighted();
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT" || e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVEAS")
            {
                _documents[sender as Document].TechProcessList.ForEach(p => p.DeleteToolpath());
                TechProcessLoader.SaveTechProsess(_documents[sender as Document]);
                _camPaletteSet.ClearCommands();
                Acad.DeleteAll();
            }
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            var document = sender as Document;
            document.CommandWillStart -= Document_CommandWillStart;
            document.BeginDocumentClose -= Document_BeginDocumentClose;
            _documents.Remove(document);
            if (!_documents.Any())
                _camPaletteSet.SetCamDocument();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
            // TODO fix
        }
    }
}
