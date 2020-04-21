using Autodesk.AutoCAD.Windows;
using System.Drawing;

namespace CAM
{
    public class CamPaletteSet
    {
        private TechProcessView _techProcessView { get; } = new TechProcessView();

        private ProgramView _programView { get; } = new ProgramView();

        public CamPaletteSet()
        {
            PaletteSet paletteSet = new PaletteSet("Технология")
            {
                Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                MinimumSize = new Size(300, 200),
                KeepFocus = false,
                Visible = true,
                Dock = DockSides.Left
            };

            paletteSet.Add("Объекты", _techProcessView);

            paletteSet.Add("Инструменты", new UtilsView());


            //var programmPalette = paletteSet.Add("Программа", _programView);
            //paletteSet.PaletteActivated += (sender, args) =>
            //{
            //    if (args.Activated.Name == "Программа")
            //        programView.SetProgram(manager.GetProgramm());
            //};
        }

        public void SetCamDocument(CamDocument camDocument = null)
        {
            _techProcessView.SetCamDocument(camDocument);
            //_programView.SetCamDocument(camDocument);
        }

        internal void ClearCommands()
        {
            _techProcessView.ClearCommandsView();
            //_programView.RefreshView();
        }
    }
}
