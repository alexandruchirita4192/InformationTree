﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using InformationTree.TextProcessing;
using NLog;

namespace InformationTree.Render.WinForms.Services
{
    public class ImportTreeFromXmlService : IImportTreeFromXmlService
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly IGraphicsFileFactory _graphicsFileRecursiveGenerator;
        private readonly ISoundProvider _soundProvider;
        private readonly IPopUpService _popUpService;
        private readonly ICompressionProvider _compressionProvider;
        
        public ImportTreeFromXmlService(
            IGraphicsFileFactory graphicsFileRecursiveGenerator,
            ISoundProvider soundProvider,
            IPopUpService popUpService,
            ICompressionProvider compressionProvider)
        {
            _graphicsFileRecursiveGenerator = graphicsFileRecursiveGenerator;
            _soundProvider = soundProvider;
            _popUpService = popUpService;
            _compressionProvider = compressionProvider;
        }

        public (TreeNodeData rootNode, string fileName) LoadTree(
            string fileName,
            Action beforeLoad,
            Action afterLoad
            )
        {
            var fileNameExists = File.Exists(fileName);
            if (fileNameExists)
            {
                var result = _popUpService.ShowCancelableQuestion($"Use default XML file {fileName}?", "Choose data file");
                if (result == PopUpResult.Yes)
                {
                    var rootNode = LoadXML(fileName, beforeLoad, afterLoad);
                    return (rootNode, fileName);
                }
                else if (result == PopUpResult.Cancel)
                {
                    if (fileNameExists)
                        fileName = GetNewFileName(fileName, _popUpService);
                    return (new TreeNodeData(), fileName);
                }
            }

            var selectedFile = _popUpService.GetXmlDataFile(fileName, fileNameExists);
            if (selectedFile.IsNotEmpty())
            {
                fileName = selectedFile;
                var rootNode = LoadXML(fileName, beforeLoad, afterLoad);
                return (rootNode, fileName);
            }

            if (fileNameExists)
                fileName = GetNewFileName(fileName, _popUpService);

            return (new TreeNodeData(), fileName);
        }

        private static string GetNewFileName(string fileName, IPopUpService popUpService)
        {
            var fileNameExists = File.Exists(fileName);
            if (!fileNameExists)
                return fileName;
            var extension = Path.GetExtension(fileName);
            if (extension == ".xml")
                return GetNewFileName($"{fileName}.1", popUpService);
            else
            {
                try
                {
                    extension = extension.Replace(".", "");
                    var newIteration = int.Parse(extension) + 1;
                    return GetNewFileName(fileName.Replace(extension, newIteration.ToString()), popUpService);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    popUpService.ShowError($"Error while trying to get new file name from file name '{fileName}'. Error occured: {ex.Message}");
                }
            }
            return null;
        }

        public TreeNodeData LoadXML(
            string fileName,
            Action beforeLoad,
            Action afterLoad 
            )
        {
            var rootNode = new TreeNodeData();
            try
            {
                beforeLoad?.Invoke();
                SplashForm.ShowDefaultSplashScreen(_graphicsFileRecursiveGenerator);
                var xDoc = new XmlDocument();
                xDoc.Load(fileName);

                if (xDoc.HasChildNodes)
                {
                    foreach (XmlElement child in xDoc.DocumentElement.ChildNodes)
                    {
                        var newNode = GetNewNodeFromTextNameAttr(child.Attributes);
                        
                        rootNode.Children.Add(newNode);
                        
                        if (child.HasChildNodes)
                            LoadTreeNodes(child, newNode);
                    }
                }
            }
            finally
            {
                SplashForm.CloseForm();
                afterLoad?.Invoke();
                _soundProvider.PlaySystemSound(4);
            }

            return rootNode;
        }

