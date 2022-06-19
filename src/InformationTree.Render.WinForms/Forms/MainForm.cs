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
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Render.WinForms;
using InformationTree.Render.WinForms.Extensions;
using MediatR;
using NLog;

namespace InformationTree.Forms
{
    public partial class MainForm : Form
    {
        #region Fields

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISoundProvider _soundProvider;
        private readonly IGraphicsFileFactory _graphicsFileRecursiveGenerator;
        private readonly ICanvasFormFactory _canvasFormFactory;
        private readonly IPopUpService _popUpService;
        private readonly IConfigurationReader _configurationReader;
        private readonly IExportNodeToRtfService _exportNodeToRtfService;
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;
        private readonly IImportExportTreeXmlService _importExportTreeXmlService;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly ITreeNodeToTreeNodeDataAdapter _treeNodeToTreeNodeDataAdapter;
        private readonly ITreeNodeDataToTreeNodeAdapter _treeNodeDataToTreeNodeAdapter;
        private readonly Configuration _configuration;

        #endregion Fields

        #region ctor

        public MainForm(
            ISoundProvider soundProvider,
            IGraphicsFileFactory graphicsFileRecursiveGenerator,
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IConfigurationReader configurationReader,
            IExportNodeToRtfService exportNodeToRtfService,
            IImportTreeFromXmlService importTreeFromXmlService,
            IExportTreeToXmlService exportTreeToXmlService,
            IImportExportTreeXmlService importExportTreeXmlService,
            IMediator mediator,
            ICachingService cachingService,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            ITreeNodeDataToTreeNodeAdapter treeNodeDataToTreeNodeAdapter)
        {
            _soundProvider = soundProvider;
            _graphicsFileRecursiveGenerator = graphicsFileRecursiveGenerator;
            _canvasFormFactory = canvasFormFactory;
            _popUpService = popUpService;
            _configurationReader = configurationReader;
            _exportNodeToRtfService = exportNodeToRtfService;
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
            _importExportTreeXmlService = importExportTreeXmlService;
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

        public async void tvTaskList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            var _clbStyle_ItemCheckEntered = _cachingService.Get<bool>(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered);

            var taskListAfterSelectRequest = new TaskListAfterSelectRequest
            {
                TreeView = tvTaskList,
                Form = this,
                SelectedNode = e.Node,
                SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(e.Node),
                TaskPercentCompleted = nudCompleteProgress.Value,
                StyleItemCheckEntered = _clbStyle_ItemCheckEntered,
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

            await _mediator.Send(taskListAfterSelectRequest);
        }

        private async void btnNoTask_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode == null)
                return;

