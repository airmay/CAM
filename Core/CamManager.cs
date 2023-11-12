using System;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core.UI;

namespace CAM
{
    public static class CamManager
    {
        public static readonly ProcessingView ProcessingView = Acad.ProcessingView;
        public static Processing Processing;
        public static ProcessCommand[] ProcessingCommands => Processing.Commands;

        public static Processing CreateProcessing()
        {
            var processing = new Processing();
            Load();
            return processing;

            void Load()
            {
                var (value, hash) = DataLoader.Load();
                if (value is GeneralOperation[] operations)
                {
                    processing.GeneralOperations = operations;
                    processing.Hash = hash;
                    // processing.GeneralOperations.ForEach(p => p.SerializeInit());
                }
                else if (value != null)
                {
                    Acad.Alert("Ошибка при загрузке данных обработки");
                }
            }
        }

        public static void SetProcessing(Processing processing)
        {
            if (Processing != null)
                UpdateProcessing();
            Processing = processing;
            ProcessingView.SetNodes(GetNodes());
            //Acad.ClearHighlighted();
            return;

            TreeNode[] GetNodes() =>
                Processing?.GeneralOperations?.Select(p =>
                    {
                        var generalOperationNode = new GeneralOperationNode(p);
                        generalOperationNode.Nodes.AddRange(p.Operations.Select(c => new OperationNode(c))
                            .ToArray());
                        return generalOperationNode;
                    })
                    .ToArray()
                ?? Array.Empty<TreeNode>();
        }

        public static void RemoveProcessing()
        {
            Processing = null;
            ProcessingView.ClearView();
        }

        public static void SaveProcessing()
        {
            UpdateProcessing();

            //Acad.Documents[sender as Document].GeneralOperations.ForEach(p => p.DeleteProcessing());

            if (Processing.GeneralOperations.Length > 0 || Processing.Hash != 0)
                DataLoader.Save(Processing.GeneralOperations, Processing.Hash);

            //ProcessingView.ClearCommandsView();
            //Acad.DeleteAll();
        }

        private static void UpdateProcessing()
        {
            Processing.GeneralOperations = ProcessingView.Nodes
                .Cast<GeneralOperationNode>()
                .Select(p => p.UpdateGeneralOperation())
                .ToArray();
        }

        public static void OnSelectAcadObject()
        {
            //if (Acad.GetToolpathObjectId() is ObjectId id && Processing.)
            //    ProcessingView.SelectProcessCommand(id);
        }

        public static void ExecuteProcessing()
        {
            UpdateProcessing();
            Processing.Execute();
        }

        public static void SendProgram() => Processing.SendProgram();
    }
}
