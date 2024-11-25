using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using System.Drawing;
using Autodesk.Windows.Palettes;
using PaletteSet = Autodesk.AutoCAD.Windows.PaletteSet;

namespace CAM
{
    public class CamPaletteSet
    {
        private CamView _techProcessView { get; } = new CamView();

        public CamPaletteSet()
        {
            PaletteSet paletteSet = new PaletteSet("Технология")
            {
                Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
                MinimumSize = new Size(300, 200),
                KeepFocus = true,
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

        public void SelectProcessCommand(ObjectId id) => _techProcessView.SelectProcessCommand(id);
    }
}
