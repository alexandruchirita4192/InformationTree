using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeExtensions
    {
        public static void Copy(this TreeNode destination, TreeNode source, bool includeChildren = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            destination.Text = source.Text;
            destination.Name = source.Name;
            destination.NodeFont = source.NodeFont;
            destination.Tag = source.Tag; // TODO: Maybe create another TreeNodeData for the Tag, even if it requires other services?
            destination.ToolTipText = source.ToolTipText;
            destination.ImageIndex = source.ImageIndex;
            destination.SelectedImageIndex = source.SelectedImageIndex;
            destination.StateImageIndex = source.StateImageIndex;
            destination.ContextMenuStrip = source.ContextMenuStrip;
            destination.Checked = source.Checked;

            if (includeChildren)
            {
                destination.Nodes.Clear();

                foreach (TreeNode child in source.Nodes)
                {
                    TreeNode newChild = destination.Nodes.Add(child.Text);
                    newChild.Copy(child);
                }
            }
        }

        public static TreeNodeData ToTreeNodeData(this TreeNode treeNode, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (treeNode == null)
                throw new ArgumentNullException(nameof(treeNode));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            TreeNodeData treeNodeData;
            var treeNodeIdentifier = treeNode.Tag as Guid?;

            if (treeNodeIdentifier == null)
                (treeNodeData, treeNodeIdentifier) = treeNode.CreateTreeNodeDataAndAddToCache(treeNodeDataCachingService);
            else
            {
                treeNodeData = treeNodeDataCachingService.GetFromCache(treeNodeIdentifier.Value);
                if (treeNodeData == null)
                    (treeNodeData, treeNodeIdentifier) = treeNode.CreateTreeNodeDataAndAddToCache(treeNodeDataCachingService);
            }
            treeNode.Tag = treeNodeData != null ? treeNodeIdentifier : null;

            // Children might already be in place because the cache gives a reference to a TreeNodeData with it's children,
            // but here children are refreshed anyway
            treeNodeData.Children.Clear();

            foreach (TreeNode childTreeNode in treeNode.Nodes)
            {
                var childTreeNodeData = childTreeNode.ToTreeNodeData(treeNodeDataCachingService);
                treeNodeData.Children.Add(childTreeNodeData);
            }

            return treeNodeData;
        }

        private static (TreeNodeData, Guid) CreateTreeNodeDataAndAddToCache(this TreeNode treeNode, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            var treeNodeData = CreateNewTreeNodeData(treeNode);
            var newGuid = treeNodeDataCachingService.AddToCache(treeNodeData);
            return (treeNodeData, newGuid);
        }

        private static TreeNodeData CreateNewTreeNodeData(TreeNode treeNode)
        {
            var fontFamily = treeNode.NodeFont?.FontFamily ?? WinFormsConstants.FontDefaults.DefaultFontFamily;

            return new TreeNodeData
            {
                Text = treeNode.Text,
                Name = treeNode.Name,
                NodeFont = new TreeNodeFont
                {
                    Bold = treeNode.NodeFont?.Bold ?? false,
                    Italic = treeNode.NodeFont?.Italic ?? false,
                    Strikeout = treeNode.NodeFont?.Strikeout ?? false,
                    Underline = treeNode.NodeFont?.Underline ?? false,
                    Size = treeNode.NodeFont?.Size ?? WinFormsConstants.FontDefaults.DefaultFontSize,
                    FontFamilyName = fontFamily?.Name
                },
                AddedDate = DateTime.Now,
                LastChangeDate = DateTime.Now
            };
        }
    }
}