        private TreeNodeData GetNewNodeFromTextNameAttr(XmlAttributeCollection attributes)
        {
            var attrText = string.Empty;
            var attrName = string.Empty;
            var attrBold = false;
            var attrItalic = false;
            var attrUnderline = false;
            var attrStrikeout = false;
            var attrForegroundColor = Constants.Colors.DefaultForeGroundColor.Name;
            var attrBackgroundColor = Constants.Colors.DefaultBackGroundColor.Name;
            var attrFontFamily = WinFormsConstants.FontDefaults.DefaultFontFamily;
            var attrFontSize = WinFormsConstants.FontDefaults.DefaultFontSize;
            var attrPercentCompleted = 0.0M;
            var attrData = string.Empty;
            var attrAddedNumber = 0;
            var attrAddedDate = (DateTime?)null;
            var attrLastChangeDate = (DateTime?)null;
            var attrUrgency = 0;
            var attrLink = string.Empty;
            var attrCategory = string.Empty;
            var attrIsStartupAlert = false;

            foreach (XmlAttribute attr in attributes)
            {
                var decodedAttrValue = attr.Value.IsNotEmpty()
                    ? HttpUtility.HtmlDecode(attr.Value)
                    : string.Empty;

                if (attr.Name == Constants.XmlAttributes.XmlAttrText)
                    attrText = decodedAttrValue;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrName)
                    attrName = decodedAttrValue;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrBold)
                    attrBold = true;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrItalic)
                    attrItalic = true;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrUnderline)
                    attrUnderline = true;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrStrikeout)
                    attrStrikeout = true;
                else if (Constants.XmlAttributes.XmlAttrForegroundColorAcceptedList.Contains(attr.Name))
                    attrForegroundColor = decodedAttrValue;
                else if (Constants.XmlAttributes.XmlAttrBackgroundColorAcceptedList.Contains(attr.Name))
                    attrBackgroundColor = decodedAttrValue;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrFontFamily)
                    attrFontFamily = FontFamily.Families.FirstOrDefault(f => f.Name == decodedAttrValue)
                        ?? WinFormsConstants.FontDefaults.DefaultFontFamily;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrFontSize
                    && float.TryParse(decodedAttrValue, out var fontSize))
                    attrFontSize = fontSize;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrPercentCompleted
                    && decimal.TryParse(decodedAttrValue, out var percentCompleted))
                    attrPercentCompleted = percentCompleted;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrData)
                    attrData = _compressionProvider.Decompress(decodedAttrValue);
                else if (attr.Name == Constants.XmlAttributes.XmlAttrAddedNumber
                    && int.TryParse(decodedAttrValue, out var addedNumber))
                    attrAddedNumber = addedNumber;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrAddedDate)
                    attrAddedDate = decodedAttrValue.ToDateTime(_logger);
                else if (attr.Name == Constants.XmlAttributes.XmlAttrLastChangeDate)
                    attrLastChangeDate = decodedAttrValue.ToDateTime(_logger);
                else if (Constants.XmlAttributes.XmlAttrUrgencyAcceptedList.Contains(attr.Name)
                    && int.TryParse(decodedAttrValue, out var urgency))
                    attrUrgency = urgency;
                else if (Constants.XmlAttributes.XmlAttrLinkAcceptedList.Contains(attr.Name))
                    attrLink = decodedAttrValue;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrCategory)
                    attrCategory = decodedAttrValue;
                else if (attr.Name == Constants.XmlAttributes.XmlAttrIsStartupAlert
                    && bool.TryParse(decodedAttrValue, out var isStartupAlert))
                    attrIsStartupAlert = isStartupAlert;
            }

            var newStyle = (attrBold ? FontStyle.Bold : FontStyle.Regular) |
                (attrItalic ? FontStyle.Italic : FontStyle.Regular) |
                (attrUnderline ? FontStyle.Underline : FontStyle.Regular) |
                (attrStrikeout ? FontStyle.Strikeout : FontStyle.Regular);

            if (attrText != null)
                attrText = TextProcessingHelper.GetTextAndProcentCompleted(attrText, ref attrPercentCompleted, true);

            var attrDataStripped = RicherTextBox.Controls.RicherTextBox.StripRTF(attrData);
            var tooltipText = TextProcessingHelper.GetToolTipText(attrText +
                (attrName.IsNotEmpty() && attrName != "0" ? $"{Environment.NewLine} TimeSpent: {attrName}" : "") +
                (attrDataStripped.IsNotEmpty() ? $"{Environment.NewLine} Data: {attrDataStripped}" : ""));

            var treeNodeData = new TreeNodeData
            {
                Name = attrName,
                ForeColorName = attrForegroundColor,
                BackColorName = attrBackgroundColor,
                NodeFont = new TreeNodeFont
                {
                    FontFamilyName = attrFontFamily.Name,
                    Size = attrFontSize,
                    Bold = attrBold,
                    Italic = attrItalic,
                    Strikeout = attrStrikeout,
                    Underline = attrUnderline,
                },
                Text = attrText,
                Data = attrData,
                AddedNumber = attrAddedNumber,
                AddedDate = attrAddedDate,
                LastChangeDate = attrLastChangeDate,
                Urgency = attrUrgency,
                Link = attrLink,
                Category = attrCategory,
                IsStartupAlert = attrIsStartupAlert,
                ToolTipText = tooltipText
            };

            return treeNodeData;
        }

        private void LoadTreeNodes(XmlNode xmlNode, TreeNodeData treeNodeData)
        {
            if (xmlNode.HasChildNodes)
                foreach (XmlElement child in xmlNode.ChildNodes)
                {
                    var newNode = GetNewNodeFromTextNameAttr(child.Attributes);
                    treeNodeData.Children.Add(newNode);
                    if (child.HasChildNodes)
                        LoadTreeNodes(child, newNode);
                }
        }

        public bool LoadTreeNodesByCategory(TreeNodeData from, TreeNodeData to, bool includeOnlyWithStartupAlert, Dictionary<string, TreeNodeData> categories = null)
        {
            if (from == null || to == null || from.Children.Count == 0) // TODO: Check why it returns early without even clearing the children of "to" parameter
                return false;

            if (categories == null || categories.Count == 0) // TODO: Check why this is needed
                to.Children.Clear();

            if (categories == null)
                categories = new Dictionary<string, TreeNodeData>();

            foreach (TreeNodeData nodeData in from.Children)
            {
                if (nodeData != null
                    && nodeData.Category.IsNotEmpty()
                    && (
                        (includeOnlyWithStartupAlert && nodeData.IsStartupAlert)
                        || (!includeOnlyWithStartupAlert)
                    ))
                {
                    if (!categories.ContainsKey(nodeData.Category))
                        categories.Add(nodeData.Category, new TreeNodeData { Text = nodeData.Category });
                    categories[nodeData.Category].Children.Add(nodeData.Clone());
                }
                else if (nodeData != null
                    && nodeData.Category.IsEmpty()
                    && includeOnlyWithStartupAlert
                    && nodeData.IsStartupAlert)
                {
                    if (!categories.ContainsKey(string.Empty))
                        categories.Add(string.Empty, new TreeNodeData { Text = nodeData.Category });
                    categories[string.Empty].Children.Add(nodeData.Clone());
                }

                if (nodeData.Children.Count > 0)
                    LoadTreeNodesByCategory(nodeData, to, includeOnlyWithStartupAlert, categories);
            }

            foreach (var key in categories.Keys.OrderBy(c => c))
            {
                if (!to.Children.Any(e => e.Text == key))
                    to.Children.Add(categories[key]);
            }

            return categories.Count > 0;
        }
    }
}