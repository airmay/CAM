// test /b "C:\Catalina\CAM\bin\Debug\netload.scr"

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CAM.Core;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using CAM.Autocad;
using CAM.Autocad.AutoCADCommands;
using CAM.Core.Processing;
using CAM.UI;
using CAM.Utils;
using CAM.UtilViews;

namespace CAM;

public class Startup : IExtensionApplication
{
    private readonly ProcessingView _camView = new();

    public void Initialize()
    {
        Tolerance.Global = new Tolerance( Consts.Epsilon, Consts.Epsilon);

        // GetProductVersion(Assembly.GetExecutingAssembly())
        Acad.Write($"Инициализация плагина. Версия сборки от {File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)}");

        var paletteSet = new PaletteSet("Технология")
        {
            Style = PaletteSetStyles.NameEditable | PaletteSetStyles.ShowPropertiesMenu | PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton,
            MinimumSize = new Size(300, 200),
            KeepFocus = true,
            Visible = true,
            Dock = DockSides.Left
        };
        paletteSet.Add("Обработка", _camView);
        paletteSet.Add("Инструменты", new UtilsView());

        Acad.DocumentManager.DocumentActivated += (sender, args) => ActivateDocument(args.Document);
        Acad.DocumentManager.DocumentToBeDeactivated += (sender, args) => args.Document.SetUserData(_camView.GetCamData());

        ActivateDocument(Acad.ActiveDocument);
    }

    private void ActivateDocument(Document document)
    {
        _camView.Clear();
        if (document == null)
            return;

        if (!document.HasUserData())
        {
            document.CommandWillStart += Document_CommandWillStart;
            document.BeginDocumentClose += Document_BeginDocumentClose;
            document.ImpliedSelectionChanged += DocumentOnImpliedSelectionChanged;

            document.LoadFromXrecord();
        }

        var data = document.GetUserData();
        var camData = data as CamData;
        if (data is not null and not CamData) 
            Acad.Alert("Данные обработки не могут быть загружены");

        ProgramBuilder.Commands = camData?.Commands;
        _camView.SetCamData(camData);
#if DEBUG
        ProgramBuilder.DwgFileCommands ??= camData?.Commands?.ToList();
#endif
    }

    private void Document_CommandWillStart(object sender, CommandEventArgs e)
    {
        if (e.GlobalCommandName.IsIn("CLOSE", "QUIT", "QSAVE", "SAVEAS"))
            ((Document)sender).SaveToXrecord(_camView.GetCamData());
    }

    private void DocumentOnImpliedSelectionChanged(object sender, EventArgs e) => _camView.SelectCommand(Acad.GetSelectedObjectId());

    private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
    {
        ((Document)sender).CommandWillStart -= Document_CommandWillStart;
        ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;
        ((Document)sender).ImpliedSelectionChanged -= DocumentOnImpliedSelectionChanged;
    }

    public void Terminate() { }

    //public string GetProductVersion(Assembly assembly)
    //{
    //    var result = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

    //    var plusIndex = result.IndexOf('+');
    //    if (plusIndex >= 0)
    //    {
    //        result = result.Substring(0, plusIndex);
    //    }

    //    return result;
    //}
}