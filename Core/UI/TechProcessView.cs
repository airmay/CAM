﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public partial class TechProcessView : UserControl
    {
	    private CamDocument _camDocument;
        private Type _currentTechProcessType;
        private ITechProcess CurrentTechProcess => (treeView.SelectedNode?.Parent ?? treeView.SelectedNode)?.Tag as ITechProcess;
        private ProcessCommand CurrentProcessCommand => processCommandBindingSource.Current as ProcessCommand;

        public TechProcessView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download });
            RefreshToolButtonsState();
#if DEBUG
            bClose.Visible = true;
#endif
        }

        private Dictionary<Type, ToolStripMenuItem[]> _techOperationItems;
        private Dictionary<Autodesk.AutoCAD.DatabaseServices.ObjectId, int> _processCommandsIdx;
        private int _startIdx;

        public void SetCamDocument(CamDocument camDocument)
        {
            _camDocument = camDocument;
            _currentTechProcessType = null;
            if (bCreateTechProcess.DropDownItems.Count == 0)
                Init();
            ClearParamsViews();
            treeView.Nodes.Clear();
            _camDocument?.TechProcessList.ForEach(p => CreateTechProcessNode(p));
            treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            toolStrip.Enabled = _camDocument != null;
            RefreshToolButtonsState();
        }

        private void Init()
        {
            bCreateTechProcess.DropDownItems.AddRange(_camDocument.GetTechProcessNames().Select(p =>
            {
                var item = new ToolStripMenuItem { Text = p };
                item.Click += new EventHandler(createTechProcessItem_Click);
                return item;
            })
            .ToArray());
            _techOperationItems = _camDocument.GetTechOperationNames().ToDictionary(p => p.Key, p => p.Select(v =>
            {
                var item = new ToolStripMenuItem { Text = v };
                item.Click += new EventHandler(bCreateTechOperation_Click);
                return item;
            })
            .ToArray());
        }

        #region Views
        private void ClearParamsViews()
        {
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            ClearCommandsView();
        }

        public void ClearCommandsView() => processCommandBindingSource.DataSource = null;

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (CurrentProcessCommand != null)
                _camDocument.SelectProcessCommand(CurrentTechProcess, CurrentProcessCommand);
        }

        public void SelectProcessCommand(Autodesk.AutoCAD.DatabaseServices.ObjectId id)
        {
            if (_processCommandsIdx.ContainsKey(id))
                processCommandBindingSource.Position = _processCommandsIdx[id] - _startIdx;
        }

        public void RefreshViews()
        {
            var dataObject = treeView.SelectedNode.Tag;
            ViewsContainer.BindData(dataObject, tabPageParams);
            var commands = ((IHasProcessCommands)dataObject).ProcessCommands;
            if (commands != null)
                for (int i = 0; i < commands.Count; i++)
                    if (commands[i].ToolpathObjectId != null)
                    {
                        _startIdx = _processCommandsIdx[commands[i].ToolpathObjectId.Value] - i;
                        break;
                    }
            var tid = CurrentProcessCommand?.ToolpathObjectId;
            processCommandBindingSource.DataSource = commands;
            if (commands != null && treeView.SelectedNode.Parent == null && tid != null && _processCommandsIdx.ContainsKey(tid.Value))
                processCommandBindingSource.Position = _processCommandsIdx[tid.Value];
        }
        #endregion

        #region Tree
        private TreeNode CreateTechProcessNode(ITechProcess techProcess)
        {
            var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
            var techProcessNode = new TreeNode(techProcess.Caption + "   ", 0, 0, children) { Tag = techProcess, Checked = true, NodeFont = new Font(treeView.Font, FontStyle.Bold) };
            treeView.Nodes.Add(techProcessNode);
            techProcessNode.ExpandAll();
            RefreshToolButtonsState();

            return techProcessNode;
        }

        private static TreeNode CreateTechOperationNode(ITechOperation techOperation) =>
            new TreeNode(techOperation.Caption, 1, 1) { Tag = techOperation, Checked = techOperation.Enabled, ForeColor = techOperation.Enabled ? Color.Black : Color.Gray };

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (CurrentTechProcess.GetType() != _currentTechProcessType)
            {
                _currentTechProcessType = CurrentTechProcess.GetType();
                bCreateTechOperation.DropDownItems.Clear();
                if (_techOperationItems.ContainsKey(_currentTechProcessType))
                    bCreateTechOperation.DropDownItems.AddRange(
                        new List<ToolStripItem> { new ToolStripMenuItem("Все операции", null, new EventHandler(bCreateTechOperation_Click)), new ToolStripSeparator() }
                        .Concat(_techOperationItems[CurrentTechProcess.GetType()]).ToArray());                
                RefreshToolButtonsState();
                CreateProcessCommandsIdx();
            }
            if (treeView.SelectedNode.Tag is ITechProcess)
                _camDocument.SelectTechProcess((ITechProcess)treeView.SelectedNode.Tag);
            else
                _camDocument.SelectTechOperation((ITechOperation)treeView.SelectedNode.Tag);
            RefreshViews();
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = true;
            e.Node.BeginEdit();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeView.LabelEdit = false;
            if (String.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            e.Node.Text = e.Label;
            switch (e.Node.Tag)
            {
                case ITechProcess techProcess:
                    techProcess.Caption = e.Label;
                    break;
                case TechOperationBase techOperation:
                    techOperation.Caption = e.Label;
                    break;
            }
        }

        private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) => e.Cancel = e.Node.Parent == null;

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            e.Node.ForeColor = e.Node.Checked ? Color.Black : Color.Gray;
            ((ITechOperation)e.Node.Tag).Enabled = e.Node.Checked;
        }

        private void EndEdit()
        {
            //if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
            // treeView.SelectedNode.EndEdit(false);
        }
        #endregion

        #region ToolButtons

        private void RefreshToolButtonsState()
        {
            bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = bBuildProcessing.Enabled = treeView.SelectedNode != null;
            bCreateTechOperation.Enabled = treeView.SelectedNode != null && bCreateTechOperation.DropDownItems.Count > 0;
            bDeleteExtraObjects.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = CurrentTechProcess?.ProcessCommands != null;
        }

        private void createTechProcessItem_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = CreateTechProcessNode(_camDocument.CreateTechProcess(((ToolStripMenuItem)sender).Text));
        }

        private void bCreateTechOperation_Click(object sender, EventArgs e)
        {
            EndEdit();
            var techOperations = _camDocument.CreateTechOperation(CurrentTechProcess, ((ToolStripMenuItem)sender).Text);
            if (techOperations.Any())
            {
                var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
                techProcessNode.Nodes.AddRange(techOperations.ConvertAll(CreateTechOperationNode).ToArray());
                treeView.SelectedNode = techProcessNode.Nodes[techProcessNode.Nodes.Count - 1];
            }
        }

	    private void bRemove_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode == null)
			    return;
		    if (MessageBox.Show($"Вы хотите удалить {(treeView.SelectedNode.Tag is ITechProcess ? "техпроцесс" : "операцию")} '{treeView.SelectedNode.Text}'?",
			        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
		    {
			    switch (treeView.SelectedNode.Tag)
			    {
				    case ITechProcess techProcess:
                        _camDocument.DeleteTechProcess(techProcess);
					    break;
				    case TechOperationBase techOperation:
                        _camDocument.DeleteTechOperation(techOperation);
					    break;
			    }
                Acad.SelectObjectIds();
                ClearParamsViews();
                treeView.SelectedNode.Remove();
                RefreshToolButtonsState();
            }
        }

	    private void MoveSelectedNode(int shift)
	    {
		    var node = treeView.SelectedNode;
		    var nodes = node.Parent.Nodes;
		    var index = nodes.IndexOf(node);
		    nodes.Remove(node);
		    nodes.Insert(index + shift, node);
		    treeView.SelectedNode = node;
	    }

	    private void bMoveUpTechOperation_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode.Tag is TechOperationBase techOperation)
		    {
			    EndEdit();
			    if (techOperation.TechProcess.TechOperationMoveUp(techOperation))
				    MoveSelectedNode(-1);
		    }
	    }

	    private void bMoveDownTechOperation_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode.Tag is TechOperationBase techOperation)
		    {
			    EndEdit();
			    if (techOperation.TechProcess.TechOperationMoveDown(techOperation))
				    MoveSelectedNode(1);
		    }
	    }

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            var node = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            //treeView.SelectedNode = node;
            SelectNextControl(ActiveControl, true, true, true, true);

            toolStrip.Enabled = false;
            if (sender == bBuildProcessing)
                _camDocument.BuildProcessing(CurrentTechProcess);
            else
                _camDocument.PartialProcessing(CurrentTechProcess, CurrentProcessCommand);
            toolStrip.Enabled = true;

            CreateProcessCommandsIdx();

            if (node.Nodes.Count == 0)
            {
                node.Nodes.AddRange(CurrentTechProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray());
                node.Expand();
            }

            UpdateCaptions();
            RefreshToolButtonsState();

            ClearCommandsView();
            RefreshViews();
            tabControl.SelectedTab = tabPageCommands;
        }

        private void CreateProcessCommandsIdx() => 
            _processCommandsIdx = CurrentTechProcess.ProcessCommands?
                    .Select((p, ind) => new { p.ToolpathObjectId, ind })
                    .Where(p => p.ToolpathObjectId != null)
                    .ToDictionary(p => p.ToolpathObjectId.Value, p => p.ind);

        private void UpdateCaptions()
        {
            var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            techProcessNode.Text = ((ITechProcess)techProcessNode.Tag).Caption;
            techProcessNode.Nodes.Cast<TreeNode>().ToList().ForEach(p => p.Text = ((ITechOperation)p.Tag).Caption);
        }

        private void bDeleteExtraObjects_Click(object sender, EventArgs e)
        {
            _camDocument.DeleteExtraObjects(CurrentTechProcess);
            Acad.Editor.UpdateScreen();
        }

        private void bPlay_Click(object sender, EventArgs e)
        {
            toolStrip.Enabled = false;
            var command = _camDocument.Play(CurrentTechProcess, processCommandBindingSource.Position);
            processCommandBindingSource.Position = CurrentTechProcess.ProcessCommands.IndexOf(command);
            toolStrip.Enabled = true;
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            dataGridViewCommand.EndEdit();
            _camDocument.SendProgram(((IHasProcessCommands)treeView.SelectedNode.Tag).ProcessCommands, CurrentTechProcess);
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            //foreach (Document doc in Application.DocumentManager)
            //    if (!doc.IsActive)
            //        doc.CloseAndDiscard();

            Autodesk.AutoCAD.ApplicationServices.DocumentExtension.CloseAndDiscard(Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument);
            //Autodesk.AutoCAD.ApplicationServices.DocumentCollectionExtension.CloseAll(Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Quit();
        }
        #endregion
    }
}
