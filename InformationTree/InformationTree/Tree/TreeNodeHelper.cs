using InformationTree.Domain.Entities;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Forms;
using InformationTree.Render.WinForms.Services;
using InformationTree.TextProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace InformationTree.Tree
{
    [Obsolete("Break into many classes with many purposes")] 
    // TODO: file reading/writing purpose (XML), Tree state (Composite/Object tree with it's state), some configuration file with colors, some constant file with attributes of XML parsing
    public static class TreeNodeHelper
    {
        #region Constants

        #region Colors

        public static Color DefaultBackGroundColor = Color.White;
        public static Color DefaultForeGroundColor = Color.Black;

        public static Color BackGroundColorSearch = Color.SlateGray;
        public static Color ForeGroundColorSearch = Color.LightBlue;

        public static Color ExceptionColor = Color.Red;

        public static Color LinkBackGroundColor = Color.DarkCyan;

        public static Color DataBackGroundColor = Color.FromArgb(200, 200, 200);

        #endregion Colors

        #region XML Attributes

        private const string XmlAttrText = "text";
        private const string XmlAttrName = "name";
        private const string XmlAttrBold = "bold";
        private const string XmlAttrItalic = "italic";
        private const string XmlAttrUnderline = "underline";
        private const string XmlAttrStrikeout = "strikeout";
        private const string XmlAttrForegroundColor = "foreground";
        private const string XmlAttrBackgroundColor = "background";
        private const string XmlAttrFontFamily = "fontFamily";
        private const string XmlAttrFontSize = "fontSize";
        private const string XmlAttrAddedNumber = "addedNumber";
        private const string XmlAttrAddedDate = "addedDate";
        private const string XmlAttrLastChangeDate = "lastChangeDate";
        private const string XmlAttrUrgency = "urgency";
        private const string XmlAttrLink = "link";
        private const string XmlAttrCategory = "category";
        private const string XmlAttrIsStartupAlert = "isStartupAlert";

        private const string XmlAttrData = "data";

        private const string XmlAttrPercentCompleted = "percentCompleted";

        #region Old XML Attribute Names

        private static List<string> XmlAttrForegroundColorAcceptedList
        {
            get
            {
                return new List<string> { XmlAttrForegroundColor, "foreColor", "color" };
            }
        }

        private static List<string> XmlAttrBackgroundColorAcceptedList
        {
            get
            {
                return new List<string> { XmlAttrBackgroundColor, "backColor" };
            }
        }

        private static List<string> XmlAttrUrgencyAcceptedList
        {
            get
            {
                return new List<string> { XmlAttrUrgency, "attrUrgency" };
            }
        }

        private static List<string> XmlAttrLinkAcceptedList
        {
            get
            {
                return new List<string> { XmlAttrLink, "attrLink" };
            }
        }

        #endregion Old XML Attribute Names

        #endregion XML Attributes

        // TODO: Create some datetime parsing service that tries both parsing formats and use it everywhere those 2 constants are used
        public const string DateTimeFormatSeparatedWithDot = "dd.MM.yyyy HH:mm:ss";
        public const string DateTimeFormatSeparatedWithSlash = "dd/MM/yyyy HH:mm:ss";

        #endregion Constants

        #region Properties

        #region FileName

        private static string fileName;

        public static string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(fileName))
                    return fileName;
                return "Data.xml";
            }
            set
            {
                fileName = value;
            }
        }

        #endregion FileName

        public static bool IsSafeToSave;
        public static bool ReadOnlyState;

        private static bool _treeUnchanged;

        public static bool TreeUnchanged
        {
            get
            {
                return _treeUnchanged;
            }
            set
            {
                if (_treeUnchanged != value)
                {
                    File.AppendAllText("TreeUnchangedIssue", $"Tree unchanged set as {value} was called by {new StackTrace()} at {DateTime.Now}");
                    _treeUnchanged = value;

                    if (TreeUnchangedChangeDelegate != null)
                        TreeUnchangedChangeDelegate(value);
                }
            }
        }

        public static bool TreeSaved { get; private set; }
        public static DateTime TreeSavedAt { get; private set; }

        public static Action<bool> TreeUnchangedChangeDelegate;

        public static int TreeNodeCounter = 0;
        private static StreamWriter streamWriter;
        private static TreeNodeCollection nodes;

        private static TreeNode currentSelection;
        private static TreeNode oldSelection;

        #endregion Properties

        #region Fix Tree

        public static bool TreeNodesNeedFix(TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                var tagTreeNodeData = tn.Tag as TreeNodeData;
                if (tagTreeNodeData == null || tagTreeNodeData.AddedNumber == 0)
                    return true;

                if (TreeNodesNeedFix(tn.Nodes))
                    return true;
            }
            return false;
        }

        public static void FixTreeNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode tn in nodes)
            {
                var tagTreeNodeData = tn.Tag as TreeNodeData;
                if (tagTreeNodeData == null)
                    tagTreeNodeData = new TreeNodeData(null, TreeNodeCounter, DateTime.Now, DateTime.Now, 0, null);
                else
                {
                    if (tagTreeNodeData.AddedDate == null)
                        tagTreeNodeData.AddedDate = DateTime.Now;
                    tagTreeNodeData.AddedNumber = TreeNodeCounter;
                }
                TreeNodeCounter++;
                tn.Tag = tagTreeNodeData;

                FixTreeNodes(tn.Nodes);
            }
        }

        public static void FixTreeNodesAndResetCounter(TreeNodeCollection nodes)
        {
            if (TreeNodesNeedFix(nodes))
            {
                TreeNodeCounter = 1;
                FixTreeNodes(nodes);
            }
        }

        #endregion Fix Tree

        #region Load

        public static DateTime? StringToDateTime(string s)
        {
            var convertedDateTime = (DateTime?)null;
            if (!string.IsNullOrEmpty(s))
            {
                try { convertedDateTime = DateTime.ParseExact(s, DateTimeFormatSeparatedWithDot, CultureInfo.InvariantCulture); } catch { }
                if (convertedDateTime == null)
                    try { convertedDateTime = DateTime.ParseExact(s, DateTimeFormatSeparatedWithSlash, CultureInfo.InvariantCulture); } catch { }
                if (convertedDateTime == null)
                    try { convertedDateTime = DateTime.Parse(s); } catch { }
            }
            return convertedDateTime;
        }

        public static Color? StringToColor(string s)
        {
            var convertedColor = (Color?)null;
            if (!string.IsNullOrEmpty(s))
                try { convertedColor = Color.FromName(s); } catch (Exception ex) { WinFormsApplication.GlobalExceptionHandling(ex); }
            return convertedColor;
        }

        public static bool LoadTree(Form t, TreeView tv, string fileName, IGraphicsFileFactory graphicsFileRecursiveGenerator)
        {
            var fileNameExists = File.Exists(fileName);
            if (fileNameExists)
            {
                var result = MessageBox.Show("Use default XML file [" + fileName + "]?", "Data file", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    FileName = fileName;
                    LoadXML(t, tv, graphicsFileRecursiveGenerator);
                    return true;
                }
                else if (result == DialogResult.Cancel)
                {
                    if (fileNameExists)
                        FileName = GetNewFileName(fileName);
                    return false;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open XML Document";
            dlg.Filter = "XML Files (*.xml)|*.xml*|All files (*.*)|*.*";
            if (fileNameExists)
                dlg.FileName = fileName;
            dlg.InitialDirectory = Application.StartupPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.FileName;
                LoadXML(t, tv, graphicsFileRecursiveGenerator);
                return true;
            }

            if (fileNameExists)
                FileName = GetNewFileName(fileName);

            return true;
        }

        private static string GetNewFileName(string fileName)
        {
            var fileNameExists = File.Exists(fileName);
            if (!fileNameExists)
                return fileName;
            var extension = Path.GetExtension(fileName);
            if (extension == ".xml")
                return GetNewFileName(fileName + ".1");
            else
            {
                try
                {
                    extension = extension.Replace(".", "");
                    var newIteration = int.Parse(extension) + 1;
                    return GetNewFileName(fileName.Replace(extension, newIteration.ToString()));
                }
                catch { }
            }
            return null;
        }

        #region CopyNode, CopyNodes

        public static void CopyNodes(TreeNodeCollection to, TreeNodeCollection from, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1)
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            foreach (TreeNode node in from)
                CopyNode(to, node, addedNumberHigherThan, addedNumberLowerThan, type);
        }

        public static void CopyNode(TreeNodeCollection to, TreeNode from, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1)
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            var node = new TreeNode();
            CopyNode(node, from, addedNumberHigherThan, addedNumberLowerThan, type);

            if ((addedNumberLowerThan == null && addedNumberHigherThan == null) || (addedNumberLowerThan != null && addedNumberHigherThan != null && node.Tag != null))
                to.Add(node);
        }

        public static void CopyNode(TreeNode to, TreeNode from, int? addedNumberHigherThan = null, int? addedNumberLowerThan = null, int type = -1)
        {
            if (from == null)
                throw new Exception("from is null");

            if (to == null)
                throw new Exception("to is null");

            if (to.Nodes == null)
                throw new Exception("to.Nodes is null");

            var tagData = from.Tag as TreeNodeData;
            if (tagData != null)
            {
                if ((type == 0) && (tagData.AddedNumber >= addedNumberLowerThan) || (tagData.AddedNumber < addedNumberHigherThan))
                    return;
                else if ((type == 1) && (tagData.Urgency >= addedNumberLowerThan) || (tagData.Urgency < addedNumberHigherThan))
                    return;

                to.Tag = new TreeNodeData(tagData.Data, tagData.AddedNumber, tagData.AddedDate, tagData.LastChangeDate, tagData.Urgency, tagData.Link);
            }
            else if (addedNumberLowerThan != null) // allow copying only what I should copy
                return;

            to.Text = from.Text;
            to.Name = from.Name;

            var font = from.NodeFont;
            to.NodeFont = font != null ? new Font(font.FontFamily, font.SizeInPoints, font.Style) : MainForm.DefaultFont;

            to.ForeColor = from.ForeColor.IsEmpty ? DefaultForeGroundColor : from.ForeColor;
            to.BackColor = from.BackColor.IsEmpty ? DefaultBackGroundColor : from.BackColor;

            foreach (TreeNode node in from.Nodes)
                CopyNode(to.Nodes, node, addedNumberHigherThan, addedNumberLowerThan, type);
        }

        #endregion CopyNode, CopyNodes

        public static TreeNode GetNewNodeFromTextNameAttr(XmlAttributeCollection attributes)
        {
            var attrText = String.Empty;
            var attrName = String.Empty;
            var attrBold = false;
            var attrItalic = false;
            var attrUnderline = false;
            var attrStrikeout = false;
            var attrForegroundColor = DefaultForeGroundColor.Name;
            var attrBackgroundColor = DefaultBackGroundColor.Name;
            var attrFontFamily = FontFamily.GenericSansSerif; // Microsoft Sans Serif
            var attrFontSize = 8.5F;
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
                if (attr.Name == XmlAttrText)
                    attrText = HttpUtility.HtmlDecode(attr.Value);
                else if (attr.Name == XmlAttrName)
                    attrName = HttpUtility.HtmlDecode(attr.Value);
                else if (attr.Name == XmlAttrBold)
                    attrBold = true;
                else if (attr.Name == XmlAttrItalic)
                    attrItalic = true;
                else if (attr.Name == XmlAttrUnderline)
                    attrUnderline = true;
                else if (attr.Name == XmlAttrStrikeout)
                    attrStrikeout = true;
                else if (XmlAttrForegroundColorAcceptedList.Contains(attr.Name))
                    attrForegroundColor = HttpUtility.HtmlDecode(attr.Value);
                else if (XmlAttrBackgroundColorAcceptedList.Contains(attr.Name))
                    attrBackgroundColor = HttpUtility.HtmlDecode(attr.Value);
                else if (attr.Name == XmlAttrFontFamily)
                    attrFontFamily = FontFamily.Families.FirstOrDefault(f => f.Name == HttpUtility.HtmlDecode(attr.Value)) ?? FontFamily.GenericSansSerif;
                else if (attr.Name == XmlAttrFontSize)
                    attrFontSize = float.Parse(HttpUtility.HtmlDecode(attr.Value));
                else if (attr.Name == XmlAttrPercentCompleted)
                    attrPercentCompleted = decimal.Parse(HttpUtility.HtmlDecode(attr.Value));
                else if (attr.Name == XmlAttrData)
                    attrData = HttpUtility.HtmlDecode(TextProcessingHelper.GetDecompressedData(attr.Value));
                else if (attr.Name == XmlAttrAddedNumber)
                    attrAddedNumber = Int32.Parse(HttpUtility.HtmlDecode(attr.Value));
                else if (attr.Name == XmlAttrAddedDate)
                    attrAddedDate = StringToDateTime(HttpUtility.HtmlDecode(attr.Value));
                else if (attr.Name == XmlAttrLastChangeDate)
                    attrLastChangeDate = StringToDateTime(HttpUtility.HtmlDecode(attr.Value));
                else if (XmlAttrUrgencyAcceptedList.Contains(attr.Name))
                    attrUrgency = Int32.Parse(HttpUtility.HtmlDecode(attr.Value));
                else if (XmlAttrLinkAcceptedList.Contains(attr.Name))
                    attrLink = HttpUtility.HtmlDecode(attr.Value);
                else if (attr.Name == XmlAttrCategory)
                    attrCategory = HttpUtility.HtmlDecode(attr.Value);
                else if (attr.Name == XmlAttrIsStartupAlert)
                    attrIsStartupAlert = Boolean.Parse(HttpUtility.HtmlDecode(attr.Value));
            }

            var newStyle = (attrBold ? FontStyle.Bold : FontStyle.Regular) |
                (attrItalic ? FontStyle.Italic : FontStyle.Regular) |
                (attrUnderline ? FontStyle.Underline : FontStyle.Regular) |
                (attrStrikeout ? FontStyle.Strikeout : FontStyle.Regular);

            var background = StringToColor(attrBackgroundColor) ?? DefaultBackGroundColor;
            var foreground = StringToColor(attrForegroundColor) ?? DefaultForeGroundColor;

            if (attrText != null)
                attrText = TextProcessingHelper.GetTextAndProcentCompleted(attrText, ref attrPercentCompleted, true);

            var treeNodeData = String.IsNullOrEmpty(attrData) && attrAddedDate == null && attrLastChangeDate == null && attrAddedNumber == 0 && string.IsNullOrEmpty(attrLink) && attrUrgency == 0 && string.IsNullOrEmpty(attrCategory) ? null as TreeNodeData : new TreeNodeData(attrData, attrAddedNumber, attrAddedDate, attrLastChangeDate, attrUrgency, attrLink, attrCategory, attrIsStartupAlert);
            var attrDataStripped = treeNodeData != null ? RicherTextBox.Controls.RicherTextBox.StripRTF(treeNodeData.Data) : null;

            var node = new TreeNode(attrText)
            {
                Name = attrName,
                NodeFont = new Font(attrFontFamily, attrFontSize, newStyle),
                BackColor = background,
                ForeColor = foreground,
                Tag = treeNodeData,
                ToolTipText = TextProcessingHelper.GetToolTipText(attrText +
                    (!string.IsNullOrEmpty(attrName) && attrName != "0" ? Environment.NewLine + " TimeSpent: " + attrName : "") +
                    (!string.IsNullOrEmpty(attrDataStripped) ? Environment.NewLine + " Data: " + attrDataStripped : ""))
            };
            return node;
        }

        // TODO: Extract loading and saving XML to separate class
        public static void LoadXML(Form t, TreeView tv, IGraphicsFileFactory graphicsFileRecursiveGenerator)
        {
            try
            {
                t.Cursor = Cursors.WaitCursor;
                SplashForm.ShowDefaultSplashScreen(graphicsFileRecursiveGenerator);
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(FileName);

                tv.Nodes.Clear();
                if (xDoc.HasChildNodes)
                {
                    foreach (XmlElement child in xDoc.DocumentElement.ChildNodes)
                    {
                        var newNode = TreeNodeHelper.GetNewNodeFromTextNameAttr(child.Attributes);
                        tv.Nodes.Add(newNode);

                        if (child.HasChildNodes)
                            LoadTreeNodes(child, newNode);
                    }
                }
                tv.ExpandAll();
            }
            catch (XmlException xExc)
            {
                WinFormsApplication.GlobalExceptionHandling(xExc);
                MessageBox.Show(xExc.Message);
            }
            finally
            {
                SplashForm.CloseForm();
                t.Cursor = Cursors.Default;
                // TODO: This might use the ISoundProvider later, if required
                //SoundHelper.PlaySystemSound(4);
            }
        }

        public static void LoadTreeNodes(XmlNode xmlNode, TreeNode treeNode)
        {
            if (xmlNode.HasChildNodes)
                foreach (XmlElement child in xmlNode.ChildNodes)
                {
                    var newNode = TreeNodeHelper.GetNewNodeFromTextNameAttr(child.Attributes);
                    treeNode.Nodes.Add(newNode);
                    if (child.HasChildNodes)
                        LoadTreeNodes(child, newNode);
                }
        }

        public static bool LoadTreeNodesByCategory(TreeNodeCollection from, TreeNodeCollection to, bool includeOnlyWithStartupAlert, Dictionary<string, TreeNode> categories = null)
        {
            if (from == null || to == null || from.Count == 0)
                return false;

            if (categories == null || categories.Count == 0)
                to.Clear();

            if (categories == null)
                categories = new Dictionary<string, TreeNode>();

            foreach (TreeNode node in from)
            {
                var nodeData = node.Tag as TreeNodeData;
                if (nodeData != null && !string.IsNullOrEmpty(nodeData.Category) && ((includeOnlyWithStartupAlert && nodeData.IsStartupAlert) || (!includeOnlyWithStartupAlert)))
                {
                    if (!categories.ContainsKey(nodeData.Category))
                        categories.Add(nodeData.Category, new TreeNode(nodeData.Category));
                    categories[nodeData.Category].Nodes.Add(node.Clone() as TreeNode);
                }
                else if (nodeData != null && string.IsNullOrEmpty(nodeData.Category) && (includeOnlyWithStartupAlert && nodeData.IsStartupAlert))
                {
                    if (!categories.ContainsKey(string.Empty))
                        categories.Add(string.Empty, new TreeNode(nodeData.Category));
                    categories[string.Empty].Nodes.Add(node.Clone() as TreeNode);
                }

                if (node.Nodes.Count > 0)
                    LoadTreeNodesByCategory(node.Nodes, to, includeOnlyWithStartupAlert, categories);
            }

            foreach (var key in categories.Keys.OrderBy(c => c))
            {
                if (to.OfType<TreeNode>().Count(e => e.Text == key) == 0)
                    to.Add(categories[key]);

                //foreach(TreeNode child in categories[key].Nodes)
                //{
                //    var n = to.OfType<TreeNode>().First(e => e.Text == key);

                //    var exists = n.Nodes.OfType<TreeNode>().First(e => e.Text == child.Text);
                //    if(exists == null)
                //        n.Nodes.Add(child);
                //}
            }

            return categories.Count > 0;
        }

        #endregion Load

        #region Save

        public static string DateTimeToString(DateTime? dt)
        {
            var convertedString = (String)null;
            if (dt.HasValue)
                convertedString = dt.Value.ToString(DateTimeFormatSeparatedWithDot);
            return convertedString;
        }

        public static void SaveTree(TreeView tv)
        {
            if (ReadOnlyState)
            {
                tv.Nodes.Clear();
                CopyNodes(nodes, tv.Nodes, null, null);
            }

            if (IsSafeToSave && !TreeUnchanged)
            {
                streamWriter = new StreamWriter(FileName, false, System.Text.Encoding.UTF8);
                //Write the header
                streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                //Write our root node
                streamWriter.WriteLine("<root>");
                SaveNode(tv.Nodes, 1);
                streamWriter.WriteLine("</root>");
                streamWriter.Close();

                TreeUnchanged = true;
                TreeSaved = true;
                TreeSavedAt = DateTime.Now;
            }
        }

        public static string GetTabsByIndent(int indent)
        {
            string tabs = String.Empty;
            for (int i = 0; i < indent; i++)
                tabs += "\t";
            return tabs;
        }

        private static string GetXmlAttributeText(string attribute, string value, bool spaceAfterAttribute = false, string emptyValue = null)
        {
            return
                (string.IsNullOrEmpty(value) || value == emptyValue) ?
                string.Empty :
                (attribute + "=\"" + HttpUtility.HtmlEncode(value) + "\"" + (spaceAfterAttribute ? " " : string.Empty));
        }

        public static void SaveNode(TreeNodeCollection tnc, int indentTabs = 1)
        {
            foreach (TreeNode node in tnc)
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

                var tag = node.Tag as TreeNodeData;
                var attrForegroundColor = node.ForeColor != null && node.ForeColor.Name != null ? node.ForeColor.Name : string.Empty;
                var attrBackgroundColor = node.BackColor != null && node.BackColor.Name != null ? node.BackColor.Name : string.Empty;
                var attrFontFamily = node.NodeFont != null && node.NodeFont.FontFamily != null && node.NodeFont.FontFamily.Name != null && node.NodeFont.FontFamily.Name != FontFamily.GenericSansSerif.Name ? node.NodeFont.FontFamily.Name : string.Empty;
                var attrFontSize = node.NodeFont != null ? node.NodeFont.Size.ToString() : string.Empty;
                var attrText = node.Text;
                var attrPercentCompleted = tag == null ? 0.0M : tag.PercentCompleted;
                var attrData = tag == null ? string.Empty : TextProcessingHelper.GetCompressedData(tag.Data);
                var attrAddedNumber = tag == null ? 0 : tag.AddedNumber;
                var attrAddedDate = (DateTime?)(tag == null ? (DateTime?)null : (DateTime?)tag.AddedDate);
                var attrLastChangeDate = (DateTime?)(tag == null ? (DateTime?)null : (DateTime?)tag.LastChangeDate);
                var attrUrgency = (int?)(tag == null ? (int?)null : (int?)tag.Urgency);
                var attrLink = (string)(tag == null || string.IsNullOrEmpty(tag.Link) ? (string)null : tag.Link);
                var attrCategory = (string)(tag == null ? (string)null : (string)tag.Category);
                var attrIsStartupAlert = (bool?)(tag == null ? (bool?)null : (bool?)tag.IsStartupAlert);

                if (attrText != null)
                    attrText = TextProcessingHelper.GetTextAndProcentCompleted(attrText, ref attrPercentCompleted, true);

                var tagNodeLine = GetTabsByIndent(indentTabs) +
                    @"<node " +
                        GetXmlAttributeText(XmlAttrText, attrText, true) +
                        GetXmlAttributeText(XmlAttrName, node.Name, true, "0") +
                        GetXmlAttributeText(XmlAttrBold, attrBold.ToString(), true, false.ToString()) +
                        GetXmlAttributeText(XmlAttrItalic, attrItalic.ToString(), true, false.ToString()) +
                        GetXmlAttributeText(XmlAttrUnderline, attrUnderline.ToString(), true, false.ToString()) +
                        GetXmlAttributeText(XmlAttrStrikeout, attrStrikeout.ToString(), true, false.ToString()) +
                        GetXmlAttributeText(XmlAttrForegroundColor, attrForegroundColor, true, DefaultForeGroundColor.Name) +
                        GetXmlAttributeText(XmlAttrBackgroundColor, attrBackgroundColor, true, DefaultBackGroundColor.Name) +
                        GetXmlAttributeText(XmlAttrFontFamily, attrFontFamily, true) +
                        GetXmlAttributeText(XmlAttrFontSize, attrFontSize, true, 8.5F.ToString()) +
                        GetXmlAttributeText(XmlAttrData, attrData, true) +
                        GetXmlAttributeText(XmlAttrAddedDate, attrAddedDate.HasValue ? ((DateTime)attrAddedDate).ToString(DateTimeFormatSeparatedWithDot) : null, true) +
                        GetXmlAttributeText(XmlAttrLastChangeDate, ((attrAddedDate.HasValue && attrLastChangeDate.HasValue && attrAddedDate.Value != attrLastChangeDate.Value) || (!attrLastChangeDate.HasValue)) ? ((DateTime)attrLastChangeDate).ToString(DateTimeFormatSeparatedWithDot) : null, true) +
                        GetXmlAttributeText(XmlAttrAddedNumber, attrAddedNumber.ToString(), true) +
                        GetXmlAttributeText(XmlAttrUrgency, attrUrgency.HasValue ? attrUrgency.ToString() : null, true, "0") +
                        GetXmlAttributeText(XmlAttrLink, attrLink, true) +
                        GetXmlAttributeText(XmlAttrPercentCompleted, attrPercentCompleted.ToString(), true, "0.0") +
                        GetXmlAttributeText(XmlAttrCategory, attrCategory, true) +
                        GetXmlAttributeText(XmlAttrIsStartupAlert, attrIsStartupAlert.HasValue ? attrIsStartupAlert.ToString() : null, true, false.ToString());

                if (streamWriter == null)
                    throw new Exception("StreamWriter \"streamWriter\" is null");

                if (node.Nodes.Count > 0)
                {
                    tagNodeLine = tagNodeLine.Substring(0, tagNodeLine.Length - 1) + ">"; // remove a space before ">"
                    streamWriter.WriteLine(tagNodeLine);
                    SaveNode(node.Nodes, indentTabs + 1);
                    streamWriter.WriteLine(GetTabsByIndent(indentTabs) + "</node>");
                }
                else
                {
                    tagNodeLine += @"/>";
                    streamWriter.WriteLine(tagNodeLine);
                }
            }
        }

        #endregion Save

        #region Load & save

        public static void SaveCurrentTreeAndLoadAnother(Form t, TreeView tv, string fileName, Action updateShowUntilNumber, IGraphicsFileFactory graphicsFileRecursiveGenerator)
        {
            SaveTree(tv);
            FileName = fileName;
            LoadTree(t, tv, FileName, graphicsFileRecursiveGenerator);
            tv.CollapseAll();
            tv.Refresh();

            IsSafeToSave = true;

            if (TreeNodesNeedFix(tv.Nodes))
            {
                TreeNodeCounter = 1;
                FixTreeNodes(tv.Nodes);
            }

            if (updateShowUntilNumber != null)
                updateShowUntilNumber();
        }

        #endregion Load & save

        #region Node update

        public static bool ParseNodeAndUpdate(TreeNode node, string taskName, string nodeValue)
        {
            if (node.Text.Equals(taskName /* StartsWith + " [" */))
            {
                node.Text = nodeValue;
                return true;
            }
            else
            {
                if (node.Nodes != null)
                {
                    bool exists = false;
                    foreach (TreeNode n in node.Nodes)
                    {
                        exists = ParseNodeAndUpdate(n, taskName, nodeValue);
                        if (exists)
                            break;
                    }
                    return exists;
                }
            }
            return false;
        }

        #endregion Node update

        #region Node percentage

        public static double GetPercentageFromChildren(TreeNode topNode)
        {
            var sum = 0.0;
            var nr = 0;
            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null && node.Nodes.Count > 0)
                {
                    var procentCompleted = (decimal)GetPercentageFromChildren(node);
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref procentCompleted, true);

                    sum += (double)procentCompleted;
                }
                else
                {
                    var value = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref value, true);

                    sum += (double)value;
                }
                nr++;
            }

            if (nr != 0)
                return (sum / nr);
            return 0;
        }

        public static void SetPercentageToChildren(TreeNode topNode, double percentage)
        {
            foreach (TreeNode node in topNode.Nodes)
            {
                if (node != null)
                {
                    var percentCompleted = (decimal)percentage;
                    node.Text = TextProcessingHelper.UpdateTextAndProcentCompleted(node.Text, ref percentCompleted, true);
                    if (node.Nodes.Count > 0)
                        SetPercentageToChildren(node, percentage);
                }
            }
        }

        #endregion Node percentage

        #region Node deletion

        public static int ParseToDelete(TreeView tv, TreeNode topNode, string nodeNameToDelete, bool fakeDelete = true)
        {
            int ret = 0;
            if (topNode.Text.Equals(nodeNameToDelete /* StartsWith + " [" */))
            {
                if (!fakeDelete)
                {
                    topNode.Nodes.Clear();
                    tv.Nodes.Remove(topNode);
                }
                ret++;
            }
            else
                foreach (TreeNode node in topNode.Nodes)
                {
                    if (node != null && node.Text.Equals(nodeNameToDelete /* StartsWith + " [" */))
                    {
                        if (!fakeDelete)
                        {
                            node.Nodes.Clear();
                            tv.Nodes.Remove(node);
                        }

                        ret++;
                    }
                    if (node != null && node.Nodes.Count > 0)
                        ret += ParseToDelete(tv, node, nodeNameToDelete);
                }
            return ret;
        }

        #endregion Node deletion

        #region Nodes completed/unfinished

        public static bool? TasksCompleteAreHidden(TreeView tv)
        {
            if (tv.Nodes.Count > 0)
            {
                foreach (TreeNode node in tv.Nodes)
                {
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                    if (completed == 100)
                    {
                        if (node.ForeColor.Name == DefaultBackGroundColor.ToString())
                            return true;
                        else
                            return false;
                    }

                    if (node.Nodes.Count > 0)
                    {
                        var ret = TasksCompleteAreHidden(tv);
                        if (ret.HasValue)
                            return ret;
                        // else continue;
                    }
                }
            }
            return null;
        }

        public static void ToggleCompletedTasks(TreeView tv, bool toggleCompletedTasksAreHidden, TreeNodeCollection nodes)
        {
            var foreColor = toggleCompletedTasksAreHidden ? DefaultForeGroundColor : DefaultBackGroundColor;

            if (tv.Nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var completed = 0.0M;
                    node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                    if (completed == 100)
                        node.ForeColor = foreColor;

                    if (node.Nodes.Count > 0)
                        ToggleCompletedTasks(tv, toggleCompletedTasksAreHidden, node.Nodes);
                }
            }
        }

        public static void MoveToNextUnfinishedNode(TreeView tv, TreeNode currentNode)
        {
            if (currentNode == null)
                return;

            foreach (TreeNode node in currentNode.Nodes)
            {
                var completed = 0.0M;
                node.Text = TextProcessingHelper.GetTextAndProcentCompleted(node.Text, ref completed, true);

                if (completed != 100 && tv.SelectedNode != node)
                {
                    tv.SelectedNode = node;
                    return;
                }
            }

            if (currentNode.Parent != null)
            {
                var foundCurrentNode = false;
                foreach (TreeNode node in currentNode.Parent.Nodes)
                {
                    if (foundCurrentNode && !object.ReferenceEquals(node, currentNode))
                    {
                        MoveToNextUnfinishedNode(tv, node);
                        return;
                    }

                    if (object.ReferenceEquals(node, currentNode))
                        foundCurrentNode = true;
                }

                MoveToNextUnfinishedNode(tv, currentNode.Parent);
            }
        }

        #endregion Nodes completed/unfinished

        #region Node search

        public static TreeNode GetFirstNode(TreeNodeCollection nodes, string text)
        {
            TreeNode ret = null;
            text = text.ToLower();
            if (nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var nodeTagData = node.Tag as TreeNodeData;
                    var nodeData = nodeTagData != null && !string.IsNullOrEmpty(nodeTagData.Data) ? nodeTagData.Data : null;
                    var foundCondition = (node.Text != null && node.Text.ToLower().Split('[')[0].Contains(text)) || (nodeData != null && nodeData.ToLower().Contains(text));

                    if (foundCondition)
                        return node;

                    if (node.Nodes != null && node.Nodes.Count > 0)
                        ret = GetFirstNode(node.Nodes, text);

                    if (ret != null)
                        return ret;
                }
            }

            return null;
        }

        public static void ExpandParents(TreeNode node)
        {
            if (node == null)
                return;

            var parent = node.Parent;
            if (parent != null)
            {
                parent.Expand();
                ExpandParents(parent);
            }
        }

        public static void SetStyleForSearch(TreeNodeCollection nodes, string text)
        {
            text = text.ToLower();
            if (nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    var nodeTagData = node.Tag as TreeNodeData;
                    var nodeData = nodeTagData != null && !string.IsNullOrEmpty(nodeTagData.Data) ? nodeTagData.Data : null;
                    var foundCondition = (node.Text != null && node.Text.ToLower().Split('[')[0].Contains(text)) || (nodeData != null && nodeData.ToLower().Contains(text));

                    if (foundCondition && (node.BackColor != BackGroundColorSearch || node.ForeColor != ForeGroundColorSearch))
                    {
                        node.BackColor = BackGroundColorSearch;
                        node.ForeColor = ForeGroundColorSearch;
                        node.Expand();

                        ExpandParents(node);
                    }

                    if (node.Nodes != null && node.Nodes.Count > 0)
                        SetStyleForSearch(node.Nodes, text);
                }
            }
        }

        public static string[] GenerateStringGraphicsLinesFromTree(TreeView tvTaskList)
        {
            List<string> lines = new List<string>();

            foreach (TreeNode task in tvTaskList.Nodes)
                lines.Add(task.Text);

            return lines.ToArray();
        }

        public static void ClearStyleAdded(TreeNodeCollection col)
        {
            if (col.Count > 0)
            {
                foreach (TreeNode node in col)
                {
                    if (node.BackColor != DefaultBackGroundColor)
                        node.BackColor = DefaultBackGroundColor;

                    if (node.ForeColor != DefaultForeGroundColor)
                        node.ForeColor = DefaultForeGroundColor;

                    if (node.Nodes.Count > 0)
                        ClearStyleAdded(node.Nodes);
                }
            }
        }

        #endregion Node search

        #region Node show by urgency or added number

        public static void ShowNodesFromTaskToNumberOfTask(TreeView tv, decimal addedNumberLowerThan, decimal addedNumberHigherThan, int type)
        {
            if (nodes == null)
                nodes = new TreeNode().Nodes;

            // copy all
            if (!ReadOnlyState)
            {
                nodes.Clear();
                CopyNodes(nodes, tv.Nodes, null, null);
            }

            // let tvTaskList with only addedNumber < addedNumberLowerThan
            tv.Nodes.Clear();
            CopyNodes(tv.Nodes, nodes, (int)addedNumberHigherThan, (int)addedNumberLowerThan, type);
            ReadOnlyState = true;
        }

        public static void ShowAllTasks(TreeView tv)
        {
            if (TreeNodeHelper.ReadOnlyState)
            {
                tv.Nodes.Clear();
                TreeNodeHelper.CopyNodes(tv.Nodes, nodes, null, null);
                TreeNodeHelper.ReadOnlyState = false;
            }
        }

        #endregion Node show by urgency or added number

        #region Node data size calculation

        public static int CalculateDataSizeFromNodeAndChildren(TreeNode node)
        {
            if (node == null)
                return 0;

            var tagData = node.Tag as TreeNodeData;
            if (tagData == null)
                return 0;

            var size = tagData.Data == null ? 0 : tagData.Data.Length;
            foreach (TreeNode n in node.Nodes)
                size += CalculateDataSizeFromNodeAndChildren(n);
            return size;
        }

        #endregion Node data size calculation

        #region Node move

        public static void UpdateCurrentSelection(TreeNode node)
        {
            oldSelection = currentSelection;
            currentSelection = node;
        }

        public static void MoveNode(TreeView tv)
        {
            if (oldSelection == null)
                return;

            bool removedNode = false;
            if (currentSelection != null)
            {
                var parentSelected = currentSelection.Parent;
                if (parentSelected != null)
                {
                    parentSelected.Nodes.Remove(oldSelection);
                    removedNode = true;
                }
            }

            if (!removedNode)
                tv.Nodes.Remove(oldSelection);

            var currentSelectionTagData = currentSelection.Tag as TreeNodeData;
            if (string.IsNullOrEmpty(currentSelection.Text) &&
                currentSelectionTagData != null && string.IsNullOrEmpty(currentSelectionTagData.Data) &&
                currentSelection.Parent == null &&
                currentSelection.Nodes.Count == 0)
                tv.Nodes.Add(oldSelection);
            else
                currentSelection.Nodes.Add(oldSelection);
        }

        #endregion Node move

        /// <summary>
        /// Changes the size of a collection of nodes recursively.
        /// </summary>
        /// <param name="treeNodeCollection">Collection of nodes.</param>
        /// <param name="changedSize">Font size changed to all the nodes (added or substracted from nodes size).</param>
        public static void UpdateSizeOfTreeNodes(TreeNodeCollection treeNodeCollection, float changedSize)
        {
            foreach (TreeNode node in treeNodeCollection)
            {
                node.NodeFont = new Font(node.NodeFont.FontFamily, node.NodeFont.Size + changedSize, node.NodeFont.Style);

                // Change size of children recursively too
                foreach (TreeNode childNode in node.Nodes)
                    UpdateSizeOfTreeNodes(childNode.Nodes, changedSize);
            }
        }
    }
}