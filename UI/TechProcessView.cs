using System;
using System.Drawing;
using System.Windows.Forms;
using CAM.Domain;
using System.Linq;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private CamManager _camManager;
        private SawingParamsView _techOperationParamsView = new SawingParamsView();
        private TechProcessParamsView _techProcessParamsView = new TechProcessParamsView();
        private BorderProcessingAreaView _borderProcessingAreaView = new BorderProcessingAreaView();
	    private UserControl _paramsView = new UserControl();

	    private TechProcess CurrentTechProcess => (treeView.SelectedNode.Parent ?? treeView.SelectedNode).Tag as TechProcess;
		
	    private static TreeNode CreateTechOperationNode(TechOperation techOperation) => new TreeNode(techOperation.Name, 1, 1) {Tag = techOperation };

	    public TechProcessView()
	    {
		    InitializeComponent();

		    imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download, Properties.Resources.folder__arrow, Properties.Resources.gear__arrow });

		    _techProcessParamsView.Dock = DockStyle.Fill;
		    _techOperationParamsView.Dock = DockStyle.Fill;
		    _borderProcessingAreaView.Dock = DockStyle.Top;
		    _paramsView.Dock = DockStyle.Fill;

		    tabPageParams.Controls.Add(_techProcessParamsView);
		    tabPageParams.Controls.Add(_paramsView);
		    _paramsView.Controls.Add(_techOperationParamsView);
		    _paramsView.Controls.Add(_borderProcessingAreaView);
	    }

	    public void Init(CamManager camManager)
        {
	        _camManager = camManager;
	        _camManager.TechProcessList.ForEach(CreateTechProcessNode);
		}

	    private void CreateTechProcessNode(TechProcess techProcess)
	    {
		    var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
			var techProcessNode = new TreeNode(techProcess.Name, 0, 0, children) {Tag = techProcess};
		    treeView.Nodes.Add(techProcessNode);
		    treeView.SelectedNode = techProcessNode;
	    }

	    private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
			switch (treeView.SelectedNode.Tag)
            {
                case TechProcess techProcess:
                    _techProcessParamsView.BringToFront();
                    _camManager.SelectTechProcess(techProcess);
                    break;
                case TechOperation techOperation:
	                _borderProcessingAreaView.Visible = techOperation.ProcessingArea is BorderProcessingArea;
					if (_borderProcessingAreaView.Visible)
						_borderProcessingAreaView.SetDataSource(techOperation.ProcessingArea as BorderProcessingArea);
	                _techOperationParamsView.sawingParamsBindingSource.DataSource = ((SawingTechOperation)techOperation).TechOperationParams;
	                _paramsView.BringToFront();
	                _camManager.SelectTechOperation(techOperation);
                    break;
            }
        }

	    private void bCreateTechProcess_Click(object sender, EventArgs e)
        {
            EndEdit();
	        CreateTechProcessNode(_camManager.CreateTechProcess());
        }

	    private void bCreateTechOperation_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (treeView.Nodes.Count == 0)
				CreateTechProcessNode(_camManager.CreateTechProcess());

			var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
		    var techOperations = _camManager.CreateTechOperations(CurrentTechProcess, TechOperationType.Sawing);
		    techProcessNode.Nodes.AddRange(techOperations.Select(CreateTechOperationNode).ToArray());
		    treeView.SelectedNode = techProcessNode.Nodes[techProcessNode.Nodes.Count - 1];
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
	        switch (treeView.SelectedNode.Tag)
	        {
		        case TechProcess techProcess:
					_camManager.RemoveTechProcess(techProcess);
			        break;
		        case TechOperation techOperation:
					_camManager.RemoveTechOperation(techOperation);
			        break;
	        } 
	        treeView.SelectedNode.Remove();
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

		private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
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

        private void bBuildProcessing_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count != 0)
				_camManager.BuildProcessing(CurrentTechProcess);
        }
    }
}
