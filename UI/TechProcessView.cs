using System;
using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private TechProcessService _techProcessService;
        private TechProcessNodeBuilder _techProcessNodeBuilder = new TechProcessNodeBuilder();
        private SawingParamsView _techOperationParamsView = new SawingParamsView();
        private TechProcessParamsView _techProcessParamsView = new TechProcessParamsView();

        public TechProcessView()
        {
            InitializeComponent();
            imageList.Images.Add(Properties.Resources.folder);
            imageList.Images.Add(Properties.Resources.layer_shape_line);

            splitContainer1.Panel2.Controls.Add(_techProcessParamsView);
            splitContainer1.Panel2.Controls.Add(_techOperationParamsView);
            foreach (Control control in splitContainer1.Panel2.Controls)
                control.Dock = DockStyle.Fill;
        }

        public void SetTechProcessService(TechProcessService techProcessService)
        {
            _techProcessService = techProcessService;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (treeView.SelectedTechProcessNode().Type)
            {
                case TreeNodeType.TechProcess:
                    _techProcessParamsView.BringToFront();
                    break;
                case TreeNodeType.TechOperation:
                    _techOperationParamsView.BringToFront();
                    _techOperationParamsView.sawingParamsBindingSource.DataSource = treeView.SelectedTechProcessNode().TechOperation.TechOperationParams;
                    break;
            }
        }

        private void bCreateTechProcess_Click(object sender, EventArgs e)
        {
            EndEdit();
            var techProcess = _techProcessService.CreateTechProcess();
            var techProcessNode = _techProcessNodeBuilder.CreateTechProcessNode(techProcess);
            treeView.Nodes.Add(techProcessNode);
            treeView.SelectedNode = techProcessNode;
            treeView.SelectedNode.ExpandAll();
        }

        private void bCreateTechOperation_Click(object sender, EventArgs e)
        {
            EndEdit();
            if (treeView.Nodes.Count == 0)
                bCreateTechProcess_Click(sender, e);
            else
            {
                var rootNode = treeView.SelectedNode;
                while (rootNode.Parent != null)
                    rootNode = rootNode.Parent;

                var techOperations = _techProcessService.CreateTechOperations(((TechProcessNode)rootNode).TechProcess);
                int index = 0;
                techOperations.ForEach(p => index = rootNode.Nodes.Add(_techProcessNodeBuilder.CreateTechOperationNode(p)));
                treeView.SelectedNode = rootNode.Nodes[index];
            }
        }

        private void EndEdit()
        {
            if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
                treeView.SelectedNode.EndEdit(false);
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                var node = treeView.SelectedTechProcessNode();
                switch (node.Type)
                {
                    case TreeNodeType.TechProcess:
                        _techProcessService.RemoveTechProcess(node.TechProcess);
                        break;
                    case TreeNodeType.TechOperation:
                        _techProcessService.RemoveTechOperation(node.TechOperation);
                        break;
                    case TreeNodeType.ProcessAction:
                        _techProcessService.RemoveProcessAction(node.ProcessAction);
                        break;
                    default:
                        return;
                } 
                treeView.SelectedNode.Remove();
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
            if (treeView.SelectedTechProcessNode()?.Type == TreeNodeType.TechOperation)
            {
                EndEdit();
                if (_techProcessService.MoveBackwardTechOperation(treeView.SelectedTechProcessNode().TechOperation))
                    MoveSelectedNode(-1);
            }
        }

        private void bMoveDownTechOperation_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedTechProcessNode()?.Type == TreeNodeType.TechOperation)
            {
                EndEdit();
                if (_techProcessService.MoveForwardTechOperation(treeView.SelectedTechProcessNode().TechOperation))
                    MoveSelectedNode(1);
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            switch (treeView.SelectedTechProcessNode().Type)
            {
                case TreeNodeType.TechProcess:
                    treeView.SelectedTechProcessNode().TechProcess.Name = e.Label;
                    break;
                case TreeNodeType.TechOperation:
                    treeView.SelectedTechProcessNode().TechOperation.Name = e.Label;
                    break;
            }
        }
    }
}
