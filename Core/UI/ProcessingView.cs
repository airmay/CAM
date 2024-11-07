using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;
using Font = System.Drawing.Font;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        private TreeNode SelectedNode => treeView.SelectedNode;
        private TreeNode SelectedProcessingNode => treeView.SelectedNode?.Parent ?? treeView.SelectedNode;
        private Command SelectedCommand => processCommandBindingSource.Current as Command;
        private IProcessing _calculatedProcessing;
        private TreeNode _processingNode;

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

        public void ClearAll()
        {
            if (_camDocument != null)
                _camDocument.Processings = GetProcessings();
            _camDocument = null;

            toolStrip.Enabled = false;
            treeView.Nodes.Clear();
            ClearProgram();
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            Acad.ClearHighlighted();
        }

        #region Tool buttons
        private void RefreshToolButtonsState()
        {
            bRemove.Enabled = bMoveUp.Enabled = bMoveDown.Enabled = bBuildProcessing.Enabled = SelectedNode != null;
            bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = _program != null;
        }

        private bool IsToolpathVisible => !bVisibility.Checked;

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);
            Acad.DocumentManager.DocumentActivationEnabled = false;
            var processingNode = SelectedProcessingNode ?? treeView.Nodes[0];
            var processing = GetProcessing(processingNode);
#if !DEBUG
            toolStrip.Enabled = false;
#endif 
            ClearProgram();

            _program = processing.Execute();

            UpdateNodeText(processingNode);
            processingNode.Nodes.Cast<TreeNode>().ForAll(UpdateNodeText);
               
            _calculatedProcessing = _program != null ? processing : null;
            _processingNode = _program != null ? processingNode : null;
            processCommandBindingSource.DataSource = _program?.ArraySegment;
            
            toolStrip.Enabled = true;
            RefreshToolButtonsState();
            treeView_AfterSelect(sender, null);
            Acad.DocumentManager.DocumentActivationEnabled = true;

            return;

            void UpdateNodeText(TreeNode node) => node.Text = node.Tag.As<ITreeNode>().Caption;
        }

        private void bVisibility_Click(object sender, EventArgs e)
        {
            _calculatedProcessing.HideToolpath(null);
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

        #region Tree nodes

        #region Get

        private CamDocument _camDocument;

        public void SetCamDocument(CamDocument camDocument)
        {
            _camDocument = camDocument;
            toolStrip.Enabled = true;

            if (_camDocument.Processings?.Any() == true)
            {
                var nodes = Array.ConvertAll(_camDocument.Processings, CreateProcessingNode);
                treeView.Nodes.AddRange(nodes);
                treeView.ExpandAll();
                treeView.SelectedNode = treeView.Nodes[0];
            }
            RefreshToolButtonsState();
        }

        private TreeNode CreateProcessingNode(IProcessing processing)
        {
            var children = processing.Operations?.Select(CreateOperationNode).ToArray()
                           ?? Array.Empty<TreeNode>();
            return new TreeNode(processing.Caption, 0, 0, children)
            {
                NodeFont = new Font(this.Font, FontStyle.Bold),
                Tag = processing
            };
        }

        private static TreeNode CreateOperationNode(OperationBase operation)
        {
            return new TreeNode(operation.Caption, 1, 1)
            {
                Checked = operation.Enabled,
                Tag = operation
            };
        }

        public void SaveCamDocument() => _camDocument.Save(GetProcessings());

        private IProcessing[] GetProcessings() => treeView.Nodes.Cast<TreeNode>().Select(GetProcessing).ToArray();

        private static IProcessing GetProcessing(TreeNode node)
        {
            var processing = (IProcessing)node.Tag;
            processing.Caption = node.Text;
            processing.Operations = node.Nodes.Cast<TreeNode>()
                .Select(p =>
                {
                    var operation = (OperationBase)p.Tag;
                    operation.Caption = p.Text;
                    operation.Enabled = p.Checked;
                    return operation;
                })
                .ToArray();
            return processing;
        }

        #endregion

        #region Add

        private void bCreateProcessing_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = AddProcessingNode(MachineType.CncWorkCenter);
            RefreshToolButtonsState();
        }

        private TreeNode AddProcessingNode(MachineType machineType)
        {
            var processing = ProcessItemFactory.CreateProcessing(machineType);
            var node = CreateProcessingNode(processing);
            treeView.Nodes.Add(node);
            return node;
        }

        private void bCreateTechOperation_Click(string caption, Type type)
        {
            var operation = ProcessItemFactory.CreateOperation(caption, type, SelectedNode?.Tag);
            var processingNode = treeView.Nodes.Count > 0
                ? SelectedProcessingNode ?? treeView.Nodes[treeView.Nodes.Count - 1]
                : null;
            if (processingNode == null || ((IProcessing)processingNode.Tag).MachineType != operation.MachineType)
                processingNode = AddProcessingNode(operation.MachineType);
            operation.ProcessingBase = (ProcessingBase)processingNode.Tag;
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
            if (_program?.TryGetCommandIndex(SelectedNode.Tag, out var commandIndex) == true)
                processCommandBindingSource.Position = commandIndex;
            SelectedNode.Tag.As<ITreeNode>().OnSelect();
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

        private Program _program;
        public void ClearProgram()
        {
            _program = null;
            processCommandBindingSource.DataSource = null;
            Acad.DeleteProcessObjects();
            ToolObject.Hide();
        }

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (SelectedCommand == null)
                return;

            if (SelectedCommand.ObjectId.HasValue)
            {
                Acad.Show(SelectedCommand.ObjectId.Value);
                Acad.SelectObjectIds(SelectedCommand.ObjectId.Value);
            }

            ToolObject.Set(SelectedCommand.Operation?.Machine, SelectedCommand.Operation?.Tool, SelectedCommand.ToolLocation);

            var node = _processingNode.Nodes.Cast<TreeNode>().FirstOrDefault(p => p.Tag == SelectedCommand.Operation);
            if (node != null)
                treeView.SelectedNode = node;
        }

        public void SelectCommand(ObjectId? objectId)
        {
            if (objectId.HasValue && _program != null && _program.TryGetCommandIndex(objectId.Value, out var commandIndex))
                processCommandBindingSource.Position = commandIndex;
        }
        #endregion
    }
}