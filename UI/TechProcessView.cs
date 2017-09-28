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
        private SawingParamsView _paramsView;
        private SawingTechOperationParams _emptyParams = new SawingTechOperationParams();

        public TechProcessView()
        {
            InitializeComponent();
            imageList.Images.Add(Properties.Resources.folder);
            imageList.Images.Add(Properties.Resources.layer_shape_line);

            _paramsView = new SawingParamsView();
            _paramsView.Dock = DockStyle.Fill;
            _paramsView.Enabled = false;
            splitContainer1.Panel2.Controls.Add(_paramsView);
        }

        public void SetTechProcessService(TechProcessService techProcessService)
        {
            _techProcessService = techProcessService;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode.Level == 0)
            {
                _techProcessService.SetCurrentTechProcess(treeView.SelectedNode.Name);
                _paramsView.Enabled = false;
                _paramsView.sawingParamsBindingSource.DataSource = _emptyParams;
            }
            else
            {
                var techOperaton = _techProcessService.SetCurrentTechOperation(treeView.SelectedNode.Parent.Name, treeView.SelectedNode.Name);
                _paramsView.Enabled = true;
                _paramsView.sawingParamsBindingSource.DataSource = techOperaton.TechOperationParams;
            }
        }

        private void CreateNodes(List<SawingTechOperation> operations)
        {
            var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            treeView.BeginUpdate();
            operations.ForEach(p => treeView.SelectedNode = techProcessNode.Nodes.Add(p.Id, p.Name, 1, 1));
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
            treeView.BeginUpdate();
            var name = src.Name;
            var text = src.Text;
            src.Name = dst.Name;
            src.Text = dst.Text;
            dst.Name = name;
            dst.Text = text;
            treeView.SelectedNode = dst;
            treeView.EndUpdate();
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
            treeView.SelectedNode = treeView.Nodes.Add(techProcess.Id, techProcess.Name, 0, 0);
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
