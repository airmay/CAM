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




        //private static void UpdateFromCommands()
        //{
        //    _toolpathCommandDictionary = Commands.Select((command, index) => new { command, index })
        //        .Where(p => p.command.ObjectId.HasValue)
        //        .GroupBy(p => p.command.ObjectId.Value)
        //        .ToDictionary(p => p.Key, p => p.Min(k => k.index));

        //    foreach (var operationGroup in Commands.Where(p => p.Operation != null).GroupBy(p => p.Operation))
        //        operationGroup.Key.ToolpathGroup = operationGroup.Select(p => p.ObjectId).CreateGroup();
        //}

    }
}
