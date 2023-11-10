using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CAM.Core;

namespace CAM
{
    public class ExtensionApplication : IExtensionApplication
    {
        public const string ProcessingKey = "Processing";

        public void Initialize()
        {
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
            if (document == null)
                return;

            if (!document.UserData.ContainsKey(ProcessingKey))
            {
                document.CommandWillStart += Document_CommandWillStart;
                document.BeginDocumentClose += Document_BeginDocumentClose;
                document.ImpliedSelectionChanged += (sender, args) => CamManager.OnSelectAcadObject();

                document.UserData[ProcessingKey] = CamManager.CreateProcessing();
            }
            CamManager.SetProcessing((Processing)document.UserData[ProcessingKey]);
        }

        private void Document_CommandWillStart(object sender, CommandEventArgs e)
        {
            if (e.GlobalCommandName == "CLOSE" || e.GlobalCommandName == "QUIT" || e.GlobalCommandName == "QSAVE" || e.GlobalCommandName == "SAVEAS")
            {
                CamManager.SaveProcessing();
            }
        }

        private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
        {
            ((Document)sender).CommandWillStart -= Document_CommandWillStart;
            ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;
            CamManager.RemoveProcessing();
        }

        public void Terminate()
        {
            //Settings.Save(_settings);
        }
    }
}
