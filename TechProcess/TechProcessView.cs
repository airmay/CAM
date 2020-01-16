using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CAM
{
    public partial class TechProcessView : UserControl
    {
	    private CamDocument _camDocument;

        private readonly TechProcessParamsView _techProcessParamsView = new TechProcessParamsView { Dock = DockStyle.Fill, Visible = false };
        private readonly UserControl _paramsView = new UserControl { Dock = DockStyle.Fill, Visible = false };
	    private readonly Dictionary<Type, Control> _paramsViews = new Dictionary<Type, Control>();
	    private BorderProcessingAreaView _borderProcessingAreaView;

	    private TechProcess CurrentTechProcess => (treeView.SelectedNode?.Parent ?? treeView.SelectedNode)?.Tag as TechProcess;

	    private static TreeNode CreateTechOperationNode(ITechOperation techOperation) => new TreeNode(techOperation.Name, 1, 1) {Tag = techOperation};

        public TechProcessView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download });

            tabPageParams.Controls.Add(_techProcessParamsView);
            tabPageParams.Controls.Add(_paramsView);

            dataGridViewCommand.DataSource = processCommandBindingSource;
            processCommandBindingSource.DataSource = null;

            SetButtonsEnabled();

#if DEBUG
            bClose.Visible = true;
#endif
        }

        public void SetCamDocument(CamDocument camDocument)
        {
            _camDocument = camDocument;

            treeView.Nodes.Clear();
            _camDocument?.TechProcessList.ForEach(CreateTechProcessNode);
            ClearCommandsView();
            SetParamsViewsVisible();
            toolStrip1.Enabled = _camDocument != null;
            SetButtonsEnabled();
        }

        private void SetParamsViewsVisible() => _techProcessParamsView.Visible = _paramsView.Visible = treeView.Nodes.Count > 0;

        private void SetButtonsEnabled() => bCreateTechOperation.Enabled = bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = 
            bSwapOuterSide.Enabled = bBuildProcessing.Enabled = bSendProgramm.Enabled = treeView.Nodes.Count > 0;

        private void CreateTechProcessNode(TechProcess techProcess)
	    {
		    var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
			var techProcessNode = new TreeNode(techProcess.Name, 0, 0, children) {Tag = techProcess};
		    treeView.Nodes.Add(techProcessNode);
		    techProcessNode.ExpandAll();
			treeView.SelectedNode = techProcessNode;
            SetParamsViewsVisible();
            bCreateTechOperation.Enabled = true;
            bRemove.Enabled = true;
        }

        public void ClearCommandsView() => processCommandBindingSource.DataSource = null;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshView();
        }

        public void RefreshView()
        {
            switch (treeView.SelectedNode.Tag)
	        {
		        case TechProcess techProcess:
					_techProcessParamsView.SetTechProcess(techProcess);
			        _techProcessParamsView.BringToFront();
                    processCommandBindingSource.DataSource = techProcess.ProcessCommands;
                    _camDocument.SelectTechProcess(techProcess);

			        break;

		        case ITechOperation techOperation:

			        if (techOperation.ProcessingArea is BorderProcessingArea && _borderProcessingAreaView == null)
			        {
				        _borderProcessingAreaView = new BorderProcessingAreaView { Dock = DockStyle.Top };
				        _paramsView.Controls.Add(_borderProcessingAreaView);
			        }

			        if (_borderProcessingAreaView != null)
			        {
				        _borderProcessingAreaView.Visible = techOperation.ProcessingArea is BorderProcessingArea;
				        if (techOperation.ProcessingArea is BorderProcessingArea)
					        _borderProcessingAreaView.SetDataSource(techOperation.ProcessingArea as BorderProcessingArea);
			        }

                    ParamsViewContainer.SetParamsView(techOperation.Type, techOperation, _paramsView);
			        _paramsView.BringToFront();
                    processCommandBindingSource.DataSource = techOperation.ProcessCommands;
                    _camDocument.SelectTechOperation(techOperation);

                    break;
            }
        }

	    private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
	    {
            switch (treeView.SelectedNode.Tag)
            {
                case TechProcess techProcess:
                    techProcess.Name = e.Label;
                    break;
                case TechOperationBase techOperation:
                    techOperation.Name = e.Label;
                    break;
            }
            treeView.LabelEdit = false;
        }

	    #region Toolbar handlers

	    private void bCreateTechProcess_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (MessageBox.Show("Создать новый техпроцесс обработки изделия?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			    CreateTechProcessNode(_camDocument.CreateTechProcess());
	    }

	    private void bCreateTechOperation_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (treeView.Nodes.Count == 0)
		    {
			    var techProcess = _camDocument.CreateTechProcess();
                _camDocument.CreateTechOperations(techProcess, ProcessingType.Sawing);
			    CreateTechProcessNode(techProcess);
		    }
		    else
		    {
			    var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
			    var techOperations = _camDocument.CreateTechOperations(CurrentTechProcess, ProcessingType.Sawing);
                if (techOperations != null)
                {
                    techProcessNode.Nodes.AddRange(Array.ConvertAll(techOperations, CreateTechOperationNode));
                    treeView.SelectedNode = techProcessNode.Nodes[techProcessNode.Nodes.Count - 1];
                }
		    }
            SetButtonsEnabled();
        }

	    private void EndEdit()
	    {
		    if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
			    treeView.SelectedNode.EndEdit(false);
	    }

	    private void bRemove_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode == null)
			    return;
		    if (MessageBox.Show($"Вы хотите удалить {(treeView.SelectedNode.Tag is TechProcess ? "техпроцесс" : "операцию")}?",
			        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
		    {
			    switch (treeView.SelectedNode.Tag)
			    {
				    case TechProcess techProcess:
                        _camDocument.DeleteTechProcess(techProcess);
					    break;
				    case TechOperationBase techOperation:
                        _camDocument.DeleteTechOperation(techOperation);
					    break;
			    }
                ClearCommandsView();
                treeView.SelectedNode.Remove();
                SetParamsViewsVisible();
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

        #endregion

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (processCommandBindingSource.Current != null)
                _camDocument.SelectProcessCommand(CurrentTechProcess, processCommandBindingSource.Current as ProcessCommand);
        }

        private void bSwapOuterSide_Click(object sender, EventArgs e)
        {
            _camDocument.SwapOuterSide(treeView.SelectedNode?.Tag as TechProcess, treeView.SelectedNode?.Tag as TechOperationBase);
            RefreshView();
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            dataGridViewCommand.EndEdit();
            _camDocument.SendProgram(CurrentTechProcess);
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = true;
            e.Node.BeginEdit();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Acad.ActiveDocument.CloseAndDiscard();
            Autodesk.AutoCAD.ApplicationServices.Application.Quit();
        }
    }
}
