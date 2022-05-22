using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Domain;
using InformationTree.Render.WinForms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.Render.WinForms.Services;
using InformationTree.TextProcessing;
using InformationTree.Tree;
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
        private readonly IPGPEncryptionAndSigningProvider _encryptionAndSigningProvider;
        private readonly ICompressionProvider _compressionProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly IExportNodeToRtfService _exportNodeToRtfService;
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;
        private readonly IImportTreeFromXmlService _importTreeFromXmlService;
        private readonly IExportTreeToXmlService _exportTreeToXmlService;
        private readonly IImportExportTreeXmlService _importExportTreeXmlService;
        private readonly IMediator _mediator;
        private readonly ITreeNodeSelectionCachingService _treeNodeSelectionCachingService;

        private readonly Configuration _configuration;

        #endregion Fields

        #region ctor

        public MainForm(
            ISoundProvider soundProvider,
            IGraphicsFileFactory graphicsFileRecursiveGenerator,
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IPGPEncryptionAndSigningProvider encryptionAndSigningProvider,
            ICompressionProvider compressionProvider,
            IConfigurationReader configurationReader,
            IExportNodeToRtfService exportNodeToRtfService,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            IImportTreeFromXmlService importTreeFromXmlService,
            IExportTreeToXmlService exportTreeToXmlService,
            IImportExportTreeXmlService importExportTreeXmlService,
            IMediator mediator,
            ITreeNodeSelectionCachingService treeNodeSelectionCachingService)
        {
            _soundProvider = soundProvider;
            _graphicsFileRecursiveGenerator = graphicsFileRecursiveGenerator;
            _canvasFormFactory = canvasFormFactory;
            _popUpService = popUpService;
            _encryptionAndSigningProvider = encryptionAndSigningProvider;
            _compressionProvider = compressionProvider;
            _configurationReader = configurationReader;
            _exportNodeToRtfService = exportNodeToRtfService;
            _treeNodeDataCachingService = treeNodeDataCachingService;
            _importTreeFromXmlService = importTreeFromXmlService;
            _exportTreeToXmlService = exportTreeToXmlService;
            _importExportTreeXmlService = importExportTreeXmlService;
            _mediator = mediator;
            _treeNodeSelectionCachingService = treeNodeSelectionCachingService;

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

            TreeNodeHelper.TreeUnchanged = true;
            TreeNodeHelper.IsSafeToSave = true;

            var beforeLoad = new Action(() => this.InvokeWrapper(f => f.Cursor = Cursors.WaitCursor));
            var afterLoad = new Action(() => this.InvokeWrapper(f => f.Cursor = Cursors.Default));
            (var root, var fileName) = _importTreeFromXmlService.LoadTree(TreeNodeHelper.FileName, beforeLoad, afterLoad);
            root.CopyToTreeView(tvTaskList, _treeNodeDataCachingService, true);
            TreeNodeHelper.FileName = fileName;

            var loadedExisting = !root.IsEmptyData || root.Children.Any();
            if (loadedExisting)
            {
                tvTaskList.CollapseAll();
                tvTaskList.Refresh();

                var countNodes = tvTaskList.GetNodeCount(true);
                TreeNodeHelper.TreeNodeCounter = countNodes;

                UpdateShowUntilNumber();
                ShowStartupAlertForm();
            }

            btnShowAll.Enabled = false;
            TreeNodeHelper.TreeUnchangedChangeDelegate = (a) =>
            {
                lblUnchanged.Text = (a ? "Tree unchanged (do not save)" : "Tree changed (save)");
            };

            _isControlPressed = false;
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

        /// <summary>
        /// Set background and foreground color for control and it's children
        /// </summary>
        private void SetStyleTo(Control ctrl, Color foreground, Color background)
        {
            if (ctrl == null)
                return;

            ctrl.ForeColor = foreground;
            ctrl.BackColor = background;

            foreach (Control child in ctrl.Controls)
                SetStyleTo(child, foreground, background);
        }

        private void StartupAlertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var form = sender as StartupAlertForm;
            if (form != null)
            {
                var selectedNode = form.SelectedItemOrCategory;
                var textToFind = selectedNode.Text;
                if (textToFind == "None")
                    return;

                var percent = 0m;
                tbSearchBox.Text = TextProcessingHelper.GetTextAndProcentCompleted(textToFind, ref percent, true);
                tbSearchBox_KeyUp(this, new KeyEventArgs(Keys.Enter));

                ////if (tvTaskList.Nodes.Contains(selectedNode))
                ////    tvTaskList.SelectedNode = selectedNode;
                ////else
                ////    ;// search whole category (do nothing for now)
            }
        }

        #endregion ctor

        #region Properties

        private bool _clbStyle_ItemCheckEntered { get; set; }

        private ICanvasForm _canvasForm;

        private Stopwatch _timer = new();
        private System.Timers.Timer _randomTimer = new();
        private static int _systemSoundNumber = -1;

        public TreeNodeData TaskListRoot
        {
            get
            {
                return tvTaskList.ToTreeNodeData(_treeNodeDataCachingService);
            }
        }

        private int _oldX = 0, _oldY = 0;

        private bool _isControlPressed;

        #endregion Properties

        #region Methods

        #region Public methods

        public void SaveTree()
        {
            var treeNodeData = tvTaskList.ToTreeNodeData(_treeNodeDataCachingService);
            _exportTreeToXmlService.SaveTree(treeNodeData, TreeNodeHelper.FileName);
        }

        public void ClearStyleAdded()
        {
            TreeNodeHelper.ClearStyleAdded(tvTaskList.Nodes);
        }

        public void ChangeResetExceptionButton(string message)
        {
            if (btnResetException == null)
                return;

            if (!TreeNodeHelper.IsSafeToSave || !String.IsNullOrEmpty(message))
            {
                btnResetException.Enabled = true;
                btnResetException.BackColor = Constants.Colors.DefaultBackGroundColor;
                btnResetException.ForeColor = Constants.Colors.ExceptionColor;
                btnResetException.Text = "Reset exception: " + message.Substring(0, 10);
            }
            else
            {
                btnResetException.Enabled = false;
                btnResetException.ForeColor = Constants.Colors.DefaultForeGroundColor;
                btnResetException.BackColor = Constants.Colors.DefaultBackGroundColor;
                btnResetException.Text = "Reset exception";
            }
        }

        #endregion Public methods

        #region Private methods

        private void UpdateShowUntilNumber()
        {
            var countNodes = tvTaskList.GetNodeCount(true);
            TreeNodeHelper.TreeNodeCounter = countNodes;
            nudShowUntilNumber.Minimum = 0;
            nudShowUntilNumber.Maximum = countNodes;
            nudShowUntilNumber.Value = countNodes;
            nudShowFromNumber.Minimum = 0;
            nudShowFromNumber.Maximum = countNodes;
            nudShowFromNumber.Value = 0;
        }

        private int ParseToDelete(TreeNode selectedTask, string taskName, bool fakeDelete)
        {
            var deletedItemsWithName = 0;

            if (selectedTask != null && selectedTask.Text.Equals(taskName /*StartsWith + " [" */))
                deletedItemsWithName = TreeNodeHelper.ParseToDelete(tvTaskList, selectedTask, taskName, fakeDelete);
            else
                foreach (TreeNode node in tvTaskList.Nodes)
                {
                    if (node != null && node.Text.Equals(taskName /*StartsWith + " [" */))
                    {
                        node.Nodes.Clear();
                        tvTaskList.Nodes.Remove(node);
                    }
                    if (node.Nodes.Count > 0)
                        deletedItemsWithName = TreeNodeHelper.ParseToDelete(tvTaskList, node, taskName, fakeDelete);
                }

            return deletedItemsWithName;
        }

        #endregion Private methods

        #region Handlers

        public async void tvTaskList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            var taskListAfterSelectRequest = new TaskListAfterSelectRequest
            {
                TaskListTreeView = tvTaskList,
                Form = this,
                SelectedNode = e.Node,
                SelectedNodeData = e.Node
                    .ToTreeNodeData(_treeNodeDataCachingService),
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

        private void btnNoTask_Click(object sender, EventArgs e)
        {
            tvTaskList.SelectedNode = null;
            tvTaskList_AfterSelect(
                sender,
                new TreeViewEventArgs(
                    new TreeNode(null)
                    {
                        ToolTipText = null,
                        Tag = new TreeNodeData(null)
                    })
            );
            //gbTimeSpent.Enabled = false;
            //tbAddedDate.Text = "-";
            //tbAddedNumber.Text = "-";
            //tbTaskName.Text = string.Empty;
            //nudHours.Value = 0;
            //nudMinutes.Value = 0;
            //nudSeconds.Value = 0;
            //nudMilliseconds.Value = 0;
            //nudUrgency.Value = 0;
        }

        private async void btnUpdateText_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode == null)
                return;

            var taskPercentCompleted = nudCompleteProgress.Value;
            var updateTextClickRequest = new UpdateTextClickRequest
            {
                SelectedNode = tvTaskList.SelectedNode
                    .ToTreeNodeData(_treeNodeDataCachingService),
                TaskPercentCompleted = taskPercentCompleted,
                TaskName = TextProcessingHelper.GetTextAndProcentCompleted(tbTaskName.Text, ref taskPercentCompleted, true),
                Link = tbLink.Text,
                Urgency = (int)nudUrgency.Value,
                Category = tbCategory.Text,
                IsStartupAlert = cbIsStartupAlert.Checked,
                TaskListTreeView = tvTaskList,
                AfterSelectRequest = new TaskListAfterSelectRequest
                {
                    TaskListTreeView = tvTaskList,
                    Form = this,
                    SelectedNode = tvTaskList.SelectedNode,
                    SelectedNodeData = tvTaskList.SelectedNode
                        .ToTreeNodeData(_treeNodeDataCachingService),
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
                }
            };

            await _mediator.Send(updateTextClickRequest);
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var taskPercentCompleted = nudCompleteProgress.Value;
            var taskName = TextProcessingHelper.GetTextAndProcentCompleted(tbTaskName.Text, ref taskPercentCompleted, true);
            var urgency = (int)nudUrgency.Value;
            var link = tbLink.Text;

            // update part
            var selectedNode = tvTaskList.SelectedNode;
            var selectedNodeLastChildren = selectedNode != null && selectedNode.Nodes.Count > 0 ? selectedNode.Nodes[selectedNode.Nodes.Count - 1] : null;
            if (selectedNode != null && selectedNode.Text.Equals(taskName /*StartsWith + " [" */))
            {
                if (selectedNode.Text != taskName)
                {
                    selectedNode.Text = taskName;
                    var tagData = selectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                    tagData.LastChangeDate = DateTime.Now;

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                    TreeNodeHelper.TreeUnchanged = false;
                }
            }
            else if (selectedNodeLastChildren != null && selectedNodeLastChildren.Text.Equals(taskName /* StartsWith + " [" */))
            {
                if (selectedNodeLastChildren.Text != taskName)
                {
                    selectedNodeLastChildren.Text = taskName;

                    var tagData = selectedNodeLastChildren.ToTreeNodeData(_treeNodeDataCachingService);
                    tagData.LastChangeDate = DateTime.Now;

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                    TreeNodeHelper.TreeUnchanged = false;
                }
            }
            else //insert
            {
                var node = new TreeNode(taskName)
                {
                    Name = 0.ToString(),
                    ForeColor = Constants.Colors.DefaultForeGroundColor,
                    BackColor = Constants.Colors.DefaultBackGroundColor,
                    NodeFont = WinFormsConstants.FontDefaults.DefaultFont.Clone() as Font,
                    ToolTipText = TextProcessingHelper.GetToolTipText(taskName)
                };
                var treeNodeData = node.ToTreeNodeData(_treeNodeDataCachingService);
                treeNodeData.AddedNumber = tvTaskList.GetNodeCount(true) + 1;
                treeNodeData.Urgency = urgency;
                treeNodeData.Link = link;
                treeNodeData.IsStartupAlert = false;
                treeNodeData.PercentCompleted = taskPercentCompleted;

                node.Text = taskName;

                if (tvTaskList.SelectedNode == null)
                    tvTaskList.Nodes.Add(node);
                else
                {
                    tvTaskList.SelectedNode.Nodes.Add(node);
                    tvTaskList.SelectedNode.Expand();
                }

                tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                //TreeNodeHelper.TreeUnchanged = false; // on control add it is added too
            }

            UpdateShowUntilNumber();

            // TODO: Fix properly
            btnUpdateText_Click(sender, e); // workaround fix for some weirdly added spaces
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedTask = tvTaskList.SelectedNode;
            var taskName = tbTaskName.Text;
            var deletedItemsWithName = ParseToDelete(selectedTask, taskName, true);

            if (deletedItemsWithName != 0)
            {
                var itemOrItems = deletedItemsWithName == 1 ? " item" : " items";
                var messageBoxText = $"Delete {deletedItemsWithName} {itemOrItems} with name {taskName}?";
                var messageBoxCaption = $"Delete";

                var result = _popUpService.ShowQuestion(messageBoxText, messageBoxCaption);
                if (result == PopUpResult.Yes)
                {
                    ParseToDelete(selectedTask, taskName, false);

                    btnNoTask_Click(this, EventArgs.Empty);
                    UpdateShowUntilNumber();
                }
            }

            //TreeNodeHelper.TreeUnchanged = false; // on control delete is added too
        }

        private void nudCompleteProgress_ValueChanged(object sender, EventArgs e)
        {
            pbPercentComplete.Maximum = 100;
            pbPercentComplete.Value = (int)nudCompleteProgress.Value;
        }

        private void btnStartCounting_Click(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null)
            {
                _timer.Start();
                gbTask.Enabled = false;
                gbTaskList.Enabled = false;
            }
        }

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

        private void btnExpand_Click(object sender, EventArgs e)
        {
            tvTaskList.ExpandAll();
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            tvTaskList.CollapseAll();
        }

        private void btnCalculatePercentage_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;

            if (selectedNode != null)
            {
                var percentage = (decimal)TreeNodeHelper.GetPercentageFromChildren(selectedNode);
                selectedNode.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(selectedNode.Text, ref percentage, true);

                TreeNodeHelper.TreeUnchanged = false;
            }
        }

        private void btnCalculatePercentage2_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;

            if (selectedNode != null)
            {
                var percentage = 0.0M;
                TextProcessingHelper.GetTextAndProcentCompleted(selectedNode.Text, ref percentage, true);
                TreeNodeHelper.SetPercentageToChildren(selectedNode, (double)percentage);

                TreeNodeHelper.TreeUnchanged = false;
            }
        }

        public void cbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null && cbFontFamily.SelectedItem != null)
            {
                var oldFont = tvTaskList.SelectedNode.NodeFont;

                if (cbFontFamily.SelectedItem is string fontFamily)
                    tvTaskList.SelectedNode.NodeFont = new Font(fontFamily, (float)nudFontSize.Value, oldFont.Style);

                TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
            }
        }

        public void nudFontSize_ValueChanged(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null)
            {
                var oldFont = tvTaskList.SelectedNode.NodeFont;
                if (oldFont != null)
                {
                    tvTaskList.SelectedNode.NodeFont = new Font(oldFont.FontFamily, (float)nudFontSize.Value, oldFont.Style);

                    TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
                }
            }
        }

        public void clbStyle_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_clbStyle_ItemCheckEntered)
                return;

            try
            {
                _clbStyle_ItemCheckEntered = true;

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

                    var nodeData = tvTaskList.SelectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                    nodeData.LastChangeDate = DateTime.Now;

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(tvTaskList.SelectedNode));

                    TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
                }
            }
            finally
            {
                _clbStyle_ItemCheckEntered = false;
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

        private void tbTextColor_TextChanged(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null)
            {
                tvTaskList.SelectedNode.ForeColor = Color.FromName(tbTextColor.Text);

                TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
            }
        }

        private void tbBackgroundColor_TextChanged(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null)
            {
                tvTaskList.SelectedNode.BackColor = Color.FromName(tbBackgroundColor.Text);

                TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
            }
        }

        private void btnToggleCompletedTasks_Click(object sender, EventArgs e)
        {
            var completedTasksAreHidden = TreeNodeHelper.TasksCompleteAreHidden(tvTaskList);
            if (!completedTasksAreHidden.HasValue)
                completedTasksAreHidden = false;
            TreeNodeHelper.ToggleCompletedTasks(tvTaskList, !completedTasksAreHidden.Value, tvTaskList.Nodes);
        }

        private void btnMoveToNextUnfinished_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (tvTaskList.Nodes.Count > 0)
            {
                if (selectedNode == null && tvTaskList.Nodes.Count > 0)
                    selectedNode = tvTaskList.Nodes[0];

                TreeNodeHelper.MoveToNextUnfinishedNode(tvTaskList, selectedNode);
            }
        }

        private void btnMoveTaskUp_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null && selectedNode.Parent != null && selectedNode.Parent.Nodes != null)
            {
                var parent = selectedNode.Parent;
                var selectedIndex = parent.Nodes.IndexOf(selectedNode);
                if (selectedIndex > 0)
                {
                    parent.Nodes.RemoveAt(selectedIndex);
                    parent.Nodes.Insert(selectedIndex - 1, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task up! Current index is {selectedIndex}.");
            }
            else if (selectedNode != null && selectedNode.Parent == null)
            {
                var selectedIndex = tvTaskList.Nodes.IndexOf(selectedNode);
                if (selectedIndex > 0)
                {
                    tvTaskList.Nodes.RemoveAt(selectedIndex);
                    tvTaskList.Nodes.Insert(selectedIndex - 1, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task up! Current index is {selectedIndex}.");
            }

            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnMoveTaskDown_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null && selectedNode.Parent != null && selectedNode.Parent.Nodes != null)
            {
                var parent = selectedNode.Parent;
                var selectedIndex = parent.Nodes.IndexOf(selectedNode);
                var count = parent.Nodes.Count;
                if (selectedIndex >= 0 && selectedIndex < count)
                {
                    parent.Nodes.RemoveAt(selectedIndex);
                    parent.Nodes.Insert(selectedIndex + 1, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task down! Current index is {selectedIndex}.");
            }
            else if (selectedNode != null && selectedNode.Parent == null)
            {
                var selectedIndex = tvTaskList.Nodes.IndexOf(selectedNode);
                var count = tvTaskList.Nodes.Count;
                if (selectedIndex >= 0 && selectedIndex < count)
                {
                    tvTaskList.Nodes.RemoveAt(selectedIndex);
                    tvTaskList.Nodes.Insert(selectedIndex + 1, selectedNode);
                    tvTaskList.SelectedNode = selectedNode;
                }
                else
                    _popUpService.ShowMessage($"Cannot move task down! Current index is {selectedIndex}.");
            }

            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnResetException_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.IsSafeToSave = true;
            ChangeResetExceptionButton(null);
        }

        private void btnShowFromToNumberOfTask_Click(object sender, EventArgs e)
        {
            var addedNumberLowerThan = nudShowUntilNumber.Value;
            var addedNumberHigherThan = nudShowFromNumber.Value;

            TreeNodeHelper.ShowNodesFromTaskToNumberOfTask(tvTaskList, addedNumberLowerThan, addedNumberHigherThan, 0, _treeNodeDataCachingService);
            btnShowAll.Enabled = true;

            gbTask.Enabled = false;
            gbStyleChange.Enabled = false;
            gbTimeSpent.Enabled = false;
        }

        private void btnShowFromToUrgencyNumber_Click(object sender, EventArgs e)
        {
            var addedNumberLowerThan = nudShowToUrgencyNumber.Value;
            var addedNumberHigherThan = nudShowFromUrgencyNumber.Value;

            TreeNodeHelper.ShowNodesFromTaskToNumberOfTask(tvTaskList, addedNumberLowerThan, addedNumberHigherThan, 1, _treeNodeDataCachingService);
            btnShowAll.Enabled = true;

            gbTask.Enabled = false;
            gbStyleChange.Enabled = false;
            gbTimeSpent.Enabled = false;
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            if (TreeNodeHelper.ReadOnlyState)
            {
                TreeNodeHelper.ShowAllTasks(tvTaskList);
                gbTask.Enabled = true;
                gbStyleChange.Enabled = true;
                gbTimeSpent.Enabled = true;
            }
        }

        private void btnDoNotSave_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.IsSafeToSave = false;
            btnResetException.Enabled = true;
        }

        public void tvTaskList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X == _oldX && e.Y == _oldY)
                return;

            // Get the node at the current mouse pointer location.
            TreeNode theNode = this.tvTaskList.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if (theNode != null)
            {
                // Change the ToolTip only if the pointer moved to a new node.
                if (theNode.ToolTipText != this.toolTip1.GetToolTip(this.tvTaskList))
                {
                    toolTip1.SetToolTip(this.tvTaskList, theNode.ToolTipText);
                }
            }
            else // Pointer is not over a node so clear the ToolTip.
            {
                toolTip1.SetToolTip(this.tvTaskList, "");
            }

            _oldX = e.X;
            _oldY = e.Y;
        }

        private void tbTaskName_DoubleClick(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null)
            {
                var tagData = selectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                var data = tagData.Data ?? string.Empty;

                var form = new PopUpEditForm(
                    _canvasFormFactory,
                    _popUpService,
                    _encryptionAndSigningProvider,
                    _configurationReader,
                    selectedNode.Text,
                    data);

                WinFormsApplication.CenterForm(form, this);

                form.FormClosing += (s, ev) =>
                {
                    var d = form.Data;

                    if (selectedNode != null)
                    {
                        var td = selectedNode.ToTreeNodeData(_treeNodeDataCachingService);
                        td.Data = d;

                        var strippedData = RicherTextBox.Controls.RicherTextBox.StripRTF(d);
                        selectedNode.ToolTipText = TextProcessingHelper.GetToolTipText(selectedNode.Text +
                            (selectedNode.Name.IsNotEmpty() && selectedNode.Name != "0" ? $"{Environment.NewLine} TimeSpent: {selectedNode.Name}" : "") +
                            (strippedData.IsNotEmpty() ? $"{Environment.NewLine} Data: {strippedData}" : ""));

                        tbTaskName.BackColor = tagData.GetTaskNameColor();

                        TreeNodeHelper.TreeUnchanged = false;
                    }
                };

                form.ShowDialog();
            }
        }

        public void tvTaskList_DoubleClick(object sender, EventArgs e)
        {
            var node = tvTaskList.SelectedNode;
            if (node != null)
            {
                var tagData = node.ToTreeNodeData(_treeNodeDataCachingService);
                if (tagData != null && !string.IsNullOrEmpty(tagData.Link))
                {
                    var afterSaveDoWithFileName = new Action<string>((fileName) => TreeNodeHelper.FileName = fileName);
                    var beforeLoadInside = new Action(() => this.InvokeWrapper(t => t.Cursor = Cursors.WaitCursor));
                    var afterLoadInside = new Action(() => this.InvokeWrapper(t => t.Cursor = Cursors.Default));
                    var afterLoad = new Action(() =>
                    {
                        tvTaskList.InvokeWrapper(tv => { tv.CollapseAll(); tv.Refresh(); });
                        TreeNodeHelper.IsSafeToSave = true;
                        UpdateShowUntilNumber();
                    });
                    var currentRoot = tvTaskList.ToTreeNodeData(_treeNodeDataCachingService);

                    (_, TreeNodeHelper.FileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(afterSaveDoWithFileName, currentRoot, tagData.Link, beforeLoadInside, afterLoadInside, afterLoad);
                }
            }
            else if (_configuration.ApplicationFeatures.EnableExtraGraphics)
            {
                var figureLines = TreeNodeHelper.GenerateStringGraphicsLinesFromTree(tvTaskList);

                if (_canvasForm == null || _canvasForm.IsDisposed)
                    _canvasForm = _canvasFormFactory.Create(figureLines);
                else
                {
                    _canvasForm.RunTimer.Enabled = false;
                    try
                    {
                        _canvasForm.GraphicsFile.Clean();
                        _canvasForm.GraphicsFile.ParseLines(figureLines);
                    }
                    finally
                    {
                        _canvasForm.RunTimer.Enabled = true;
                    }
                }
                _canvasForm.Show();
            }
        }

        private void btnMakeSubTreeWithChildrenOfSelectedNode_Click(object sender, EventArgs e)
        {
            var useSelectedNode = cbUseSelectedNode.Checked;
            var node = tvTaskList.SelectedNode;
            if (node != null)
            {
                if (node.Nodes.Count == 0 && !useSelectedNode)
                    return;

                var tagData = node.ToTreeNodeData(_treeNodeDataCachingService);
                tagData.Link = tbLink.Text;

                var percentCompleted = 0M;
                if (tagData.Link.IsEmpty() || !tagData.Link.EndsWith(".xml") || tagData.Link.Contains(" "))
                    tagData.Link = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref percentCompleted, true).Replace(" ", "_") + ".xml";
                tbLink.Text = tagData.Link;

                var treeView = new TreeView();
                if (useSelectedNode)
                {
                    var parentNode = new TreeNode();
                    TreeNodeHelper.CopyNode(parentNode, node, _treeNodeDataCachingService);
                    treeView.Nodes.Add(parentNode);
                    tagData.Data = null;
                }
                else
                {
                    TreeNodeHelper.CopyNodes(treeView.Nodes, node.Nodes, _treeNodeDataCachingService);
                }

                var auxFileName = TreeNodeHelper.FileName;
                TreeNodeHelper.FileName = tagData.Link;
                var root = treeView.ToTreeNodeData(_treeNodeDataCachingService);
                _exportTreeToXmlService.SaveTree(root, TreeNodeHelper.FileName);
                TreeNodeHelper.FileName = auxFileName;
                node.Nodes.Clear();
            }

            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnMoveNode_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.MoveNode(tvTaskList, _treeNodeDataCachingService, _treeNodeSelectionCachingService);
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnGoToDefaultTree_Click(object sender, EventArgs e)
        {
            var afterSaveDoWithFileName = new Action<string>((fileName) => TreeNodeHelper.FileName = fileName);
            var beforeLoadInside = new Action(() => this.InvokeWrapper(t => t.Cursor = Cursors.WaitCursor));
            var afterLoadInside = new Action(() => this.InvokeWrapper(t => t.Cursor = Cursors.Default));
            var afterLoad = new Action(() =>
            {
                tvTaskList.InvokeWrapper(tv => { tv.CollapseAll(); tv.Refresh(); });
                TreeNodeHelper.IsSafeToSave = true;
                UpdateShowUntilNumber();
            });
            var currentRoot = tvTaskList.ToTreeNodeData(_treeNodeDataCachingService);
            (_, TreeNodeHelper.FileName) = _importExportTreeXmlService.SaveCurrentTreeAndLoadAnother(afterSaveDoWithFileName, currentRoot, null, beforeLoadInside, afterLoadInside, afterLoad);
        }

        private void tbSearchBox_DoubleClick(object sender, EventArgs e)
        {
            var form = new SearchForm(_popUpService, tbSearchBox.Text);

            WinFormsApplication.CenterForm(form, this);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is SearchForm form)
            {
                var textToFind = form.TextToFind;
                tbSearchBox.Text = textToFind;
                tbSearchBox_KeyUp(this, new KeyEventArgs(Keys.Enter));
            }
        }

        private void tbSearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            MainForm_KeyUp(sender, e);
            if (e.KeyData == Keys.Enter)
            {
                var searchText = tbSearchBox.Text;

                TreeNodeHelper.ClearStyleAdded(tvTaskList.Nodes);

                if (searchText.Length < 3)
                    return;

                if (!string.IsNullOrEmpty(searchText))
                {
                    TreeNodeHelper.SetStyleForSearch(tvTaskList.Nodes, searchText, _treeNodeDataCachingService);
                }
            }
        }

        private void btnShowCanvasPopUp_Click(object sender, EventArgs e)
        {
            if (_canvasForm == null || _canvasForm.IsDisposed)
            {
                var figureLines = TreeNodeHelper.GenerateStringGraphicsLinesFromTree(tvTaskList);

                _canvasForm = _canvasFormFactory.Create(figureLines);
            }

            _canvasForm.Show();
        }

        public void tvTaskList_ControlAdded(object sender, ControlEventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        public void tvTaskList_ControlRemoved(object sender, ControlEventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        public void tvTaskList_FontChanged(object sender, EventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void ShowStartupAlertForm()
        {
            var alertNodesRoot = new TreeNodeData();
            var root = tvTaskList.ToTreeNodeData(_treeNodeDataCachingService);
            var haveAlerts = _importTreeFromXmlService.LoadTreeNodesByCategory(root, alertNodesRoot, true);

            if (haveAlerts)
            {
                var form = new StartupAlertForm(_treeNodeDataCachingService, alertNodesRoot);
                form.FormClosing += StartupAlertForm_FormClosing;
                form.ShowDialog();
                tvTaskList.Refresh();
            }
        }

        private void btnShowStartupAlertForm_Click(object sender, EventArgs e)
        {
            ShowStartupAlertForm();
        }

        private void btnExecCommand_Click(object sender, EventArgs e)
        {
            if (tbCommand.Lines.Length <= 0)
                return;

            var figureLines = tbCommand.Lines;

            if (_canvasForm == null || _canvasForm.IsDisposed)
            {
                _canvasForm = _canvasFormFactory.Create(figureLines);
            }
            else
            {
                _canvasForm.RunTimer.Enabled = false;
                try
                {
                    _canvasForm.GraphicsFile.Clean();
                    _canvasForm.GraphicsFile.ParseLines(figureLines);
                }
                finally
                {
                    _canvasForm.RunTimer.Enabled = false;
                }
            }

            _canvasForm.Show();
        }

        private void btnDeleteCanvas_Click(object sender, EventArgs e)
        {
            if (_canvasForm != null && !_canvasForm.IsDisposed)
            {
                _canvasForm.Close();
                _canvasForm.Dispose();
                _canvasForm = null;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
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
                var nodeData = node.ToTreeNodeData(_treeNodeDataCachingService);
                nodeData.Data += commandData + Environment.NewLine;

                TreeNodeHelper.TreeUnchanged = false;
            }

            if (cbUseDefaultsChecked)
                tbCommand.Lines = _graphicsFileRecursiveGenerator.GenerateFigureLines(radius, iterations, computeType).Distinct().ToArray();
            else
                tbCommand.Lines = _graphicsFileRecursiveGenerator.GenerateFigureLines(points, x, y, radius, theta, number, iterations, computeType).Distinct().ToArray();
        }

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

        private void btnGenerateFiguresAndExec_Click(object sender, EventArgs e)
        {
            btnGenerate_Click(sender, e);

            btnExecCommand_Click(sender, e);
        }

        private void lblUnchanged_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = !TreeNodeHelper.TreeUnchanged;
        }

        public void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            _isControlPressed = false;
        }

        public void tvTaskList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                _isControlPressed = true;
            else
                _isControlPressed = false;

            if (e.Control && e.KeyCode == Keys.F)
                tbSearchBox_DoubleClick(sender, EventArgs.Empty);

            if (ReferenceEquals(sender, tvTaskList))
            {
                if (e.KeyCode == Keys.Delete)
                    btnDelete_Click(sender, EventArgs.Empty);
                else if (e.KeyCode == Keys.Enter)
                    tbTaskName_DoubleClick(sender, EventArgs.Empty);
                else if (e.KeyCode == Keys.M)
                    btnMoveNode_Click(sender, EventArgs.Empty);
            }
        }

        public void tvTaskList_MouseClick(object sender, MouseEventArgs e)
        {
            if (_isControlPressed && tvTaskList != null)
            {
                var deltaFloat = (float)e.Delta;
                var changedSize = deltaFloat / 120f;

                TreeNodeHelper.UpdateSizeOfTreeNodes(tvTaskList.Nodes, changedSize);
            }
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            if (FormBorderStyle == FormBorderStyle.Sizable)
                FormBorderStyle = FormBorderStyle.None;
            else
                FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void tbTreeChange_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MainForm_DoubleClick(sender, e);
        }

        private void lblChangeTreeType_Click(object sender, EventArgs e)
        {
            if (_randomTimer == null)
                return;
            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return;

            if (!_randomTimer.Enabled)
            {
                _randomTimer.Elapsed += RandomTimer_Elapsed; // timer disposed in Dispose(bool)
                RandomTimer_ChangeIntervalAndSound();
                _randomTimer.Start();
            }
            else
            {
                _randomTimer.Elapsed -= RandomTimer_Elapsed;
                _randomTimer.Stop();
            }
        }

        // TODO: I guess better this will be handled as an MediatR event and this will be solved, it will be separate handler for alerts (if possible with the timer access and all)
        private void RandomTimer_ChangeIntervalAndSound()
        {
            if (_randomTimer == null)
                return;
            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return;

            var ticks = DateTime.Now.Ticks;
            var ticksSeedAsInt = unchecked((int)ticks);
            var interval = 0;

            while (interval == 0)
                interval = (new Random(ticksSeedAsInt).Next() % 10000);

            _randomTimer.Interval = interval;

            _systemSoundNumber = -1;

            while (_systemSoundNumber < 1 || _systemSoundNumber > 4)
                _systemSoundNumber = (new Random(ticksSeedAsInt).Next() % 4) + 1;
        }

        private void RandomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_configuration.ApplicationFeatures.EnableAlerts == false)
                return;
            if (_configuration.ApplicationFeatures.EnableExtraSound == false)
                return;

            _soundProvider.PlaySystemSound(_systemSoundNumber);
            RandomTimer_ChangeIntervalAndSound();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tbTaskName_DoubleClick(sender, e);
        }

        private void showChildrenAsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = tvTaskList.SelectedNode;
            var form = new TaskListForm(selectedItem);
            form.ShowDialog();
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _popUpService.ShowWarning("Not implemented.");
            // TODO: Call handler from btnPgpEncryptData_Click after it is a MediatR handler
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _popUpService.ShowWarning("Not implemented.");
            // TODO: Call handler from btnPgpEncryptData_Click after it is a MediatR handler
        }

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

        private void tbTask_DoubleClick(object sender, EventArgs e)
        {
            MainForm_DoubleClick(sender, e);
        }

        #endregion Handlers

        #endregion Methods
    }
}