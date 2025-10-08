using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.CncWorkCenter;
using CAM.Core;
using Font = System.Drawing.Font;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        private Program _program;

        private TreeNode SelectedNode => treeView.SelectedNode;
        private TreeNode SelectedProcessingNode => treeView.SelectedNode?.Parent ?? treeView.SelectedNode;
        private Command SelectedCommand => (Command)processCommandBindingSource.Current;

        public ProcessingView()
        {
            InitializeComponent();

            imageList.Images.AddRange([Properties.Resources.folder, Properties.Resources.drive_download]);
            bCreateTechOperation.DropDownItems.AddRange(OperationItems.GetMenuItems(bCreateTechOperation_Click));
            //RefreshToolButtonsState();
#if DEBUG
            bClose.Visible = true;
#endif
        }

        public void UpdateCamDocument() => _camDocument?.Set(GetProcessings(), _program);

        public void Clear()
        {
            _camDocument = null;
            toolStrip.Enabled = false;
            treeView.Nodes.Clear();
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            processCommandBindingSource.DataSource = null;
            _program = null;
            Acad.ClearHighlighted();
            ToolObject.Delete();
        }

        #region Tool buttons
        private void RefreshToolButtonsState()
        {
            bRemove.Enabled = bMoveUp.Enabled = bMoveDown.Enabled = bBuildProcessing.Enabled = SelectedNode != null;
            bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = _program != null;
        }

        #region Execute

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
#if RELEASE
            toolStrip.Enabled = false;
