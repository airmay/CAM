using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core.UI;

namespace CAM
{
    public static class CamManager
    {
        private static CamDocument _camDocument;
        public static readonly ProcessingView ProcessingView = Acad.ProcessingView;

        public static CamDocument CreateCamDocument()
        {
            var processing = new CamDocument();
            Load();
            return processing;

            void Load()
            {
                var (value, hash) = DataLoader.Load();
                if (value is Processing[] operations)
                {
                    processing.Processings = operations;
                    processing.Hash = hash;
                    foreach (var operation in operations)
                        operation.Init();
                }
                else if (value != null)
                {
                    Acad.Alert("Ошибка при загрузке данных обработки");
                }
            }
        }

        public static void SetProcessing(CamDocument camDocument)
        {
            if (_camDocument != null)
                UpdateProcessing();
            _camDocument = camDocument;
            //Commands?.Clear();
            ProcessingView.SetNodes(GetNodes());
            //Acad.ClearHighlighted();
            return;

            TreeNode[] GetNodes() =>
                _camDocument?.Processings?.Select(p =>
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
            _camDocument = null;
            ProcessingView.ClearView();
        }

        public static void SaveProcessing()
        {
            UpdateProcessing();

            //Acad.Documents[sender as Document].GeneralOperations.ForEach(p => p.DeleteProcessing());

            if (_camDocument.Processings.Length > 0 || _camDocument.Hash != 0)
                DataLoader.Save(_camDocument.Processings, _camDocument.Hash);

            //ProcessingView.ClearCommandsView();
            //Acad.DeleteAll();
        }

        private static void UpdateProcessing()
        {
            _camDocument.Processings = ProcessingView.Nodes
                .Cast<GeneralOperationNode>()
                .Select(p => p.UpdateGeneralOperation())
                .ToArray();
            _camDocument.HideTool();
        }

        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathId() is ObjectId id && _camDocument.GetCommandIndex(id) is int commandIndex)
                ProcessingView.SelectProcessCommand(commandIndex);
        }

        public static List<Command> ExecuteProcessing()
        {
            UpdateProcessing();
            _camDocument.Execute();
            UpdateNodeText();

            return Commands;
        }

        private static void UpdateNodeText()
        {
            foreach (TreeNode node in ProcessingView.Nodes)
            {
                node.Text = GetCaption(node.Text, node.Nodes.Cast<OperationNode>().Sum(p => p.Operation.Duration));
                foreach (OperationNode operationNode in node.Nodes)
                    operationNode.Text = GetCaption(operationNode.Text, operationNode.Operation.Duration);
            }

            return;

            string GetCaption(string caption, double duration)
            {
                var ind = caption.IndexOf('(');
                var timeSpan = new TimeSpan(0, 0, 0, (int)duration);
                return $"{(ind > 0 ? caption.Substring(0, ind).Trim() : caption)} ({timeSpan})";
            }
        }

        public static void SendProgram()
        {
            if (!Commands.Any())
            {
                Acad.Alert("Программа не сформирована");
                return;
            }

            var machine = Settings.Machines[_camDocument.MachineCodes];
            var fileName = Acad.SaveFileDialog("Программа", machine.ProgramFileExtension, _camDocument.MachineCodes.ToString());
            if (fileName == null)
                return;
            try
            {
                var contents = Commands
                    .Select(p => $"{string.Format(machine.ProgramLineNumberFormat, p.Number)}{p.Text}")
                    .ToArray();
                File.WriteAllLines(fileName, contents);
                Acad.Write($"Создан файл {fileName}");
                //if (machineType == MachineType.CableSawing)
                //    CreateImitationProgramm(contents, fileName);
            }
            catch (Exception ex)
            {
                Acad.Alert($"Ошибка при записи файла {fileName}", ex);
            }
        }

        public static void ShowTool(Command command)
        {
            _camDocument.ShowTool(command);
        }
    }
}
