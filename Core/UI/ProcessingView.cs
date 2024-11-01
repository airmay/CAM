using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.Core;
using Font = System.Drawing.Font;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        private TreeNode SelectedNode => treeView.SelectedNode;
        private ProcessItem ProcessItem => treeView.SelectedNode?.Tag as ProcessItem;
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

        public void ClearAll()
        {
            if (_camDocument != null)
                _camDocument.ProcessItems = GetProcessItems();
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
            var processingNode = SelectedNode?.Parent ?? SelectedNode ?? treeView.Nodes[0];
            var processing = (ProcessingBase)GetProcessItem(processingNode);

#if !DEBUG
            toolStrip.Enabled = false;
#endif 
            ClearProgram();

            _program = processing.Execute();

            UpdateNodeText(processingNode);
            processingNode.Nodes.Cast<TreeNode>().ForAll(UpdateNodeText);
                
            processCommandBindingSource.DataSource = _program?.ArraySegment;
            
            toolStrip.Enabled = true;
            RefreshToolButtonsState();
            treeView_AfterSelect(sender, null);
            
            return;

            void UpdateNodeText(TreeNode node) => node.Text = ((ProcessItem)node.Tag).Caption;
        }

        private void bVisibility_Click(object sender, EventArgs e)
        {
            //SelectedNode.SetVisibility(IsToolpathVisible);
            //Acad.Editor.UpdateScreen();
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

            if (_camDocument.ProcessItems?.Any() == true)
            {
                var nodes = Array.ConvertAll(_camDocument.ProcessItems, p => CreateNode(p, 0));
                treeView.Nodes.AddRange(nodes);
                treeView.ExpandAll();
                treeView.SelectedNode = treeView.Nodes[0];
            }
            RefreshToolButtonsState();
        }

        public void SaveCamDocument()
        {
            _camDocument.Save(GetProcessItems());
        }

        public ProcessItem[] GetProcessItems() => treeView.Nodes.Cast<TreeNode>().Select(GetProcessItem).ToArray();

        public static ProcessItem GetProcessItem(TreeNode node)
        {
            var processItem = (ProcessItem)node.Tag;
            processItem.Caption = node.Text;
            processItem.Enabled = node.Checked;
            processItem.Children = node.Nodes.Cast<TreeNode>().Select(GetProcessItem).ToArray();
            return processItem;
        }

        #endregion

        #region Add

        private void bCreateProcessing_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = AddProcessingNode(MachineType.CncWorkCenter);
        }

        private void bCreateTechOperation_Click(string caption, Type type)
        {
            var operation = ProcessItemFactory.CreateOperation(caption, type, SelectedNode?.Tag);
            var node = CreateNode(operation, 1);
            var processingNode = treeView.Nodes.Count > 0
                ? SelectedNode?.Parent ?? SelectedNode ?? treeView.Nodes[treeView.Nodes.Count - 1]
                : null;
            if (processingNode == null || ((ProcessingBase)processingNode.Tag).MachineType != operation.MachineType)
                processingNode = AddProcessingNode(operation.MachineType);
            processingNode.Nodes.Add(node);
            treeView.SelectedNode = node;
            RefreshToolButtonsState();
        }

        private TreeNode AddProcessingNode(MachineType machineType)
        {
            var processing = ProcessItemFactory.CreateProcessing(machineType);
            var node = CreateNode(processing, 0);
            treeView.Nodes.Add(node);
            return node;
        }

        private TreeNode CreateNode(ProcessItem item, int level)
        {
            var children = item.Children?.Select(p => CreateNode(p, level + 1)).ToArray()
                           ?? Array.Empty<TreeNode>();
            return new TreeNode(item.Caption, level, level, children)
            {
                Checked = item.Enabled,
                NodeFont = new Font(this.Font, level == 0 ? FontStyle.Bold : FontStyle.Regular),
                Tag = item
            };
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

                ProcessItem.OnDelete();
                Acad.UnhighlightAll();
                RefreshToolButtonsState();
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

        #endregion

        #region Select

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshParamsView();
            processCommandBindingSource.Position = ProcessItem.GetCommandIndex();
            ProcessItem.OnSelect();
        }

        #endregion

        #endregion

        #region Params view
        private readonly Dictionary<Type, ParamsView> _paramsViews = new Dictionary<Type, ParamsView>();

        public void RefreshParamsView()
        {
            var type = ProcessItem.GetType();
            if (!_paramsViews.TryGetValue(type, out var paramsView))
            {
                paramsView = new ParamsView(type);
                paramsView.Dock = DockStyle.Fill;
                tabPageParams.Controls.Add(paramsView);
                _paramsViews[type] = paramsView;
            }

            paramsView.BindingSource.DataSource = ProcessItem;
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
            ToolObject.Hide();
            Acad.DeleteAll();
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

            ToolObject.Set(SelectedCommand.OperationBase?.Machine, SelectedCommand.OperationBase?.Tool, SelectedCommand.ToolLocation);
        }

        public void SelectCommand(ObjectId? objectId)
        {
            if (objectId.HasValue && _program != null 
                                  && _program.TryGetCommandIndex(objectId.Value, out var commandIndex))
                processCommandBindingSource.Position = commandIndex;
        }
        #endregion
    }
}