#endif
            SelectNextControl(ActiveControl, true, true, true, true);
            processCommandBindingSource.DataSource = null;
            Acad.DocumentManager.DocumentActivationEnabled = false;
            Acad.DeleteProcessObjects();
            Acad.ClearHighlighted();
            ToolObject.Delete();

            var processing = GetProcessing(SelectedProcessingNode);
            _program = processing.Execute();
            if (_program != null)
            {
                UpdateTechProcessNodesText(SelectedProcessingNode);
                processCommandBindingSource.DataSource = _program.Commands;
                RefreshToolButtonsState();

                if (Program.DwgFileCommands != null)
                    Acad.Write(_program.Commands.SequenceEqual(Program.DwgFileCommands, Command.Comparer)
                        ? "Программа не изменилась"
                        : "Внимание! Программа изменена!");
            }

            Acad.DocumentManager.DocumentActivationEnabled = true;
            toolStrip.Enabled = true;
        }

        private void bPartialProcessing_Click(object sender, EventArgs e)
        {
            if (processCommandBindingSource?.Position == null || MessageBox.Show($"Сформировать программу со строки {SelectedCommand.Number}?", 
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;

            _program.Commands.Take(processCommandBindingSource.Position).SelectMany(p => new[] { p.ObjectId, p.ObjectId2 }).Delete();
            var command = (Command)processCommandBindingSource[processCommandBindingSource.Position - 1];
            _program = _program.Processing.ExecutePartial(processCommandBindingSource.Position, _program.Commands.Count, command.OperationNumber, command.ToolPosition);
            processCommandBindingSource.DataSource = _program.Commands;
            processCommandBindingSource.Position = 0;
            UpdateTechProcessNodesText(GetProgramProcessingNode());
        }

        private static void UpdateTechProcessNodesText(TreeNode techProcessNode)
        {
            techProcessNode.Text = techProcessNode.Tag.As<IProcessing>().Caption;
            techProcessNode.Nodes.Cast<TreeNode>().ForAll(p => p.Text = p.Tag.As<IOperation>().Caption);
        }

        #endregion

        private void bVisibility_Click(object sender, EventArgs e)
        {
            _program.SetToolpathVisibility(false);
            Acad.Editor.UpdateScreen();
        }

        private void bPlay_Click(object sender, EventArgs e)
        {
            toolStrip.Enabled = false;
            Acad.CreateProgressor("Проигрывание обработки");
            Acad.SetLimitProgressor(processCommandBindingSource.Count - processCommandBindingSource.Position);
            while (processCommandBindingSource.Position < processCommandBindingSource.Count - 1 &&
                   Acad.ReportProgressor(false))
            {
                processCommandBindingSource.MoveNext();
                System.Threading.Thread.Sleep(100);
                // System.Threading.Thread.Sleep((int)((Command)processCommandBindingSource.Current).Duration * 10);
            }

            Acad.CloseProgressor();
            toolStrip.Enabled = true;
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            dataGridViewCommand.EndEdit();
            _program.Export();
        }

        private void bClose_Click(object sender, EventArgs e) => Acad.CloseAndDiscard();
#endregion

        #region CamDocument

        private CamDocument _camDocument;

        public void SetCamDocument(CamDocument camDocument)
        {
            if (camDocument == _camDocument)
                return;

            _camDocument = camDocument;
            toolStrip.Enabled = true;

            if (_camDocument.Processings?.Any() == true)
            {
                var nodes = Array.ConvertAll(_camDocument.Processings, CreateProcessingNode);
                treeView.Nodes.AddRange(nodes);
                treeView.ExpandAll();
                treeView.SelectedNode = treeView.Nodes[0];
            }

            if (_camDocument.Commands != null)
            {
                var programBuilder = new ProgramBuilder(_camDocument.Commands);
                _program = programBuilder.CreateProgram(_camDocument.Processings[_camDocument.ProcessingIndex.Value]);
                processCommandBindingSource.DataSource = _camDocument.Commands;
            }
            RefreshToolButtonsState();
        }

        private TreeNode CreateProcessingNode(IProcessing processing)
        {
            var children = processing.Operations?.Select(CreateOperationNode).ToArray() ?? [];
            return new TreeNode(processing.Caption, 0, 0, children)
            {
                Checked = true,
                NodeFont = new Font(this.Font, FontStyle.Bold),
                Tag = processing
            };
        }

        private static TreeNode CreateOperationNode(IOperation operation)
        {
            return new TreeNode(operation.Caption, 1, 1)
            {
                Checked = operation.Enabled,
                Tag = operation
            };
        }

        public void SaveCamDocument()
        {
            _camDocument.Set(GetProcessings(), _program);
            _camDocument.Save();
            _program?.SetToolpathVisibility(true);
        }

        private TreeNode GetProgramProcessingNode() => _program == null
            ? null
            : treeView.Nodes.Cast<TreeNode>().FirstOrDefault(p => p.Tag == _program.Processing);

        private IProcessing[] GetProcessings() => treeView.Nodes.Cast<TreeNode>().Select(GetProcessing).ToArray();

        private static IProcessing GetProcessing(TreeNode node)
        {
            var processing = (IProcessing)node.Tag;
            processing.Caption = node.Text;
            processing.Operations = node.Nodes.Cast<TreeNode>()
                .Select(p =>
                {
                    var operation = (IOperation)p.Tag;
                    operation.Caption = p.Text;
                    operation.Enabled = p.Checked;
                    return operation;
                })
                .ToArray();
            return processing;
        }

        #endregion

        #region Tree nodes

        #region Add

        private void bCreateProcessing_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = AddProcessingNode(typeof(ProcessingCnc));
            RefreshToolButtonsState();
        }

        private TreeNode AddProcessingNode(Type techProcessType)
        {
            var processing = (IProcessing)Activator.CreateInstance(techProcessType);
            var node = CreateProcessingNode(processing);
            treeView.Nodes.Add(node);
            return node;
        }

        private void bCreateTechOperation_Click(string caption, Type operationType)
        {
            var techProcessType = operationType.BaseType.BaseType.GetGenericArguments()[0];
            var processingNode = treeView.Nodes.Count > 0
                ? SelectedProcessingNode ?? treeView.Nodes[treeView.Nodes.Count - 1]
                : null;
            if (processingNode == null || processingNode.Tag.GetType() != techProcessType)
                processingNode = AddProcessingNode(techProcessType);
            var operationNumber = ++processingNode.Tag.As<IProcessing>().LastOperationNumber;
            var operation = OperationFactory.CreateOperation(caption, operationType, operationNumber, SelectedNode?.Tag);

            var node = CreateOperationNode(operation);
            processingNode.Nodes.Add(node);
            treeView.SelectedNode = node;
            RefreshToolButtonsState();
        }

        #endregion

        #region Delete

        private void bRemove_Click(object sender, EventArgs e) => DeleteNode();

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                DeleteNode();
        }

        private void DeleteNode()
        {
            if (SelectedNode != null && MessageBox.Show($"Вы хотите удалить {SelectedNode.Text}?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                // удалить обработку
                SelectedNode.Remove();
                treeView.Focus();
                RefreshToolButtonsState();
                Acad.UnhighlightAll();
            }
        }
        #endregion

        #region Move
        private void bMoveUpTechOperation_Click(object sender, EventArgs e) => MoveNode(-1);

        private void bMoveDownTechOperation_Click(object sender, EventArgs e) => MoveNode(1);

        private void MoveNode(int direction)
        {
            var node = SelectedNode;
            var nodes = node.Parent?.Nodes ?? treeView.Nodes;
            var index = node.Index + direction;
            if (index >= 0 && index < nodes.Count)
            {
                nodes.Remove(node);
                nodes.Insert(index, node);
                treeView.SelectedNode = node;
            }
        }
        #endregion

        #region Edit

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = true;
            e.Node.BeginEdit();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeView.LabelEdit = false;
            if (string.IsNullOrWhiteSpace(e.Label))
                e.CancelEdit = true;
        }

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 1)
                e.Node.ForeColor = e.Node.Checked ? Color.Black : Color.Gray;
            else if (!e.Node.Checked)
                e.Node.Checked = true;
        }

        #endregion

        #region Select

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshParamsView();
            if (SelectedNode.Tag is IOperation operation)
            {
                if (e.Action != TreeViewAction.Unknown && _program?.TryGetCommandIndexByOperationNumber(operation.Number, out var commandIndex) == true)
                    processCommandBindingSource.Position = commandIndex;

                _program?.ShowOperationToolpath(operation.Number);
                Acad.SelectObjectIds(operation.ProcessingArea?.ObjectIds);
                Acad.Editor.UpdateScreen();
            }
        }

        #endregion

        #endregion

        #region Params view
        private readonly Dictionary<Type, ParamsControl> _paramsViews = [];

        private void RefreshParamsView()
        {
            var type = SelectedNode.Tag.GetType();
            if (!_paramsViews.TryGetValue(type, out var paramsView))
            {
                paramsView = new ParamsControl(type);
                paramsView.Dock = DockStyle.Fill;
                tabPageParams.Controls.Add(paramsView);
                _paramsViews[type] = paramsView;
            }

            paramsView.SetData(SelectedNode.Tag);
            paramsView.Show();
            paramsView.BringToFront();
        }
        #endregion

        #region Program view

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (processCommandBindingSource.Current == null)
                return;

            if (SelectedCommand.ObjectId.HasValue)
            {
                Acad.Show(SelectedCommand.ObjectId.Value);
                Acad.SelectObjectIds(SelectedCommand.ObjectId.Value);
            }

            ToolObject.Set(_program.Processing.Operations.FirstOrDefault(p => p.Number == SelectedCommand.OperationNumber)?.GetTool(), SelectedCommand.ToolPosition);
            
            var node = GetProgramProcessingNode()?.Nodes.Cast<TreeNode>().FirstOrDefault(p => ((IOperation)p.Tag).Number == SelectedCommand.OperationNumber);
            if (node != null) 
                treeView.SelectedNode = node;
        }

        public void SelectCommand(ObjectId? objectId)
        {
            if (objectId.HasValue && _program?.TryGetCommandIndexByObjectId(objectId.Value, out var commandIndex) == true)
                processCommandBindingSource.Position = commandIndex;
        }
        #endregion

    }
}