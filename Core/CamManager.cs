using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.CncWorkCenter;
using CAM.Core;

namespace CAM
{
    public static class CamManager
    {
        private static CamDocument _camDocument;
        public static readonly ProcessingView ProcessingView = new ProcessingView();
        public static Program<CommandCnc> Program = new Program<CommandCnc>();
        private static ToolObject ToolObject { get; } = new ToolObject();
        private static IProcessing _processing;

        public static void SetDocument(CamDocument camDocument)
        {
            if (_camDocument != null)
                _camDocument.ProcessItems = ProcessingView.GetProcessItems();
            _camDocument = camDocument;

            ProcessingView.Reset(camDocument.ProcessItems);
            ToolObject.Hide();
            Acad.ClearHighlighted();
        }

        public static void RemoveDocument()
        {
            _camDocument = null;
            ProcessingView.ClearView();
        }

        public static void SaveDocument()
        {
            DeleteGenerated();
            _camDocument.Save(ProcessingView.GetProcessItems());
            ProcessingView.ClearCommandsView();
            Acad.DeleteAll();
        }

        public static IProgram ExecuteProcessing(ProcessItem processItem)
        {
            DeleteGenerated();
            Acad.Editor.UpdateScreen();
            Acad.Write($"Выполняется расчет обработки {processItem.Caption}");
            Acad.CreateProgressor("Расчет обработки");
            var processing = (IProcessing)processItem;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                processing.Execute();
                stopwatch.Stop();
                Acad.Write($"Расчет обработки завершен {stopwatch.Elapsed}");
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex) when (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.UserBreak)
            {
                Acad.Write("Расчет прерван");
            }
#if !DEBUG  
            catch (Exception ex)
            {
                Acad.Alert("Ошибка при выполнении расчета", ex);
            }
#endif
            Acad.CloseProgressor();

            return processing.Program;
        }

        private static void DeleteGenerated()
        {
            ToolObject.Hide();
            _processing?.RemoveAcadObjects();
        }

        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathId() is ObjectId id && Program.TryGetCommandIndex(id, out var commandIndex))
                ProcessingView.SelectCommand(commandIndex);
        }

        //private static void UpdateFromCommands()
        //{
        //    _toolpathCommandDictionary = Commands.Select((command, index) => new { command, index })
        //        .Where(p => p.command.ObjectId.HasValue)
        //        .GroupBy(p => p.command.ObjectId.Value)
        //        .ToDictionary(p => p.Key, p => p.Min(k => k.index));

        //    foreach (var operationGroup in Commands.Where(p => p.Operation != null).GroupBy(p => p.Operation))
        //        operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.ObjectId).CreateGroup();
        //}

        public static void ShowTool(Command command)
        { 
            ToolObject.Set(command?.Operation?.Processing.Machine, command?.Operation?.Tool, command.Position, command.AngleC, command.AngleA);
        }
    }
}
