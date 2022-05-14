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
        public const string BulletLevel1 = "• ";
        public const string BulletLevel2 = "◦ ";
        public const string BulletLevel3 = "▪ ";
        public const string Line = "\\line";

        public string GetRtfExport(TreeNodeData node)
        {
            var sb = new StringBuilder();
            sb.Append(RtfHeader);

            GetRtfExportRecursively(node, sb);

            sb.AppendLine(RtfFooter);

            return sb.ToString();
        }

        private void GetRtfExportRecursively(TreeNodeData node, StringBuilder sb, int currentBulletLevel = 0)
        {
            var bulletRtf = GetBulletForLevel(currentBulletLevel);

            sb.AppendLine($"{bulletRtf}{SanitizeBasicTextForRtf(node.Text)}{AddExtraColonForChildren(node)}{Line}");

            if (node.Children != null && node.Children.Count > 0)
            {
                currentBulletLevel++;

                foreach (var child in node.Children)
                {
                    GetRtfExportRecursively(child, sb, currentBulletLevel);
                }
            }
        }

        private string GetBulletForLevel(int currentBulletLevel)
        {
            if (currentBulletLevel == 0)
                return string.Empty;

            var sb = new StringBuilder();

            sb.Append('\t', currentBulletLevel);

            var bulletTypeNumber = currentBulletLevel % 3;
            switch (bulletTypeNumber)
            {
                case 0:
                    sb.Append($"{SanitizeBasicTextForRtf(BulletLevel3)}");
                    break;
                case 1:
                    sb.Append($"{SanitizeBasicTextForRtf(BulletLevel1)}");
                    break;
                case 2:
                    sb.Append($"{SanitizeBasicTextForRtf(BulletLevel2)}");
                    break;
            }

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