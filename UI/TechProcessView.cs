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
        //private TechProcessTreeBuilder _techProcessTreeBuilder = new TechProcessTreeBuilder();
        private SawingParamsView _techOperationParamsView = new SawingParamsView();
        private TechProcessParamsView _techProcessParamsView = new TechProcessParamsView();

        public TechProcessView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new Image[] { Properties.Resources.drive, Properties.Resources.drive_download, Properties.Resources.folder__arrow, Properties.Resources.gear__arrow });
            tabPageParams.Controls.Add(_techProcessParamsView);
            tabPageParams.Controls.Add(_techOperationParamsView);
            foreach (Control control in tabPageParams.Controls)
                control.Dock = DockStyle.Fill;
        }

        public void SetTechProcessService(CamManager camManager)
        {
            _camManager = camManager;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (treeView.SelectedNode)
            {
                case TechProcessNode techProcessNode:
                    _techProcessParamsView.BringToFront();
                    _camManager.SelectTechProcess(techProcessNode.TechProcess);
                    break;
                case TechOperationNode techOperationNode:
                    _techOperationParamsView.BringToFront();
                    _techOperationParamsView.sawingParamsBindingSource.DataSource = ((SawingTechOperation)techOperationNode.TechOperation).TechOperationParams;
                    _camManager.SelectTechOperation(techOperationNode.TechOperation);
                    break;
            }
        }

        private void bCreateTechProcess_Click(object sender, EventArgs e)
        {
            EndEdit();
            var techProcess = _camManager.CreateTechProcess();
            var techProcessNode = new TechProcessNode(techProcess);
            treeView.Nodes.Add(techProcessNode);
            treeView.SelectedNode = techProcessNode;
        }

	    private void bCreateTechOperation_Click(object sender, EventArgs e)
	    {
		    EndEdit();
		    if (treeView.Nodes.Count == 0)
			    bCreateTechProcess_Click(sender, e);

		    var techProcessNode = GetTechProcessNode();
		    var techOperations = _camManager.CreateTechOperations(techProcessNode.TechProcess, TechOperationType.Sawing);
		    var index = treeView.SelectedNode.Index;
		    foreach (var techOperation in techOperations)
			    index = techProcessNode.Nodes.Add(new TechOperationNode(techOperation));
		    treeView.SelectedNode = techProcessNode.Nodes[index];
	    }

	    private TechProcessNode GetTechProcessNode() => (treeView.SelectedNode.Parent ?? treeView.SelectedNode) as TechProcessNode;

        private void EndEdit()
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
                treeView.SelectedNode.EndEdit(false);
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
	        if (treeView.SelectedNode == null)
		        return;
	        switch (treeView.SelectedNode)
	        {
		        case TechProcessNode techProcessNode:
					_camManager.RemoveTechProcess(techProcessNode.TechProcess);
			        break;
		        case TechOperationNode techOperationNode:
					_camManager.RemoveTechOperation(techOperationNode.TechOperation);
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
			if (treeView.SelectedNode is TechOperationNode techOperationNode)
            {
                EndEdit();
                if (_camManager.MoveBackwardTechOperation(techOperationNode.TechOperation))
                    MoveSelectedNode(-1);
            }
        }

        private void bMoveDownTechOperation_Click(object sender, EventArgs e)
        {
	        if (treeView.SelectedNode is TechOperationNode techOperationNode)
            {
				EndEdit();
                if (_camManager.MoveForwardTechOperation(techOperationNode.TechOperation))
                    MoveSelectedNode(1);
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
			switch (treeView.SelectedNode)
			{
				case TechProcessNode techProcessNode:
					techProcessNode.TechProcess.Name = e.Label;
                    break;
				case TechOperationNode techOperationNode:
					techOperationNode.TechOperation.Name = e.Label;
                    break;
            }
        }

        private void bBuildProcessing_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count != 0)
				_camManager.BuildProcessing(GetTechProcessNode().TechProcess);
        }
    }
}
