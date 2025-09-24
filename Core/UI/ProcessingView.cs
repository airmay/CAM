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
    public interface ITreeNode
    {
        string Caption { get; set; }
        void OnSelect();
    }

    public partial class ProcessingView : UserControl
    {
        private Program _program;

        private TreeNode SelectedNode => treeView.SelectedNode;
        private TreeNode SelectedProcessingNode => treeView.SelectedNode?.Parent ?? treeView.SelectedNode;
        private Command SelectedCommand => processCommandBindingSource.Current as Command;

        public ProcessingView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new System.Drawing.Image[] { Properties.Resources.folder, Properties.Resources.drive_download });
            bCreateTechOperation.DropDownItems.AddRange(OperationItemsContainer.GetMenuItems(bCreateTechOperation_Click));
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
            _program?.Processing?.Operations?.ForAll(p => p.ToolpathGroupId = null);
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

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);
            Acad.DocumentManager.DocumentActivationEnabled = false;
            var processingNode = SelectedProcessingNode ?? treeView.Nodes[0];
            var processing = GetProcessing(processingNode);
#if !DEBUG
            toolStrip.Enabled = false;
#endif
            processCommandBindingSource.DataSource = null;
            Acad.DeleteProcessObjects();
            Acad.ClearHighlighted();
            ToolObject.Delete();

            _program = processing.Execute();
            if (_program != null)
            {
                UpdateNodeText(processingNode);
                processingNode.Nodes.Cast<TreeNode>().ForAll(UpdateNodeText);

                processCommandBindingSource.DataSource = _program.ArraySegment;

                RefreshToolButtonsState();
                treeView_AfterSelect(sender, null);
            }

            toolStrip.Enabled = true;
            Acad.DocumentManager.DocumentActivationEnabled = true;

            if (Program.DwgFileCommands != null)
                Acad.Write(_program.ArraySegment.SequenceEqual(Program.DwgFileCommands, Command.Comparer)
                    ? "Программа не изменилась"
                    : "Внимание! Программа изменена!");

            return;

            void UpdateNodeText(TreeNode node) => node.Text = node.Tag.As<ITreeNode>().Caption;
        }

        private void bVisibility_Click(object sender, EventArgs e)
        {
            _program.Processing.HideToolpath(null);
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
                _program = new Program(_camDocument.Processings[_camDocument.ProcessingIndex.Value], _camDocument.ProgramFileExtension, _camDocument.Commands);
                processCommandBindingSource.DataSource = _camDocument.Commands;
            }
            RefreshToolButtonsState();
        }

        private TreeNode CreateProcessingNode(IProcessing processing)
        {
            var children = processing.Operations?.Select(CreateOperationNode).ToArray()
                           ?? Array.Empty<TreeNode>();
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
            _program?.Processing.Operations?.ForAll(p => p.ToolpathGroupId?.SetGroupVisibility(true));
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
            if (SelectedNode.Tag is IOperation operation && _program?.TryGetCommandIndexByOperationNumber(operation.Number, out var commandIndex) == true)
                processCommandBindingSource.Position = commandIndex;
            SelectedNode.Tag.As<ITreeNode>().OnSelect();
            Acad.Editor.UpdateScreen();
        }

        #endregion

        #endregion

        #region Params view
        private readonly Dictionary<Type, ParamsView> _paramsViews = new Dictionary<Type, ParamsView>();

        private void RefreshParamsView()
        {
            var type = SelectedNode.Tag.GetType();
            if (!_paramsViews.TryGetValue(type, out var paramsView))
            {
                paramsView = new ParamsView(type);
                paramsView.Dock = DockStyle.Fill;
                tabPageParams.Controls.Add(paramsView);
                _paramsViews[type] = paramsView;
            }

            paramsView.BindingSource.DataSource = SelectedNode.Tag;
            paramsView.Show();
            paramsView.BringToFront();
        }
        #endregion

        #region Program view

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (SelectedCommand == null)
                return;

            if (SelectedCommand.ObjectId.HasValue)
            {
                Acad.Show(SelectedCommand.ObjectId.Value);
                Acad.SelectObjectIds(SelectedCommand.ObjectId.Value);
            }

            var node = GetProgramProcessingNode()?.Nodes.Cast<TreeNode>().FirstOrDefault(p => ((IOperation)p.Tag).Number == SelectedCommand.OperationNumber);
            if (node == null) 
                return;

            treeView.SelectedNode = node;
            ToolObject.Set(((IOperation)node.Tag).GetTool(), SelectedCommand.ToolPosition);
        }

        public void SelectCommand(ObjectId? objectId)
        {
            if (objectId.HasValue && _program?.TryGetCommandIndexByObjectId(objectId.Value, out var commandIndex) == true)
                processCommandBindingSource.Position = commandIndex;
        }
        #endregion

        private void bPartialProcessing_Click(object sender, EventArgs e)
        {
            if (processCommandBindingSource?.Position != null &&
                MessageBox.Show($"Сформировать программу со строки {SelectedCommand?.Number}?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                _program.Processing.ExecutePartial(processCommandBindingSource.Position);
                
                var processingNode = GetProgramProcessingNode();
                UpdateNodeText(processingNode);
                processingNode.Nodes.Cast<TreeNode>().ForAll(UpdateNodeText);

                processCommandBindingSource.DataSource = _program.ArraySegment;

                treeView_AfterSelect(sender, null);
            }

            return;

            void UpdateNodeText(TreeNode node) => node.Text = node.Tag.As<ITreeNode>().Caption;
        }
    }
}