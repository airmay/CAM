using System;
using Autodesk.AutoCAD.ApplicationServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core.UI;

namespace CAM
{
    public static class CamManager
    {
        public static readonly Dictionary<Document, Processing> Documents = new Dictionary<Document, Processing>();
        public static ProcessingView ProcessingView = Acad.ProcessingView;
        public static Processing Processing;
        public static ProcessCommand[] ProcessingCommands => Processing.Commands;

        public static void AddDocument(Document document)
        {
            var processing = new Processing();
            Load();
            Documents.Add(document, processing);
            return;
            // TODO document.UserData

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

        public static void OnActivateDocument(Document document)
        {
            if (Processing != null)
                UpdateProcessing();
            Processing = Documents[document];
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

        public static void RemoveDocument(Document document)
        {
            Documents.Remove(document);
            if (!Documents.Any())
            {
                Processing = null;
                ProcessingView.ClearView();
            }
        }

        public static void SaveDocument(Document document)
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
            if (Acad.GetToolpathObjectId() is ObjectId id)
                ProcessingView.SelectProcessCommand(id);
        }

        public static void ExecuteProcessing()
        {
            
        }
    }
}
