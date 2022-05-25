using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Render.WinForms;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.TextProcessing;
using MediatR;

namespace InformationTree.Tree
{
    // TODO: Change from static to service (facade hiding subsystem complexity with an interface) with instance and get services in constructor and remove them from static methods
    [Obsolete("Break into many classes with many purposes")]
    public static class TreeNodeHelper
    {
        private const string NodesListKey = "Nodes";

        // TODO: Maybe use a mediator like MediatR to handle all events, commands, etc. (each of those could be a separate command handler class)

        #region CopyNode, CopyNodes

        public static void CopyNodes(
            TreeNodeCollection to,
            TreeNodeCollection from,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            int? filterHigherThan = null,
            int? filterLowerThan = null,
            CopyNodeFilterType filterType = CopyNodeFilterType.NoFilter)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            foreach (TreeNode node in from)
                CopyNode(to, node, treeNodeDataCachingService, filterHigherThan, filterLowerThan, filterType);
        }

        public static void CopyNode(
            TreeNodeCollection to,
            TreeNode from,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            int? filterHigherThan = null,
            int? filterLowerThan = null,
            CopyNodeFilterType filterType = CopyNodeFilterType.NoFilter)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var node = new TreeNode();
            CopyNode(node, from, treeNodeDataCachingService, filterHigherThan, filterLowerThan, filterType);

