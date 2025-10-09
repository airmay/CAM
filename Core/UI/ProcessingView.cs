using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using CAM.CncWorkCenter;
using CAM.Core;

namespace CAM;

public partial class ProcessingView : UserControl
{
    public Program Program { get; private set; }

    private TreeNode SelectedNode => treeView.SelectedNode;
    private TreeNode SelectedTechProcessNode => treeView.SelectedNode?.Parent ?? treeView.SelectedNode;
    private Command SelectedCommand => (Command)processCommandBindingSource.Current;

    public ProcessingView()
    {
        InitializeComponent();

        imageList.Images.AddRange([Properties.Resources.folder, Properties.Resources.drive_download]);
        bCreateTechOperation.DropDownItems.AddRange(OperationItems.GetMenuItems(bCreateTechOperation_Click));
#if DEBUG
        bClose.Visible = true;
#endif
    }

    public void SetCamData(IProcessing[] techProcesses, Command[] commands, int? techProcessIndex)
    {
        if (techProcesses.IsNotNullOrEmpty())
        {
            var nodes = techProcesses.ConvertAll(CreateTechProcessNode);
            treeView.Nodes.AddRange(nodes);
            treeView.ExpandAll();
            treeView.SelectedNode = treeView.Nodes[0];
        }

        if (commands != null)
        {
            var programBuilder = new ProgramBuilder(commands);
            Program = programBuilder.CreateProgram(techProcesses[techProcessIndex.Value]);
            processCommandBindingSource.DataSource = commands;
        }

        toolStrip.Enabled = true;
        RefreshToolButtonsState();
    }

    public void Clear()
    {
        toolStrip.Enabled = false;
        treeView.Nodes.Clear();
        foreach (Control control in tabPageParams.Controls)
            control.Hide();
        processCommandBindingSource.DataSource = null;
        Program = null;
        Acad.ClearHighlighted();
        ToolModel.Delete();
    }

    private void RefreshToolButtonsState()
    {
        bRemove.Enabled = bMoveUp.Enabled = bMoveDown.Enabled = bBuildProcessing.Enabled = SelectedNode != null;
        bVisibility.Enabled = bSendProgramm.Enabled = bPartialProcessing.Enabled = bPlay.Enabled = Program != null;
    }

    private void bClose_Click(object sender, EventArgs e) => Acad.CloseAndDiscard();

    #region Execute

    private void bBuildProcessing_ButtonClick(object sender, EventArgs e)
    {
#if RELEASE
            toolStrip.Enabled = false;
#endif
        SelectNextControl(ActiveControl, true, true, true, true);
        processCommandBindingSource.DataSource = null;
        Acad.DocumentManager.DocumentActivationEnabled = false;
        Acad.DeleteProcessObjects();
        Acad.ClearHighlighted();
        ToolModel.Delete();

        var techProcess = FillTechProcess(SelectedTechProcessNode);
        Program = techProcess.Execute();
        if (Program != null)
        {
            UpdateTechProcessNodesText(SelectedTechProcessNode);
            processCommandBindingSource.DataSource = Program.Commands;
            RefreshToolButtonsState();

            if (Program.DwgFileCommands != null)
                Acad.Write(Program.Commands.SequenceEqual(Program.DwgFileCommands, Command.Comparer)
                    ? "Программа не изменилась"
                    : "Внимание! Программа изменена!");
        }

        Acad.DocumentManager.DocumentActivationEnabled = true;
        toolStrip.Enabled = true;
    }

    private void bPartialProcessing_Click(object sender, EventArgs e)
    {
        if (Acad.Confirm($"Сформировать программу со строки {SelectedCommand.Number}?"))
        {
            Program.Commands.Take(processCommandBindingSource.Position).SelectMany(p => new[] { p.ObjectId, p.ObjectId2 }).Delete();
            var command = (Command)processCommandBindingSource[processCommandBindingSource.Position - 1];
            Program = Program.Processing.ExecutePartial(processCommandBindingSource.Position, Program.Commands.Count, command.OperationNumber, command.ToolPosition);
            processCommandBindingSource.DataSource = Program.Commands;
            processCommandBindingSource.Position = 0;
            UpdateTechProcessNodesText(GetProgramProcessingNode());
        }
    }

    private static void UpdateTechProcessNodesText(TreeNode techProcessNode)
    {
        techProcessNode.Text = techProcessNode.Tag.As<IProcessing>().Caption;
        techProcessNode.Nodes.Cast<TreeNode>().ForAll(p => p.Text = p.Tag.As<IOperation>().Caption);
    }

    #endregion

    #region TechProcess

    public IProcessing[] GetTechProcesses() => treeView.Nodes.Cast<TreeNode>().Select(FillTechProcess).ToArray();

    private TreeNode GetProgramProcessingNode() => treeView.Nodes.Cast<TreeNode>().FirstOrDefault(p => p.Tag == Program?.Processing);

    private static IProcessing FillTechProcess(TreeNode node)
    {
        var techProcess = (IProcessing)node.Tag;
        techProcess.Caption = node.Text;
        techProcess.Operations = node.Nodes.Cast<TreeNode>()
            .Select(p =>
            {
                var operation = (IOperation)p.Tag;
                operation.Caption = p.Text;
                operation.Enabled = p.Checked;
                return operation;
            })
            .ToArray();

        return techProcess;
    }

    private void bCreateProcessing_Click(object sender, EventArgs e)
    {
        treeView.SelectedNode = AddTechProcessNode(typeof(ProcessingCnc));
        RefreshToolButtonsState();
    }

