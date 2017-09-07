using CAM.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAM.UI.Test
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            TreeNode[] nodes = new TreeNode[1];
            nodes[0] = new TreeNode("Изделие1", new TreeNode[2] { new TreeNode("Распил по прямой"), new TreeNode("Распил по дуге") });

            techProcessView.treeView.Nodes.AddRange(nodes);

            //var segments = new List<Segment>
            //{
            //    new Segment
            //    {
            //        Name = "Отрезок1",
            //        IsExactlyBegin = true,
            //        Depth = 30,
            //        DepthStep = 3,
            //        Speed = 50
            //    },
            //    new Segment
            //    {
            //        Name = "Дуга1",
            //        IsExactlyEnd = true,
            //        Depth = 30,
            //        DepthStep = 3,
            //        Speed = 50
            //    }
            //};
            //var products = new List<Product>
            //    {
            //    new Product
            //    {
            //        Name = "Изделие 1",
            //        Segments = segments
            //    },
            //    new Product
            //    {
            //        Name = "Изделие 2",
            //        Segments = segments.FindAll(p => p.Name == "Дуга1")
            //    }
            //};
            //productView.SetData(products);
        }
    }
}
