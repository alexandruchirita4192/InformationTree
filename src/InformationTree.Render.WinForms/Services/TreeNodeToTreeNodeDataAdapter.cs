using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeNodeToTreeNodeDataAdapter : ITreeNodeToTreeNodeDataAdapter
    {
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;

        public TreeNodeToTreeNodeDataAdapter(ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            _treeNodeDataCachingService = treeNodeDataCachingService;
        }

        public TreeNodeData Adapt(MarshalByRefObject treeNode)
        {
            return treeNode is not TreeNode treeNodeControl ? null : InternalAdapt(treeNodeControl);
        }

        private TreeNodeData InternalAdapt(TreeNode treeNode)
        {
            if (treeNode == null)
                throw new ArgumentNullException(nameof(treeNode));

            TreeNodeData treeNodeData;
            var treeNodeIdentifier = treeNode.Tag as Guid?;

            if (treeNodeIdentifier == null)
                (treeNodeData, treeNodeIdentifier) = CreateTreeNodeDataAndAddToCache(treeNode);
            else
            {
                treeNodeData = _treeNodeDataCachingService.GetFromCache(treeNodeIdentifier.Value);
                if (treeNodeData == null)
                    (treeNodeData, treeNodeIdentifier) = CreateTreeNodeDataAndAddToCache(treeNode);
            }
            treeNode.Tag = treeNodeData != null ? treeNodeIdentifier : null;

            // Children might already be in place because the cache gives a reference to a TreeNodeData with it's children,
            // but here children are refreshed anyway
            treeNodeData.Children.Clear();

            foreach (TreeNode childTreeNode in treeNode.Nodes)
            {
                var childTreeNodeData = InternalAdapt(childTreeNode);
                treeNodeData.Children.Add(childTreeNodeData);
            }

            return treeNodeData;
        }

        public TreeNodeData AdaptTreeView(MarshalByRefObject treeView)
        {
            return treeView is not TreeView treeViewControl ? null : InternalAdaptTreeView(treeViewControl);
        }

        private TreeNodeData InternalAdaptTreeView(TreeView treeView)
        {
            var root = new TreeNodeData();

            foreach (TreeNode childTreeNode in treeView.Nodes)
            {
                var childTreeNodeData = InternalAdapt(childTreeNode);
                root.Children.Add(childTreeNodeData);
            }

            return root;
        }

        private (TreeNodeData, Guid) CreateTreeNodeDataAndAddToCache(TreeNode treeNode)
        {
            var treeNodeData = CreateNewTreeNodeData(treeNode);
            var newGuid = _treeNodeDataCachingService.AddToCache(treeNodeData);
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