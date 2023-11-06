﻿using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CAM.Core.UI;
using CAM.Operations.Sawing;

namespace CAM
{
    public partial class ProcessingView : UserControl
    {
        public List<GeneralOperationNode> Nodes => treeView.Nodes.Cast<GeneralOperationNode>().ToList();

        private Type _currentTechProcessType;
        private ITechProcess CurrentTechProcess => (treeView.SelectedNode?.Parent ?? treeView.SelectedNode)?.Tag as ITechProcess;
        private ProcessCommand CurrentProcessCommand => processCommandBindingSource.Current as ProcessCommand;

        private OperationNodeBase SelectedDocumentNode => (OperationNodeBase)treeView.SelectedNode;

        public ProcessingView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new System.Drawing.Image[] { Properties.Resources.drive, Properties.Resources.drive_download });
            RefreshToolButtonsState();
#if DEBUG
            bClose.Visible = true;
#endif
            bCreateTechOperation.DropDownItems.Add(new ToolStripMenuItem("Распиловка", null,
                new EventHandler(bCreateTechOperation_Click11)));

        }

        public void SetNodes(TreeNode[] nodes)
        {
            ClearParamsViews();
            treeView.Nodes.Clear();
            treeView.Nodes.AddRange(nodes);
            treeView.ExpandAll();

            RefreshToolButtonsState();
            toolStrip.Enabled = CamManager.Processing != null;
        }

        public void ClearView()
        {
            ClearParamsViews();
            treeView.Nodes.Clear();
            toolStrip.Enabled = false;
        }

        private TreeNode GeneralOperationNode => treeView.SelectedNode.Parent ?? treeView.SelectedNode;

        private void bCreateTechOperation_Click11(object sender, EventArgs e)
        {
            var node = new OperationNode(new SawingOperation(), "Распиловка");
            GeneralOperationNode.Nodes.Add(node);
            //GeneralOperationNode.ExpandAll();
            treeView.SelectedNode = node;
            //var techOperations = Acad.Processing.CreateTechOperation(SelectedDocumentNode.TechProcess, ((ToolStripMenuItem)sender).Text);
            //if (techOperations.Any())
            //{
            //    var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            //    var techOperationNodes = techOperations.Select(p => DocumentTreeNode.Create(SelectedDocumentNode.TechProcess, p)).ToArray();
            //    techProcessNode.Nodes.AddRange(techOperationNodes);
            //    treeView.SelectedNode = techOperationNodes.Last();
            //}
        }

        #region Views
        private void ClearParamsViews()
        {
            foreach (Control control in tabPageParams.Controls)
                control.Hide();
            ClearCommandsView();
        }

        public void ClearCommandsView() => processCommandBindingSource.DataSource = null;

        private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (CurrentProcessCommand != null)
            {
                if (CurrentProcessCommand.ToolpathObjectId.HasValue)
                {
                    Acad.Show(CurrentProcessCommand.ToolpathObjectId.Value);
                    Acad.SelectObjectIds(CurrentProcessCommand.ToolpathObjectId.Value);
                }
                Acad.RegenToolObject(CurrentTechProcess.Tool, CurrentProcessCommand.HasTool, CurrentProcessCommand.ToolLocation, CurrentTechProcess.MachineType.Value);  //Settongs.IsFrontPlaneZero
            }
        }

        public void SelectProcessCommand(Autodesk.AutoCAD.DatabaseServices.ObjectId id)
        {
            if (CurrentTechProcess.GetToolpathObjectIds().TryGetValue(id, out var index))
                processCommandBindingSource.Position = index;
        }

        private Dictionary<Type, ParamsView> _paramsViews = new Dictionary<Type, ParamsView>();

        public void RefreshParamsView()
        {
            var type = SelectedDocumentNode.Tag.GetType();
            if (!_paramsViews.TryGetValue(type, out var paramsView))
            {
                paramsView = new ParamsView(type);
                paramsView.Dock = DockStyle.Fill;
                tabPageParams.Controls.Add(paramsView);
                _paramsViews[type] = paramsView;
            }
            paramsView.BindingSource.DataSource = SelectedDocumentNode.Tag;
            paramsView.Show();
            paramsView.BringToFront();
        }
        #endregion

        #region Tree

        public bool IsToolpathVisible => !bVisibility.Checked;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //RefreshTechProcess();

            //if (SelectedDocumentNode.TechProcess.GetType() != _currentTechProcessType)
            //{
            //    _currentTechProcessType = SelectedDocumentNode.TechProcess.GetType();
            //    bCreateTechOperation.DropDownItems.Clear();
            //    if (_techOperationItems.TryGetValue(_currentTechProcessType, out var menuItems))
            //    {
            //        bCreateTechOperation.DropDownItems.Add(new ToolStripMenuItem("Все операции", null, new EventHandler(bCreateTechOperation_Click)));
            //        bCreateTechOperation.DropDownItems.Add(new ToolStripSeparator());
            //        bCreateTechOperation.DropDownItems.AddRange(menuItems);
            //    }
            //    RefreshToolButtonsState();
            //}
        }

        private void RefreshTechProcess()
        {
            RefreshParamsView();

            processCommandBindingSource.DataSource = SelectedDocumentNode.ProcessCommands;
            processCommandBindingSource.Position = SelectedDocumentNode.FirstCommandIndex;

            SelectedDocumentNode.SelectAcadObject();

            if (IsToolpathVisible)
            {
                SelectedDocumentNode.ShowToolpath();
                Acad.Editor.UpdateScreen();
            }
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = true;
            e.Node.BeginEdit();
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeView.LabelEdit = false;
            if (String.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            e.Node.Text = e.Label;
            //switch (e.Node.Tag)
            //{
            //    case ITechProcess techProcess:
            //        techProcess.Caption = e.Label;
            //        break;
            //    case TechOperation techOperation:
            //        techOperation.Caption = e.Label;
            //        break;
            //}
        }

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ((OperationNodeBase)e.Node).RefreshColor();
            //e.Node.ForeColor = e.Node.Checked ? Color.Black : Color.Gray;
            //((TechOperation)e.Node.Tag).Enabled = e.Node.Checked;
        }
        #endregion

        #region ToolButtons

        private void RefreshToolButtonsState()
        {
            // bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = bBuildProcessing.Enabled = treeView.SelectedNode != null;
            // bCreateTechOperation.Enabled = treeView.SelectedNode != null && bCreateTechOperation.DropDownItems.Count > 0;
            // bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = SelectedDocumentNode?.ProcessCommands != null;
        }

        //private void createTechProcessItem_Click(object sender, EventArgs e)
        //{
        //    var techProcess = Acad.Processing.CreateTechProcess(((ToolStripMenuItem)sender).Text);
        //    var techProcessNode = DocumentTreeNode.Create(techProcess);
        //    treeView.Nodes.Add(techProcessNode);
        //    treeView.SelectedNode = techProcessNode;
        //    RefreshToolButtonsState();
        //}

        private void bCreateTechOperation_Click(object sender, EventArgs e)
        {
            //var techOperations = Acad.Processing.CreateTechOperation(SelectedDocumentNode.TechProcess, ((ToolStripMenuItem)sender).Text);
            //if (techOperations.Any())
            //{
            //    var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            //    var techOperationNodes = techOperations.Select(p => DocumentTreeNode.Create(SelectedDocumentNode.TechProcess, p)).ToArray();
            //    techProcessNode.Nodes.AddRange(techOperationNodes);
            //    treeView.SelectedNode = techOperationNodes.Last();
            //}
        }

        private void bRemove_Click(object sender, EventArgs e) => Delete();

        private void bMoveUpTechOperation_Click(object sender, EventArgs e)
        {
            SelectedDocumentNode.MoveUp();
        }

        private void bMoveDownTechOperation_Click(object sender, EventArgs e) => SelectedDocumentNode.MoveDown();

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            SelectNextControl(ActiveControl, true, true, true, true);

            toolStrip.Enabled = false;
            //SelectedDocumentNode.BuildProcessing(sender == bPartialProcessing ? CurrentProcessCommand : null);
            toolStrip.Enabled = true;

            RefreshToolButtonsState();
            RefreshTechProcess();
        }

        private void bVisibility_Click(object sender, EventArgs e)
        {
            //SelectedDocumentNode.SetVisibility(IsToolpathVisible);
            //Acad.Editor.UpdateScreen();
        }

        private void bPlay_Click(object sender, EventArgs e)
        {
            toolStrip.Enabled = false;
            Acad.CreateProgressor("Проигрывание обработки");
            Acad.SetLimitProgressor(processCommandBindingSource.Count - processCommandBindingSource.Position);
            while (processCommandBindingSource.Position < processCommandBindingSource.Count - 1 && Acad.ReportProgressor(false))
            {
                processCommandBindingSource.MoveNext();
                System.Threading.Thread.Sleep((int)((ProcessCommand)processCommandBindingSource.Current).Duration * 10);
            }
            Acad.CloseProgressor();
            toolStrip.Enabled = true;
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            //dataGridViewCommand.EndEdit();
            //SelectedDocumentNode.SendProgram();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Autodesk.AutoCAD.ApplicationServices.DocumentExtension.CloseAndDiscard(Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.CurrentDocument);
            //Autodesk.AutoCAD.ApplicationServices.DocumentCollectionExtension.CloseAll(Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager);
            Autodesk.AutoCAD.ApplicationServices.Core.Application.Quit();
        }
        #endregion

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                Delete();
        }

        private void Delete()
        {
            if (treeView.SelectedNode != null && MessageBox.Show($"Вы хотите удалить {treeView.SelectedNode.Text}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                SelectedDocumentNode.Remove();
                
                //Acad.UnhighlightAll();
                //ClearParamsViews();
                //treeView.SelectedNode.Remove();
                //treeView.Focus();
                //RefreshToolButtonsState();
            }
        }

        private void bCreateProsessing_Click(object sender, EventArgs e)
        {
            var node = new GeneralOperationNode();
            treeView.Nodes.Add(node);
            treeView.SelectedNode = node;
            //RefreshToolButtonsState();
        }
    }
}
