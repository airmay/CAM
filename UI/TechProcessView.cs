﻿using System;
using System.Windows.Forms;
using CAM.Domain;
using System.Linq;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private CamManager _camManager;
        private TechProcessTreeBuilder _techProcessTreeBuilder = new TechProcessTreeBuilder();
        private SawingParamsView _techOperationParamsView = new SawingParamsView();
        private TechProcessParamsView _techProcessParamsView = new TechProcessParamsView();

        public TechProcessView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new[] { Properties.Resources.drive, Properties.Resources.drive_download, Properties.Resources.folder__arrow, Properties.Resources.gear__arrow });
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
            var node = treeView.SelectedTechProcessNode();
            switch (node.Type)
            {
                case TechProcessNodeType.TechProcess:
                    _techProcessParamsView.BringToFront();
                    _camManager.SelectTechProcess(node.TechProcess);
                    break;
                case TechProcessNodeType.TechOperation:
                    _techOperationParamsView.BringToFront();
                    _techOperationParamsView.sawingParamsBindingSource.DataSource = node.TechOperation.TechOperationParams;
                    _camManager.SelectTechOperation(node.TechOperation);
                    break;
            }
        }

        private void bCreateTechProcess_Click(object sender, EventArgs e)
        {
            EndEdit();
            var techProcess = _camManager.CreateTechProcess();
            var techProcessNode = _techProcessTreeBuilder.CreateTechProcessTree(techProcess);
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
                var rootNode = GetRootNode();
                var techOperations = _camManager.CreateTechOperations(((TechProcessNode)rootNode).TechProcess, TechOperationType.Sawing);
	            var index = treeView.SelectedNode.Index;
	            foreach (var techOperation in techOperations)
                    index = rootNode.Nodes.Add(new TechProcessNode(techOperation));
                treeView.SelectedNode = rootNode.Nodes[index];
            }
        }

        private TreeNode GetRootNode()
        {
            var rootNode = treeView.SelectedNode;
            while (rootNode.Parent != null)
                rootNode = rootNode.Parent;
            return rootNode;
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
                    case TechProcessNodeType.TechProcess:
                        _camManager.RemoveTechProcess(node.TechProcess);
                        break;
                    case TechProcessNodeType.TechOperation:
                        _camManager.RemoveTechOperation(node.TechOperation);
                        break;
                    case TechProcessNodeType.ProcessAction:
                        var parentNode = node.Parent as TechProcessNode;
                        var toNode = parentNode.Type == TechProcessNodeType.TechOperation ? parentNode : (TechProcessNode)parentNode.Parent;
                        _camManager.RemoveProcessAction(toNode.TechOperation, node.ProcessAction);
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
            if (treeView.SelectedTechProcessNode()?.Type == TechProcessNodeType.TechOperation)
            {
                EndEdit();
                if (_camManager.MoveBackwardTechOperation(treeView.SelectedTechProcessNode().TechOperation))
                    MoveSelectedNode(-1);
            }
        }

        private void bMoveDownTechOperation_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedTechProcessNode()?.Type == TechProcessNodeType.TechOperation)
            {
                EndEdit();
                if (_camManager.MoveForwardTechOperation(treeView.SelectedTechProcessNode().TechOperation))
                    MoveSelectedNode(1);
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            switch (treeView.SelectedTechProcessNode().Type)
            {
                case TechProcessNodeType.TechProcess:
                    treeView.SelectedTechProcessNode().TechProcess.Name = e.Label;
                    break;
                case TechProcessNodeType.TechOperation:
                    treeView.SelectedTechProcessNode().TechOperation.Name = e.Label;
                    break;
            }
        }

        private void bBuildProcessing_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
                return;
            var rootNode = GetRootNode();
            _camManager.BuildProcessing(((TechProcessNode)rootNode).TechProcess);
            _techProcessTreeBuilder.RebuildTechProcessTree(rootNode);
            rootNode.Expand();
            foreach (TreeNode node in rootNode.Nodes)
                node.Expand();
        }
    }
}
