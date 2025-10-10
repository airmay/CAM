// test /b "C:\Catalina\CAM\bin\Debug\netload.scr"

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Dreambuild.AutoCAD;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace CAM;

public class Startup : IExtensionApplication
{
    private readonly ProcessingView _camView = new();

    public void Initialize()
    {
        Tolerance.Global = new Tolerance( Consts.Epsilon, Consts.Epsilon);

        // Assembly.GetExecutingAssembly().GetProductVersion()
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
        Acad.DocumentManager.DocumentToBeDeactivated += (sender, args) => args.Document.SetUserData(_camView.GetData());

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

            var data = document.LoadFromXrecord();
            data?.As<IProcessing[]>().ForEach(t => t.Setup());
        }

        _camView.SetData(document.GetUserData());
    }

    private void Document_CommandWillStart(object sender, CommandEventArgs e)
    {
        if (e.GlobalCommandName.IsIn("CLOSE", "QUIT", "QSAVE", "SAVEAS"))
            ((Document)sender).SaveToXrecord(_camView.GetData());
    }

    private void DocumentOnImpliedSelectionChanged(object sender, EventArgs e) => _camView.SelectCommand(Acad.GetSelectedObjectId());

    private void Document_BeginDocumentClose(object sender, DocumentBeginCloseEventArgs e)
    {
        ((Document)sender).CommandWillStart -= Document_CommandWillStart;
        ((Document)sender).BeginDocumentClose -= Document_BeginDocumentClose;
        ((Document)sender).ImpliedSelectionChanged -= DocumentOnImpliedSelectionChanged;
    }

    public void Terminate() { }
}