            if ((filterLowerThan == null && filterHigherThan == null) || (filterLowerThan != null && filterHigherThan != null))
                to.Add(node);
        }

        public static void CopyNode(
            TreeNode to,
            TreeNode from,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            int? filterHigherThan = null,
            int? filterLowerThan = null,
            CopyNodeFilterType filterType = CopyNodeFilterType.NoFilter)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (to.Nodes == null)
                throw new ArgumentNullException(nameof(to.Nodes));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var tagData = from.ToTreeNodeData(treeNodeDataCachingService);

            // Filter nodes by added number or urgency depending on the type
            if ((filterType == CopyNodeFilterType.FilterByAddedNumber) && (tagData.AddedNumber >= filterLowerThan) || (tagData.AddedNumber < filterHigherThan))
                return;
            else if ((filterType == CopyNodeFilterType.FilterByUrgency) && (tagData.Urgency >= filterLowerThan) || (tagData.Urgency < filterHigherThan))
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
                CopyNode(to.Nodes, node, treeNodeDataCachingService, filterHigherThan, filterLowerThan, filterType);
        }

        #endregion CopyNode, CopyNodes

        #region Node percentage

        public static double GetPercentageFromChildren(TreeNode topNode)
        {
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));

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
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage));

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
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));
            if (topNode == null)
                throw new ArgumentNullException(nameof(topNode));
            if (nodeNameToDelete.IsEmpty())
                throw new ArgumentNullException(nameof(nodeNameToDelete));

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
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));

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
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

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
            if (tv == null || currentNode == null)
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

                    if (ReferenceEquals(node, currentNode))
                        foundCurrentNode = true;
                }

                MoveToNextUnfinishedNode(tv, currentNode.Parent);
            }
        }

        #endregion Nodes completed/unfinished

        #region Node search

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
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (text.IsEmpty())
                throw new ArgumentNullException(nameof(text));

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
            if (tvTaskList == null)
                throw new ArgumentNullException(nameof(tvTaskList));

            List<string> lines = new List<string>();

            foreach (TreeNode task in tvTaskList.Nodes)
                lines.Add(task.Text);

            return lines.ToArray();
        }

        public static void ClearStyleAdded(TreeNodeCollection col)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));

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

        public static void ShowNodesFromTaskToNumberOfTask(
            TreeView tv,
            decimal filterLowerThan,
            decimal filterHigherThan,
            CopyNodeFilterType filterType,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            IMediator mediator,
            IListCachingService listCachingService)
        {
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (listCachingService == null)
                throw new ArgumentNullException(nameof(listCachingService));

            if (listCachingService.Get(NodesListKey) is not TreeNodeCollection nodes)
            {
                nodes = new TreeNode().Nodes;
                listCachingService.Set(NodesListKey, nodes);
            }

            var getTreeStateRequest = new GetTreeStateRequest();
            if (Task.Run(async () => await mediator.Send(getTreeStateRequest))
            .Result is not GetTreeStateResponse getTreeStateResponse)
                return;

            // copy all
            if (!getTreeStateResponse.ReadOnlyState)
            {
                nodes.Clear();
                CopyNodes(nodes, tv.Nodes, treeNodeDataCachingService, null, null);
                listCachingService.Set(NodesListKey, nodes);
            }

            // let tvTaskList with only addedNumber < addedNumberLowerThan
            tv.Nodes.Clear();
            CopyNodes(tv.Nodes, nodes, treeNodeDataCachingService, (int)filterHigherThan, (int)filterLowerThan, filterType);

            var setTreeStateRequest = new SetTreeStateRequest
            {
                ReadOnlyState = true
            };
            Task.Run(async () =>
            {
                return await mediator.Send(setTreeStateRequest);
            }).Wait();
        }

        public static void ShowAllTasks(
            TreeView tv,
            IMediator mediator,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            IListCachingService listCachingService)
        {
            if (tv == null)
                throw new ArgumentNullException(nameof(tv));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));
            if (listCachingService == null)
                throw new ArgumentNullException(nameof(listCachingService));

            if (listCachingService.Get(NodesListKey) is not TreeNodeCollection nodes)
            {
                nodes = new TreeNode().Nodes;
                listCachingService.Set(NodesListKey, nodes);
            }

            var getTreeStateRequest = new GetTreeStateRequest();
            if (Task.Run(async () => await mediator.Send(getTreeStateRequest))
            .Result is not GetTreeStateResponse getTreeStateResponse)
                return;

            if (getTreeStateResponse.ReadOnlyState)
            {
                tv.Nodes.Clear();
                CopyNodes(tv.Nodes, nodes, treeNodeDataCachingService, null, null);

                var setTreeStateRequest = new SetTreeStateRequest
                {
                    ReadOnlyState = false
                };
                Task.Run(async () =>
                {
                    return await mediator.Send(setTreeStateRequest);
                }).Wait();
            }
        }

        #endregion Node show by urgency or added number

        #region Node data size calculation

        public static int CalculateDataSizeFromNodeAndChildren(TreeNode node, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (node == null)
                return 0;
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var tagData = node.ToTreeNodeData(treeNodeDataCachingService);
            return CalculateDataSizeFromNodeAndChildren(tagData, treeNodeDataCachingService);
        }

        public static int CalculateDataSizeFromNodeAndChildren(TreeNodeData tagData, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (tagData == null)
                return 0;
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var size = tagData.Data == null ? 0 : tagData.Data.Length;
            foreach (TreeNodeData nd in tagData.Children)
                size += CalculateDataSizeFromNodeAndChildren(nd, treeNodeDataCachingService);
            return size;
        }

        #endregion Node data size calculation

        #region Node move

        public static void MoveNode(TreeView tv,
            ITreeNodeDataCachingService treeNodeDataCachingService,
            ITreeNodeSelectionCachingService treeNodeSelectionCachingService)
        {
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));
            if (treeNodeSelectionCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeSelectionCachingService));

            var oldSelectionObj = treeNodeSelectionCachingService.GetOldSelectionFromCache();
            if (oldSelectionObj == null)
                return;

            if (oldSelectionObj is TreeNode oldSelection)
            {
                bool removedNode = false;
                var currentSelectionObj = treeNodeSelectionCachingService.GetCurrentSelectionFromCache();
                if (currentSelectionObj is TreeNode currentSelection)
                {
                    var parentSelected = currentSelection.Parent;
                    if (parentSelected != null)
                    {
                        parentSelected.Nodes.Remove(oldSelection);
                        removedNode = true;
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
            }
        }

        #endregion Node move

        /// <summary>
        /// Changes the size of a collection of nodes recursively.
        /// </summary>
        /// <param name="treeNodeCollection">Collection of nodes.</param>
        /// <param name="changedSize">Font size changed to all the nodes (added or substracted from nodes size).</param>
        public static void UpdateSizeOfTreeNodes(TreeNodeCollection treeNodeCollection, float changedSize)
        {
            if (treeNodeCollection == null)
                throw new ArgumentNullException(nameof(treeNodeCollection));

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