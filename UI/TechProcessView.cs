using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAM.Commands;
using CAM.Domain;

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private TechProcessService _techProcessService;

        private Dictionary<string, CommandBase> _commands { get; set; }

        public TreeNodeCollection TreeNodeCollection => treeView.Nodes;

        public TechProcessView()
        {
            InitializeComponent();
        }

        public void SetTechProcessService(TechProcessService techProcessService)
        {
            _techProcessService = techProcessService;
        }

        public void RegisterCommands(params CommandBase[] commands)
        {
            _commands = commands.ToDictionary(k => k.Name);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //_commands[(string)e.ClickedItem.Tag].Execute();
            //_commands[CommandNames.SelectTechOperationCommand].Execute();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode.Level == 0)
                _techProcessService.SetCurrentTechProcess(treeView.SelectedNode.Name);
            else
                _techProcessService.SetCurrentTechOperation(treeView.SelectedNode.Parent.Name, treeView.SelectedNode.Name);

            //_commands[CommandNames.SetTechOperationCommand].Execute();
            //textBox1.Text = e.Node.Index.ToString();
            //textBox2.Text = e.Node.Name;
            //RefreshConrtols();
        }

        private void RefreshConrtols()
        {
            foreach (var item in toolStrip1.Items)
            {
                var name = (item as ToolStripButton)?.Tag as string;
                if (name != null)
                    ((ToolStripButton)item).Enabled = _commands[name].CanExecute();
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var prev = e.Node.Parent.Nodes[e.Node.Index - 1];
            var prevName = prev.Name;
            var prevText = prev.Text;
            prev.Name = e.Node.Name;
            prev.Text = e.Node.Text;
            e.Node.Name = prevName;
            e.Node.Text = prevText;

            //e.Node.Parent.Nodes[e.Node.Index - 1] = e.Node;
        }

        private void SwapElements(int index)
        {
            treeView.BeginUpdate();
            TreeNode n1 = treeView.Nodes[index];
            TreeNode n2 = treeView.Nodes[index - 1];
            treeView.Nodes.RemoveAt(index);
            treeView.Nodes.RemoveAt(index - 1);
            treeView.Nodes.Insert(index - 1, n1);
            treeView.Nodes.Insert(index, n2);
            treeView.Focus();
            treeView.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode.Remove();
            textBox3.Text = treeView.SelectedNode.Name;
            treeView.Focus();
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
            var operations = _techProcessService.CreateTechOperation();
            CreateNodes(operations);
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            _techProcessService.Remove();
            treeView.SelectedNode.Remove();
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
    }
}
