using System;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeCollectionExtensions
    {
        public static void Copy(
            this TreeNodeCollection destination,
            TreeNodeCollection source,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            int? filterHigherThan = null,
            int? filterLowerThan = null,
            CopyNodeFilterType filterType = CopyNodeFilterType.NoFilter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (treeNodeToTreeNodeDataAdapter == null)
                throw new ArgumentNullException(nameof(treeNodeToTreeNodeDataAdapter));

            foreach (TreeNode node in source)
                destination.Copy(node, treeNodeToTreeNodeDataAdapter, filterHigherThan, filterLowerThan, filterType);
        }

        public static void Copy(
            this TreeNodeCollection destination,
            TreeNode source,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            int? filterHigherThan = null,
            int? filterLowerThan = null,
            CopyNodeFilterType filterType = CopyNodeFilterType.NoFilter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (treeNodeToTreeNodeDataAdapter == null)
                throw new ArgumentNullException(nameof(treeNodeToTreeNodeDataAdapter));

            var node = new TreeNode();
            node.Copy(source, treeNodeToTreeNodeDataAdapter, true, filterHigherThan, filterLowerThan, filterType);

            if ((filterLowerThan == null && filterHigherThan == null) || (filterLowerThan != null && filterHigherThan != null))
                destination.Add(node);
        }

        public static void ClearStyleAdded(this TreeNodeCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Count > 0)
            {
                foreach (TreeNode node in collection)
                {
                    if (node.BackColor != Constants.Colors.DefaultBackGroundColor)
                        node.BackColor = Constants.Colors.DefaultBackGroundColor;
                    if (node.ForeColor != Constants.Colors.DefaultForeGroundColor)
                        node.ForeColor = Constants.Colors.DefaultForeGroundColor;

                    if (node.Nodes.Count > 0)
                        node.Nodes.ClearStyleAdded();
                }
            }
        }

        public static void SetStyleForSearch(this TreeNodeCollection collection, string text, ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (text.IsEmpty())
                throw new ArgumentNullException(nameof(text));
            if (treeNodeToTreeNodeDataAdapter == null)
                throw new ArgumentNullException(nameof(treeNodeToTreeNodeDataAdapter));

            text = text.ToLower();
            if (collection.Count > 0)
            {
                foreach (TreeNode node in collection)
                {
                    var nodeTagData = treeNodeToTreeNodeDataAdapter.Adapt(node);
                    var nodeData = nodeTagData != null && nodeTagData.Data.IsNotEmpty() ? nodeTagData.Data : null;
                    var foundCondition = (node.Text != null && node.Text.ToLower().Split('[')[0].Contains(text))
                        || (nodeData != null && nodeData.ToLower().Contains(text));

                    if (foundCondition
                        && (node.BackColor != Constants.Colors.BackGroundColorSearch
                        || node.ForeColor != Constants.Colors.ForeGroundColorSearch))
                    {
                        node.BackColor = Constants.Colors.BackGroundColorSearch;
                        node.ForeColor = Constants.Colors.ForeGroundColorSearch;
                        node.Expand();
                        node.ExpandParents();
                    }

                    if (node.Nodes != null && node.Nodes.Count > 0)
                        node.Nodes.SetStyleForSearch(text, treeNodeToTreeNodeDataAdapter);
                }
            }
        }
    }
}