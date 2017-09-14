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

namespace CAM.UI
{
    public partial class TechProcessView : UserControl
    {
        private Dictionary<string, CommandBase> _commands { get; set; }

        public TreeNodeCollection TreeNodeCollection => treeView.Nodes;

        public TechProcessView()
        {
            InitializeComponent();

            bCreateTechOperation.Tag = CommandNames.CreateTechOperationCommand;
            bRemove.Tag = CommandNames.RemoveTechOperationCommand;
            bMoveUpTechOperation.Tag = CommandNames.MoveUpTechOperationCommand;
            bMoveDownTechOperation.Tag = CommandNames.MoveDownTechOperationCommand;
            bCreateTechProcess.Tag = CommandNames.CreateTechProcessCommand;
        }

        public void RegisterCommands(params CommandBase[] commands)
        {
            _commands = commands.ToDictionary(k => k.Name);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _commands[(string)e.ClickedItem.Tag].Execute();
            _commands[CommandNames.SelectTechOperationCommand].Execute();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _commands[CommandNames.SetTechOperationCommand].Execute();
            textBox1.Text = e.Node.Index.ToString();
            textBox2.Text = e.Node.Name;
            RefreshConrtols();
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
    }
}
