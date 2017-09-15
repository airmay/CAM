using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAM.Domain;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private TechProcessService _techProcessService;

        public TechProcessView()
        {
            InitializeComponent();
        }

        public void SetTechProcessService(TechProcessService techProcessService)
        {
            _techProcessService = techProcessService;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode.Level == 0)
                _techProcessService.SetCurrentTechProcess(treeView.SelectedNode.Name);
            else
                _techProcessService.SetCurrentTechOperation(treeView.SelectedNode.Parent.Name, treeView.SelectedNode.Name);
        }

        private void CreateNodes(List<SawingTechOperation> operations)
        {
            var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            treeView.BeginUpdate();
            operations.ForEach(p => treeView.SelectedNode = techProcessNode.Nodes.Add(p.Id, p.Name));
            treeView.EndUpdate();
        }

        private void bCreateTechOperation_Click(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0)
                bCreateTechProcess_Click(sender, e);
            else
                CreateNodes(_techProcessService.CreateTechOperation());
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                _techProcessService.Remove();
                treeView.SelectedNode.Remove();
            }
        }

        private void SwapNodes(TreeNode src, TreeNode dst)
        {
            var name = src.Name;
            var text = src.Text;
            src.Name = dst.Name;
            src.Text = dst.Text;
            dst.Name = name;
            dst.Text = text;
            treeView.SelectedNode = dst;
        }

        private void bMoveUpTechOperation_Click(object sender, EventArgs e)
        {
            if (_techProcessService.MoveBackwardTechOperation())
                SwapNodes(treeView.SelectedNode, treeView.SelectedNode.PrevNode);
        }

        private void bMoveDownTechOperation_Click(object sender, EventArgs e)
        {
            if (_techProcessService.MoveForwardTechOperation())
                SwapNodes(treeView.SelectedNode, treeView.SelectedNode.NextNode);
        }

        private void bCreateTechProcess_Click(object sender, EventArgs e)
        {
            var techProcess = _techProcessService.CreateTechProcess();
            treeView.SelectedNode = treeView.Nodes.Add(techProcess.Id, techProcess.Name);
            CreateNodes(techProcess.TechOperations);
            treeView.SelectedNode.Expand();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (treeView.SelectedNode.Level == 0)
                _techProcessService.RenameTechProcess(e.Label);
            else
                _techProcessService.RenameTechOperation(e.Label);
        }
    }
}
