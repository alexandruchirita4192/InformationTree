using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Extensions;
using MediatR;
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
        private readonly IMediator _mediator;

        public ImportTreeFromXmlService(
            IGraphicsFileFactory graphicsFileRecursiveGenerator,
            ISoundProvider soundProvider,
            IPopUpService popUpService,
            ICompressionProvider compressionProvider,
            IMediator mediator)
        {
            _graphicsFileRecursiveGenerator = graphicsFileRecursiveGenerator;
            _soundProvider = soundProvider;
            _popUpService = popUpService;
            _compressionProvider = compressionProvider;
            _mediator = mediator;
        }

        public (TreeNodeData rootNode, string fileName) LoadTree(
            string fileName,
            Component controlToSetWaitCursor
            )
        {
            var fileNameExists = File.Exists(fileName);
            if (fileNameExists)
            {
                var result = _popUpService.ShowCancelableQuestion($"Use default XML file {fileName}?", "Choose data file");
                if (result == PopUpResult.Yes)
                {
                    var rootNode = LoadXML(fileName, controlToSetWaitCursor);
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
                var rootNode = LoadXML(fileName, controlToSetWaitCursor);
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
            Component controlToSetWaitCursor
            )
        {
            var rootNode = new TreeNodeData();
            try
            {
                if (controlToSetWaitCursor is Control control)
                {
                    var waitCursorRequest = new SetControlCursorRequest
                    {
                        Control = control,
                        IsWaitCursor = true
                    };
                    Task.Run(async () => await _mediator.Send(waitCursorRequest))
                        .Wait();
                }

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

                if (controlToSetWaitCursor is Control control)
                {
                    var defaultCursorRequest = new SetControlCursorRequest
                    {
                        Control = control,
                        IsWaitCursor = false
                    };
                    Task.Run(async () => await _mediator.Send(defaultCursorRequest))
                        .Wait();
                }

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
                var attrValue = attr.Value.IsNotEmpty() ? attr.Value : string.Empty;
                var decodedAttrValue = attrValue.IsNotEmpty()
                    ? HttpUtility.HtmlDecode(attrValue)
                    : string.Empty;

                switch (attr.Name)
                {
                    case Constants.XmlAttributes.XmlAttrText:
                        attrText = decodedAttrValue;
                        break;

                    case Constants.XmlAttributes.XmlAttrName:
                        attrName = decodedAttrValue;
                        break;

                    case Constants.XmlAttributes.XmlAttrBold:
                        attrBold = true;
                        break;

                    case Constants.XmlAttributes.XmlAttrItalic:
                        attrItalic = true;
                        break;

                    case Constants.XmlAttributes.XmlAttrUnderline:
                        attrUnderline = true;
                        break;

                    case Constants.XmlAttributes.XmlAttrStrikeout:
                        attrStrikeout = true;
                        break;

                    case Constants.XmlAttributes.XmlAttrFontFamily:
                        attrFontFamily = FontFamily.Families.FirstOrDefault(f => 
                            string.Compare(f.Name, decodedAttrValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                            ?? WinFormsConstants.FontDefaults.DefaultFontFamily;
                        break;

                    case Constants.XmlAttributes.XmlAttrData:
                        attrData = _compressionProvider.Decompress(decodedAttrValue);
                        
                        if (string.Compare(attrData, decodedAttrValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            _logger.Debug("Decompressed data '{data}' is the same as compressed data, " +
                                "decompressing again using the fallback mechanism without using the HtmlDecode returned data," +
                                " but using the data before decoding with HtmlDecode instead: '{attrValue}'.",
                                attrData,
                                attrValue);
                            
                            // Fallback mechanism in case of issues because of HtmlDecode
                            // (hopefully that's not because of older version of CompressionProvider using UrlTokenDecode)
                            attrData = _compressionProvider.Decompress(attrValue);

                            // If even this got the same value, then assume it's not compressed and get something after decoding part (as usually should happen)
                            if (string.Compare(attrData, attrValue, StringComparison.InvariantCultureIgnoreCase) == 0)
                                attrData = decodedAttrValue;
                        }
                        break;

                    case Constants.XmlAttributes.XmlAttrAddedDate:
                        attrAddedDate = decodedAttrValue.ToDateTime(_logger);
                        break;

                    case Constants.XmlAttributes.XmlAttrLastChangeDate:
                        attrLastChangeDate = decodedAttrValue.ToDateTime(_logger);
                        break;

                    case Constants.XmlAttributes.XmlAttrCategory:
                        attrCategory = decodedAttrValue;
                        break;

                    default:
                        if (Constants.XmlAttributes.XmlAttrForegroundColorAcceptedList.Contains(attr.Name))
                            attrForegroundColor = decodedAttrValue;
                        else if (Constants.XmlAttributes.XmlAttrBackgroundColorAcceptedList.Contains(attr.Name))
                            attrBackgroundColor = decodedAttrValue;
                        else if (Constants.XmlAttributes.XmlAttrUrgencyAcceptedList.Contains(attr.Name)
                            && int.TryParse(decodedAttrValue, out var urgency))
                            attrUrgency = urgency;
                        else if (Constants.XmlAttributes.XmlAttrLinkAcceptedList.Contains(attr.Name))
                            attrLink = decodedAttrValue;
                        else if (attr.Name == Constants.XmlAttributes.XmlAttrFontSize
                            && float.TryParse(decodedAttrValue, out var fontSize))
                            attrFontSize = fontSize;
                        else if (attr.Name == Constants.XmlAttributes.XmlAttrPercentCompleted
                            && decimal.TryParse(decodedAttrValue, out var percentCompleted))
                            attrPercentCompleted = percentCompleted;
                        else if (attr.Name == Constants.XmlAttributes.XmlAttrAddedNumber
                            && int.TryParse(decodedAttrValue, out var addedNumber))
                            attrAddedNumber = addedNumber;
                        else if (attr.Name == Constants.XmlAttributes.XmlAttrIsStartupAlert
                            && bool.TryParse(decodedAttrValue, out var isStartupAlert))
                            attrIsStartupAlert = isStartupAlert;
                        break;
                }
            }

            var newStyle = GetStyleFrom(attrBold, attrItalic, attrUnderline, attrStrikeout);
            
            attrPercentCompleted = attrPercentCompleted.ValidatePercentage();

            var attrDataStripped = attrData.StripRTF();
            var tooltipText = (attrText +
                (attrName.IsNotEmpty() && attrName != "0" ? $"{Environment.NewLine} TimeSpent: {attrName}" : "") +
                (attrDataStripped.IsNotEmpty() ? $"{Environment.NewLine} Data: {attrDataStripped}" : ""))
                .GetToolTipText();

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
                ToolTipText = tooltipText,
                PercentCompleted = attrPercentCompleted
            };

            return treeNodeData;
        }

        private static FontStyle GetStyleFrom(bool bold, bool italic, bool underline, bool strikeout)
        {
            return (bold ? FontStyle.Bold : FontStyle.Regular) |
                (italic ? FontStyle.Italic : FontStyle.Regular) |
                (underline ? FontStyle.Underline : FontStyle.Regular) |
                (strikeout ? FontStyle.Strikeout : FontStyle.Regular);
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

        public bool LoadTreeNodesByCategory(TreeNodeData source, TreeNodeData destination, bool includeOnlyWithStartupAlert, Dictionary<string, TreeNodeData> categories = null)
        {
            if (source == null || destination == null)
                return false;

            if (source.Children.Count == 0)
            {
                // Not much to do here
                destination.Children.Clear();
                return false;
            }

            // Clear the children of destination tree only once, because this function is called recursively
            // (and immediately categories will be not null)
            var isFirstCall = categories == null;
            if (isFirstCall)
                destination.Children.Clear();

            if (categories == null)
                categories = new Dictionary<string, TreeNodeData>();

            foreach (TreeNodeData nodeData in source.Children)
            {
                if (nodeData != null
                    && nodeData.Category.IsNotEmpty()
                    && (
                        (includeOnlyWithStartupAlert && nodeData.IsStartupAlert)
                        || (!includeOnlyWithStartupAlert)
                    ))
                {
                    // Create a node with category data and add a clone of the real node as a child
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
                        categories.Add(string.Empty, new TreeNodeData { Text = "Category without text" });
                    categories[string.Empty].Children.Add(nodeData.Clone());
                }

                if (nodeData.Children.Count > 0)
                    LoadTreeNodesByCategory(nodeData, destination, includeOnlyWithStartupAlert, categories);
            }

            // Add missing categories to the destination tree
            foreach (var key in categories.Keys.OrderBy(c => c))
            {
                if (!destination.Children.Any(e => string.Compare(e.Text, key, StringComparison.InvariantCultureIgnoreCase) == 0))
                    destination.Children.Add(categories[key]);
            }

            return categories.Count > 0;
        }
    }
}