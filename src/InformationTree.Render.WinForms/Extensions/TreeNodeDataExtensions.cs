using System.Drawing;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class TreeNodeDataExtensions
    {
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

        public static TreeNodeData GetFirstNodeWith(this TreeNodeData rootNode, string text)
        {
            TreeNodeData ret = null;
            text = text.ToLower();

            if (rootNode.Children.Count > 0)
            {
                foreach (TreeNodeData child in rootNode.Children)
                {
                    var foundInText = child.Text.IsNotEmpty()
                        && child.Text.ToLower().Split('[')[0].Contains(text);
                    if (foundInText)
                        return child;

                    if (child.Children.Count > 0)
                    {
                        ret = GetFirstNodeWith(child, text);
                        if (ret != null)
                            return ret;
                    }
                }
            }

            return ret;
        }
    }
}