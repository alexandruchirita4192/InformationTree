﻿using System;
using System.Drawing;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeDataExtensions
    {
        public static TreeNode ToTreeNode(this TreeNodeData treeNodeData, ITreeNodeDataCachingService treeNodeDataCachingService, bool includeChildren = true)
        {
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            var guid = treeNodeDataCachingService.AddToCache(treeNodeData);

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
                    var childTreeNode = childTreeNodeData.ToTreeNode(treeNodeDataCachingService, includeChildren);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        public static void CopyToTreeView(this TreeNodeData treeNodeData, TreeView tv, ITreeNodeDataCachingService treeNodeDataCachingService, bool includeChildren = true)
        {
            if (tv?.Nodes == null)
                throw new ArgumentNullException(nameof(tv));
            if (treeNodeDataCachingService == null)
                throw new ArgumentNullException(nameof(treeNodeDataCachingService));

            tv.Nodes.Clear();

            var isEmptyRootNode = treeNodeData.IsEmptyData;
            if (isEmptyRootNode)
            {
                foreach (var childTreeNodeData in treeNodeData.Children)
                {
                    var childTreeNode = childTreeNodeData.ToTreeNode(treeNodeDataCachingService, includeChildren);
                    tv.Nodes.Add(childTreeNode);
                }
            }
            else
            {
                var treeNode = treeNodeData.ToTreeNode(treeNodeDataCachingService, includeChildren);
                tv.Nodes.Add(treeNode);
            }

            tv.ExpandAll();
        }

        public static Color GetTaskNameColor(this TreeNodeData tagData)
        {
            return tagData.Data.IsEmpty()
                ? (
                    tagData.Link.IsEmpty()
                    ? Constants.Colors.DefaultBackGroundColor
                    : Constants.Colors.LinkBackGroundColor
                )
                : Constants.Colors.DataBackGroundColor;
        }

        public static TreeNodeData GetFirstNode(this TreeNodeData rootNode, string text)
        {
            TreeNodeData ret = null;
            text = text.ToLower();
            
            if (rootNode.Children.Count > 0)
            {
                foreach (TreeNodeData child in rootNode.Children)
                {
                    var nodeData = child.Data.IsNotEmpty() ? child.Data : null;
                    var foundInText = child.Text.IsNotEmpty()
                        && child.Text.ToLower().Split('[')[0].Contains(text);
                    var foundInData = nodeData != null && nodeData.ToLower().Contains(text);
                    var foundCondition = foundInText || foundInData;
                    if (foundCondition)
                        return child;

                    if (child.Children.Count > 0)
                    {
                        ret = GetFirstNode(child, text);
                        if (ret != null)
                            return ret;
                    }
                }
            }

            return ret;
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