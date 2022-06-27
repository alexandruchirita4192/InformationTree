using System;
using System.Drawing;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class TreeNodeDataToTreeNodeAdapter : ITreeNodeDataToTreeNodeAdapter
    {
        private readonly ITreeNodeDataCachingService _treeNodeDataCachingService;

        public TreeNodeDataToTreeNodeAdapter(ITreeNodeDataCachingService treeNodeDataCachingService)
        {
            _treeNodeDataCachingService = treeNodeDataCachingService;
        }
        
        public MarshalByRefObject Adapt(TreeNodeData treeNodeData, bool includeChildren = true)
        {
            return AdaptToTreeNode(treeNodeData, includeChildren);
        }
        
        private TreeNode AdaptToTreeNode(TreeNodeData treeNodeData, bool includeChildren = true)
        {
            var guid = _treeNodeDataCachingService.AddToCache(treeNodeData);

            var background = StringToColor(treeNodeData.BackColorName) ?? Constants.Colors.DefaultBackGroundColor;
            var foreground = StringToColor(treeNodeData.ForeColorName) ?? Constants.Colors.DefaultForeGroundColor;
            var nodeFont = treeNodeData?.NodeFont;
            var fontStyle = GetFontStyle(nodeFont);
            var font = GetFont(nodeFont, fontStyle);

            var treeNode = new TreeNode(treeNodeData.Text)
            {
                Name = treeNodeData.Name,
                NodeFont = font,
                BackColor = background,
                ForeColor = foreground,
                Tag = guid,
                ToolTipText = treeNodeData.ToolTipText
            };

            if (includeChildren)
            {
                foreach (var childTreeNodeData in treeNodeData.Children)
                {
                    var childTreeNode = AdaptToTreeNode(childTreeNodeData, includeChildren);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        public void AdaptToTreeView(TreeNodeData treeNodeData, MarshalByRefObject treeView, bool includeChildren = true)
        {
            if (treeView is not TreeView treeViewControl)
                return;
            AdaptToTreeView(treeNodeData, treeViewControl, includeChildren);
        }
        
        public void AdaptToTreeView(TreeNodeData treeNodeData, TreeView treeView, bool includeChildren = true)
        {
            if (treeView?.Nodes == null)
                throw new ArgumentNullException(nameof(treeView));

            treeView.Nodes.Clear();

            var isEmptyRootNode = treeNodeData.IsEmptyData;
            if (isEmptyRootNode)
            {
                foreach (var childTreeNodeData in treeNodeData.Children)
                {
                    var childTreeNode = AdaptToTreeNode(childTreeNodeData, includeChildren);
                    treeView.Nodes.Add(childTreeNode);
                }
            }
            else
            {
                var treeNode = AdaptToTreeNode(treeNodeData, includeChildren);
                treeView.Nodes.Add(treeNode);
            }

            treeView.ExpandAll();
        }


        private static Font GetFont(TreeNodeFont nodeFont, FontStyle fontStyle)
        {
            return nodeFont == null
                || nodeFont.FontFamilyName.IsEmpty()
                || nodeFont.Size <= 0
                ? null
                : new Font(nodeFont.FontFamilyName, nodeFont.Size, fontStyle);
        }

        private static FontStyle GetFontStyle(TreeNodeFont nodeFont)
        {
            return nodeFont == null
                ? FontStyle.Regular
                : FontStyle.Regular
                | (nodeFont.Italic ? FontStyle.Italic : FontStyle.Regular)
                | (nodeFont.Bold ? FontStyle.Bold : FontStyle.Regular)
                | (nodeFont.Strikeout ? FontStyle.Strikeout : FontStyle.Regular)
                | (nodeFont.Underline ? FontStyle.Underline : FontStyle.Regular);
        }

        private static Color? StringToColor(string s)
        {
            var convertedColor = s.IsNotEmpty() ? Color.FromName(s) : (Color?)null;
            return convertedColor;
        }
    }
}