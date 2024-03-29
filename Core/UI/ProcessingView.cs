﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CAM.Core;
using CAM.Core.UI;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        public TreeNodeCollection Nodes => treeView.Nodes;
        private GeneralOperationNode GeneralOperationNode => (GeneralOperationNode)(treeView.SelectedNode.Parent ?? treeView.SelectedNode);
        private GeneralOperation GeneralOperation => GeneralOperationNode.GeneralOperation;
        private OperationNodeBase SelectedNode => treeView.SelectedNode as OperationNodeBase;
        private Operation SelectedOperation => SelectedNode?.Tag as Operation;
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
            var node = new OperationNode(OperationFactory.Create(type, SelectedOperation), caption);

            if (Nodes.Count == 0)
                bCreateGeneralOperation_Click(null, null);

            var generalOperationNode = SelectedNode is null
                ? treeView.Nodes[treeView.Nodes.Count - 1]
                : treeView.SelectedNode.Parent ?? treeView.SelectedNode;

            generalOperationNode.Nodes.Add(node);
            generalOperationNode.ExpandAll();
            treeView.SelectedNode = node;
        }

        private void RefreshToolButtonsState()
        {
            // bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = bBuildProcessing.Enabled = treeView.SelectedNode != null;
            // bCreateTechOperation.Enabled = treeView.SelectedNode != null && bCreateTechOperation.DropDownItems.Count > 0;
            // bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = SelectedNode?.ProcessCommands != null;
        }

        private void bCreateGeneralOperation_Click(object sender, EventArgs e)
        {
            var node = new GeneralOperationNode();
            treeView.Nodes.Add(node);
            treeView.SelectedNode = node;
        }

        private void bRemove_Click(object sender, EventArgs e) => Delete();

        private void bMoveUpTechOperation_Click(object sender, EventArgs e) => SelectedNode?.MoveUp();

        private void bMoveDownTechOperation_Click(object sender, EventArgs e) => SelectedNode?.MoveDown();

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);

            toolStrip.Enabled = false;
            processCommandBindingSource.DataSource = CamManager.ExecuteProcessing();
            toolStrip.Enabled = true;
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
                System.Threading.Thread.Sleep((int)((ProcessCommand)processCommandBindingSource.Current).Duration * 10);
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

        public void SetNodes(TreeNode[] nodes)
        {
            ClearView();

            treeView.Nodes.AddRange(nodes);
            treeView.ExpandAll();
            //RefreshToolButtonsState();
            toolStrip.Enabled = true;
        }

        public void ClearView()
        {
            treeView.Nodes.Clear();
            toolStrip.Enabled = false;
            ClearParamsViews();
        }

        private bool IsToolpathVisible => !bVisibility.Checked;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshParamsView();
            processCommandBindingSource.Position = SelectedNode.FirstCommandIndex;
            SelectedNode.SelectAcadObject();

            if (IsToolpathVisible)
            {
                SelectedNode.ShowToolpath();
                Acad.Editor.UpdateScreen();
            }
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