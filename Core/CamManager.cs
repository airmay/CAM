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
        private static Processing _processing;
        public static readonly ProcessingView ProcessingView = Acad.ProcessingView;
        public static List<Command> Commands;

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
                    foreach (var operation in operations)
                        operation.Init();
                }
                else if (value != null)
                {
                    Acad.Alert("Ошибка при загрузке данных обработки");
                }
            }
        }

        public static void SetProcessing(Processing processing)
        {
            if (_processing != null)
                UpdateProcessing();
            _processing = processing;
            Commands?.Clear();
            ProcessingView.SetNodes(GetNodes());
            //Acad.ClearHighlighted();
            return;

            TreeNode[] GetNodes() =>
                _processing?.GeneralOperations?.Select(p =>
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
            _processing = null;
            ProcessingView.ClearView();
        }

        public static void SaveProcessing()
        {
            UpdateProcessing();

            //Acad.Documents[sender as Document].GeneralOperations.ForEach(p => p.DeleteProcessing());

            if (_processing.GeneralOperations.Length > 0 || _processing.Hash != 0)
                DataLoader.Save(_processing.GeneralOperations, _processing.Hash);

            //ProcessingView.ClearCommandsView();
            //Acad.DeleteAll();
        }

        private static void UpdateProcessing()
        {
            _processing.GeneralOperations = ProcessingView.Nodes
                .Cast<GeneralOperationNode>()
                .Select(p => p.UpdateGeneralOperation())
                .ToArray();
            _processing.HideTool();
        }

        public static void OnSelectAcadObject()
        {
            if (Acad.GetToolpathId() is ObjectId id && _processing.GetCommandIndex(id) is int commandIndex)
                ProcessingView.SelectProcessCommand(commandIndex);
        }

        private const int CommandListCapacity = 10_000;

        public static List<Command> ExecuteProcessing()
        {
            if (Commands == null)
                Commands = new List<Command>(CommandListCapacity);

            UpdateProcessing();
            _processing.Execute();
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

            var machine = Settings.Machines[_processing.MachineType];
            var fileName = Acad.SaveFileDialog("Программа", machine.ProgramFileExtension, _processing.MachineType.ToString());
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
            _processing.ShowTool(command);
        }
    }
}