    private TreeNode AddTechProcessNode(Type techProcessType)
    {
        var processing = (IProcessing)Activator.CreateInstance(techProcessType);
        var node = CreateTechProcessNode(processing);
        treeView.Nodes.Add(node);
        return node;
    }

    private TreeNode CreateTechProcessNode(IProcessing processing)
    {
        var children = processing.Operations?.ConvertAll(CreateOperationNode) ?? [];
        return new TreeNode(processing.Caption, 0, 0, children)
        {
            Checked = true,
            NodeFont = new System.Drawing.Font(Font, FontStyle.Bold),
            Tag = processing
        };
    }

    #endregion

    #region Operation
    private void bCreateTechOperation_Click(string caption, Type operationType)
    {
        var techProcessType = operationType.BaseType.BaseType.GetGenericArguments()[0];
        var techProcessNode = SelectedTechProcessNode;
        if (techProcessNode == null || techProcessNode.Tag.GetType() != techProcessType)
            techProcessNode = AddTechProcessNode(techProcessType);
        var operation = OperationFactory.CreateOperation(caption, operationType, techProcessNode.Tag.As<IProcessing>(), SelectedNode?.Tag);

        var node = CreateOperationNode(operation);
        techProcessNode.Nodes.Add(node);
        treeView.SelectedNode = node;
        RefreshToolButtonsState();
    }

    private static TreeNode CreateOperationNode(IOperation operation)
    {
        return new TreeNode(operation.Caption, 1, 1)
        {
            Checked = operation.Enabled,
            Tag = operation
        };
    }
    #endregion

    #region Tree nodes
    #region Delete

    private void bRemove_Click(object sender, EventArgs e) => DeleteNode();

    private void treeView_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
            DeleteNode();
    }

    private void DeleteNode()
    {
        if (Acad.Confirm($"Вы хотите удалить \"{SelectedNode.Text}\"?"))
        {
            SelectedNode.Remove();
            treeView.Focus();
            RefreshToolButtonsState();
        }
    }
    #endregion

    #region Move
    private void bMoveUpTechOperation_Click(object sender, EventArgs e) => MoveNode(-1);

    private void bMoveDownTechOperation_Click(object sender, EventArgs e) => MoveNode(1);

    private void MoveNode(int direction)
    {
        var node = SelectedNode;
        var nodes = node.Parent?.Nodes ?? treeView.Nodes;
        var index = node.Index + direction;
        if (index >= 0 && index < nodes.Count)
        {
            nodes.Remove(node);
            nodes.Insert(index, node);
            treeView.SelectedNode = node;
        }
    }
    #endregion

    #region Edit

    private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        treeView.LabelEdit = true;
        e.Node.BeginEdit();
    }

    private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        treeView.LabelEdit = false;
        if (string.IsNullOrWhiteSpace(e.Label))
            e.CancelEdit = true;
    }

    private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
    {
        if (e.Node.Level == 1)
            e.Node.ForeColor = e.Node.Checked ? Color.Black : Color.Gray;
        else 
        if (!e.Node.Checked)
            e.Node.Checked = true;
    }
    #endregion
    #endregion

    #region ParamsControl
    private readonly Dictionary<Type, ParamsControl> _paramsViews = [];

    private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        var type = SelectedNode.Tag.GetType();
        if (!_paramsViews.TryGetValue(type, out var paramsView))
        {
            paramsView = new ParamsControl(type);
            paramsView.Dock = DockStyle.Fill;
            tabPageParams.Controls.Add(paramsView);
            _paramsViews[type] = paramsView;
        }

        paramsView.SetData(SelectedNode.Tag);
        paramsView.Show();
        paramsView.BringToFront();

        if (SelectedNode.Tag is IOperation operation)
        {
            if (e.Action != TreeViewAction.Unknown && Program?.TryGetCommandIndexByOperationNumber(operation.Number, out var commandIndex) == true)
                processCommandBindingSource.Position = commandIndex;

            Program?.ShowOperationToolpath(operation.Number);
            operation.ProcessingArea?.Select();
        }
    }
    #endregion

    #region Program
    private void bVisibility_Click(object sender, EventArgs e)
    {
        Program.SetToolpathVisibility(false);
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
            System.Threading.Thread.Sleep(100);
            // System.Threading.Thread.Sleep((int)((Command)processCommandBindingSource.Current).Duration * 10);
        }

        Acad.CloseProgressor();
        toolStrip.Enabled = true;
    }

    private void bSend_Click(object sender, EventArgs e)
    {
        dataGridViewCommand.EndEdit();
        Program.Export();
    }

    private void processCommandBindingSource_CurrentChanged(object sender, EventArgs e)
    {
        if (processCommandBindingSource.Current == null)
            return;

        if (SelectedCommand.ObjectId.HasValue)
        {
            Acad.Show(SelectedCommand.ObjectId.Value);
            Acad.SelectObjectIds(SelectedCommand.ObjectId.Value);
        }

        ToolModel.Set(Program.Processing.GetOperation(SelectedCommand.OperationNumber)?.GetTool(), SelectedCommand.ToolPosition);
            
        var node = GetProgramProcessingNode()?.Nodes.Cast<TreeNode>().FirstOrDefault(p => ((IOperation)p.Tag).Number == SelectedCommand.OperationNumber);
        if (node != null) 
            treeView.SelectedNode = node;
    }

    public void SelectCommand(ObjectId? objectId)
    {
        if (objectId.HasValue && Program?.TryGetCommandIndexByObjectId(objectId.Value, out var commandIndex) == true)
            processCommandBindingSource.Position = commandIndex;
    }
    #endregion
}