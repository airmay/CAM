using CAM.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
	    private CamManager _camManager;
        private readonly TechProcessParamsView _techProcessParamsView = new TechProcessParamsView { Dock = DockStyle.Fill, Visible = false };
        private readonly UserControl _paramsView = new UserControl { Dock = DockStyle.Fill, Visible = false };
	    private readonly Dictionary<Type, Control> _paramsViews = new Dictionary<Type, Control>();
	    private BorderProcessingAreaView _borderProcessingAreaView;

	    private TechProcess CurrentTechProcess => (treeView.SelectedNode?.Parent ?? treeView.SelectedNode)?.Tag as TechProcess;

	    private static TreeNode CreateTechOperationNode(TechOperation techOperation) => new TreeNode(techOperation.Name, 1, 1) {Tag = techOperation};

        public TechProcessView(CamManager camManager)
        {
            InitializeComponent();

            imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download });

            tabPageParams.Controls.Add(_techProcessParamsView);
            tabPageParams.Controls.Add(_paramsView);

            dataGridViewCommand.DataSource = processCommandBindingSource;
            processCommandBindingSource.DataSource = null;

            _camManager = camManager;
            _camManager.TechProcessView = this;

            SetButtonsEnabled();
        }

        public void Refresh(List<TechProcess> techProcessList = null)
        {
            treeView.Nodes.Clear();
            ClearCommandsView();
            techProcessList?.ForEach(CreateTechProcessNode);
            SetParamsViewsVisible();
            toolStrip1.Enabled = techProcessList != null;
            SetButtonsEnabled();
        }

        private void SetParamsViewsVisible() => _techProcessParamsView.Visible = _paramsView.Visible = treeView.Nodes.Count > 0;

        private void SetButtonsEnabled() => bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = 
            bSwapOuterSide.Enabled = bBuildProcessing.Enabled = bSendProgramm.Enabled = treeView.Nodes.Count > 0;

        private void CreateTechProcessNode(TechProcess techProcess)
	    {
		    var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
			var techProcessNode = new TreeNode(techProcess.Name, 0, 0, children) {Tag = techProcess};
		    treeView.Nodes.Add(techProcessNode);
		    techProcessNode.ExpandAll();
			treeView.SelectedNode = techProcessNode;
            SetParamsViewsVisible();
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
                    _camManager.SelectTechProcess(techProcess);

			        break;

		        case TechOperation techOperation:

			        if (techOperation.ProcessingArea is BorderProcessingArea && _borderProcessingAreaView == null)
			        {
				        _borderProcessingAreaView = new BorderProcessingAreaView { Dock = DockStyle.Top };
				        _paramsView.Controls.Add(_borderProcessingAreaView);
			        }

			        if (_borderProcessingAreaView != null)
			        {
				        _borderProcessingAreaView.Visible = techOperation.ProcessingArea is BorderProcessingArea;
				        if (_borderProcessingAreaView.Visible)
					        _borderProcessingAreaView.SetDataSource(techOperation.ProcessingArea as BorderProcessingArea);
			        }

			        switch (techOperation)
	                {
		                case SawingTechOperation sawingTechOperation:
			                var view = GetParamsView<SawingParamsView>();
			                view.SetDataSource(sawingTechOperation.TechOperationParams);

			                break;
	                }
			        _paramsView.BringToFront();
                    processCommandBindingSource.DataSource = techOperation.ProcessCommands;
                    _camManager.SelectTechOperation(techOperation);

                    break;
            }
        }

        private T GetParamsView<T>() where T : Control, new()
	    {
		    if (!_paramsViews.TryGetValue(typeof(T), out var view))
		    {
			    view = new T { Dock = DockStyle.Fill };
				_paramsViews.Add(typeof(T), view);
				_paramsView.Controls.Add(view);
		    }
            view.BringToFront();

			return (T)view;
	    }

	    private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
	    {
            e.Node.Text = e.Label;
            switch (treeView.SelectedNode.Tag)
            {
                case TechProcess techProcess:
                    techProcess.Name = e.Label;
                    break;
                case TechOperation techOperation:
                    techOperation.Name = e.Label;
                    break;
            }
        }

	    #region Toolbar handlers

	    private void bCreateTechProcess_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (MessageBox.Show("Создать новый техпроцесс обработки изделия?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			    CreateTechProcessNode(_camManager.CreateTechProcess());
	    }

	    private void bCreateTechOperation_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (treeView.Nodes.Count == 0)
		    {
			    var techProcess = _camManager.CreateTechProcess();
			    _camManager.CreateTechOperations(techProcess, TechOperationType.Sawing);
			    CreateTechProcessNode(techProcess);
		    }
		    else
		    {
			    var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
			    var techOperations = _camManager.CreateTechOperations(CurrentTechProcess, TechOperationType.Sawing);
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
					    _camManager.DeleteTechProcess(techProcess);
					    break;
				    case TechOperation techOperation:
					    _camManager.DeleteTechOperation(techOperation);
					    break;
			    }
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
		    if (treeView.SelectedNode.Tag is TechOperation techOperation)
		    {
			    EndEdit();
			    if (_camManager.MoveBackwardTechOperation(techOperation))
				    MoveSelectedNode(-1);
		    }
	    }

	    private void bMoveDownTechOperation_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode.Tag is TechOperation techOperation)
		    {
			    EndEdit();
			    if (_camManager.MoveForwardTechOperation(techOperation))
				    MoveSelectedNode(1);
		    }
	    }

	    private void bBuildProcessing_Click(object sender, EventArgs e)
	    {
	        if (CurrentTechProcess != null)
	        {
                //var view = GetParamsView<SawingParamsView>();
                treeView.Focus();
                _camManager.BuildProcessing(CurrentTechProcess);
	        }
	    }

        private void bSave_Click(object sender, EventArgs e)
        {
            //_camManager.SaveTechProsess();
        }

        #endregion

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            _camManager.SelectProcessCommand(processCommandBindingSource.Current as ProcessCommand);
        }

        private void bSwapOuterSide_Click(object sender, EventArgs e)
        {
            _camManager.SwapOuterSide(treeView.SelectedNode?.Tag as TechProcess, treeView.SelectedNode?.Tag as TechOperation);
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            _camManager.SendProgramm(CurrentTechProcess);
        }
    }
}
