using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using System.Drawing;

namespace CAM
{
    public class CamPaletteSet
    {
        //private CamView _techProcessView { get; } = new CamView();

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

            paletteSet.Add("Объекты", Acad.CamView);

            paletteSet.Add("Инструменты", new UtilsView());


            //var programmPalette = paletteSet.Add("Программа", _programView);
            //paletteSet.PaletteActivated += (sender, args) =>
            //{
            //    if (args.Activated.Name == "Программа")
            //        programView.SetProgram(manager.GetProgramm());
            //};
        }

        //public void SetCamDocument(CamDocument camDocument = null)
        //{
        //    Acad.CamView.SetCamDocument(camDocument);
        //    //_programView.SetCamDocument(camDocument);
        //}

        //internal void ClearCommands()
        //{
        //    //Acad.CamView.ClearCommandsView();
        //    //_programView.RefreshView();
        //}

        public void SelectProcessCommand(ObjectId id) => Acad.CamView.SelectProcessCommand(id);
    }
}
