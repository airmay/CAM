using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using CAM.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAM
{
    public class Bootstrapper : IExtensionApplication
    {
        public void Initialize()
        {
            var acad = new AcadGateway();
            PaletteSet paletteSet = acad.CreatePaletteSet();
            //AutocadUtils.WriteMessage("Инициализация плагина ProcessingProgram. Версия с режимом обработки."); // + DateTime.Today.ToShortDateString()); TODO Assemlly.DateTime()

            paletteSet.Add("Объекты", new TechProcessView());
            paletteSet.Add("Программа", new ProgramView());

            //ProcessingForm.CurrentChanged += (sender, args) => AutocadUtils.SelectObjects(args.Data.ObjectId, args.Data.ToolObjectId);
            //ProcessingForm.DeleteProcessing += (sender, args) => DeleteProcessing();
            //ProcessingForm.Calculate += (sender, args) => Calculate();
            //AutocadUtils.AddPaletteSet("Обработка", ProcessingForm);

            //AutocadUtils.AddPaletteSet("Программа", ProgramForm);
            //AutocadUtils.AddPaletteSet("Настройки", SettingForm);

            //AutocadUtils.Selected += (sender, args) => ProcessingForm.SelectObjects(args.Data);
            //AutocadUtils.Focused += (sender, args) => ProcessingForm.SetFocus(args.Data);
            //AutocadUtils.Focused += (sender, args) => ObjectForm.SetFocus(args.Data);

            //CalcUtils.Init(ProcessObjects, ProcessCurves);
            //ProcessObjectFactory.Init(ProcessObjects, ProcessCurves);

            //var machine = new Machine();
            //machine.ChangeActionsCount += (sender, args) => ObjectForm.ShowProgress(String.Format("Генерация обработки... {0} строк", args.Data));
            //ActionGenerator.SetMachine(machine);

            //AutocadUtils.CreateTest();
            //RunTest();
        }

        public void Terminate()
        {
            //SettingForm.RefreshSettings();
            //ProcessingParams.SaveDefault();
            //Settings.Save();
        }

        [CommandMethod("show")]
        public void ShowPaletteSet()
        {
            //AutocadUtils.ShowPaletteSet();
        }

        [Conditional("DEBUG")]
        private void RunTest()
        {
            //var selectedObjects = AutocadUtils.GetAllCurves();
            //if (selectedObjects == null || !Tools.Any())
            //    return;
            //ProcessObjectFactory.Create(selectedObjects.FindAll(p => p.GetLength() > 100), Tools.Where(p => p.No == 2));
            ///*
            //SectionCurves.AddRange(selectedObjects.FindAll(p => p.GetLength() < 100).Cast<Curve>().ToList());
            //var points = SectionCurves.Select(p => p.StartPoint.Y).Concat(SectionCurves.Select(p => p.EndPoint.Y));
            //Settings.GetInstance().HeightMax = points.Max();
            //Settings.GetInstance().HeightMin = points.Min();
            //SettingForm.RefreshForm();
            // * */
            //Calculate();
            //ObjectForm.RefreshList();
        }
    }
}
