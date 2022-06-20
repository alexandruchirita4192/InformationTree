using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Render.WinForms.Extensions;
using MediatR;

namespace InformationTree.Forms
{
    public partial class MainForm : Form
    {
        #region Fields

        private readonly IConfigurationReader _configurationReader;
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;
        private readonly Configuration _configuration;

        #endregion Fields

        #region ctor

        public MainForm(
            IConfigurationReader configurationReader,
            IImportTreeFromXmlService importTreeFromXmlService,
            IExportTreeToXmlService exportTreeToXmlService,
            IMediator mediator,
            ICachingService cachingService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter)
        {
            _configurationReader = configurationReader;
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
            _mediator = mediator;
            _cachingService = cachingService;
            _treeNodeToTreeNodeDataAdapter = treeNodeToTreeNodeDataAdapter;
            _treeNodeDataToTreeNodeAdapter = treeNodeDataToTreeNodeAdapter;

            InitializeComponent();

            // SetStyleTo(this, Color.Black, Color.White);

            nudMilliseconds.Maximum = 999;
            nudSeconds.Maximum = 59;
            nudMinutes.Maximum = 59;
            nudHours.Maximum = 1000;
            nudCompleteProgress.Maximum = 100;
            nudFontSize.Minimum = 1;
            nudFontSize.Maximum = 24;
            nudUrgency.Minimum = 0;
            nudUrgency.Maximum = 100;
            nudShowFromUrgencyNumber.Value = 0;
            nudShowFromUrgencyNumber.Minimum = 0;
            nudShowFromUrgencyNumber.Maximum = 100;
            nudShowToUrgencyNumber.Minimum = 0;
            nudShowToUrgencyNumber.Maximum = 100;
            nudShowToUrgencyNumber.Value = 100;

            AcceptButton = btnAddTask;

            if (cbFontFamily.Items.Count == 0)
                foreach (var fontFamily in FontFamily.Families)
                    cbFontFamily.Items.Add(fontFamily.Name);

            btnResetException.Enabled = false;
            if (clbStyle.Items.Count == 0)
            {
                clbStyle.Items.Add(FontStyle.Regular);
                clbStyle.Items.Add(FontStyle.Italic);
                clbStyle.Items.Add(FontStyle.Bold);
                clbStyle.Items.Add(FontStyle.Underline);
                clbStyle.Items.Add(FontStyle.Strikeout);
            }

            var getTreeStateRequest = new GetTreeStateRequest();
            if (Task.Run(async () => await _mediator.Send(getTreeStateRequest))
            .Result is not GetTreeStateResponse getTreeStateResponse)
                return;

            (var root, var fileName) = _importTreeFromXmlService.LoadTree(getTreeStateResponse.FileName, this);
            _treeNodeDataToTreeNodeAdapter.AdaptToTreeView(root, tvTaskList, true);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = true,
                IsSafeToSave = true,
                FileInformation = new FileInformation { FileName = fileName }
            };
            setTreeStateRequest.TreeUnchangedValueChanged += (s, e) =>
            {
                File.AppendAllText("TreeUnchangedIssue.txt", $"Tree unchanged set as {e.NewValue} was called by {new StackTrace()} at {DateTime.Now}");

                lblUnchanged.InvokeWrapper(lblUnchanged => lblUnchanged.Text = (e.NewValue ? "Tree unchanged (do not save)" : "Tree changed (save)"));
            };
            Task.Run(async () =>
            {
                return await _mediator.Send(setTreeStateRequest);
            }).Wait();

            var loadedExisting = !root.IsEmptyData || root.Children.Any();
            if (loadedExisting)
            {
                var treeViewCollapseAndRefreshRequest = new TreeViewCollapseAndRefreshRequest
                {
                    TreeView = tvTaskList
                };
                var updateNodeCountRequest = new UpdateNodeCountRequest
                {
                    TreeView = tvTaskList,
                    ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                    ShowFromNumberNumericUpDown = nudShowFromNumber,
                };
                var showStartupAlertFormRequest = new ShowStartupAlertFormRequest
                {
                    TreeView = tvTaskList,
                    SearchBoxTextBox = tbSearchBox,
                };
                Task.Run(async () =>
                {
                    await _mediator.Send(treeViewCollapseAndRefreshRequest);
                    await _mediator.Send(updateNodeCountRequest);
                    await _mediator.Send(showStartupAlertFormRequest);
                }).Wait();
            }

