using System;
using System.Text;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Services;

namespace InformationTree.Render.WinForms.Services
{
    public class ExportNodeToRtfService : IExportNodeToRtfService
    {
        public const string RtfHeader = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}";
        public const string RtfFooter = @"}";
        public const string BulletLevel1 = "●\\tab ";
        public const string BulletLevel2 = "○\\tab ";
        public const string Line = "\\line";

        public string GetRtfExport(TreeNodeData node)
        {
            var sb = new StringBuilder();

            sb.Append(RtfHeader);

            sb.AppendLine($"{SanitizeBasicTextForRtf(node.Text)}{AddExtraColonForChildren(node)}{Line}");

            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var child in node.Children)
                {
                    sb.AppendLine($"\t{SanitizeBasicTextForRtf(BulletLevel1)}{SanitizeBasicTextForRtf(child.Text)}{AddExtraColonForChildren(child)}{Line}");

                    if (child.Children != null && child.Children.Count > 0)
                    {
                        foreach (var grandChild in child.Children)
                        {
                            sb.AppendLine($"\t\t{SanitizeBasicTextForRtf(BulletLevel2)}{SanitizeBasicTextForRtf(grandChild.Text)}{Line}");
                        }
                    }
                }
            }
            
            sb.AppendLine(RtfFooter);
            
            return sb.ToString();
        }

        private string SanitizeBasicTextForRtf(string text)
        {
            return text != null
                ? GetRtfUnicodeEscapedString(text).Replace("{", "\'7b").Replace("}", "\'7d")
                : string.Empty;
        }

        static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }
        
        private string AddExtraColonForChildren(TreeNodeData treeNodeData)
        {
            return treeNodeData == null || treeNodeData.Children == null || treeNodeData.Children.Count == 0
                ? string.Empty
                : ":";
        }
    }
}