using System.Windows.Forms;
using InformationTree.Domain.Entities;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeExtensions
    {
        public static TreeNodeData GetTreeNodeData(this TreeNode treeNode)
        {
            if (treeNode == null)
                return null;

            // TODO: Fix this way of associating TreeNodeData to TreeNode
            // Keeping data in TreeNode.Tag is not a good idea because slows down the application because TreeNode is a control on the TreeView
            // Keeping the tree separated as a TreeNodeData tree and rendering it to TreeView when required is a better idea
            if (treeNode.Tag is not TreeNodeData treeNodeData)
            {
                treeNodeData = new TreeNodeData();
                treeNode.Tag = treeNodeData;
            }

            return treeNodeData;
        }
    }
}