            btnShowAll.Enabled = false;

            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, false);
            StartPosition = FormStartPosition.CenterScreen;

            _configuration = _configurationReader.GetConfiguration();

            // Add events
            var addEventsRequest = new MainFormInitializeComponentAddEventsRequest
            {
                Form = this,
                TaskListTreeView = tvTaskList,
                StyleCheckedListBox = clbStyle,
                FontFamilyComboBox = cbFontFamily,
                FontSizeNumericUpDown = nudFontSize
            };
            Task.Run(async () =>
            {
                return await _mediator.Send(addEventsRequest);
            }).Wait();

            HideComponentsBasedOnFeatures();
        }

        private void HideComponentsBasedOnFeatures()
        {
            if (_configuration == null)
                return;

            // Let the designer add it by default and remove it based on configuration if it's not enabled (helping with changes in designer)
            if (!_configuration.ApplicationFeatures.EnableExtraGraphics)
                tbTreeChange.Controls.Remove(tbGraphics);

            SetVisibleAndEnabled(encryptToolStripMenuItem, _configuration.TreeFeatures.EnableManualEncryption);
            SetVisibleAndEnabled(decryptToolStripMenuItem, _configuration.TreeFeatures.EnableManualEncryption);
        }

        private void SetVisibleAndEnabled(ToolStripItem toolStripItem, bool visibleAndEnabled)
        {
            if (toolStripItem == null)
                return;
            toolStripItem.Visible = visibleAndEnabled;
            toolStripItem.Enabled = visibleAndEnabled;
        }

        #endregion ctor

        #region Properties

        private Stopwatch _timer = new();
        private System.Timers.Timer _randomTimer = new();

        public TreeNodeData TaskListRoot
        {
            get
            {
                return _treeNodeToTreeNodeDataAdapter.AdaptTreeView(tvTaskList);
            }
        }

        #endregion Properties

        #region Methods

        #region Public methods

        public void SaveTree()
        {
            var getTreeStateRequest = new GetTreeStateRequest();
            if (Task.Run(async () => await _mediator.Send(getTreeStateRequest))
            .Result is not GetTreeStateResponse getTreeStateResponse)
                return;

            _exportTreeToXmlService.SaveTree(TaskListRoot, getTreeStateResponse.FileName);
        }

        public void ClearStyleAdded()
        {
            tvTaskList.Nodes.ClearStyleAdded();
        }

        #endregion Public methods

        #region Handlers

        private TaskListAfterSelectRequest CreateAfterSelectRequest(TreeNode selectedNode)
        {
            var taskListAfterSelectRequest = new TaskListAfterSelectRequest
            {
                TreeView = tvTaskList,
                Form = this,
                SelectedNode = selectedNode,
                TaskPercentCompleted = nudCompleteProgress.Value,
                StyleItemCheckEntered = _cachingService.Get<bool>(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered),
                TimeSpentGroupBox = gbTimeSpent,
                StyleCheckedListBox = clbStyle,
                FontFamilyComboBox = cbFontFamily,
                TaskNameTextBox = tbTaskName,
                AddedDateTextBox = tbAddedDate,
                LastChangeDateTextBox = tbLastChangeDate,
                AddedNumberTextBox = tbAddedNumber,
                UrgencyNumericUpDown = nudUrgency,
                LinkTextBox = tbLink,
                IsStartupAlertCheckBox = cbIsStartupAlert,
                CompleteProgressNumericUpDown = nudCompleteProgress,
                CategoryTextBox = tbCategory,
                DataSizeTextBox = tbDataSize,
                HoursNumericUpDown = nudHours,
                MinutesNumericUpDown = nudMinutes,
                SecondsNumericUpDown = nudSeconds,
                MillisecondsNumericUpDown = nudMilliseconds,
                FontSizeNumericUpDown = nudFontSize,
                TextColorTextBox = tbTextColor,
                BackgroundColorTextBox = tbBackgroundColor,
            };
            return taskListAfterSelectRequest;
        }

        public async void tvTaskList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            var taskListAfterSelectRequest = CreateAfterSelectRequest(e.Node);
            await _mediator.Send(taskListAfterSelectRequest);
        }

        private async void btnNoTask_Click(object sender, EventArgs e)
        {
            var request = new TreeViewNoTaskRequest
            {
                TreeView = tvTaskList,
                AfterSelectRequest = CreateAfterSelectRequest(null),
            };
            await _mediator.Send(request);
        }

        private async void btnUpdateText_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode == null)
                return;

            var updateTextClickRequest = new UpdateTextClickRequest
            {
                AfterSelectRequest = CreateAfterSelectRequest(tvTaskList.SelectedNode),
            };

            await _mediator.Send(updateTextClickRequest);
        }

        private async void btnAddTask_Click(object sender, EventArgs e)
        {
            var request = new MainFormAddTaskClickRequest
            {
                AfterSelectRequest = CreateAfterSelectRequest(tvTaskList.SelectedNode),
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber
            };
            await _mediator.Send(request);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode == null)
                return;

            var request = new TreeViewDeleteRequest
            {
                TreeView = tvTaskList,
                TaskNameText = tbTaskName.Text,
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber,
                AfterSelectRequest = CreateAfterSelectRequest(tvTaskList.SelectedNode),
            };
            await _mediator.Send(request);
        }

        private async void nudCompleteProgress_ValueChanged(object sender, EventArgs e)
        {
            var request = new MainFormCompleteProgressValueChangedRequest
            {
                PercentCompleteProgressBar = pbPercentComplete,
                CompleteProgressNumericUpDown = nudCompleteProgress,
                SelectedNode = tvTaskList.SelectedNode,
            };

            await _mediator.Send(request);
        }

        private async void btnStartCounting_Click(object sender, EventArgs e)
        {
            var request = new MainFormChangeCountingClickRequest
            {
                ChangeCountingType = ChangeCountingType.StartCounting,
                SelectedNode = tvTaskList.SelectedNode,
                Timer = _timer,
                TaskGroupBox = gbTask,
                TaskListGroupBox = gbTaskList,
            };
            await _mediator.Send(request);
        }

        private async void btnStopCounting_Click(object sender, EventArgs e)
        {
            var request = new MainFormChangeCountingClickRequest
            {
                ChangeCountingType = ChangeCountingType.StopCounting,
                SelectedNode = tvTaskList.SelectedNode,
                Timer = _timer,
                TaskGroupBox = gbTask,
                TaskListGroupBox = gbTaskList,
                HoursNumericUpDown = nudHours,
                MinutesNumericUpDown = nudMinutes,
                SecondsNumericUpDown = nudSeconds,
                MillisecondsNumericUpDown = nudMilliseconds,
            };
            await _mediator.Send(request);
        }

        private async void btnExpand_Click(object sender, EventArgs e)
        {
            var request = new TreeViewExpandOrCollapseRequest
            {
                TreeView = tvTaskList,
                ChangeType = ExpandOrCollapseChangeType.ExpandAll,
            };
            await _mediator.Send(request);
        }

        private async void btnCollapse_Click(object sender, EventArgs e)
        {
            var request = new TreeViewExpandOrCollapseRequest
            {
                TreeView = tvTaskList,
                ChangeType = ExpandOrCollapseChangeType.CollapseAll,
            };
            await _mediator.Send(request);
        }

        private async void btnCalculatePercentageFromLeafsToSelectedNode_Click(object sender, EventArgs e)
        {
            var request = new CalculatePercentageRequest
            {
                SelectedNode = tvTaskList.SelectedNode,
                Direction = CalculatePercentageDirection.FromLeafsToSelectedNode
            };
            await _mediator.Send(request);
        }

        private async void btnCalculatePercentageFromSelectedNodeToLeafs_Click(object sender, EventArgs e)
        {
            var request = new CalculatePercentageRequest
            {
                SelectedNode = tvTaskList.SelectedNode,
                Direction = CalculatePercentageDirection.FromSelectedNodeToLeafs
            };
            await _mediator.Send(request);
        }

        public async void cbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            var request = new FontFamilySelectedIndexChangedRequest
            {
                TreeView = tvTaskList,
                FontFamilyComboBox = cbFontFamily,
                FontSizeNumericUpDown = nudFontSize,
            };
            await _mediator.Send(request);
        }

        public async void nudFontSize_ValueChanged(object sender, EventArgs e)
        {
            var request = new MainFormFontSizeValueChangedRequest
            {
                SelectedTreeNode = tvTaskList.SelectedNode,
                FontSizeNumericUpDown = nudFontSize,
            };
            await _mediator.Send(request);
        }

        public async void clbStyle_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var request = new MainFormStyleItemCheckRequest
            {
                ItemCheckEventArgs = e,
                AfterSelectRequest = CreateAfterSelectRequest(tvTaskList.SelectedNode),
            };
            await _mediator.Send(request);
        }

        private async void tbTextColor_TextChanged(object sender, EventArgs e)
        {
            var request = new MainFormColorTextChangedRequest
            {
                SelectedTreeNode = tvTaskList.SelectedNode,
                ColorTextBox = tbTextColor,
                ChangeType = ColorChangeType.TextColor
            };
            await _mediator.Send(request);
        }

        private async void tbBackgroundColor_TextChanged(object sender, EventArgs e)
        {
            var request = new MainFormColorTextChangedRequest
            {
                SelectedTreeNode = tvTaskList.SelectedNode,
                ColorTextBox = tbBackgroundColor,
                ChangeType = ColorChangeType.BackColor
            };
            await _mediator.Send(request);
        }

        private async void btnToggleCompletedTasks_Click(object sender, EventArgs e)
        {
            var request = new TreeViewToggleCompletedTasksRequest
            {
                TreeView = tvTaskList
            };
            await _mediator.Send(request);
        }

        private async void btnMoveToNextUnfinished_Click(object sender, EventArgs e)
        {
            var request = new TreeViewMoveToNextUnfinishedRequest
            {
                TreeView = tvTaskList
            };
            await _mediator.Send(request);
        }

        private async void btnMoveTaskUp_Click(object sender, EventArgs e)
        {
            var request = new MainFormMoveTaskRequest
            {
                TreeView = tvTaskList,
                MoveDirection = MoveTaskDirection.Up
            };
            await _mediator.Send(request);
        }

        private async void btnMoveTaskDown_Click(object sender, EventArgs e)
        {
            var request = new MainFormMoveTaskRequest
            {
                TreeView = tvTaskList,
                MoveDirection = MoveTaskDirection.Down
            };
            await _mediator.Send(request);
        }

        private async void btnResetException_Click(object sender, EventArgs e)
        {
            var request = new MainFormResetExceptionClickRequest
            {
                ResetExceptionButton = btnResetException
            };
            await _mediator.Send(request);
        }

        private async void btnShowFromToNumberOfTask_Click(object sender, EventArgs e)
        {
            var request = new ShowTreeFilteredByRangeRequest
            {
                TreeView = tvTaskList,
                Min = (int)nudShowFromNumber.Value,
                Max = (int)nudShowUntilNumber.Value,
                FilterType = CopyNodeFilterType.FilterByAddedNumber,
                ControlsToEnableForFiltered = new List<System.ComponentModel.Component> { btnShowAll },
                ControlsToEnableForNotFiltered = new List<System.ComponentModel.Component> {
                    gbTask, gbStyleChange, gbStyleChange, gbTimeSpent, gbTaskOperations, btnMoveNode,
                    gbTreeToXML, btnShowFromToUrgencyNumber, btnShowFromToNumberOfTask, gbTaskDetails,
                    nudShowFromUrgencyNumber, nudShowToUrgencyNumber, nudShowFromNumber, nudShowUntilNumber,
                    btnMoveTaskUp, btnMoveTaskDown, btnCalculatePercentageFromLeafsToSelectedNode,
                    btnCalculatePercentageFromSelectedNodeToLeafs,
                }
            };
            await _mediator.Send(request);
        }

        private async void btnShowFromToUrgencyNumber_Click(object sender, EventArgs e)
        {
            var request = new ShowTreeFilteredByRangeRequest
            {
                TreeView = tvTaskList,
                Min = (int)nudShowFromUrgencyNumber.Value,
                Max = (int)nudShowToUrgencyNumber.Value,
                FilterType = CopyNodeFilterType.FilterByUrgency,
                ControlsToEnableForFiltered = new List<System.ComponentModel.Component> { btnShowAll },
                ControlsToEnableForNotFiltered = new List<System.ComponentModel.Component> {
                    gbTask, gbStyleChange, gbStyleChange, gbTimeSpent, gbTaskOperations, btnMoveNode,
                    gbTreeToXML, btnShowFromToUrgencyNumber, btnShowFromToNumberOfTask, gbTaskDetails,
                    nudShowFromUrgencyNumber, nudShowToUrgencyNumber, nudShowFromNumber, nudShowUntilNumber,
                    btnMoveTaskUp, btnMoveTaskDown, btnCalculatePercentageFromLeafsToSelectedNode,
                    btnCalculatePercentageFromSelectedNodeToLeafs,
                }
            };
            await _mediator.Send(request);
        }

        private async void btnShowAll_Click(object sender, EventArgs e)
        {
            var request = new ShowTreeFilteredByRangeRequest
            {
                TreeView = tvTaskList,
                Min = (int)nudShowFromUrgencyNumber.Value,
                Max = (int)nudShowToUrgencyNumber.Value,
                FilterType = CopyNodeFilterType.NoFilter,
                ControlsToEnableForFiltered = new List<System.ComponentModel.Component> { btnShowAll },
                ControlsToEnableForNotFiltered = new List<System.ComponentModel.Component> {
                    gbTask, gbStyleChange, gbStyleChange, gbTimeSpent, gbTaskOperations, btnMoveNode,
                    gbTreeToXML, btnShowFromToUrgencyNumber, btnShowFromToNumberOfTask, gbTaskDetails,
                    nudShowFromUrgencyNumber, nudShowToUrgencyNumber, nudShowFromNumber, nudShowUntilNumber,
                    btnMoveTaskUp, btnMoveTaskDown, btnCalculatePercentageFromLeafsToSelectedNode,
                    btnCalculatePercentageFromSelectedNodeToLeafs,
                }
            };
            await _mediator.Send(request);
        }

        private async void btnDoNotSave_Click(object sender, EventArgs e)
        {
            var request = new MainFormDoNotSaveClickRequest
            {
                ResetExceptionButton = btnResetException
            };
            await _mediator.Send(request);
        }

        public async void tvTaskList_MouseMove(object sender, MouseEventArgs e)
        {
            var request = new TreeViewMouseMoveRequest
            {
                TreeView = tvTaskList,
                X = e.X,
                Y = e.Y,
                Tooltip = ttTaskList
            };
            await _mediator.Send(request);
        }

        private async void tbTaskName_DoubleClick(object sender, EventArgs e)
        {
            var request = new TreeViewDoubleClickRequest
            {
                Form = this,
                TreeView = tvTaskList,
                TaskNameTextBox = tbTaskName,
            };
            await _mediator.Send(request);
        }

        public async void tvTaskList_DoubleClick(object sender, EventArgs e)
        {
            var request = new MainFormTreeViewDoubleClickRequest
            {
                ControlToSetWaitCursor = this,
                TreeView = tvTaskList,
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber,
            };
            await _mediator.Send(request);
        }

        private async void btnMakeSubTreeWithChildrenOfSelectedNode_Click(object sender, EventArgs e)
        {
            var request = new MakeSubTreeWithChildrenOfSelectedNodeRequest
            {
                UseSelectedNode = cbUseSelectedNode.Checked,
                SelectedTreeNode = tvTaskList.SelectedNode,
                LinkText = tbLink.Text,
                LinkTextBox = tbLink,
            };
            await _mediator.Send(request);
        }

        private async void btnMoveNode_Click(object sender, EventArgs e)
        {
            var request = new MoveNodeRequest
            {
                TreeView = tvTaskList
            };
            await _mediator.Send(request);
        }

        private async void btnGoToDefaultTree_Click(object sender, EventArgs e)
        {
            var request = new MainFormGoToDefaultTreeClickRequest
            {
                ControlToSetWaitCursor = this,
                TreeView = tvTaskList,
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber,
            };
            await _mediator.Send(request);
        }

        private async void tbSearchBox_DoubleClick(object sender, EventArgs e)
        {
            var request = new SearchBoxDoubleClickRequest
            {
                Form = this,
                SearchBoxTextBox = tbSearchBox,
                TreeView = tvTaskList,
            };
            await _mediator.Send(request);
        }

        private async void tbSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            var request = new SearchBoxKeyUpRequest
            {
                TreeView = tvTaskList,
                SearchBoxTextBox = tbSearchBox,
                KeyData = (int)e.KeyData,
            };
            await _mediator.Send(request);
        }

        private async void btnShowCanvasPopUp_Click(object sender, EventArgs e)
        {
            var figureLines = tvTaskList.GenerateStringGraphicsLinesFromTree();

            var request = new ShowCanvasPopUpRequest
            {
                FigureLines = figureLines
            };
            await _mediator.Send(request);
        }

        public async void tvTaskList_ControlAdded(object sender, ControlEventArgs e)
        {
            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest);
        }

        public async void tvTaskList_ControlRemoved(object sender, ControlEventArgs e)
        {
            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest);
        }

        public async void tvTaskList_FontChanged(object sender, EventArgs e)
        {
            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = false
            };
            await _mediator.Send(setTreeStateRequest);
        }

        private async void btnShowStartupAlertForm_Click(object sender, EventArgs e)
        {
            var request = new ShowStartupAlertFormRequest
            {
                TreeView = tvTaskList,
                SearchBoxTextBox = tbSearchBox,
            };
            await _mediator.Send(request);
        }

        private async void btnExecCommand_Click(object sender, EventArgs e)
        {
            if (tbCommand.Lines.Length <= 0)
                return;

            var figureLines = tbCommand.Lines;

            var request = new ShowCanvasPopUpRequest
            {
                FigureLines = figureLines
            };
            await _mediator.Send(request);
        }

        private async void btnDeleteCanvas_Click(object sender, EventArgs e)
        {
            var request = new MainFormDeleteCanvasClickRequest();
            await _mediator.Send(request);
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            var request = new MainFormGenerateClickRequest
            {
                SelectedNode = tvTaskList.SelectedNode,
                XNumericUpDown = nudX,
                YNumericUpDown = nudY,
                NumberNumericUpDown = nudNumber,
                PointsNumericUpDown = nudPoints,
                RadiusNumericUpDown = nudRadius,
                ThetaNumericUpDown = nudTheta,
                IterationsNumericUpDown = nudIterations,
                UseDefaultsCheckBox = cbUseDefaults,
                ComputeTypeNumericUpDown = nudComputeType,
                LogCheckBox = cbLog,
                CommandTextBox = tbCommand,
            };
            await _mediator.Send(request);
        }

        private async void cbUseDefaults_CheckedChanged(object sender, EventArgs e)
        {
            var request = new MainFormUseDefaultsCheckedChangedRequest
            {
                UseDefaultsCheckBox = cbUseDefaults,
                XNumericUpDown = nudX,
                YNumericUpDown = nudY,
                NumberNumericUpDown = nudNumber,
                PointsNumericUpDown = nudPoints,
                RadiusNumericUpDown = nudRadius,
                ThetaNumericUpDown = nudTheta
            };
            await _mediator.Send(request);
        }

        private async void btnGenerateFiguresAndExec_Click(object sender, EventArgs e)
        {
            var request = new MainFormGenerateFiguresAndExecClickRequest
            {
                MainFormGenerateClickRequest = new MainFormGenerateClickRequest
                {
                    SelectedNode = tvTaskList.SelectedNode,
                    XNumericUpDown = nudX,
                    YNumericUpDown = nudY,
                    NumberNumericUpDown = nudNumber,
                    PointsNumericUpDown = nudPoints,
                    RadiusNumericUpDown = nudRadius,
                    ThetaNumericUpDown = nudTheta,
                    IterationsNumericUpDown = nudIterations,
                    UseDefaultsCheckBox = cbUseDefaults,
                    ComputeTypeNumericUpDown = nudComputeType,
                    LogCheckBox = cbLog,
                    CommandTextBox = tbCommand,
                }
            };
            await _mediator.Send(request);
        }

        private async void lblUnchanged_Click(object sender, EventArgs e)
        {
            var request = new MainFormUnchangedClickRequest();
            await _mediator.Send(request);
        }

        public async void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            var request = new MainFormKeyUpRequest();
            await _mediator.Send(request);
        }

        public async void tvTaskList_KeyDown(object sender, KeyEventArgs e)
        {
            var request = new MainFormTreeViewKeyDownRequest
            {
                AfterSelectRequest = CreateAfterSelectRequest(tvTaskList.SelectedNode),
                KeyData = (int)e.KeyData,
                SearchBoxTextBox = tbSearchBox,
                Sender = sender,
                ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                ShowFromNumberNumericUpDown = nudShowFromNumber,
            };
            await _mediator.Send(request);
        }

        public async void tvTaskList_MouseClick(object sender, MouseEventArgs e)
        {
            var request = new TreeViewMouseClickRequest
            {
                IsControlPressed = _cachingService.Get<bool>(Constants.CacheKeys.IsControlKeyPressed),
                TreeView = tvTaskList,
                MouseDelta = e.Delta
            };
            await _mediator.Send(request);
        }

        private async void MainForm_DoubleClick(object sender, EventArgs e)
        {
            var request = new FormMouseDoubleClickRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }

        private async void tbTreeChange_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var request = new FormMouseDoubleClickRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }

        private async void lblChangeTreeType_Click(object sender, EventArgs e)
        {
            var request = new MainFormChangeTreeTypeClickRequest
            {
                Timer = _randomTimer
            };
            await _mediator.Send(request);
        }

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var request = new TreeViewDoubleClickRequest
            {
                Form = this,
                TreeView = tvTaskList,
                TaskNameTextBox = tbTaskName,
            };
            await _mediator.Send(request);
        }

        private async void showChildrenAsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var request = new MainFormShowChildrenAsListToolStripMenuItemClickRequest
            {
                SelectedNode = tvTaskList.SelectedNode
            };
            await _mediator.Send(request);
        }

        private async void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mouseCoordinates = tvTaskList.PointToClient(Cursor.Position);
            var treeNodeAtCoordinates = tvTaskList.GetNodeAt(mouseCoordinates);
            var treeNodeDataAtCoordinates = _treeNodeToTreeNodeDataAdapter.Adapt(treeNodeAtCoordinates);

            var data = treeNodeDataAtCoordinates.Data;
            var dataStrippedRtf = data.StripRTF();
            var isPgpEncrypted = dataStrippedRtf.IsPgpEncrypted();

            var request = new PgpEncryptDecryptDataRequest
            {
                InputDataRtf = data,
                InputDataText = dataStrippedRtf,
                FormToCenterTo = this,
                ActionType = PgpActionType.Encrypt,
                FromFile = false,
                DataIsPgpEncrypted = isPgpEncrypted,
            };
            if (await _mediator.Send(request) is PgpEncryptDecryptDataResponse pgpEncryptDecryptDataResponse)
            {
                if (pgpEncryptDecryptDataResponse.ResultRtf.IsNotEmpty())
                    treeNodeDataAtCoordinates.Data = pgpEncryptDecryptDataResponse.ResultRtf;
                else if (pgpEncryptDecryptDataResponse.ResultText.IsNotEmpty())
                    treeNodeDataAtCoordinates.Data = pgpEncryptDecryptDataResponse.ResultText;
            }
        }

        private async void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mouseCoordinates = tvTaskList.PointToClient(Cursor.Position);
            var treeNodeAtCoordinates = tvTaskList.GetNodeAt(mouseCoordinates);
            var treeNodeDataAtCoordinates = _treeNodeToTreeNodeDataAdapter.Adapt(treeNodeAtCoordinates);

            var data = treeNodeDataAtCoordinates.Data;
            var dataStrippedRtf = data.StripRTF();
            var dataIsPgpEncrypted = dataStrippedRtf.IsPgpEncrypted();

            var request = new PgpEncryptDecryptDataRequest
            {
                InputDataRtf = data,
                InputDataText = dataStrippedRtf,
                FormToCenterTo = this,
                ActionType = PgpActionType.Decrypt,
                FromFile = false,
                DataIsPgpEncrypted = dataIsPgpEncrypted,
            };
            if (await _mediator.Send(request) is PgpEncryptDecryptDataResponse pgpEncryptDecryptDataResponse)
            {
                if (pgpEncryptDecryptDataResponse.ResultRtf.IsNotEmpty())
                    treeNodeDataAtCoordinates.Data = pgpEncryptDecryptDataResponse.ResultRtf;
                else if (pgpEncryptDecryptDataResponse.ResultText.IsNotEmpty())
                    treeNodeDataAtCoordinates.Data = pgpEncryptDecryptDataResponse.ResultText;
            }
        }

        private async void btnExportToRtf_Click(object sender, EventArgs e)
        {
            var request = new MainFormExportToRtfClickRequest
            {
                SelectedNode = tvTaskList.SelectedNode,
            };
            await _mediator.Send(request);
        }

        private async void tbTask_DoubleClick(object sender, EventArgs e)
        {
            var request = new FormMouseDoubleClickRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }

        #endregion Handlers

        #endregion Methods
    }
}