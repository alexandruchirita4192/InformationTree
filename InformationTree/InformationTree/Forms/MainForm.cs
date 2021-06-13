﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FormsGame.TextProcessing;
using FormsGame.Tree;
using FormsGame.Graphics;
using FormsGame.Sound;

namespace FormsGame.Forms
{
    public partial class MainForm : Form
    {
        #region ctor

        public MainForm()
        {
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

            this.AcceptButton = this.btnAddTask;

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

            var loadedExisting = TreeNodeHelper.LoadTree(this, tvTaskList, TreeNodeHelper.FileName);
            if (loadedExisting)
            {
                tvTaskList.CollapseAll();
                tvTaskList.Refresh();

                TreeNodeHelper.FixTreeNodesAndResetCounter(tvTaskList.Nodes);
                UpdateShowUntilNumber();

                ShowStartupAlertForm();
            }

            btnShowAll.Enabled = false;
            TreeNodeHelper.TreeUnchangedChangeDelegate = (a) =>
            {
                lblUnchanged.Text = (a ? "Tree unchanged (do not save)" : "Tree changed (save)");
            };

            this.MouseWheel += tvTaskList_MouseClick;
            IsControlPressed = false;
            StartPosition = FormStartPosition.CenterScreen;
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
            if(form != null)
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

        #region Constants
        static readonly new Font DefaultFont = new Font(FontFamily.GenericSansSerif, 8.5F, FontStyle.Regular);
        #endregion Constants

        #region Properties

        public bool clbStyle_ItemCheckEntered { get; set; }

        CanvasPopUpForm CanvasForm;

        private static Stopwatch timer = new Stopwatch();
        private static Timer randomTimer = new Timer();
        private static int systemSoundNumber = -1;

        public TreeView TaskList
        {
            get
            {
                return tvTaskList;
            }
        }

        private int oldX = 0, oldY = 0;

        bool IsControlPressed;

        #endregion Properties

        #region Methods

        #region Public methods
        public void SaveTree()
        {
            TreeNodeHelper.SaveTree(tvTaskList);
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
                btnResetException.BackColor = TreeNodeHelper.DefaultBackGroundColor;
                btnResetException.ForeColor = TreeNodeHelper.ExceptionColor;
                btnResetException.Text = "Reset exception: " + message.Substring(0, 10);
            }
            else
            {
                btnResetException.Enabled = false;
                btnResetException.ForeColor = TreeNodeHelper.DefaultForeGroundColor;
                btnResetException.BackColor = TreeNodeHelper.DefaultBackGroundColor;
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

        private void tvTaskList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNodeHelper.TreeUnchangedFreeze = true;

                var node = e.Node;
                if (node == null)
                    return;

                var percentCompleted = nudCompleteProgress.Value;
                tbTaskName.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref percentCompleted, true);
                nudCompleteProgress.Value = percentCompleted;

                var tagData = node.Tag as TreeNodeData;

                TreeNodeHelper.UpdateCurrentSelection(node.Text == null && tagData != null && tagData.IsEmptyData ? null : node);

                if (tagData != null)
                {
                    tbAddedDate.Text = (tagData.AddedDate.HasValue ? tagData.AddedDate.Value.ToString(TreeNodeHelper.DateTimeFormat) : "-");
                    tbLastChangeDate.Text = (tagData.LastChangeDate.HasValue ? tagData.LastChangeDate.Value.ToString(TreeNodeHelper.DateTimeFormat) : "-");
                    tbAddedNumber.Text = tagData.AddedNumber.ToString();
                    tbTaskName.BackColor = string.IsNullOrEmpty(tagData.Data) ? (string.IsNullOrEmpty(tagData.Link) ? TreeNodeHelper.DefaultBackGroundColor : TreeNodeHelper.LinkBackGroundColor) : TreeNodeHelper.DataBackGroundColor;

                    nudUrgency.Value = tagData.Urgency;
                    tbLink.Text = tagData.Link;
                    cbIsStartupAlert.Checked = tagData.IsStartupAlert;
                    nudCompleteProgress.Value = tagData.PercentCompleted;
                    tbCategory.Text = tagData.Category;

                    var size = TreeNodeHelper.CalculateDataSizeFromNodeAndChildren(node);
                    var sizeMb = size / 1024 / 1024;
                    tbDataSize.Text = size.ToString() + "b " + sizeMb + "M";
                }

                gbTimeSpent.Enabled = true;
                var timeSpanTotal = !string.IsNullOrEmpty(node.Name) ? TimeSpan.FromMilliseconds(long.Parse(node.Name)) : new TimeSpan(0);
                nudHours.Value = timeSpanTotal.Hours;
                nudMinutes.Value = timeSpanTotal.Minutes;
                nudSeconds.Value = timeSpanTotal.Seconds;
                nudMilliseconds.Value = timeSpanTotal.Milliseconds;

                if (node.NodeFont != null)
                {
                    var defaultSize = 8;
                    var size = ((decimal)node.NodeFont.Size) > nudFontSize.Maximum ? defaultSize : ((decimal)node.NodeFont.Size);
                    size = size < nudFontSize.Minimum ? defaultSize : size;
                    nudFontSize.Value = size;
                }

                if (!clbStyle_ItemCheckEntered)
                {
                    if (node != null && node.NodeFont != null)
                    {
                        int idx = clbStyle.Items.IndexOf(FontStyle.Regular);
                        if ((!clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx)))
                            clbStyle.SetItemCheckState(idx, CheckState.Checked);

                        idx = clbStyle.Items.IndexOf(FontStyle.Italic);
                        if (((!clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx))) && (node.NodeFont.Style & FontStyle.Italic) != 0)
                            clbStyle.SetItemCheckState(idx, CheckState.Checked);
                        else if ((clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx)))
                            clbStyle.SetItemCheckState(idx, CheckState.Unchecked);

