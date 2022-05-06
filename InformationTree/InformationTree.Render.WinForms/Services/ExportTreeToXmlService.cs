using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.TextProcessing;
using InformationTree.Tree;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InformationTree.Render.WinForms.Services
{
    public class ExportTreeToXmlService : IExportTreeToXmlService
    {
        private readonly ICompressionProvider _compressionProvider;

        public ExportTreeToXmlService(ICompressionProvider compressionProvider)
        {
            _compressionProvider = compressionProvider;
        }

        public string DateTimeToString(DateTime? dt)
        {
            var convertedString = (string)null;
            if (dt.HasValue)
                convertedString = dt.Value.ToFormattedString();
            return convertedString;
        }

        public void SaveTree(TreeNodeData treeNodeData, string fileName)
        {
            if (TreeNodeHelper.IsSafeToSave && !TreeNodeHelper.TreeUnchanged)
            {
                var _streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                //Write the header
                _streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                //Write our root node
                _streamWriter.WriteLine("<root>");
                SaveNode(_streamWriter, treeNodeData, 0);
                _streamWriter.WriteLine("</root>");
                _streamWriter.Close();

                TreeNodeHelper.TreeUnchanged = true;
                TreeNodeHelper.TreeSaved = true;
                TreeNodeHelper.TreeSavedAt = DateTime.Now;
            }
        }

        private string GetTabsByIndent(int indent)
        {
            var sb = new StringBuilder();
            sb.Append('\t',indent);
            return sb.ToString();
        }

        private string GetXmlAttributeText<T>(string attribute, T value, T defaultValue = default)
        {
            return
                (value.Equals(defaultValue)
                || value == null
                || value.ToString().IsEmpty())
                ? string.Empty
                : ($"{attribute}=\"{HttpUtility.HtmlEncode(value)}\" ");
        }

        private void SaveNode(StreamWriter _streamWriter, TreeNodeData node, int indentTabs = 0)
        {
            var isRootNode = indentTabs == 0;
            if (isRootNode)
            {
                // Root node is empty and not written, it only adds it's children if any
                if (node.Children.Count > 0)
                {
                    foreach (var child in node.Children)
                    {
                        SaveNode(_streamWriter, child, indentTabs + 1);
                    }
                }
            }
            else if (indentTabs > 0)
            {
                var attrBold = false;
                var attrItalic = false;
                var attrUnderline = false;
                var attrStrikeout = false;

                if (node.NodeFont != null)
                {
                    attrBold = node.NodeFont.Bold;
                    attrItalic = node.NodeFont.Italic;
                    attrUnderline = node.NodeFont.Underline;
                    attrStrikeout = node.NodeFont.Strikeout;
                }

                var attrForegroundColor = node.ForeColorName.IsNotEmpty() ? node.ForeColorName : string.Empty;
                var attrBackgroundColor = node.BackColorName.IsNotEmpty() ? node.BackColorName : string.Empty;
                var attrFontFamily = node.NodeFont != null
                    && node.NodeFont.FontFamilyName.IsNotEmpty()
                    && node.NodeFont.FontFamilyName != WinFormsConstants.FontDefaults.DefaultFontFamily.Name
                    ? node.NodeFont.FontFamilyName
                    : string.Empty;
                var attrFontSize = node.NodeFont != null
                    ? node.NodeFont.Size.ToString()
                    : string.Empty;
                var attrText = node.Text;
                var attrPercentCompleted = node.PercentCompleted;
                var attrData = _compressionProvider.Compress(node.Data);
                var attrAddedNumber = node.AddedNumber;
                var attrAddedDate = node.AddedDate;
                var attrAddedDateFormatted = attrAddedDate.HasValue ? attrAddedDate.Value.ToFormattedString() : null;
                var attrLastChangeDate = node.LastChangeDate;
                var attrLastChangeDateFormatted = (attrAddedDate.HasValue
                    && attrLastChangeDate.HasValue
                    && attrAddedDate.Value != attrLastChangeDate.Value)
                    ? attrLastChangeDate.Value.ToFormattedString()
                    : null;
                var attrUrgency = (int?)node.Urgency;
                var attrLink = node.Link.IsEmpty() ? string.Empty : node.Link;
                var attrCategory = node.Category;
                var attrIsStartupAlert = node.IsStartupAlert;

                if (attrText != null)
                    attrText = TextProcessingHelper.GetTextAndProcentCompleted(attrText, ref attrPercentCompleted, true);

                var tagNodeLineSb = new StringBuilder();
                
                tagNodeLineSb.Append(GetTabsByIndent(indentTabs));
                tagNodeLineSb.Append(@"<node ");
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrText, attrText));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrName, node.Name, "0"));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrBold, attrBold, false));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrItalic, attrItalic, false));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrUnderline, attrUnderline, false));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrStrikeout, attrStrikeout, false));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrForegroundColor, attrForegroundColor, Constants.Colors.DefaultForeGroundColor.Name));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrBackgroundColor, attrBackgroundColor, Constants.Colors.DefaultBackGroundColor.Name));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrFontFamily, attrFontFamily));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrFontSize, attrFontSize, WinFormsConstants.FontDefaults.DefaultFontSize.ToString()));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrData, attrData));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrAddedDate, attrAddedDateFormatted));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrLastChangeDate, attrLastChangeDateFormatted));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrAddedNumber, attrAddedNumber));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrUrgency, attrUrgency?.ToString(), "0"));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrLink, attrLink));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrPercentCompleted, attrPercentCompleted, 0m));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrCategory, attrCategory));
                tagNodeLineSb.Append(GetXmlAttributeText(Constants.XmlAttributes.XmlAttrIsStartupAlert, attrIsStartupAlert));
                
                if (_streamWriter == null)
                    throw new Exception("StreamWriter \"streamWriter\" is null");

                if (node.Children.Count > 0)
                {
                    // Remove a space before ">"; we don't know which attribute is last, if any,
                    // so spaces cannot be removed by attributes (it depends based on data if attribute is added,
                    // if data is not empty)
                    tagNodeLineSb.Length -= 1;
                    tagNodeLineSb.Append(">");
                    
                    _streamWriter.WriteLine(tagNodeLineSb.ToString());
                    
                    foreach (var child in node.Children)
                    {
                        SaveNode(_streamWriter, child, indentTabs + 1);
                    }

                    var endTag = $"{GetTabsByIndent(indentTabs)}</node>";
                    _streamWriter.WriteLine(endTag);
                }
                else
                {
                    tagNodeLineSb.Append(@"/>");
                    _streamWriter.WriteLine(tagNodeLineSb.ToString());
                }
            }
        }
    }
}