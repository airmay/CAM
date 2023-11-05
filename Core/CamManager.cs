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

        public static void AddDocument(Document document)
        {
            var processing = new Processing();
            TechProcessLoader.LoadTechProsess(processing);
            Documents.Add(document, processing);
            //document.UserData
        }

        public static void OnActivateDocument(Document document)
        {
            Processing = Documents[document];
            ProcessingView.RefreshView();
            Acad.ClearHighlighted();
        }

        public static TreeNode[] GetNodes()
        {
            return Processing?.TechProcessList.Select(p =>
                {
                    var generalOperationNode = new GeneralOperationNode(p);
                    generalOperationNode.Nodes.AddRange(p.Operations.Select(c => new OperationNode(c)).ToArray());
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
                ProcessingView.RefreshView(); //TODO ClearView
            }
        }

        public static void SaveDocument(Document document)
        {
            var operations = ProcessingView.treeView.Nodes.Cast<GeneralOperationNode>().Select(p =>
                {
                    p.GeneralOperation.Caption = p.Text;
                    p.GeneralOperation.Enabled = p.Checked;
                    p.GeneralOperation.Operations = p.Nodes.Cast<OperationNode>().Select(c =>
                        {
                            c.Operation.Caption = c.Text;
                            c.Operation.Enabled = c.Checked;
                            return c.Operation;
                        })
                        .ToArray();
                    return p.GeneralOperation;
                }
            ).ToArray();

            //Acad.Documents[sender as Document].TechProcessList.ForEach(p => p.DeleteProcessing());
            TechProcessLoader.SaveTechProsess(operations, Processing.Hash);
            //ProcessingView.ClearCommandsView();
            //Acad.DeleteAll();
        }

        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathObjectId() is ObjectId id)
                ProcessingView.SelectProcessCommand(id);
        }
    }
}
