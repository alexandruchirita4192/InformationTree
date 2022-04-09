using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InformationTree.Domain.Services;
using NLog;

namespace RicherTextBox.Controls
{
    public partial class RicherTextBox : UserControl
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPopUpService _popUpService;

        #region Settings

        private int indent = 10;

        [Category("Settings")]
        [Description("Value indicating the number of characters used for indentation")]
        public int INDENT
        {
            get { return indent; }
            set { indent = value; }
        }

        #endregion Settings

        #region Properties for toolstrip items visibility

        [Category("Global visibility")]
        public bool GlobalVisibility
        {
            get { return Visible; }
            set
            {
                Visible = value;

                if (Controls != null)
                    foreach (Control control in Controls)
                    {
                        control.Visible = value;

                        if (control != null && control.Controls != null)
                            foreach (Control ctrl in control.Controls)
                            {
                                ctrl.Visible = value;

                                if (ctrl != null && ctrl.Controls != null)
                                    foreach (Control c in ctrl.Controls)
                                        c.Visible = value;
                            }
                    }
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupSaveAndLoadVisible
        {
            get { return tsbtnSave.Visible && tsbtnOpen.Visible && toolStripSeparator6.Visible; }
            set
            {
                var parent = tsbtnSave.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnSave.Visible = value;
                tsbtnOpen.Visible = value;
                toolStripSeparator6.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupFontNameAndSizeVisible
        {
            get { return tscmbFont.Visible && tscmbFontSize.Visible && tsbtnChooseFont.Visible && toolStripSeparator1.Visible; }
            set
            {
                var parent = tscmbFont.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tscmbFont.Visible = value;
                tscmbFontSize.Visible = value;
                tsbtnChooseFont.Visible = value;
                toolStripSeparator1.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupBoldUnderlineItalicVisible
        {
            get { return tsbtnBold.Visible && tsbtnItalic.Visible && tsbtnUnderline.Visible && toolStripSeparator2.Visible; }
            set
            {
                var parent = tsbtnBold.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnBold.Visible = value;
                tsbtnItalic.Visible = value;
                tsbtnUnderline.Visible = value;
                toolStripSeparator2.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupAlignmentVisible
        {
            get { return tsbtnAlignLeft.Visible && tsbtnAlignRight.Visible && tsbtnAlignCenter.Visible && toolStripSeparator3.Visible; }
            set
            {
                var parent = tsbtnAlignLeft.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnAlignLeft.Visible = value;
                tsbtnAlignRight.Visible = value;
                tsbtnAlignCenter.Visible = value;
                toolStripSeparator3.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupFontColorVisible
        {
            get { return tsbtnFontColor.Visible && tsbtnWordWrap.Visible && toolStripSeparator4.Visible; }
            set
            {
                var parent = tsbtnFontColor.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnFontColor.Visible = value;
                tsbtnWordWrap.Visible = value;
                toolStripSeparator4.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupIndentationAndBulletsVisible
        {
            get { return tsbtnIndent.Visible && tsbtnOutdent.Visible && tsbtnBullets.Visible && toolStripSeparator5.Visible; }
            set
            {
                var parent = tsbtnIndent.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnIndent.Visible = value;
                tsbtnOutdent.Visible = value;
                tsbtnBullets.Visible = value;
                toolStripSeparator5.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupInsertVisible
        {
            get { return tsbtnInsertPicture.Visible && toolStripSeparator7.Visible; }
            set
            {
                var parent = tsbtnInsertPicture.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnInsertPicture.Visible = value;
                toolStripSeparator7.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool GroupZoomVisible
        {
            get { return tsbtnZoomOut.Visible && tsbtnZoomIn.Visible && tstxtZoomFactor.Visible; }
            set
            {
                var parent = tsbtnZoomOut.GetCurrentParent(); if (parent != null) parent.Visible = value;
                tsbtnZoomOut.Visible = value;
                tsbtnZoomIn.Visible = value;
                tstxtZoomFactor.Visible = value;
            }
        }

        [Category("Toolstip items visibility")]
        public bool ToolStripVisible
        {
            get { return toolStripMenu.Visible; }
            set { toolStripMenu.Visible = value; }
        }

        [Category("Toolstip items visibility")]
        public bool FindReplaceVisible
        {
            get { return toolStripFindReplace.Visible; }
            set { toolStripFindReplace.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SaveVisible
        {
            get { return tsbtnSave.Visible; }
            set { tsbtnSave.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool LoadVisible
        {
            get { return tsbtnOpen.Visible; }
            set { tsbtnOpen.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorSaveLoadVisible
        {
            get { return toolStripSeparator6.Visible; }
            set { toolStripSeparator6.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool FontFamilyVisible
        {
            get { return tscmbFont.Visible; }
            set { tscmbFont.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool FontSizeVisible
        {
            get { return tscmbFontSize.Visible; }
            set { tscmbFontSize.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool ChooseFontVisible
        {
            get { return tsbtnChooseFont.Visible; }
            set { tsbtnChooseFont.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorFontVisible
        {
            get { return toolStripSeparator1.Visible; }
            set { toolStripSeparator1.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool BoldVisible
        {
            get { return tsbtnBold.Visible; }
            set { tsbtnBold.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool ItalicVisible
        {
            get { return tsbtnItalic.Visible; }
            set { tsbtnItalic.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool UnderlineVisible
        {
            get { return tsbtnUnderline.Visible; }
            set { tsbtnUnderline.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorBoldUnderlineItalicVisible
        {
            get { return toolStripSeparator2.Visible; }
            set { toolStripSeparator2.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool AlignLeftVisible
        {
            get { return tsbtnAlignLeft.Visible; }
            set { tsbtnAlignLeft.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool AlignRightVisible
        {
            get { return tsbtnAlignRight.Visible; }
            set { tsbtnAlignRight.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool AlignCenterVisible
        {
            get { return tsbtnAlignCenter.Visible; }
            set { tsbtnAlignCenter.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorAlignVisible
        {
            get { return toolStripSeparator3.Visible; }
            set { toolStripSeparator3.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool FontColorVisible
        {
            get { return tsbtnFontColor.Visible; }
            set { tsbtnFontColor.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool WordWrapVisible
        {
            get { return tsbtnWordWrap.Visible; }
            set { tsbtnWordWrap.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorFontColorVisible
        {
            get { return toolStripSeparator4.Visible; }
            set { toolStripSeparator4.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool IndentVisible
        {
            get { return tsbtnIndent.Visible; }
            set { tsbtnIndent.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool OutdentVisible
        {
            get { return tsbtnOutdent.Visible; }
            set { tsbtnOutdent.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool BulletsVisible
        {
            get { return tsbtnBullets.Visible; }
            set { tsbtnBullets.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorIndentAndBulletsVisible
        {
            get { return toolStripSeparator5.Visible; }
            set { toolStripSeparator5.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool InsertPictureVisible
        {
            get { return tsbtnInsertPicture.Visible; }
            set { tsbtnInsertPicture.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorInsertVisible
        {
            get { return toolStripSeparator7.Visible; }
            set { toolStripSeparator7.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool ZoomInVisible
        {
            get { return tsbtnZoomIn.Visible; }
            set { tsbtnZoomIn.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool ZoomOutVisible
        {
            get { return tsbtnZoomOut.Visible; }
            set { tsbtnZoomOut.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool ZoomFactorTextVisible
        {
            get { return tstxtZoomFactor.Visible; }
            set { tstxtZoomFactor.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool SeparatorNewButtonsVisible
        {
            get { return toolStripMenuItem5.Visible; }
            set { toolStripMenuItem5.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool TableVisible
        {
            get { return tsbtnTable.Visible; }
            set { tsbtnTable.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool CalculateVisible
        {
            get { return tsbtnCalculate.Visible; }
            set { tsbtnCalculate.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool EncryptDecryptCategoryVisible
        {
            get { return encryptDecryptToolStripMenuItem.Visible; }
            set { encryptDecryptToolStripMenuItem.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool EncryptVisible
        {
            get { return tsbtnEncrypt.Visible; }
            set { tsbtnEncrypt.Visible = value; }
        }

        [Category("Toolstrip single items visibility")]
        public bool DecryptVisible
        {
            get { return tsbtnDecrypt.Visible; }
            set { tsbtnDecrypt.Visible = value; }
        }

        #endregion Properties for toolstrip items visibility

        #region data properties

        [Category("Text length")]
        [Description("RicherTextBox text length")]
        [Browsable(true)]
        public int TextLength
        {
            get { return rtbDocument.TextLength; }
        }

        [Category("Document data")]
        [Description("RicherTextBox content in plain text")]
        [Browsable(true)]
        public override string Text
        {
            get { return rtbDocument.Text; }
            set { rtbDocument.Text = value; }
        }

        [Category("Document data")]
        [Description("RicherTextBox content in rich-text format")]
        public string Rtf
        {
            get
            {
                return rtbDocument.Rtf;
            }
            set
            {
                if (IsRichText(value))
                {
                    try
                    {
                        rtbDocument.Rtf = value;
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.Error(ex);
                        rtbDocument.Text = value;
                    }
                }
                else
                    rtbDocument.Text = value;
            }
        }

        [Category("RichTextBox")]
        [Description("RichTextBox document")]
        public RichTextBox TextBox
        {
            get { return rtbDocument; }
            set { rtbDocument = value; }
        }

        #endregion data properties

        #region Events

        public event LinkClickedEventHandler LinkClicked
        {
            add { rtbDocument.LinkClicked += value; }
            remove { rtbDocument.LinkClicked -= value; }
        }

        public event KeyEventHandler DocumentKeyUp
        {
            add { rtbDocument.KeyUp += value; }
            remove { rtbDocument.KeyUp -= value; }
        }

        #endregion Events

        #region Construction and initial loading

        public RicherTextBox()
        {
            InitializeComponent();

            TableFunction = (target) =>
            {
                var tableControl = new TableControl(target, this);

                ////rtbDocument.Rtf = rtbDocument.Rtf.Replace() // TODO: change here to see the table in RTF
                return target;
            };
        }

        public RicherTextBox(IPopUpService popUpService)
            : this()
        {
            _popUpService = popUpService;
        }

        private void RicherTextBox_Load(object sender, EventArgs e)
        {
            // load system fonts
            foreach (FontFamily family in FontFamily.Families)
            {
                tscmbFont.Items.Add(family.Name);
            }
            tscmbFont.SelectedItem = "Microsoft Sans Serif";

            tscmbFontSize.SelectedItem = "9";

            tstxtZoomFactor.Text = Convert.ToString(rtbDocument.ZoomFactor * 100);
            tsbtnWordWrap.Checked = rtbDocument.WordWrap;
        }

        #endregion Construction and initial loading

        #region Toolstrip items handling

        private void tsbtnBIUS_Click(object sender, EventArgs e)
        {
            // bold, italic, underline, strikeout
            if (rtbDocument.SelectionFont != null)
            {
                Font currentFont = rtbDocument.SelectionFont;
                FontStyle newFontStyle = rtbDocument.SelectionFont.Style;
                string txt = (sender as ToolStripButton).Name;
                if (txt.IndexOf("Bold") >= 0)
                    newFontStyle = rtbDocument.SelectionFont.Style ^ FontStyle.Bold;
                else if (txt.IndexOf("Italic") >= 0)
                    newFontStyle = rtbDocument.SelectionFont.Style ^ FontStyle.Italic;
                else if (txt.IndexOf("Underline") >= 0)
                    newFontStyle = rtbDocument.SelectionFont.Style ^ FontStyle.Underline;
                else if (txt.IndexOf("Strikeout") >= 0)
                    newFontStyle = rtbDocument.SelectionFont.Style ^ FontStyle.Strikeout;

                rtbDocument.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            }
        }

        private void rtbDocument_SelectionChanged(object sender, EventArgs e)
        {
            if (rtbDocument.SelectionFont != null)
            {
                tsbtnBold.Checked = rtbDocument.SelectionFont.Bold;
                tsbtnItalic.Checked = rtbDocument.SelectionFont.Italic;
                tsbtnUnderline.Checked = rtbDocument.SelectionFont.Underline;
                tsbtnStrikeout.Checked = rtbDocument.SelectionFont.Strikeout;

                boldToolStripMenuItem.Checked = rtbDocument.SelectionFont.Bold;
                italicToolStripMenuItem.Checked = rtbDocument.SelectionFont.Italic;
                underlineToolStripMenuItem.Checked = rtbDocument.SelectionFont.Underline;
                strikeoutToolStripMenuItem.Checked = rtbDocument.SelectionFont.Strikeout;

                switch (rtbDocument.SelectionAlignment)
                {
                    case HorizontalAlignment.Left:
                        tsbtnAlignLeft.Checked = true;
                        tsbtnAlignCenter.Checked = false;
                        tsbtnAlignRight.Checked = false;

                        leftToolStripMenuItem.Checked = true;
                        centerToolStripMenuItem.Checked = false;
                        rightToolStripMenuItem.Checked = false;
                        break;

                    case HorizontalAlignment.Center:
                        tsbtnAlignLeft.Checked = false;
                        tsbtnAlignCenter.Checked = true;
                        tsbtnAlignRight.Checked = false;

                        leftToolStripMenuItem.Checked = false;
                        centerToolStripMenuItem.Checked = true;
                        rightToolStripMenuItem.Checked = false;
                        break;

                    case HorizontalAlignment.Right:
                        tsbtnAlignLeft.Checked = false;
                        tsbtnAlignCenter.Checked = false;
                        tsbtnAlignRight.Checked = true;

                        leftToolStripMenuItem.Checked = false;
                        centerToolStripMenuItem.Checked = false;
                        rightToolStripMenuItem.Checked = true;
                        break;
                }

                tsbtnBullets.Checked = rtbDocument.SelectionBullet;
                bulletsToolStripMenuItem.Checked = rtbDocument.SelectionBullet;

                tscmbFont.SelectedItem = rtbDocument.SelectionFont.FontFamily.Name;
                tscmbFontSize.SelectedItem = rtbDocument.SelectionFont.Size.ToString();
            }
        }

        private void tsbtnAlignment_Click(object sender, EventArgs e)
        {
            // alignment: left, center, right
            string txt = (sender as ToolStripButton).Name;
            if (txt.IndexOf("Left") >= 0)
            {
                rtbDocument.SelectionAlignment = HorizontalAlignment.Left;
                tsbtnAlignLeft.Checked = true;
                tsbtnAlignCenter.Checked = false;
                tsbtnAlignRight.Checked = false;
            }
            else if (txt.IndexOf("Center") >= 0)
            {
                rtbDocument.SelectionAlignment = HorizontalAlignment.Center;
                tsbtnAlignLeft.Checked = false;
                tsbtnAlignCenter.Checked = true;
                tsbtnAlignRight.Checked = false;
            }
            else if (txt.IndexOf("Right") >= 0)
            {
                rtbDocument.SelectionAlignment = HorizontalAlignment.Right;
                tsbtnAlignLeft.Checked = false;
                tsbtnAlignCenter.Checked = false;
                tsbtnAlignRight.Checked = true;
            }
        }

        private void tsbtnFontColor_Click(object sender, EventArgs e)
        {
            // font color
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = rtbDocument.SelectionColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    rtbDocument.SelectionColor = dlg.Color;
                }
            }
        }

        private void tsbtnBulletsAndNumbering_Click(object sender, EventArgs e)
        {
            // bullets, indentation
            string name = (sender as ToolStripButton).Name;
            if (name.IndexOf("Bullets") >= 0)
                rtbDocument.SelectionBullet = tsbtnBullets.Checked;
            else if (name.IndexOf("Indent") >= 0)
                rtbDocument.SelectionIndent += INDENT;
            else if (name.IndexOf("Outdent") >= 0)
                rtbDocument.SelectionIndent -= INDENT;
        }

        private void tscmbFontSize_Click(object sender, EventArgs e)
        {
            // font size
            if (!(rtbDocument.SelectionFont == null))
            {
                Font currentFont = rtbDocument.SelectionFont;
                float newSize = Convert.ToSingle(tscmbFontSize.SelectedItem.ToString());
                rtbDocument.SelectionFont = new Font(currentFont.FontFamily, newSize, currentFont.Style);
            }
        }

        private void tscmbFontSize_TextChanged(object sender, EventArgs e)
        {
            // font size custom
            if (!(rtbDocument.SelectionFont == null))
            {
                Font currentFont = rtbDocument.SelectionFont;
                float newSize = Convert.ToSingle(tscmbFontSize.Text);
                rtbDocument.SelectionFont = new Font(currentFont.FontFamily, newSize, currentFont.Style);
            }
        }

        private void tscmbFont_Click(object sender, EventArgs e)
        {
            // font
            if (!(rtbDocument.SelectionFont == null))
            {
                Font currentFont = rtbDocument.SelectionFont;
                FontFamily newFamily = new FontFamily(tscmbFont.SelectedItem.ToString());
                rtbDocument.SelectionFont = new Font(newFamily, currentFont.Size, currentFont.Style);
            }
        }

        private void btnChooseFont_Click(object sender, EventArgs e)
        {
            using (FontDialog dlg = new FontDialog())
            {
                if (rtbDocument.SelectionFont != null) dlg.Font = rtbDocument.SelectionFont;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    rtbDocument.SelectionFont = dlg.Font;
                }
            }
        }

        public void InsertPicture()
        {
            var selectedFileName = _popUpService.GetImageFile();
            
            if (!string.IsNullOrWhiteSpace(selectedFileName))
            {
                try
                {
                    var img = Image.FromFile(selectedFileName);
                    Clipboard.SetDataObject(img);
                    DataFormats.Format df;
                    df = DataFormats.GetFormat(DataFormats.Bitmap);

                    if (rtbDocument.CanPaste(df))
                        rtbDocument.Paste(df);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _popUpService.ShowError("Unable to insert image.");
                }
            }
        }

        private void tsbtnInsertPicture_Click(object sender, EventArgs e)
        {
            InsertPicture();
        }

        public void SaveFile()
        {
            var selectedFileName = _popUpService.SaveRtfFile();
            
            if (!string.IsNullOrWhiteSpace(selectedFileName))
            {
                try
                {
                    rtbDocument.SaveFile(selectedFileName, RichTextBoxStreamType.RichText);
                }
                catch (IOException ex)
                {
                    _logger.Error(ex);
                    _popUpService.ShowError($"Error writing file: \n{ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    _logger.Error(ex);
                    _popUpService.ShowError($"Error writing file: \n{ex.Message}");
                }
            }
        }

        private void tsbtnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        public void OpenFile()
        {
            var selectedFileName = _popUpService.GetRtfFile();
            
            if (!string.IsNullOrEmpty(selectedFileName))
            {
                try
                {
                    rtbDocument.LoadFile(selectedFileName, RichTextBoxStreamType.RichText);
                }
                catch (IOException ex)
                {
                    _logger.Error(ex);
                    _popUpService.ShowError($"Error reading file: \n{ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    _logger.Error(ex);
                    _popUpService.ShowError($"Error reading file: \n{ex.Message}");
                }
            }
        }

        private void tsbtnOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void tsbtnZoomIn_Click(object sender, EventArgs e)
        {
            if (rtbDocument.ZoomFactor < 64.0f - 0.20f)
            {
                rtbDocument.ZoomFactor += 0.20f;
                tstxtZoomFactor.Text = string.Format("{0:F0}", rtbDocument.ZoomFactor * 100);
            }
        }

        private void tsbtnZoomOut_Click(object sender, EventArgs e)
        {
            if (rtbDocument.ZoomFactor > 0.16f + 0.20f)
            {
                rtbDocument.ZoomFactor -= 0.20f;
                tstxtZoomFactor.Text = string.Format("{0:F0}", rtbDocument.ZoomFactor * 100);
            }
        }

        private void tstxtZoomFactor_Leave(object sender, EventArgs e)
        {
            try
            {
                rtbDocument.ZoomFactor = Convert.ToSingle(tstxtZoomFactor.Text) / 100.0f;
            }
            catch (FormatException ex)
            {
                _logger.Error(ex);
                _popUpService.ShowError($"Enter a valid number: \n{ex.Message}");
                tstxtZoomFactor.Focus();
                tstxtZoomFactor.SelectAll();
            }
            catch (OverflowException ex)
            {
                _logger.Error(ex);
                _popUpService.ShowError($"Enter a valid number: \n{ex.Message}");
                tstxtZoomFactor.Focus();
                tstxtZoomFactor.SelectAll();
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex);
                _popUpService.ShowError("Zoom factor should be between 20% and 6400%");
                tstxtZoomFactor.Focus();
                tstxtZoomFactor.SelectAll();
            }
        }

        private void tsbtnWordWrap_Click(object sender, EventArgs e)
        {
            rtbDocument.WordWrap = tsbtnWordWrap.Checked;
        }

        private void tsbtnDecrypt_Click(object sender, EventArgs e)
        {
            // TODO: Create a feature and activate/deactivate this button based on that feature
            if (DecryptFunction == null)
                return;// throw new NotImplementedException();
            rtbDocument.Rtf = DecryptFunction(rtbDocument.Rtf);
        }

        private void tsbtnEncrypt_Click(object sender, EventArgs e)
        {
            // TODO: Create a feature and activate/deactivate this button based on that feature
            if (EncryptFunction == null)
                return;// throw new NotImplementedException();
            rtbDocument.Rtf = EncryptFunction(rtbDocument.Rtf);
        }

        private void tsbtnCalculate_Click(object sender, EventArgs e)
        {
            // TODO: Create a feature and activate/deactivate this button based on that feature
            if (CalculateFunction == null)
                return;// throw new NotImplementedException();
            rtbDocument.Rtf = CalculateFunction(rtbDocument.Rtf);
        }

        private void tsbtnTable_Click(object sender, EventArgs e)
        {
            // TODO: Create a feature and activate/deactivate this button based on that feature
            if (TableFunction == null)
                return;// throw new NotImplementedException();

            //var selectedString = IsRichText(rtbDocument.SelectedRtf) ? rtbDocument.SelectedRtf : rtbDocument.SelectedText;
            TableFunction(rtbDocument.SelectedText);
        }

        #endregion Toolstrip items handling

        #region Public methods

        public Func<string, string> EncryptFunction, DecryptFunction, CalculateFunction, TableFunction;

        public static bool IsRichText(string testString)
        {
            return (testString != null) && (testString.Trim().StartsWith("{\\rtf"));
        }

        public static string StripRTF(string rtfString)
        {
            string result = rtfString;

            try
            {
                if (IsRichText(rtfString))
                {
                    // Put body into a RichTextBox so we can strip RTF
                    using (var rtfTemp = new RichTextBox())
                    {
                        rtfTemp.Rtf = rtfString;
                        result = rtfTemp.Text;
                    }
                }
                else
                {
                    result = rtfString;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }

        public void SetFontFamily(FontFamily family)
        {
            if (family != null)
            {
                tscmbFont.SelectedItem = family.Name;
            }
        }

        public void SetFontSize(float newSize)
        {
            tscmbFontSize.Text = newSize.ToString();
        }

        public void ToggleBold()
        {
            tsbtnBold.PerformClick();
        }

        public void ToggleItalic()
        {
            tsbtnItalic.PerformClick();
        }

        public void ToggleUnderline()
        {
            tsbtnUnderline.PerformClick();
        }

        public void ToggleStrikeOut()
        {
            tsbtnStrikeout.PerformClick();
        }

        public void SetAlign(HorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    tsbtnAlignCenter.PerformClick();
                    break;

                case HorizontalAlignment.Left:
                    tsbtnAlignLeft.PerformClick();
                    break;

                case HorizontalAlignment.Right:
                    tsbtnAlignRight.PerformClick();
                    break;
            }
        }

        public void Indent()
        {
            tsbtnIndent.PerformClick();
        }

        public void Outdent()
        {
            tsbtnOutdent.PerformClick();
        }

        public void ToggleBullets()
        {
            tsbtnBullets.PerformClick();
        }

        public void ZoomIn()
        {
            tsbtnZoomIn.PerformClick();
        }

        public void ZoomOut()
        {
            tsbtnZoomOut.PerformClick();
        }

        public void ZoomTo(float factor)
        {
            rtbDocument.ZoomFactor = factor;
        }

        public void SetWordWrap(bool activated)
        {
            rtbDocument.WordWrap = activated;
        }

        #endregion Public methods

        #region Overrides

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                foreach (Control control in Controls)
                    control.BackColor = value;
                base.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                foreach (Control control in Controls)
                    control.ForeColor = value;
                base.ForeColor = value;
            }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                foreach (Control control in Controls)
                    control.Font = value;
                base.Font = value;
            }
        }

        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                //foreach (Control control in Controls)
                //    control.Anchor = value;
                base.Anchor = value;
            }
        }

        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                //foreach (Control control in Controls)
                //    control.AutoSize = value;
                base.AutoSize = value;
            }
        }

        #endregion Overrides

        #region Context menu handlers

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Clear();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbDocument.Redo();
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnAlignLeft.PerformClick();

            leftToolStripMenuItem.Checked = true;
            centerToolStripMenuItem.Checked = false;
            rightToolStripMenuItem.Checked = false;
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnAlignCenter.PerformClick();

            leftToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = true;
            rightToolStripMenuItem.Checked = false;
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnAlignRight.PerformClick();

            leftToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = false;
            rightToolStripMenuItem.Checked = true;
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnBold.PerformClick();
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnItalic.PerformClick();
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnUnderline.PerformClick();
        }

        private void strikeoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnStrikeout.PerformClick();
        }

        private void increaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnIndent.PerformClick();
        }

        private void decreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnOutdent.PerformClick();
        }

        private void bulletsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnBullets.PerformClick();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnZoomIn.PerformClick();
        }

        private void zoomOuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnZoomOut.PerformClick();
        }

        private void insertPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnInsertPicture.PerformClick();
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnTable.PerformClick();
        }

        private void calculateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnCalculate.PerformClick();
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnEncrypt.PerformClick();
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsbtnDecrypt.PerformClick();
        }

        #endregion Context menu handlers

        #region Find and Replace

        private void tsbtnFind_Click(object sender, EventArgs e)
        {
            FindForm findForm = new FindForm();
            findForm.RtbInstance = this.rtbDocument;
            findForm.InitialText = this.tstxtSearchText.Text;
            findForm.Show();
        }

        private void tsbtnReplace_Click(object sender, EventArgs e)
        {
            ReplaceForm replaceForm = new ReplaceForm();
            replaceForm.RtbInstance = this.rtbDocument;
            replaceForm.InitialText = this.tstxtSearchText.Text;
            replaceForm.Show();
        }

        #endregion Find and Replace
    }
}