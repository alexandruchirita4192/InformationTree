using System;
using System.Drawing;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeExtensions
    {
        public static void ExpandParents(this TreeNode node)
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

        public static void Copy(
            this TreeNode destination,
            TreeNode source,
            ITreeNodeToTreeNodeDataAdapter treeNodeToTreeNodeDataAdapter,
            bool includeChildren = true,
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

            var sourceTreeNodeData = treeNodeToTreeNodeDataAdapter.Adapt(source);

            // Filter nodes by added number or urgency depending on the type
            if ((filterType == CopyNodeFilterType.FilterByAddedNumber) && (sourceTreeNodeData.AddedNumber >= filterLowerThan) || (sourceTreeNodeData.AddedNumber < filterHigherThan))
                return;
            else if ((filterType == CopyNodeFilterType.FilterByUrgency) && (sourceTreeNodeData.Urgency >= filterLowerThan) || (sourceTreeNodeData.Urgency < filterHigherThan))
                return;

            var destinationTreeNodeData = treeNodeToTreeNodeDataAdapter.Adapt(destination);
            destinationTreeNodeData.Copy(sourceTreeNodeData);

            // Shallow copy of the used properties only
            destination.Text = source.Text;
            destination.Name = source.Name;
            destination.ToolTipText = source.ToolTipText;

            var font = source.NodeFont;
            destination.NodeFont = font != null ? new Font(font.FontFamily, font.SizeInPoints, font.Style) : WinFormsConstants.FontDefaults.DefaultFont;

            destination.ForeColor = source.ForeColor.IsEmpty ? Constants.Colors.DefaultForeGroundColor : source.ForeColor;
            destination.BackColor = source.BackColor.IsEmpty ? Constants.Colors.DefaultBackGroundColor : source.BackColor;

            if (includeChildren)
            {
                foreach (TreeNode node in source.Nodes)
                    destination.Nodes.Copy(node, treeNodeToTreeNodeDataAdapter, filterHigherThan, filterLowerThan, filterType);
            }
        }
    }
}