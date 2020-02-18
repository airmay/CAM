using System;
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

        public TechProcessView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download });
            dataGridViewCommand.DataSource = processCommandBindingSource;
            processCommandBindingSource.DataSource = null;
            SetButtonsEnabled();

            bSwapOuterSide.Visible = false;
            bAttachDrawing.Visible = false;
#if DEBUG
            bClose.Visible = true;
#endif
        }

        private Dictionary<Type, ToolStripMenuItem[]> _techOperationItems;

        public void SetCamDocument(CamDocument camDocument)
        {
            _camDocument = camDocument;

            if (bCreateTechProcess.DropDownItems.Count == 0)
                CreateTechProcessItems();
            ClearParamsViews();
            treeView.Nodes.Clear();
            _camDocument?.TechProcessList.ForEach(p => CreateTechProcessNode(p));
            treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            toolStrip1.Enabled = _camDocument != null;
            SetButtonsEnabled();
        }

        private void CreateTechProcessItems()
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

        private void SetButtonsEnabled() => bCreateTechOperation.Enabled = bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = 
            bSwapOuterSide.Enabled = bBuildProcessing.Enabled = bSendProgramm.Enabled = treeView.Nodes.Count > 0;

        private TreeNode CreateTechProcessNode(ITechProcess techProcess)
	    {
            var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
            var techProcessNode = new TreeNode(techProcess.Caption, 0, 0, children) {Tag = techProcess};
		    treeView.Nodes.Add(techProcessNode);
            techProcessNode.ExpandAll();
            bCreateTechOperation.Enabled = true;
            bRemove.Enabled = true;
            return techProcessNode;
        }

        private static TreeNode CreateTechOperationNode(ITechOperation techOperation) => new TreeNode(techOperation.Caption, 1, 1) { Tag = techOperation };

        private void ClearParamsViews()
        {
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            ClearCommandsView();
        }

        public void ClearCommandsView() => processCommandBindingSource.DataSource = null;

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (processCommandBindingSource.Current != null)
                _camDocument.SelectProcessCommand(CurrentTechProcess, processCommandBindingSource.Current as ProcessCommand);
        }

        public void RefreshView()
        {
            var dataObject = treeView.SelectedNode.Tag;
            ObjectViewsContainer.SetObjectView(dataObject, tabPageParams);
            processCommandBindingSource.DataSource = ((IHasProcessCommands)dataObject).ProcessCommands;
            bDeleteProcessing.Enabled = CurrentTechProcess.ProcessCommands != null;
        }


        #region treeView
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (CurrentTechProcess.GetType() != _currentTechProcessType)
            {
                bCreateTechOperation.DropDownItems.Clear();
                var items = new List<ToolStripItem> { new ToolStripMenuItem("Все операции", null, new EventHandler(bCreateTechOperation_Click)), new ToolStripSeparator() }
                    .Concat(_techOperationItems[CurrentTechProcess.GetType()]).ToArray();
                bCreateTechOperation.DropDownItems.AddRange(items);
                _currentTechProcessType = CurrentTechProcess.GetType();
            }
            RefreshView();
            _camDocument.SelectTechProcess(CurrentTechProcess);
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = true;
            e.Node.BeginEdit();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            switch (treeView.SelectedNode.Tag)
            {
                case ITechProcess techProcess:
                    techProcess.Caption = e.Label;
                    break;
                case TechOperationBase techOperation:
                    techOperation.Caption = e.Label;
                    break;
            }
            treeView.LabelEdit = false;
        } 
        #endregion

        #region Toolbar handlers

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
            SetButtonsEnabled();
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
                SetButtonsEnabled();
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

	    private void bBuildProcessing_Click(object sender, EventArgs e)
	    {
	        if (CurrentTechProcess != null)
	        {
                treeView.Focus();
                _camDocument.BuildProcessing(CurrentTechProcess);
                RefreshView();
	        }
	    }

        private void bDeleteProcessing_Click(object sender, EventArgs e)
        {
            _camDocument.HideShowProcessing(CurrentTechProcess);
        }

        private void bSwapOuterSide_Click(object sender, EventArgs e)
        {
            _camDocument.SwapOuterSide(treeView.SelectedNode?.Tag as ITechProcess, treeView.SelectedNode?.Tag as TechOperationBase);
            RefreshView();
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            Acad.Register();
            //Acad.SaveToPdf();
            //dataGridViewCommand.EndEdit();
            //_camDocument.SendProgram(((IHasProcessCommands)treeView.SelectedNode.Tag).ProcessCommands, CurrentTechProcess);
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            foreach (Autodesk.AutoCAD.ApplicationServices.Document doc in Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager)
                doc.CloseAndDiscard();

            //Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.CloseAll();
            Autodesk.AutoCAD.ApplicationServices.Application.Quit();
        }

        private void bAttachDrawing_Click(object sender, EventArgs e)
        {
            Acad.SaveToPdf();
        }
        #endregion

        private void EndEdit()
        {
            //if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
            // treeView.SelectedNode.EndEdit(false);
        }
    }
}
