using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CAM.UI;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace CAM
{
    public class Bootstrapper : IExtensionApplication
    {
        public void Initialize()
        {
            Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

            var manager = new CamManager();

            PaletteSet paletteSet = new PaletteSet("Технология")
            {
                Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                MinimumSize = new Size(300, 200),
                KeepFocus = true,
                Visible = true
            };
            paletteSet.Add("Объекты", new TechProcessView(manager));
            var programView = new ProgramView(manager);
            var programmPalette = paletteSet.Add("Программа", programView);
            paletteSet.PaletteActivated += (sender, args) =>
            {
                if (args.Activated.Name == "Программа")
                    programView.SetProgram(manager.GetProgramm());
            };

            Application.DocumentManager.DocumentActivated += (sender, args) => manager.SetActiveDocument(args.Document);

            manager.SetActiveDocument(Acad.Document);

            //PaletteSet focus use Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView();
            //AutocadUtils.AddPaletteSet("Настройки", SettingForm);
            //machine.ChangeActionsCount += (sender, args) => ObjectForm.ShowProgress(String.Format("Генерация обработки... {0} строк", args.Data));
        }

        public void Terminate()
        {
            //Settings.Save();
        }
    }
}
