using Autodesk.AutoCAD.DatabaseServices;
using Dreambuild.AutoCAD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CAM
{
    public partial class CamView : UserControl
    {
	    private CamDocument _camDocument;
        private Type _currentTechProcessType;
        private ITechProcess CurrentTechProcess => (treeView.SelectedNode?.Parent ?? treeView.SelectedNode)?.Tag as ITechProcess;
        private ProcessCommand CurrentProcessCommand => processCommandBindingSource.Current as ProcessCommand;

        public CamView()
        {
            InitializeComponent();

            imageList.Images.AddRange(new System.Drawing.Image[] { Properties.Resources.drive, Properties.Resources.drive_download });
            RefreshToolButtonsState();
#if DEBUG
            bClose.Visible = true;
#endif
        }

        private Dictionary<Type, ToolStripMenuItem[]> _techOperationItems;

        public void SetCamDocument(CamDocument camDocument)
        {
            _camDocument = camDocument;
            _currentTechProcessType = null;
            if (bCreateTechProcess.DropDownItems.Count == 0)
                Init();
            ClearParamsViews();
            treeView.Nodes.Clear();
            _camDocument?.TechProcessList.ForEach(p => CreateTechProcessNode(p));
            treeView.SelectedNode = treeView.Nodes.Count > 0 ? treeView.Nodes[0] : null;
            toolStrip.Enabled = _camDocument != null;
            RefreshToolButtonsState();
        }

        private void Init()
        {
            bCreateTechProcess.DropDownItems.AddRange(_camDocument.GetTechProcessNames().Select(p =>
            {
                var item = new ToolStripMenuItem { Text = p };
                item.Click += new EventHandler(createTechProcessItem_Click);
                return item;
            })
            .ToArray());
            _techOperationItems = _camDocument.GetTechOperationNames().ToDictionary(p => p.Key, p => p.Select(v =>
            {
                var item = new ToolStripMenuItem { Text = v };
                item.Click += new EventHandler(bCreateTechOperation_Click);
                return item;
            })
            .ToArray());
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
                Acad.RegenToolObject(CurrentTechProcess.Tool, CurrentProcessCommand.HasTool, CurrentProcessCommand.ToolLocation, CurrentTechProcess.MachineType == MachineType.Donatoni);  //Settongs.IsFrontPlaneZero
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
            var dataObject = treeView.SelectedNode.Tag;
            var type = dataObject.GetType();
            if (!_paramsViews.TryGetValue(type, out var paramsView))
            {
                paramsView = new ParamsView(type);
                paramsView.Dock = DockStyle.Fill;
                tabPageParams.Controls.Add(paramsView);
                _paramsViews[type] = paramsView;
            }
            paramsView.BindingSource.DataSource = dataObject;
            paramsView.Show();
            paramsView.BringToFront();
        }
        #endregion

        #region Tree
        private TreeNode CreateTechProcessNode(ITechProcess techProcess)
        {
            var children = techProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray();
            var techProcessNode = new TreeNode(techProcess.Caption + "   ", 0, 0, children) { Tag = techProcess, Checked = true, NodeFont = new System.Drawing.Font(treeView.Font, FontStyle.Bold) };
            treeView.Nodes.Add(techProcessNode);
            techProcessNode.ExpandAll();
            RefreshToolButtonsState();

            return techProcessNode;
        }

        private static TreeNode CreateTechOperationNode(TechOperation techOperation) =>
            new TreeNode(techOperation.Caption, 1, 1) { Tag = techOperation, Checked = techOperation.Enabled, ForeColor = techOperation.Enabled ? Color.Black : Color.Gray };

        public bool IsToolpathVisible => !bVisibility.Checked;

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (CurrentTechProcess.GetType() != _currentTechProcessType)
            {
                _currentTechProcessType = CurrentTechProcess.GetType();
                bCreateTechOperation.DropDownItems.Clear();
                if (_techOperationItems.ContainsKey(_currentTechProcessType))
                    bCreateTechOperation.DropDownItems.AddRange(
                        new List<ToolStripItem> { new ToolStripMenuItem("Все операции", null, new EventHandler(bCreateTechOperation_Click)), new ToolStripSeparator() }
                        .Concat(_techOperationItems[CurrentTechProcess.GetType()]).ToArray());                
                RefreshToolButtonsState();
            }
            RefreshParamsView();

            if (IsToolpathVisible)
            {
                if (treeView.SelectedNode.Tag is MillingTechOperation oper)
                {
                    CurrentTechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(false);
                    oper.ToolpathObjectsGroup?.SetGroupVisibility(true);
                }
                else
                {
                    CurrentTechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(true);
                }
                Acad.Editor.UpdateScreen();
            }

            if (treeView.SelectedNode.Tag is MillingTechOperation techOperation)
            {
                if (techOperation.ProcessingArea != null)
                    Acad.SelectObjectIds(techOperation.ProcessingArea.ObjectId);

                if (techOperation.ProcessCommandIndex != null)
                    processCommandBindingSource.Position = techOperation.ProcessCommandIndex.Value;
            }
            else
                processCommandBindingSource.Position = 0;

            processCommandBindingSource.DataSource = CurrentTechProcess.ProcessCommands;
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
            switch (e.Node.Tag)
            {
                case ITechProcess techProcess:
                    techProcess.Caption = e.Label;
                    break;
                case MillingTechOperation techOperation:
                    techOperation.Caption = e.Label;
                    break;
            }
        }

        private void treeView_BeforeCheck(object sender, TreeViewCancelEventArgs e) => e.Cancel = e.Node.Parent == null;

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            e.Node.ForeColor = e.Node.Checked ? Color.Black : Color.Gray;
            ((MillingTechOperation)e.Node.Tag).Enabled = e.Node.Checked;
        }

        private void EndEdit()
        {
            //if (treeView.SelectedNode != null && treeView.SelectedNode.IsEditing)
            // treeView.SelectedNode.EndEdit(false);
        }
        #endregion

        #region ToolButtons

        private void RefreshToolButtonsState()
        {
            bRemove.Enabled = bMoveUpTechOperation.Enabled = bMoveDownTechOperation.Enabled = bBuildProcessing.Enabled = treeView.SelectedNode != null;
            bCreateTechOperation.Enabled = treeView.SelectedNode != null && bCreateTechOperation.DropDownItems.Count > 0;
            bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = CurrentTechProcess?.ProcessCommands != null;
        }

        private void createTechProcessItem_Click(object sender, EventArgs e)
        {
            treeView.SelectedNode = CreateTechProcessNode(_camDocument.CreateTechProcess(((ToolStripMenuItem)sender).Text));
            tabControl.SelectedTab = tabPageParams;
        }

        private void bCreateTechOperation_Click(object sender, EventArgs e)
        {
            EndEdit();
            var techOperations = _camDocument.CreateTechOperation(CurrentTechProcess, ((ToolStripMenuItem)sender).Text);
            if (techOperations.Any())
            {
                var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
                techProcessNode.Nodes.AddRange(techOperations.ConvertAll(CreateTechOperationNode).ToArray());
                treeView.SelectedNode = techProcessNode.Nodes[techProcessNode.Nodes.Count - 1];
                tabControl.SelectedTab = tabPageParams;
            }
        }

        private void bRemove_Click(object sender, EventArgs e) => Delete();

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
		    if (treeView.SelectedNode.Tag is MillingTechOperation techOperation)
		    {
			    EndEdit();
			    if (techOperation.TechProcessBase.TechOperations.SwapPrev(techOperation))
				    MoveSelectedNode(-1);
		    }
	    }

	    private void bMoveDownTechOperation_Click(object sender, EventArgs e)
	    {
		    if (treeView.SelectedNode.Tag is MillingTechOperation techOperation)
		    {
			    EndEdit();
			    if (techOperation.TechProcessBase.TechOperations.SwapNext(techOperation))
				    MoveSelectedNode(1);
		    }
	    }

        private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
        {
            var node = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            //treeView.SelectedNode = node;
            SelectNextControl(ActiveControl, true, true, true, true);

            toolStrip.Enabled = false;
            if (sender == bBuildProcessing)
                _camDocument.BuildProcessing(CurrentTechProcess);
            else
                _camDocument.PartialProcessing(CurrentTechProcess, CurrentProcessCommand);
            toolStrip.Enabled = true;

            if (node.Nodes.Count == 0)
            {
                node.Nodes.AddRange(CurrentTechProcess.TechOperations.ConvertAll(CreateTechOperationNode).ToArray());
                node.Expand();
            }

            UpdateCaptions();
            RefreshToolButtonsState();

            processCommandBindingSource.Position = 0;
            processCommandBindingSource.DataSource = CurrentTechProcess.ProcessCommands;

            RefreshParamsView();

            if (IsToolpathVisible)
            {
                CurrentTechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(true);
                Acad.Editor.UpdateScreen();
            }
        }

        private void UpdateCaptions()
        {
            var techProcessNode = treeView.SelectedNode.Parent ?? treeView.SelectedNode;
            techProcessNode.Text = ((ITechProcess)techProcessNode.Tag).Caption;
            techProcessNode.Nodes.Cast<TreeNode>().ToList().ForEach(p => p.Text = ((MillingTechOperation)p.Tag).Caption);
        }

        private void bVisibility_Click(object sender, EventArgs e)
        {
            CurrentTechProcess.GetToolpathObjectsGroup()?.SetGroupVisibility(IsToolpathVisible);
            CurrentTechProcess.GetExtraObjectsGroup()?.SetGroupVisibility(IsToolpathVisible);
            Acad.Editor.UpdateScreen();
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
            dataGridViewCommand.EndEdit();
            _camDocument.SendProgram(CurrentTechProcess);
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            //foreach (Document doc in Application.DocumentManager)
            //    if (!doc.IsActive)
            //        doc.CloseAndDiscard();

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
            if (treeView.SelectedNode == null)
                return;
            if (MessageBox.Show($"Вы хотите удалить {(treeView.SelectedNode.Tag is ITechProcess ? "техпроцесс" : "операцию")} '{treeView.SelectedNode.Text}'?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                switch (treeView.SelectedNode.Tag)
                {
                    case ITechProcess techProcess:
                        _camDocument.DeleteTechProcess(techProcess);
                        _currentTechProcessType = null;
                        break;
                    case MillingTechOperation techOperation:
                        _camDocument.DeleteTechOperation(techOperation);
                        break;
                }
                Acad.UnhighlightAll();
                ClearParamsViews();
                tabControl.SelectedTab = tabPageParams;
                treeView.SelectedNode.Remove();
                treeView.Focus();
                RefreshToolButtonsState();
            }
        }
    }
}
