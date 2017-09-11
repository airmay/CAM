using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CAM.Commands;
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

            var context = new Context();
            var techProcessList = new List<TechProcess>();
            var treeNodeCollection = techProcessView.TreeNodeCollection;

            var techProcessParams = new TechProcessParams();
            var sawingLineTechOperationParamsDefault = new SawingTechOperationParams();
            var sawingArcTechOperationParamsDefault = new SawingTechOperationParams();
            var techOperationFactory = new SawingTechOperationFactory(techProcessParams, sawingLineTechOperationParamsDefault, sawingArcTechOperationParamsDefault);

            var createTechOperationCommand = new CreateTechOperationCommand(context, techOperationFactory, treeNodeCollection);
            var createTechProcessCommand = new CreateTechProcessCommand(context, techProcessList, treeNodeCollection, createTechOperationCommand);

            createTechProcessCommand.Execute(new Line(new Point3d(0, 0, 0), new Point3d(1, 0, 0)));
            createTechProcessCommand.Execute(new Arc(new Point3d(0, 0, 0), 1, 0, 1));

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