                        idx = clbStyle.Items.IndexOf(FontStyle.Bold);
                        if (((!clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx))) && ((node.NodeFont.Style & FontStyle.Bold) != 0))
                            clbStyle.SetItemCheckState(idx, CheckState.Checked);
                        else if ((clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx)))
                            clbStyle.SetItemCheckState(idx, CheckState.Unchecked);

                        idx = clbStyle.Items.IndexOf(FontStyle.Strikeout);
                        if (((!clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx))) && ((node.NodeFont.Style & FontStyle.Strikeout) != 0))
                            clbStyle.SetItemCheckState(idx, CheckState.Checked);
                        else if ((clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx)))
                            clbStyle.SetItemCheckState(idx, CheckState.Unchecked);

                        idx = clbStyle.Items.IndexOf(FontStyle.Underline);
                        if (((!clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx))) && ((node.NodeFont.Style & FontStyle.Underline) != 0))
                            clbStyle.SetItemCheckState(idx, CheckState.Checked);
                        else if ((clbStyle.GetItemChecked(idx)) && (!clbStyle.SelectedIndices.Contains(idx)))
                            clbStyle.SetItemCheckState(idx, CheckState.Unchecked);

                        if (node.NodeFont.FontFamily != null && cbFontFamily.Items.Count != 0)
                            for (int i = 0; i < cbFontFamily.Items.Count; i++)
                            {
                                var fontFamily = cbFontFamily.Items[i] as String;
                                if (fontFamily != null && fontFamily == node.NodeFont.FontFamily.Name)
                                {
                                    cbFontFamily.SelectedIndex = i;
                                    break;
                                }
                            }

                        tbTextColor.Text = node.ForeColor.Name;
                        tbBackgroundColor.Text = node.BackColor.Name;
                    }
                }
            }
            finally
            {
                TreeNodeHelper.TreeUnchangedFreeze = false;
            }
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
                        Tag = new TreeNodeData()
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

        private void btnUpdateText_Click(object sender, EventArgs e)
        {
            var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null)
            {
                var taskPercentCompleted = nudCompleteProgress.Value;
                var taskName = TextProcessingHelper.GetTextAndProcentCompleted(tbTaskName.Text, ref taskPercentCompleted, true);
                var link = tbLink.Text;
                var urgency = (int)nudUrgency.Value;
                var category = tbCategory.Text;
                var isStartupAlert = cbIsStartupAlert.Checked;

                var data = selectedNode.Tag as TreeNodeData;
                if (data == null)
                    data = new TreeNodeData(null, 0, DateTime.Now, DateTime.Now, urgency, link, category, isStartupAlert, taskPercentCompleted);
                else
                {
                    data.Urgency = urgency;
                    data.Link = link;
                    data.Category = category;
                    data.IsStartupAlert = isStartupAlert;
                    data.PercentCompleted = taskPercentCompleted;
                    data.LastChangeDate = DateTime.Now;
                }

                selectedNode.Text = taskName;

                tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                TreeNodeHelper.TreeUnchanged = false;
            }
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            var taskPercentCompleted = nudCompleteProgress.Value;
            var taskName = TextProcessingHelper.GetTextAndProcentCompleted(tbTaskName.Text, ref taskPercentCompleted, true);
            var urgency = (int)nudUrgency.Value;
            var link = tbLink.Text;
            var nodeCategory = string.Empty;

            // update part
            var selectedNode = tvTaskList.SelectedNode;
            var selectedNodeLastChildren = selectedNode != null && selectedNode.Nodes.Count > 0 ? selectedNode.Nodes[selectedNode.Nodes.Count - 1] : null;
            if (selectedNode != null && selectedNode.Text.Equals(taskName /*StartsWith + " [" */))
            {
                if (selectedNode.Text != taskName)
                {
                    selectedNode.Text = taskName;
                    var tagData = selectedNode.Tag as TreeNodeData;

                    if (tagData != null)
                        tagData.LastChangeDate = DateTime.Now;
                    else
                    {
                        var nodeData = new TreeNodeData();
                        nodeData.LastChangeDate = DateTime.Now;
                        selectedNode.Tag = nodeData;
                    }

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                    TreeNodeHelper.TreeUnchanged = false;
                }
            }
            else if (selectedNodeLastChildren != null && selectedNodeLastChildren.Text.Equals(taskName /* StartsWith + " [" */))
            {
                if (selectedNodeLastChildren.Text != taskName)
                {
                    selectedNodeLastChildren.Text = taskName;

                    var tagData = selectedNodeLastChildren.Tag as TreeNodeData;

                    if (tagData != null)
                        tagData.LastChangeDate = DateTime.Now;
                    else
                    {
                        var nodeData = new TreeNodeData();
                        nodeData.LastChangeDate = DateTime.Now;
                        selectedNodeLastChildren.Tag = nodeData;
                    }

                    tvTaskList_AfterSelect(sender, new TreeViewEventArgs(selectedNode));

                    TreeNodeHelper.TreeUnchanged = false;
                }
            }
            else //insert
            {
                var node = new TreeNode(taskName)
                {
                    Name = 0.ToString(),
                    Tag = new TreeNodeData(null, tvTaskList.GetNodeCount(true) + 1, DateTime.Now, DateTime.Now, urgency, link, null, false, taskPercentCompleted),
                    ForeColor = TreeNodeHelper.DefaultForeGroundColor,
                    BackColor = TreeNodeHelper.DefaultBackGroundColor,
                    NodeFont = DefaultFont.Clone() as Font,
                    ToolTipText = TextProcessingHelper.GetToolTipText(taskName)
                };
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
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedTask = tvTaskList.SelectedNode;
            var taskName = tbTaskName.Text;
            var deletedItemsWithName = ParseToDelete(selectedTask, taskName, true);

            if (deletedItemsWithName != 0)
            {
                var result = MessageBox.Show("Deleted " + deletedItemsWithName.ToString() + (deletedItemsWithName == 1 ? " item" : " items") + " with name " + taskName, "Deleted " + deletedItemsWithName.ToString() + (deletedItemsWithName == 1 ? " item" : " items"), MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
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
                timer.Start();
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
                timer.Stop();
                var oldElapsedTime = long.Parse(node.Name);
                var elapsedTime = timer.ElapsedMilliseconds;
                var totalElapsedTime = (oldElapsedTime + elapsedTime);
                node.Name = totalElapsedTime.ToString();
                timer.Reset();

                var timeSpanTotal = TimeSpan.FromMilliseconds(totalElapsedTime);
                nudHours.Value = timeSpanTotal.Hours;
                nudMinutes.Value = timeSpanTotal.Minutes;
                nudSeconds.Value = timeSpanTotal.Seconds;
                nudMilliseconds.Value = timeSpanTotal.Milliseconds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception caught");
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

        private void cbFontFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tvTaskList.SelectedNode != null && cbFontFamily.SelectedItem != null)
            {
                var fontFamily = cbFontFamily.SelectedItem as String;
                var oldFont = tvTaskList.SelectedNode.NodeFont;

                if (fontFamily != null)
                    tvTaskList.SelectedNode.NodeFont = new Font(fontFamily, (float)nudFontSize.Value, oldFont.Style);

                TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
            }
        }

        private void nudFontSize_ValueChanged(object sender, EventArgs e)
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

        private void clbStyle_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            clbStyle_ItemCheckEntered = true;
            if (tvTaskList.SelectedNode != null)
            {
                if (clbStyle.SelectedItem != null)
                {
                    var newFontStyle = FontStyle.Regular;
                    var currentFontStyle = clbStyle.SelectedItem as FontStyle?;
                    if (currentFontStyle.HasValue)
                    {
                        int idx = clbStyle.Items.IndexOf(FontStyle.Regular);
                        var checkedState = idx == e.Index ? e.NewValue : clbStyle.GetItemCheckState(idx);
                        if (checkedState == CheckState.Checked)
                            newFontStyle = newFontStyle | FontStyle.Regular;

                        idx = clbStyle.Items.IndexOf(FontStyle.Italic);
                        checkedState = idx == e.Index ? e.NewValue : clbStyle.GetItemCheckState(idx);
                        if (checkedState == CheckState.Checked)
                            newFontStyle = newFontStyle | FontStyle.Italic;

                        idx = clbStyle.Items.IndexOf(FontStyle.Bold);
                        checkedState = idx == e.Index ? e.NewValue : clbStyle.GetItemCheckState(idx);
                        if (checkedState == CheckState.Checked)
                            newFontStyle = newFontStyle | FontStyle.Bold;

                        idx = clbStyle.Items.IndexOf(FontStyle.Underline);
                        checkedState = idx == e.Index ? e.NewValue : clbStyle.GetItemCheckState(idx);
                        if (checkedState == CheckState.Checked)
                            newFontStyle = newFontStyle | FontStyle.Underline;

                        idx = clbStyle.Items.IndexOf(FontStyle.Strikeout);
                        checkedState = idx == e.Index ? e.NewValue : clbStyle.GetItemCheckState(idx);
                        if (checkedState == CheckState.Checked)
                            newFontStyle = newFontStyle | FontStyle.Strikeout;

                        var font = tvTaskList.SelectedNode.NodeFont ?? (tvTaskList.SelectedNode.Parent != null ? tvTaskList.SelectedNode.Parent.NodeFont : null);
                        var backColor = tvTaskList.SelectedNode.BackColor;
                        var foreColor = tvTaskList.SelectedNode.ForeColor;

                        if (font != null)
                        {
                            tvTaskList.SelectedNode.NodeFont = new Font(font.FontFamily, font.Size, newFontStyle);
                            tvTaskList.SelectedNode.BackColor = backColor != null && !backColor.IsEmpty ? backColor : TreeNodeHelper.DefaultBackGroundColor;
                            tvTaskList.SelectedNode.ForeColor = foreColor != null && !foreColor.IsEmpty ? foreColor : TreeNodeHelper.DefaultForeGroundColor;
                        }

                        var nodeData = tvTaskList.SelectedNode.Tag as TreeNodeData;
                        if (nodeData != null)
                            nodeData.LastChangeDate = DateTime.Now;
                        else
                        {
                            nodeData = new TreeNodeData();
                            nodeData.LastChangeDate = DateTime.Now;
                            tvTaskList.SelectedNode.Tag = nodeData;
                        }

                        tvTaskList_AfterSelect(sender, new TreeViewEventArgs(tvTaskList.SelectedNode));

                        TreeNodeHelper.TreeUnchanged = false; // on font changed is added too??
                    }
                }
            }
            clbStyle_ItemCheckEntered = false;
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
                    MessageBox.Show("Cannot move task up! [selectedIndex=" + selectedIndex.ToString() + "]");
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
                    MessageBox.Show("Cannot move task up! [selectedIndex=" + selectedIndex.ToString() + "]");
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
                    MessageBox.Show("Cannot move task down! [selectedIndex=" + selectedIndex.ToString() + "]");
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
                    MessageBox.Show("Cannot move task down! [selectedIndex=" + selectedIndex.ToString() + "]");
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

            TreeNodeHelper.ShowNodesFromTaskToNumberOfTask(tvTaskList, addedNumberLowerThan, addedNumberHigherThan, 0);
            btnShowAll.Enabled = true;

            gbTask.Enabled = false;
            gbStyleChange.Enabled = false;
            gbTimeSpent.Enabled = false;

        }

        private void btnShowFromToUrgencyNumber_Click(object sender, EventArgs e)
        {
            var addedNumberLowerThan = nudShowToUrgencyNumber.Value;
            var addedNumberHigherThan = nudShowFromUrgencyNumber.Value;

            TreeNodeHelper.ShowNodesFromTaskToNumberOfTask(tvTaskList, addedNumberLowerThan, addedNumberHigherThan, 1);
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

        private void tvTaskList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X == oldX && e.Y == oldY)
                return;

            // Get the node at the current mouse pointer location.
            TreeNode theNode = this.tvTaskList.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null)
                {
                    // Change the ToolTip only if the pointer moved to a new node.
                    if (theNode.ToolTipText != this.toolTip1.GetToolTip(this.tvTaskList))
                    {
                        this.toolTip1.SetToolTip(this.tvTaskList, theNode.ToolTipText);
                    }
                }
                else
                {
                    this.toolTip1.SetToolTip(this.tvTaskList, "");
                }
            }
            else     // Pointer is not over a node so clear the ToolTip.
            {
                this.toolTip1.SetToolTip(this.tvTaskList, "");
            }

            oldX = e.X;
            oldY = e.Y;
        }

        private void tbTaskName_DoubleClick(object sender, EventArgs e)
        {
                var selectedNode = tvTaskList.SelectedNode;
            if (selectedNode != null)
            {
                var tagData = selectedNode.Tag as TreeNodeData;
                var data = string.Empty;

                if (tagData != null)
                    data = tagData.Data;

                var form = new PopUpEditForm(selectedNode.Text, data);

                Program.CenterForm(form, this);

                form.FormClosing += (s, ev) =>
                {
                    var d = form.Data;

                    if (selectedNode != null)
                    {
                        var td = selectedNode.Tag as TreeNodeData;

                        if (td == null)
                            td = new TreeNodeData(d, 0, DateTime.Now, DateTime.Now, 0, null, null, false);
                        else
                            td.Data = d;

                        selectedNode.Tag = td;

                        var strippedData = RicherTextBox.Controls.RicherTextBox.StripRTF(d);
                        selectedNode.ToolTipText = TextProcessingHelper.GetToolTipText(selectedNode.Text +
                            (!string.IsNullOrEmpty(selectedNode.Name) && selectedNode.Name != "0" ? Environment.NewLine + " TimeSpent: " + selectedNode.Name : "") +
                            (!string.IsNullOrEmpty(strippedData) ? Environment.NewLine + " Data: " + strippedData : ""));

                        tbTaskName.BackColor = string.IsNullOrEmpty(tagData.Data) ? (string.IsNullOrEmpty(tagData.Link) ? TreeNodeHelper.DefaultBackGroundColor : TreeNodeHelper.LinkBackGroundColor) : TreeNodeHelper.DataBackGroundColor;

                        TreeNodeHelper.TreeUnchanged = false;
                    }
                };

                form.ShowDialog();
            }
        }
        
        private void tvTaskList_DoubleClick(object sender, EventArgs e)
        {
            var node = tvTaskList.SelectedNode;
            if (node != null)
            {
                var tagData = node.Tag as TreeNodeData;
                if (tagData != null && !string.IsNullOrEmpty(tagData.Link))
                {
                    TreeNodeHelper.SaveCurrentTreeAndLoadAnother(this, tvTaskList, tagData.Link, UpdateShowUntilNumber);
                }
            }
            else
            {
                if (CanvasForm == null || CanvasForm.IsDisposed)
                    CanvasForm = new CanvasPopUpForm();
                CanvasForm.RunTimer.Stop();
                CanvasForm.GraphicsFile.Clean();
                CanvasForm.GraphicsFile.ParseLines(TreeNodeHelper.GenerateStringGraphicsLinesFromTree(tvTaskList));
                //CanvasForm.RunTimer.Start();
                CanvasForm.Show();
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

                var tagData = node.Tag as TreeNodeData;
                if (tagData != null && string.IsNullOrEmpty(tagData.Link))
                    tagData.Link = tbLink.Text;
                else
                    tagData = new TreeNodeData(null, 0, DateTime.Now, DateTime.Now, 0, tbLink.Text);

                var percentCompleted = 0M;
                if (string.IsNullOrEmpty(tagData.Link) || !tagData.Link.EndsWith(".xml") || tagData.Link.Contains(" "))
                    tagData.Link = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref percentCompleted, true).Replace(" ", "_") + ".xml";
                tbLink.Text = tagData.Link;

                var treeView = new TreeView();
                if (useSelectedNode)
                {
                    var parentNode = new TreeNode();
                    TreeNodeHelper.CopyNode(parentNode, node);
                    treeView.Nodes.Add(parentNode);
                    tagData.Data = null;
                }
                else
                {
                    TreeNodeHelper.CopyNodes(treeView.Nodes, node.Nodes);
                }

                var auxFileName = TreeNodeHelper.FileName;
                TreeNodeHelper.FileName = tagData.Link;
                TreeNodeHelper.SaveTree(treeView);
                TreeNodeHelper.FileName = auxFileName;
                node.Nodes.Clear();
            }

            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnMoveNode_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.MoveNode(tvTaskList);
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void btnGoToDefaultTree_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.SaveCurrentTreeAndLoadAnother(this, tvTaskList, null, UpdateShowUntilNumber);
        }

        private void tbSearchBox_DoubleClick(object sender, EventArgs e)
        {
            var form = new SearchForm(tbSearchBox.Text);

            Program.CenterForm(form, this);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as SearchForm;
            if (form != null)
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

                try
                {
                    TreeNodeHelper.ClearStyleAdded(tvTaskList.Nodes);
                }
                catch (Exception ex)
                {
                    Program.GlobalExceptionHandling(ex);
                }

                if (searchText.Length < 3)
                    return;

                if (!String.IsNullOrEmpty(searchText))
                {
                    TreeNodeHelper.SetStyleForSearch(tvTaskList.Nodes, searchText);
                }
            }
        }

        private void btnShowCanvasPopUp_Click(object sender, EventArgs e)
        {
            if (CanvasForm == null || CanvasForm.IsDisposed)
                CanvasForm = new CanvasPopUpForm();
            CanvasForm.Show();
        }
        
        private void tvTaskList_ControlAdded(object sender, ControlEventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void tvTaskList_ControlRemoved(object sender, ControlEventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void tvTaskList_FontChanged(object sender, EventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = false;
        }

        private void ShowStartupAlertForm()
        {
            var alertNodes = new TreeNode().Nodes;
            var haveAlerts = TreeNodeHelper.LoadTreeNodesByCategory(tvTaskList.Nodes, alertNodes, true);

            if (haveAlerts)
            {
                var form = new StartupAlertForm(alertNodes);
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

            if (CanvasForm == null || CanvasForm.IsDisposed)
                CanvasForm = new CanvasPopUpForm();
            CanvasForm.RunTimer.Stop();
            CanvasForm.GraphicsFile.Clean();
            CanvasForm.GraphicsFile.ParseLines(tbCommand.Lines);
            CanvasForm.RunTimer.Start();
            CanvasForm.Show();
        }

        private void btnDeleteCanvas_Click(object sender, EventArgs e)
        {
            CanvasForm = null;
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
            var computeType = (int)nudComputeType.Value;


            var cbLogChecked = cbLog.Checked;
            var node = tvTaskList.SelectedNode;
            var commandData = cbUseDefaultsChecked ?
                "ComputeXComputeY " + radius + " " + theta + " " + iterations + " " + computeType :
                "ComputeXComputeY " + points + " " + x + " " + y + " " + radius + " " + theta + " " + number + " " + iterations + " " + computeType;

            if (cbLogChecked && node != null)
            {
                var nodeData = node.Tag as TreeNodeData;
                if (nodeData != null)
                    nodeData.Data += commandData + Environment.NewLine;
                else
                    node.Tag = new TreeNodeData(commandData + Environment.NewLine);

                TreeNodeHelper.TreeUnchanged = false;
            }

            if (cbUseDefaultsChecked)
                tbCommand.Lines = GraphicsGenerator.ComputeXComputeY(radius, iterations, computeType).Distinct().ToArray<string>();
            else
                tbCommand.Lines = GraphicsGenerator.ComputeXComputeY(points, x, y, radius, theta, number, iterations, computeType).Distinct().ToArray<string>();
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
                nudX.Value = (decimal)GraphicsGenerator.DefaultX;
                nudY.Value = (decimal)GraphicsGenerator.DefaultY;
                nudNumber.Value = GraphicsGenerator.DefaultNumber;
                nudPoints.Value = GraphicsGenerator.DefaultPoints;
                nudTheta.Value = 0;
            }
        }

        private void btnGenerateFiguresAndExec_Click(object sender, EventArgs e)
        {
            btnGenerate_Click(sender, e);

            btnExecCommand_Click(sender, e);
        }

        private void cbLog_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void lblUnchanged_Click(object sender, EventArgs e)
        {
            TreeNodeHelper.TreeUnchanged = !TreeNodeHelper.TreeUnchanged;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            IsControlPressed = false;
        }

        private void tvTaskList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                IsControlPressed = true;
            else
                IsControlPressed = false;

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

        private void tvTaskList_MouseClick(object sender, MouseEventArgs e)
        {
            if(IsControlPressed && tvTaskList != null)
            {
                TreeNodeHelper.UpdateSizeOfTreeNodes(tvTaskList.Nodes, (e.Delta / 120));
                //var fontIsNotNull = tvTaskList.Font != null;
                //var currentSize = fontIsNotNull ? tvTaskList.Font.Size : 0f;
                //currentSize += (e.Delta / 120);
                //var newFont = fontIsNotNull ?
                //    new Font(tvTaskList.Font.FontFamily, currentSize, tvTaskList.Font.Style)
                //    : new Font(DefaultFont.FontFamily, currentSize, DefaultFont.Style);
                //tvTaskList.Font = newFont;
            }
        }

        private void MainForm_DoubleClick(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            else
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void tbTreeChange_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MainForm_DoubleClick(sender, e);
        }

        private void lblChangeTreeType_Click(object sender, EventArgs e)
        {
            if (randomTimer == null)
                return;

            if(!randomTimer.Enabled)
            {
                randomTimer.Tick += RandomTimer_Tick;
                RandomTimer_ChangeIntervalAndSound();
                randomTimer.Start();
            }
            else
            {
                randomTimer.Tick -= RandomTimer_Tick;
                randomTimer.Stop();
            }
        }

        private void RandomTimer_ChangeIntervalAndSound()
        {
            if (randomTimer == null)
                return;

            var ticks = DateTime.Now.Ticks;
            var ticksSeedAsInt = unchecked((int)ticks);
            var interval = 0;

            while (interval == 0)
                interval = (new Random(ticksSeedAsInt).Next() % 10000);

            randomTimer.Interval = interval;

            systemSoundNumber = -1;

            while(systemSoundNumber < 1 || systemSoundNumber > 4)
                systemSoundNumber = (new Random(ticksSeedAsInt).Next() % 4) + 1;


        }
        private void RandomTimer_Tick(object sender, EventArgs e)
        {
            SoundHelper.PlaySystemSound(systemSoundNumber);
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

        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        //private void tvTaskList_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.ControlKey)
        //    {
        //        var leKeyValue = e.KeyValue;
        //        var leKeyCode = e.KeyCode;
        //        var leKeyData = e.KeyData;

        //    }
        //}

        //private void tvTaskList_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if(e.Delta != 0)
        //    {

        //    }
        //}

        private void tbTask_DoubleClick(object sender, EventArgs e)
        {
            MainForm_DoubleClick(sender, e);
        }

        #endregion Handlers

        #endregion Methods

    }
}
