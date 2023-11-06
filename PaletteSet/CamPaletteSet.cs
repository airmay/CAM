using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Windows;
using System.Drawing;

namespace CAM
{
    public class CamPaletteSet
    {
        //private ProcessingView _techProcessView { get; } = new ProcessingView();

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

            paletteSet.Add("Объекты", Acad.ProcessingView);

            paletteSet.Add("Инструменты", new UtilsView());


            //var programmPalette = paletteSet.Add("Программа", _programView);
            //paletteSet.PaletteActivated += (sender, args) =>
            //{
            //    if (args.Activated.Name == "Программа")
            //        programView.SetProgram(manager.GetProgramm());
            //};
        }

        //public void SetCamDocument(Processing processing = null)
        //{
        //    Acad.ProcessingView.SetCamDocument(processing);
        //    //_programView.SetCamDocument(processing);
        //}

        //internal void ClearCommands()
        //{
        //    //Acad.ProcessingView.ClearCommandsView();
        //    //_programView.SetNodes();
        //}
    }
}
