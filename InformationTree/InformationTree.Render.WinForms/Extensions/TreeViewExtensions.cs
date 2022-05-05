using System;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeViewExtensions
    {
        public static TreeNodeData ToTreeNodeData(this TreeView treeView, ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var root = new TreeNodeData();

            foreach (TreeNode childTreeNode in treeView.Nodes)
            {
                var childTreeNodeData = childTreeNode.ToTreeNodeData(treeNodeDataCachingService);
                root.Children.Add(childTreeNodeData);
            }

            return root;
        }
    }
}