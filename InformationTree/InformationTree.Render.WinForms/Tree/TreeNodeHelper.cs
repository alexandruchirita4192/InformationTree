using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Services;
using InformationTree.Forms;
using InformationTree.Render.WinForms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.TextProcessing;
using NLog;

namespace InformationTree.Tree
{
    // TODO: Change from static to service (facade hiding subsystem complexity with an interface) with instance and get services in constructor and remove them from static methods
    [Obsolete("Break into many classes with many purposes")]
    public static class TreeNodeHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Properties

        #region FileName

        private static string fileName;

        public static string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(fileName))
                    return fileName;
                return "Data.xml"; // TODO: Move to constants
            }
            set
            {
                fileName = value;
            }
        }

        #endregion FileName

        // TODO: Create a new class for keeping state of the tree
        public static bool IsSafeToSave;
        public static bool ReadOnlyState;

        private static bool _treeUnchanged;

        public static bool TreeUnchanged
        {
            get
            {
                return _treeUnchanged;
            }
            set
            {
                if (_treeUnchanged != value)
                {
                    File.AppendAllText("TreeUnchangedIssue.txt", $"Tree unchanged set as {value} was called by {new StackTrace()} at {DateTime.Now}");
                    _treeUnchanged = value;

                    if (TreeUnchangedChangeDelegate != null)
                        TreeUnchangedChangeDelegate(value);
                }
            }
        }

        public static bool TreeSaved { get; set; }
        public static DateTime TreeSavedAt { get; set; }

        public static Action<bool> TreeUnchangedChangeDelegate;

        public static int TreeNodeCounter = 0;
        private static TreeNodeCollection nodes;

        private static TreeNode currentSelection;
        private static TreeNode oldSelection;

        #endregion Properties

        #region Node update

        public static bool ParseNodeAndUpdate(TreeNode node, string taskName, string nodeValue)
        {
            if (node.Text.Equals(taskName /* StartsWith + " [" */))
            {
                node.Text = nodeValue;
                return true;
            }
            else
            {
                if (node.Nodes != null)
                {
                    bool exists = false;
                    foreach (TreeNode n in node.Nodes)
                    {
                        exists = ParseNodeAndUpdate(n, taskName, nodeValue);
                        if (exists)
                            break;
                    }
                    return exists;
                }
            }
            return false;
        }

        #endregion Node update

        #region CopyNode, CopyNodes

        public static void CopyNodes(TreeNodeCollection to, TreeNodeCollection from, ITreeNodeDataCachingService treeNodeDataCachingService, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1)
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            foreach (TreeNode node in from)
                CopyNode(to, node, treeNodeDataCachingService, addedNumberHigherThan, addedNumberLowerThan, type);
        }

        public static void CopyNode(TreeNodeCollection to, TreeNode from, ITreeNodeDataCachingService treeNodeDataCachingService, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1)
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            var node = new TreeNode();
            CopyNode(node, from, treeNodeDataCachingService, addedNumberHigherThan, addedNumberLowerThan, type);

            if ((addedNumberLowerThan == null && addedNumberHigherThan == null) || (addedNumberLowerThan != null && addedNumberHigherThan != null))
                to.Add(node);
        }

        public static void CopyNode(TreeNode to, TreeNode from, ITreeNodeDataCachingService treeNodeDataCachingService, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1) // TODO: Change type to an enum explaining clearly what it means
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            if (to.Nodes == null)
                throw new Exception("to.Nodes is null");

            var tagData = from.ToTreeNodeData(treeNodeDataCachingService);
            if ((type == 0) && (tagData.AddedNumber >= addedNumberLowerThan) || (tagData.AddedNumber < addedNumberHigherThan))
                return;
            else if ((type == 1) && (tagData.Urgency >= addedNumberLowerThan) || (tagData.Urgency < addedNumberHigherThan))
                return;

            var toTag = to.ToTreeNodeData(treeNodeDataCachingService);
            toTag.Copy(tagData);

            to.Text = from.Text;
            to.Name = from.Name;

            var font = from.NodeFont;
            to.NodeFont = font != null ? new Font(font.FontFamily, font.SizeInPoints, font.Style) : WinFormsConstants.FontDefaults.DefaultFont;

            to.ForeColor = from.ForeColor.IsEmpty ? Constants.Colors.DefaultForeGroundColor : from.ForeColor;
            to.BackColor = from.BackColor.IsEmpty ? Constants.Colors.DefaultBackGroundColor : from.BackColor;

            foreach (TreeNode node in from.Nodes)
                CopyNode(to.Nodes, node, treeNodeDataCachingService, addedNumberHigherThan, addedNumberLowerThan, type);
        }

        #endregion CopyNode, CopyNodes

        #region Node percentage

        public static double GetPercentageFromChildren(TreeNode topNode)
        {
            var sum = 0.0;
            var nr = 0;
            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null && node.Nodes.Count > 0)
                {
                    var procentCompleted = (decimal)GetPercentageFromChildren(node);
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref procentCompleted, true);

                    sum += (double)procentCompleted;
                }
                else
                {
                    var value = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref value, true);

                    sum += (double)value;
                }
                nr++;
            }

            if (nr != 0)
                return (sum / nr);
            return 0;
        }

        public static void SetPercentageToChildren(TreeNode topNode, double percentage)
        {
            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null)
                {
                    var percentCompleted = (decimal)percentage;
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref percentCompleted, true);
                    if (node.Nodes.Count > 0)
                        SetPercentageToChildren(node, percentage);
                }
            }
        }

        #endregion Node percentage

        #region Node deletion

        public static int ParseToDelete(TreeView tv, TreeNode topNode, string nodeNameToDelete, bool fakeDelete = true)
        {
            int ret = 0;
            if (topNode.Text.Equals(nodeNameToDelete /* StartsWith + " [" */))
            {
                if (!fakeDelete)
                {
                    topNode.Nodes.Clear();
                    tv.Nodes.Remove(topNode);
                }
                ret++;
            }
            else
                foreach (TreeNode node in topNode.Nodes)
                {
                    if (node != null && node.Text.Equals(nodeNameToDelete /* StartsWith + " [" */))
                    {
                        if (!fakeDelete)
                        {
                            node.Nodes.Clear();
                            tv.Nodes.Remove(node);
                        }

                        ret++;
                    }
                    if (node != null && node.Nodes.Count > 0)
                        ret += ParseToDelete(tv, node, nodeNameToDelete);
                }
            return ret;
        }

        #endregion Node deletion

        #region Nodes completed/unfinished

        public static bool? TasksCompleteAreHidden(TreeView tv)
        {
            if (tv.Nodes.Count > 0)
            {
                foreach (TreeNode node in tv.Nodes)
                {
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                    if (completed == 100)
                    {
                        if (node.ForeColor.Name == Constants.Colors.DefaultBackGroundColor.ToString())
                            return true;
                        else
                            return false;
                    }

                    if (node.Nodes.Count > 0)
                    {
                        var ret = TasksCompleteAreHidden(tv);
                        if (ret.HasValue)
                            return ret;
                        // else continue;
                    }
                }
            }
            return null;
        }

        public static void ToggleCompletedTasks(TreeView tv, bool toggleCompletedTasksAreHidden, TreeNodeCollection nodes)
        {
            var foreColor = toggleCompletedTasksAreHidden ? Constants.Colors.DefaultForeGroundColor : Constants.Colors.DefaultBackGroundColor;

            if (tv.Nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                    if (completed == 100)
                        node.ForeColor = foreColor;

                    if (node.Nodes.Count > 0)
                        ToggleCompletedTasks(tv, toggleCompletedTasksAreHidden, node.Nodes);
                }
            }
        }

        public static void MoveToNextUnfinishedNode(TreeView tv, TreeNode currentNode)
        {
            if (currentNode == null)
                return;

            foreach (TreeNode node in currentNode.Nodes)
            {
                var completed = 0.0M;
                node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                if (completed != 100 && tv.SelectedNode != node)
                {
                    tv.SelectedNode = node;
                    return;
                }
            }

            if (currentNode.Parent != null)
            {
                var foundCurrentNode = false;
                foreach (TreeNode node in currentNode.Parent.Nodes)
                {
                    if (foundCurrentNode && !object.ReferenceEquals(node, currentNode))
                    {
                        MoveToNextUnfinishedNode(tv, node);
                        return;
                    }

                    if (object.ReferenceEquals(node, currentNode))
                        foundCurrentNode = true;
                }

                MoveToNextUnfinishedNode(tv, currentNode.Parent);
            }
        }

        #endregion Nodes completed/unfinished

        #region Node search

        public static TreeNode GetFirstNode(TreeNodeCollection nodes, string text, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            TreeNode ret = null;
            text = text.ToLower();
            if (nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var nodeTagData = node.ToTreeNodeData(treeNodeDataCachingService);
                    var nodeData = nodeTagData != null && !string.IsNullOrEmpty(nodeTagData.Data) ? nodeTagData.Data : null;
                    var foundCondition = (node.Text != null && node.Text.ToLower().Split('[')[0].Contains(text)) || (nodeData != null && nodeData.ToLower().Contains(text));

                    if (foundCondition)
                        return node;

                    if (node.Nodes != null && node.Nodes.Count > 0)
                        ret = GetFirstNode(node.Nodes, text, treeNodeDataCachingService);

                    if (ret != null)
                        return ret;
                }
            }

            return null;
        }

        public static void ExpandParents(TreeNode node)
        {
            if (node == null)
                return;

            var parent = node.Parent;
            if (parent != null)
            {
                parent.Expand();
                ExpandParents(parent);
            }
        }

        public static void SetStyleForSearch(TreeNodeCollection nodes, string text, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            text = text.ToLower();
            if (nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var nodeTagData = node.ToTreeNodeData(treeNodeDataCachingService);
                    var nodeData = nodeTagData != null && !string.IsNullOrEmpty(nodeTagData.Data) ? nodeTagData.Data : null;
                    var foundCondition = (node.Text != null && node.Text.ToLower().Split('[')[0].Contains(text))
                        || (nodeData != null && nodeData.ToLower().Contains(text));

                    if (foundCondition
                        && (node.BackColor != Constants.Colors.BackGroundColorSearch
                        || node.ForeColor != Constants.Colors.ForeGroundColorSearch))
                    {
                        node.BackColor = Constants.Colors.BackGroundColorSearch;
                        node.ForeColor = Constants.Colors.ForeGroundColorSearch;
                        node.Expand();

                        ExpandParents(node);
                    }

                    if (node.Nodes != null && node.Nodes.Count > 0)
                        SetStyleForSearch(node.Nodes, text, treeNodeDataCachingService);
                }
            }
        }

        public static string[] GenerateStringGraphicsLinesFromTree(TreeView tvTaskList)
        {
            List<string> lines = new List<string>();

            foreach (TreeNode task in tvTaskList.Nodes)
                lines.Add(task.Text);

            return lines.ToArray();
        }

        public static void ClearStyleAdded(TreeNodeCollection col)
        {
            if (col.Count > 0)
            {
                foreach (TreeNode node in col)
                {
                    if (node.BackColor != Constants.Colors.DefaultBackGroundColor)
                        node.BackColor = Constants.Colors.DefaultBackGroundColor;

                    if (node.ForeColor != Constants.Colors.DefaultForeGroundColor)
                        node.ForeColor = Constants.Colors.DefaultForeGroundColor;

                    if (node.Nodes.Count > 0)
                        ClearStyleAdded(node.Nodes);
                }
            }
        }

        #endregion Node search

        #region Node show by urgency or added number

        public static void ShowNodesFromTaskToNumberOfTask(TreeView tv, decimal addedNumberLowerThan, decimal addedNumberHigherThan, int type, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (nodes == null)
                nodes = new TreeNode().Nodes;

            // copy all
            if (!ReadOnlyState)
            {
                nodes.Clear();
                CopyNodes(nodes, tv.Nodes, null, null);
            }

            // let tvTaskList with only addedNumber < addedNumberLowerThan
            tv.Nodes.Clear();
            CopyNodes(tv.Nodes, nodes, treeNodeDataCachingService, (int)addedNumberHigherThan, (int)addedNumberLowerThan, type);
            ReadOnlyState = true;
        }

        public static void ShowAllTasks(TreeView tv)
        {
            if (TreeNodeHelper.ReadOnlyState)
            {
                tv.Nodes.Clear();
                TreeNodeHelper.CopyNodes(tv.Nodes, nodes, null, null);
                TreeNodeHelper.ReadOnlyState = false;
            }
        }

        #endregion Node show by urgency or added number

        #region Node data size calculation

        public static int CalculateDataSizeFromNodeAndChildren(TreeNode node, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (node == null)
                return 0;

            var tagData = node.ToTreeNodeData(treeNodeDataCachingService);
            if (tagData == null)
                return 0;

            var size = tagData.Data == null ? 0 : tagData.Data.Length;
            foreach (TreeNode n in node.Nodes)
                size += CalculateDataSizeFromNodeAndChildren(n, treeNodeDataCachingService);
            return size;
        }

        #endregion Node data size calculation

        #region Node move

        public static void UpdateCurrentSelection(TreeNode node)
        {
            oldSelection = currentSelection;
            currentSelection = node;
        }

        public static void MoveNode(TreeView tv, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (oldSelection == null)
                return;

            bool removedNode = false;
            if (currentSelection != null)
            {
                var parentSelected = currentSelection.Parent;
                if (parentSelected != null)
                {
                    parentSelected.Nodes.Remove(oldSelection);
                    removedNode = true;
                }
            }

            if (!removedNode)
                tv.Nodes.Remove(oldSelection);

            var currentSelectionTagData = currentSelection.ToTreeNodeData(treeNodeDataCachingService);
            if (string.IsNullOrEmpty(currentSelection.Text) &&
                currentSelectionTagData != null && string.IsNullOrEmpty(currentSelectionTagData.Data) &&
                currentSelection.Parent == null &&
                currentSelection.Nodes.Count == 0)
                tv.Nodes.Add(oldSelection);
            else
                currentSelection.Nodes.Add(oldSelection);
        }

        #endregion Node move

        /// <summary>
        /// Changes the size of a collection of nodes recursively.
        /// </summary>
        /// <param name="treeNodeCollection">Collection of nodes.</param>
        /// <param name="changedSize">Font size changed to all the nodes (added or substracted from nodes size).</param>
        public static void UpdateSizeOfTreeNodes(TreeNodeCollection treeNodeCollection, float changedSize)
        {
            foreach (TreeNode node in treeNodeCollection)
            {
                node.NodeFont = new Font(node.NodeFont.FontFamily, node.NodeFont.Size + changedSize, node.NodeFont.Style);

                // Change size of children recursively too
                foreach (TreeNode childNode in node.Nodes)
                    UpdateSizeOfTreeNodes(childNode.Nodes, changedSize);
            }
        }
    }
}