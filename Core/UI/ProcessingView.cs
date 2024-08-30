using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CAM.Core;
using CAM.Core.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        public TreeNodeCollection Nodes => treeView.Nodes;
        private ProcessingNode ProcessingNode => (ProcessingNode)(treeView.SelectedNode.Parent ?? treeView.SelectedNode);
        private IProcessing Processing => ProcessingNode.Processing;
        private OperationNodeBase SelectedNode => treeView.SelectedNode as OperationNodeBase;
        private OperationCnc SelectedOperation => treeView.SelectedNode?.Tag as OperationCnc;
        private Command SelectedCommand => processCommandBindingSource.Current as Command;

        public ProcessingView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new System.Drawing.Image[] { Properties.Resources.folder, Properties.Resources.drive_download });
            bCreateTechOperation.DropDownItems.AddRange(OperationItemsContainer.GetMenuItems(bCreateTechOperationClick));
            //RefreshToolButtonsState();
#if DEBUG
            bClose.Visible = true;
#endif
        }

        #region ToolButtons

        private void bCreateTechOperationClick(string caption, Type type)
        {
            var machineType = type.GetCustomAttribute<MachineTypeNewAttribute>().MachineType;
            var processing = Nodes.Any()
                ? (IProcessing)(treeView.SelectedNode?.Parent ?? treeView.SelectedNode ?? treeView.Nodes[treeView.Nodes.Count - 1])
            var processing = (IProcessing)processingNode.Tag;
            if (processing.MachineType != machineType)
                AddProcessingNode(machineType);

            var operation = OperationFactory.Create(type, SelectedNode.Tag);
            var node = CreateOperationNode(operation);
            processingNode.Nodes.Add(node);
            treeView.SelectedNode = node;
        }

        private void AddProcessingNode(MachineType machineType)
        {
            var processing = ProcessingFactory.Create(machineType);
            var node = CreateProcessingNode(processing);
            treeView.Nodes.Add(node);
            treeView.SelectedNode = node;
        }

        TreeNode CreateProcessingNode(IProcessing processing)
        {
            return new TreeNode(processing.Caption, 0, 0, processing.Operations.Select(CreateOperationNode).ToArray());
        }

        TreeNode CreateOperationNode(IOperation operation) => CreateNode(operation.Caption, 1, operation);

        TreeNode CreateNode(string text, int imageIndex, object tag) => new TreeNode(text, imageIndex, imageIndex) { Tag = tag };

        private void RefreshToolButtonsState()
        {
            // bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = bBuildProcessing.Enabled = treeView.SelectedNode != null;
            // bCreateTechOperation.Enabled = treeView.SelectedNode != null && bCreateTechOperation.DropDownItems.Count > 0;
            // bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = SelectedNode?.ProcessCommands != null;
        }

        private void bCreateProcessing_Click(object sender, EventArgs e) => AddProcessingNode(MachineType.CncWorkCenter);

        private void bRemove_Click(object sender, EventArgs e) => Delete();

        private void bMoveUpTechOperation_Click(object sender, EventArgs e) => SelectedNode?.MoveUp();

        private void bMoveDownTechOperation_Click(object sender, EventArgs e) => SelectedNode?.MoveDown();

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);
            if (treeView.SelectedNode == null)
                treeView.SelectedNode = treeView.Nodes[0];
            toolStrip.Enabled = false;
            processCommandBindingSource.DataSource = CamManager.ExecuteProcessing(ProcessingNode.GetProcessing());
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
            CamManager.SendProgram();
        }

        private void bClose_Click(object sender, EventArgs e) => Acad.CloseAndDiscard();

        #endregion

        #region Tree

        public void Reset(IProcessing[] processings)
        {
            treeView.Nodes.Clear();
            ClearParamsViews();
            if (processings?.Any() != true)
                return;

            treeView.Nodes.AddRange(processings.Select(CreateProcessingNode1).ToArray());
            treeView.ExpandAll();
            treeView.SelectedNode = treeView.Nodes[0];
            return;
            
            TreeNode CreateProcessingNode1(IProcessing processing)
            {
                return new TreeNode(processing.Caption, 0, 0, processing.Operations.Select(CreateOperationNode1).ToArray());
            }

            TreeNode CreateOperationNode1(IOperation operation)
            {
                return new TreeNode(operation.Caption, 1, 1);
            }
        }

        public IProcessing[] GetProcessings()
        {
            return treeView.Nodes.Cast<TreeNode>().Select(GetProcessing).ToArray();
        }

        private IProcessing GetProcessing(TreeNode node)
        {
            var processing = (IProcessing)node.Tag;
            processing.Caption = node.Text;
            processing.Operations = node.Nodes.Cast<TreeNode>().Select(p =>
                {
                    var operation = (IOperation)p.Tag;
                    operation.Caption = node.Text;
                    operation.Enabled = node.Checked;
                    return operation;
                })
                .ToArray();
            return processing;
        }

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

        public void ClearView()
        {
            toolStrip.Enabled = false;
            treeView.Nodes.Clear();
            ClearParamsViews();
        }

        private bool IsToolpathVisible => !bVisibility.Checked;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshParamsView();
            processCommandBindingSource.Position = SelectedNode.FirstCommandIndex;
            SelectedNode.SelectAcadObject();

            if (IsToolpathVisible)
                SelectedNode.ShowToolpath();
        }

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

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                Delete();
        }

        private void Delete()
        {
            if (SelectedNode != null && MessageBox.Show($"Вы хотите удалить {SelectedNode.Text}?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                SelectedNode.RemoveOperation();

                //Acad.UnhighlightAll();
                ClearParamsViews();
                treeView.SelectedNode.Remove();
                treeView.Focus();
                //RefreshToolButtonsState();
            }
        }

        #endregion

        #region Views

        private readonly Dictionary<Type, ParamsView> _paramsViews = new Dictionary<Type, ParamsView>();

        public void RefreshParamsView()
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

        private void ClearParamsViews()
        {
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            ClearCommandsView();
        }

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
            CamManager.ShowTool(SelectedCommand);
        }

        public void SelectProcessCommand(int commandIndex) => processCommandBindingSource.Position = commandIndex;

        #endregion
    }
}