            var request = new TreeViewNoTaskRequest
            {
                TreeView = tvTaskList,
                AfterSelectRequest = new TaskListAfterSelectRequest
                {
                    TreeView = tvTaskList,
                    Form = this,
                    SelectedNode = tvTaskList.SelectedNode,
                    SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode),
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
                },
            };
            await _mediator.Send(request);
        }

        private async void btnUpdateText_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode == null)
                return;

            var updateTextClickRequest = new UpdateTextClickRequest
            {
                AfterSelectRequest = new TaskListAfterSelectRequest
                {
                    TreeView = tvTaskList,
                    Form = this,
                    SelectedNode = tvTaskList.SelectedNode,
                    SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode),
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
                }
            };

            await _mediator.Send(updateTextClickRequest);
        }

        private async void btnAddTask_Click(object sender, EventArgs e)
        {
            var request = new MainFormAddTaskClickRequest
            {
                AfterSelectRequest = new TaskListAfterSelectRequest
                {
                    TreeView = tvTaskList,
                    Form = this,
                    SelectedNode = tvTaskList.SelectedNode,
                    SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode),
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
                },
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
                AfterSelectRequest = new TaskListAfterSelectRequest
                {
                    TreeView = tvTaskList,
                    Form = this,
                    SelectedNode = tvTaskList.SelectedNode,
                    SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode),
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
                }
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
            var request = new MainFormStartCountingClickRequest
            {
                SelectedNode = tvTaskList.SelectedNode,
                Timer = _timer,
                TaskGroupBox = gbTask,
                TaskListGroupBox = gbTaskList,
            };
            await _mediator.Send(request);
        }

        // TODO: Handler for MainFormStopCountingClickRequest
        private void btnStopCounting_Click(object sender, EventArgs e)
        {
            var node = tvTaskList.SelectedNode;
            if (node == null)
                return;
            try
            {
                _timer.Stop();
                var oldElapsedTime = long.Parse(node.Name);
                var elapsedTime = _timer.ElapsedMilliseconds;
                var totalElapsedTime = (oldElapsedTime + elapsedTime);
                node.Name = totalElapsedTime.ToString();
                _timer.Reset();

                var timeSpanTotal = TimeSpan.FromMilliseconds(totalElapsedTime);
                nudHours.Value = timeSpanTotal.Hours;
                nudMinutes.Value = timeSpanTotal.Minutes;
                nudSeconds.Value = timeSpanTotal.Seconds;
                nudMilliseconds.Value = timeSpanTotal.Milliseconds;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _popUpService.ShowError(ex.Message, "Exception caught in stop counting handler");
            }

            gbTask.Enabled = true;
            gbTaskList.Enabled = true;
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

        // TODO: Handler for MainFormStyleItemCheckRequest
        public async void clbStyle_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var _clbStyle_ItemCheckEntered = _cachingService.Get<bool>(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered);
            if (_clbStyle_ItemCheckEntered)
                return;

            try
            {
                _cachingService.Set(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered, true);

                if (tvTaskList.SelectedNode != null)
                {
                    var newFontStyle = FontStyle.Regular;

                    AppendTheCheckedFontStyleToCurrentFontStyle(e, clbStyle, ref newFontStyle, FontStyle.Italic);
                    AppendTheCheckedFontStyleToCurrentFontStyle(e, clbStyle, ref newFontStyle, FontStyle.Bold);
                    AppendTheCheckedFontStyleToCurrentFontStyle(e, clbStyle, ref newFontStyle, FontStyle.Underline);
                    AppendTheCheckedFontStyleToCurrentFontStyle(e, clbStyle, ref newFontStyle, FontStyle.Strikeout);

                    var font = tvTaskList.SelectedNode.NodeFont ?? tvTaskList.SelectedNode.Parent?.NodeFont ?? WinFormsConstants.FontDefaults.DefaultFont;
                    if (font != null)
                        tvTaskList.SelectedNode.NodeFont = new Font(font.FontFamily, font.Size, newFontStyle);

                    var backColor = tvTaskList.SelectedNode.BackColor;
                    tvTaskList.SelectedNode.BackColor = backColor != null && !backColor.IsEmpty ? backColor : Constants.Colors.DefaultBackGroundColor;
                    var foreColor = tvTaskList.SelectedNode.ForeColor;
                    tvTaskList.SelectedNode.ForeColor = foreColor != null && !foreColor.IsEmpty ? foreColor : Constants.Colors.DefaultForeGroundColor;

                    var nodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode);
                    nodeData.LastChangeDate = DateTime.Now;

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(tvTaskList.SelectedNode));

                    // on font changed is added too??
                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        TreeUnchanged = false
                    };
                    await _mediator.Send(setTreeStateRequest);
                }
            }
            finally
            {
                _cachingService.Set(Constants.CacheKeys.StyleCheckedListBox_ItemCheckEntered, false);
            }
        }

        private FontStyle AppendTheCheckedFontStyleToCurrentFontStyle(ItemCheckEventArgs e, CheckedListBox clb, ref FontStyle currentFontStyle, FontStyle checkedFontStyle)
        {
            if (e == null || clb == null)
                return currentFontStyle;

            var idx = clb.Items.IndexOf(checkedFontStyle);
            var checkedState = idx == e.Index ? e.NewValue : clb.GetItemCheckState(idx);
            if (checkedState == CheckState.Checked)
                currentFontStyle = currentFontStyle | checkedFontStyle;
            return currentFontStyle;
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

        // TODO: Handler for MainFormDoNotSaveClickRequest
        private async void btnDoNotSave_Click(object sender, EventArgs e)
        {
            var setTreeStateRequest = new SetTreeStateRequest
            {
                IsSafeToSave = false
            };
            await _mediator.Send(setTreeStateRequest);
            btnResetException.Enabled = true;
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

        // TODO: Handler for MainFormTreeViewDoubleClickRequest
        public async void tvTaskList_DoubleClick(object sender, EventArgs e)
        {
            var node = tvTaskList.SelectedNode;
            if (node != null)
            {
                var treeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(node);
                if (treeNodeData != null && treeNodeData.Link.IsNotEmpty())
                {
                    (_, var fileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(
                        TaskListRoot,
                        this,
                        tvTaskList,
                        nudShowUntilNumber,
                        nudShowFromNumber,
                        treeNodeData.Link);

                    var setTreeStateRequest = new SetTreeStateRequest
                    {
                        FileInformation = new FileInformation { FileName = fileName }
                    };
                    await _mediator.Send(setTreeStateRequest);
                }
            }
            else
            {
                var figureLines = tvTaskList.GenerateStringGraphicsLinesFromTree();

                var request = new ShowCanvasPopUpRequest
                {
                    FigureLines = figureLines
                };
                await _mediator.Send(request);
            }
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

        // TODO: Handler for MainFormGoToDefaultTreeClickRequest
        private async void btnGoToDefaultTree_Click(object sender, EventArgs e)
        {
            (_, var fileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(
                TaskListRoot,
                this,
                tvTaskList,
                nudShowUntilNumber,
                nudShowFromNumber,
                null);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                FileInformation = new FileInformation { FileName = fileName }
            };
            await _mediator.Send(setTreeStateRequest);
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

        // TODO: Handler for MainFormDeleteCanvasClickRequest
        private void btnDeleteCanvas_Click(object sender, EventArgs e)
        {
            var _canvasForm = _cachingService.Get<ICanvasForm>(Constants.CacheKeys.CanvasForm);
            if (_canvasForm != null && !_canvasForm.IsDisposed)
            {
                _canvasForm.Close();
                _canvasForm.Dispose();
                _canvasForm = null;
                _cachingService.Set(Constants.CacheKeys.CanvasForm, _canvasForm);
            }
        }

        // TODO: Handler for MainFormGenerateClickRequest
        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            var x = (double)nudX.Value;
            var y = (double)nudY.Value;
            var number = (int)nudNumber.Value;
            var points = (int)nudPoints.Value;
            var radius = (double)nudRadius.Value;
            var theta = (double)nudTheta.Value;
            var iterations = (int)nudIterations.Value;

            var cbUseDefaultsChecked = cbUseDefaults.Checked;
            var computeType = (ComputeType)nudComputeType.Value;
            var computeTypeInt = (int)computeType;

            var cbLogChecked = cbLog.Checked;
            var node = tvTaskList.SelectedNode;
            var commandData = cbUseDefaultsChecked ?
                $"{GraphicsFileConstants.GenerateFigureLines.DefaultName} {radius} {theta} {iterations} {computeTypeInt}" :
                $"{GraphicsFileConstants.GenerateFigureLines.DefaultName} {points} {x} {y} {radius} {theta} {number} {iterations} {computeTypeInt}";

            if (cbLogChecked && node != null)
            {
                var treeNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(node);
                treeNodeData.Data += commandData + Environment.NewLine;

                var setTreeStateRequest = new SetTreeStateRequest
                {
                    TreeUnchanged = false
                };
                await _mediator.Send(setTreeStateRequest);
            }

            if (cbUseDefaultsChecked)
                tbCommand.Lines = _graphicsFileRecursiveGenerator.GenerateFigureLines(radius, iterations, computeType).Distinct().ToArray();
            else
                tbCommand.Lines = _graphicsFileRecursiveGenerator.GenerateFigureLines(points, x, y, radius, theta, number, iterations, computeType).Distinct().ToArray();
        }

        // TODO: Handler for MainFormUseDefaultsCheckedChangedRequest
        private void cbUseDefaults_CheckedChanged(object sender, EventArgs e)
        {
            var cbUseDefaultsChecked = cbUseDefaults.Checked;

            nudX.ReadOnly = cbUseDefaultsChecked;
            nudY.ReadOnly = cbUseDefaultsChecked;
            nudNumber.ReadOnly = cbUseDefaultsChecked;
            nudPoints.ReadOnly = cbUseDefaultsChecked;
            nudTheta.ReadOnly = cbUseDefaultsChecked;

            if (cbUseDefaultsChecked)
            {
                nudX.Value = (decimal)_graphicsFileRecursiveGenerator.DefaultX;
                nudY.Value = (decimal)_graphicsFileRecursiveGenerator.DefaultY;
                nudNumber.Value = _graphicsFileRecursiveGenerator.DefaultNumber;
                nudPoints.Value = _graphicsFileRecursiveGenerator.DefaultPoints;
                nudTheta.Value = 0;
            }
        }

        // TODO: Handler for MainFormGenerateFiguresAndExecClickRequest
        private async void btnGenerateFiguresAndExec_Click(object sender, EventArgs e)
        {
            btnGenerate_Click(sender, e);

            if (tbCommand.Lines.Length <= 0)
                return;

            var figureLines = tbCommand.Lines;

            var request = new ShowCanvasPopUpRequest
            {
                FigureLines = figureLines
            };
            await _mediator.Send(request);
        }

        // TODO: Handler for MainFormUnchangedClickRequest
        private async void lblUnchanged_Click(object sender, EventArgs e)
        {
            if (await _mediator.Send(new GetTreeStateRequest()) is not GetTreeStateResponse getTreeStateResponse)
                return;

            var setTreeStateRequest = new SetTreeStateRequest
            {
                TreeUnchanged = !getTreeStateResponse.TreeUnchanged
            };
            await _mediator.Send(setTreeStateRequest);
        }

        // TODO: Handler for MainFormKeyUpRequest
        public void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, false);
        }

        // TODO: Handler for MainFormTreeViewKeyDownRequest
        public async void tvTaskList_KeyDown(object sender, KeyEventArgs e)
        {
            _cachingService.Set(Constants.CacheKeys.IsControlKeyPressed, e.Control);

            if (e.Control && e.KeyCode == Keys.F)
            {
                var request = new SearchBoxDoubleClickRequest
                {
                    Form = this,
                    SearchBoxTextBox = tbSearchBox,
                    TreeView = tvTaskList,
                };
                await _mediator.Send(request);
            }

            if (ReferenceEquals(sender, tvTaskList))
            {
                if (e.KeyCode == Keys.Delete)
                {
                    if (tvTaskList.SelectedNode == null)
                        return;

                    var request = new TreeViewDeleteRequest
                    {
                        TreeView = tvTaskList,
                        TaskNameText = tbTaskName.Text,
                        ShowUntilNumberNumericUpDown = nudShowUntilNumber,
                        ShowFromNumberNumericUpDown = nudShowFromNumber,
                        AfterSelectRequest = new TaskListAfterSelectRequest
                        {
                            TreeView = tvTaskList,
                            Form = this,
                            SelectedNode = tvTaskList.SelectedNode,
                            SelectedNodeData = _treeNodeToTreeNodeDataAdapter.Adapt(tvTaskList.SelectedNode),
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
                        }
                    };
                    await _mediator.Send(request);
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var request = new TreeViewDoubleClickRequest
                    {
                        Form = this,
                        TreeView = tvTaskList,
                        TaskNameTextBox = tbTaskName,
                    };
                    await _mediator.Send(request);
                }
                else if (e.KeyCode == Keys.M)
                {
                    var request = new MoveNodeRequest
                    {
                        TreeView = tvTaskList
                    };
                    await _mediator.Send(request);
                }
            }
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

        // TODO: Handler for MainFormShowChildrenAsListToolStripMenuItemClickRequest
        private void showChildrenAsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = tvTaskList.SelectedNode;
            var form = new TaskListForm(selectedItem);
            form.ShowDialog();
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

        // TODO: Handler for MainFormExportToRtfClickRequest
        private void btnExportToRtf_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode == null)
                return;

            var treeNodeData = ParseTreeRecursively(selectedNode);
            var exportedRtf = _exportNodeToRtfService.GetRtfExport(treeNodeData);

            Clipboard.SetText(exportedRtf, TextDataFormat.Rtf);
        }

        private TreeNodeData ParseTreeRecursively(TreeNode currentNode)
        {
            if (currentNode == null)
                return new TreeNodeData();

            var treeNodeData = new TreeNodeData { Text = currentNode.Text };

            if (currentNode.Nodes != null && currentNode.Nodes.Count > 0)
            {
                foreach (TreeNode child in currentNode.Nodes)
                {
                    var childTreeNodeDAta = ParseTreeRecursively(child);
                    treeNodeData.Children.Add(childTreeNodeDAta);
                }
            }

            return treeNodeData;
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