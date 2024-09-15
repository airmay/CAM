using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using CAM.Core;

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

        #region Views
        public void Reset(ProcessItem[] items)
        {
            treeView.Nodes.Clear();
            ClearParamsViews();
            if (items?.Any() != true)
                return;

            treeView.Nodes.AddRange(items.Select(p => CreateNode(p, 0)).ToArray());
            treeView.ExpandAll();
            treeView.SelectedNode = treeView.Nodes[0];
        }

        public void ClearView() // Document = null
        {
            toolStrip.Enabled = false;
            treeView.Nodes.Clear();
            ClearParamsViews();
        }

        private void ClearParamsViews()
        {
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            ClearCommandsView();
        }
        #endregion

        #region Tool buttons
        private void RefreshToolButtonsState()
        {
            bRemove.Enabled = bMoveUp.Enabled = bMoveDown.Enabled = bBuildProcessing.Enabled = (SelectedNode != null);
            //TODO
            //bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = SelectedNode?.ProcessCommands != null;
        }

        // TODO перенести
        public void UpdateNodeText()
        {
            foreach (TreeNode node in treeView.Nodes)
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

        private bool IsToolpathVisible => !bVisibility.Checked;

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);
            var processing = GetProcessItem(SelectedNode?.Parent ?? SelectedNode ?? treeView.Nodes[0]) as IProcessing;
            toolStrip.Enabled = false;
            DeleteGenerated();
            _program = processing.Execute();
            processCommandBindingSource.DataSource = _program.GetCommands();
            UpdateNodeText();
            toolStrip.Enabled = true;
            treeView_AfterSelect(sender, null);
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
                System.Threading.Thread.Sleep((int)((Command)processCommandBindingSource.Current).Duration * 10);
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
            var machineType = type.GetCustomAttribute<MachineTypeNewAttribute>().MachineType;
            var processingNode = treeView.Nodes.Count > 0
                ? SelectedNode?.Parent ?? SelectedNode ?? treeView.Nodes[treeView.Nodes.Count - 1]
                : null;
            if (((IProcessing)processingNode?.Tag)?.MachineType != machineType)
                processingNode = AddProcessingNode(machineType);

            var operation = ProcessItemFactory.CreateOperation(type, SelectedNode?.Tag);
            var node = CreateNode(operation, 1);
            processingNode.Nodes.Add(node);
            treeView.SelectedNode = node;
        }

        private TreeNode AddProcessingNode(MachineType machineType)
        {
            var processing = ProcessItemFactory.CreateProcessing(machineType);
            var node = CreateNode(processing, 0);
            treeView.Nodes.Add(node);
            return node;
        }

        private static TreeNode CreateNode(ProcessItem item, int level)
        {
            var children = item.Children?.Select(p => CreateNode(p, level + 1)).ToArray()
                           ?? Array.Empty<TreeNode>();
            return new TreeNode(item.Caption, level, level, children)
            {
                Checked = item.Enabled,
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
                ClearParamsViews();
                SelectedNode.Remove();
                treeView.Focus();
                RefreshToolButtonsState();

                ProcessItem.OnDelete();
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

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e) => ((OperationNodeBase)e.Node).RefreshColor();

        #endregion

        #endregion

        #region Select

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshParamsView();
            processCommandBindingSource.Position = ProcessItem.CommandIndex;
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

        private IProgram _program;
        public void ClearCommandsView() => processCommandBindingSource.DataSource = null;

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (SelectedCommand == null)
                return;

            if (SelectedCommand.Toolpath.HasValue)
            {
                Acad.Show(SelectedCommand.Toolpath.Value);
                Acad.SelectObjectIds(SelectedCommand.Toolpath.Value);
            }
            Acad.ToolObject.Set(SelectedCommand?.Operation?.Processing.Machine, SelectedCommand?.Operation?.Tool, SelectedCommand.Position, SelectedCommand.AngleC, SelectedCommand.AngleA);
        }

        public void SelectCommand(ObjectId? objectId)
        {
            if (objectId.HasValue && _program.TryGetCommandIndex(objectId.Value, out var commandIndex))
                processCommandBindingSource.Position = commandIndex;
        }
        #endregion

        private void DeleteGenerated()
        {
            ToolObject.Hide();
            //_processing?.RemoveAcadObjects();
        }
    }
}