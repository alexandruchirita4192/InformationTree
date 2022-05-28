using System;
using System.Collections.Generic;
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
        
        public static string[] GenerateStringGraphicsLinesFromTree(this TreeView tvTaskList)
        {
            if (tvTaskList == null)
                throw new ArgumentNullException(nameof(tvTaskList));

            var lines = new List<string>();

            foreach (TreeNode task in tvTaskList.Nodes)
                lines.Add(task.Text);

            return lines.ToArray();
        }
    }
}