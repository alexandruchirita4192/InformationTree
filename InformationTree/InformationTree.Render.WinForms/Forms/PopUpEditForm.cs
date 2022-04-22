using System;
using System.IO;
using System.Windows.Forms;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Render.WinForms.Extensions;
using InformationTree.Render.WinForms.Services;
using InformationTree.Tree;
using NLog;
using RicherTextBox.Controls;

namespace InformationTree.Forms
{
    public partial class PopUpEditForm : Form
    {
        #region Fields

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ICanvasFormFactory _canvasFormFactory;
        private readonly IPopUpService _popUpService;
        private readonly IPGPEncryptionAndSigningProvider _encryptionAndSigningProvider;
        private readonly IConfigurationReader _configurationReader;
        private readonly Configuration _configuration;
        
        private ICanvasForm _canvasForm;
        private Button tbExitPopUpAndSave;
        private RicherTextBox.Controls.RicherTextBox tbData;

        #endregion Fields

        #region Properties

        private string data;

        public string Data
        {
            get
            {
                if (data != null)
                    return data;
                if (tbData.TextLength == 0)
                    return string.Empty;
                return tbData.Rtf;
            }
        }

        private bool DataIsPgpEncrypted
        {
            get
            {
                return tbData.Text.Trim().StartsWith("-----BEGIN PGP");
            }
        }

        public bool FromFile;

        public string PgpPassword;
        public string PgpKeyFile;
        public string PgpKeyText;

        #endregion Properties

        #region Constructor

        [Obsolete("Designer use only")]
        public PopUpEditForm()
        {
            InitializeComponent();
            InitializeRicherTextBoxControlAndSaveAndCloseButton();
        }

        private PopUpEditForm(
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IPGPEncryptionAndSigningProvider encryptionAndSigningProvider,
            IConfigurationReader configurationReader)
        {
            _canvasFormFactory = canvasFormFactory;
            _popUpService = popUpService;
            _encryptionAndSigningProvider = encryptionAndSigningProvider;
            _configurationReader = configurationReader;
            _configuration = _configurationReader.GetConfiguration();

            InitializeComponent();
            InitializeRicherTextBoxControlAndSaveAndCloseButton();

            FormClosing += tbExitPopUpAndSave_Click;

            if (tbData != null && tbData.TextBox != null)
                tbData.TextBox.AllowDrop = true;
        }

        public PopUpEditForm(
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IPGPEncryptionAndSigningProvider encryptionAndSigningProvider,
            IConfigurationReader configurationReader,
            string title,
            string data)
            : this(canvasFormFactory, popUpService, encryptionAndSigningProvider, configurationReader)
        {
            Text += $": {title}";

            try
            {
                tbData.Rtf = data;
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, $"RTF data is not valid: {data}");
                tbData.Text = data;
            }

            var richTextBoxForeColor = tbData.ForeColor;
            if (richTextBoxForeColor == TreeNodeHelper.DefaultBackGroundColor)
                tbData.ForeColor = TreeNodeHelper.DefaultForeGroundColor;

            var richTextBoxBackColor = tbData.BackColor;
            if (richTextBoxBackColor != TreeNodeHelper.DefaultBackGroundColor)
                tbData.BackColor = TreeNodeHelper.DefaultBackGroundColor;

            if (tbData?.TextBox?.Text != null)
            {
                for (int i = 0; i < tbData.TextBox.Text.Length; i++)
                {
                    tbData.TextBox.Select(i, 1);
                    if (tbData.TextBox.SelectionColor == TreeNodeHelper.DefaultBackGroundColor)
                        tbData.TextBox.SelectionColor = TreeNodeHelper.DefaultForeGroundColor;

                    if (tbData.TextBox.SelectionBackColor != TreeNodeHelper.DefaultBackGroundColor)
                        tbData.TextBox.SelectionBackColor = TreeNodeHelper.DefaultBackGroundColor;
                }
            }
        }

        private void InitializeRicherTextBoxControlAndSaveAndCloseButton()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(PopUpEditForm));

            tbExitPopUpAndSave = new Button
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Location = new System.Drawing.Point(778, 557),
                Margin = new Padding(4, 3, 4, 3),
                Name = "tbExitPopUpAndSave",
                Size = new System.Drawing.Size(168, 27),
                TabIndex = 1,
                Text = "Exit Pop-Up and Save",
                UseVisualStyleBackColor = true
            };
            tbExitPopUpAndSave.Click += tbExitPopUpAndSave_Click;

            // Note: CalculateVisible, EncryptDecryptCategoryVisible, DecryptVisible, EncryptVisible, SeparatorNewButtonsVisible, TableVisible
            // shouldn't be set manually in here because RicherTextBox constructor uses _configurationReader.GetConfiguration()
            // to set the values based on configuration (visible and enabled set too).

            tbData = new RicherTextBox.Controls.RicherTextBox(_popUpService, _configurationReader)
            {
                AlignCenterVisible = true,
                AlignLeftVisible = true,
                AlignRightVisible = true,
                AllowDrop = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoScroll = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BoldVisible = true,
                BulletsVisible = true,
                ChooseFontVisible = true,
                FindReplaceVisible = true,
                FontColorVisible = true,
                FontFamilyVisible = true,
                FontSizeVisible = true,
                GlobalVisibility = true,
                GroupAlignmentVisible = true,
                GroupBoldUnderlineItalicVisible = true,
                GroupFontColorVisible = true,
                GroupFontNameAndSizeVisible = true,
                GroupIndentationAndBulletsVisible = true,
                GroupInsertVisible = true,
                GroupSaveAndLoadVisible = true,
                GroupZoomVisible = true,
                INDENT = 10,
                IndentVisible = true,
                InsertPictureVisible = true,
                ItalicVisible = true,
                LoadVisible = true,
                Location = new System.Drawing.Point(15, 15),
                Margin = new Padding(5, 5, 5, 5),
                Name = "tbData",
                OutdentVisible = true,
                Rtf = resources.GetString("tbData.Rtf"),
                SaveVisible = true,
                SeparatorAlignVisible = true,
                SeparatorBoldUnderlineItalicVisible = true,
                SeparatorFontColorVisible = true,
                SeparatorFontVisible = true,
                SeparatorIndentAndBulletsVisible = true,
                SeparatorInsertVisible = true,
                SeparatorSaveLoadVisible = true,
                Size = new System.Drawing.Size(931, 497),
                TabIndex = 0,
                ToolStripVisible = true,
                UnderlineVisible = true,
                WordWrapVisible = true,
                ZoomFactorTextVisible = true,
                ZoomInVisible = true,
                ZoomOutVisible = true
            };
            tbData.LinkClicked += tbData_LinkClicked;
            tbData.DocumentKeyUp += tbData_KeyUp;
            tbData.KeyUp += PopUpEditForm_KeyUp;
            tbData.TextBox.KeyUp += PopUpEditForm_KeyUp;

            tbData.TableFunction = (target) =>
            {
                var tableControl = new TableControl(target, tbData);

                ////rtbDocument.Rtf = rtbDocument.Rtf.Replace() // TODO: change here to see the table in RTF
                return target;
            };

            tbData.EncryptFunction = (target) =>
            {
                if (btnPgpEncryptData != null)
                    btnPgpEncryptData.PerformClick();

                // Data should be already set by btnPgpEncryptData.PerformClick() if everything was ok
                return tbData.Rtf;
            };

            tbData.DecryptFunction = (target) =>
            {
                if (btnPgpDecryptData != null)
                    btnPgpDecryptData.PerformClick();

                // Data should be already set by btnPgpDecryptData.PerformClick() if everything was ok
                return tbData.Rtf;
            };

            // TODO: Make tbData.CalculateFunction work with a new facade, interface for calculation service

            Controls.Add(tbData);
            Controls.Add(tbExitPopUpAndSave);
        }

        #endregion Constructor

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.V:
                case Keys.Shift | Keys.Insert:
                    if (Clipboard.ContainsImage())
                    {
                        var image = Clipboard.GetImage();
                        var maxWidth = tbData.Width - 10;
                        var maxHeight = tbData.Height - 10;

                        var newWidth = maxWidth;
                        var newHeight = image.Height * newWidth / image.Width;

                        if (newHeight > maxHeight)
                        {
                            // Resize with height instead
                            newWidth = image.Width * maxHeight / image.Height;
                            newHeight = maxHeight;
                        }

                        if (newWidth != image.Width || newHeight != image.Height)
                        {
                            image = image.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
                            Clipboard.SetImage(image);
                        }
                    }

                    return base.ProcessCmdKey(ref msg, keyData);

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void tbExitPopUpAndSave_Click(object sender, EventArgs e)
        {
            if (object.ReferenceEquals(sender, tbExitPopUpAndSave))
                this.Close();
        }

        private void tbData_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void tbData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.B | Keys.Control))
                tbData.ToggleBold();
            //else if (e.KeyData == (Keys.I | Keys.Control))
            //    tbData.ToggleItalic();
            else if (e.KeyData == (Keys.U | Keys.Control))
                tbData.ToggleUnderline();
            else if (e.KeyData == (Keys.S | Keys.Control))
                tbData.ToggleStrikeOut();
            //else if (e.KeyData == (Keys.Left | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Left);
            //else if (e.KeyData == (Keys.Up | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Center);
            //else if (e.KeyData == (Keys.Right | Keys.Control))
            //    tbData.SetAlign(HorizontalAlignment.Right);
            else if (e.KeyData == (Keys.O | Keys.Control))
                tbData.OpenFile();
            else if (e.KeyData == (Keys.S | Keys.Control))
                tbData.SaveFile();
            else if (e.KeyData == (Keys.P | Keys.Control))
                tbData.InsertPicture();
        }

        private void btnPgpDecryptData_Click(object sender, EventArgs e)
        {
            data = cbKeepCrypt.Checked ? Data : null;

            if (string.IsNullOrWhiteSpace(tbData.Text))
            {
                _popUpService.ShowError("No data to decrypt.");
                return;
            }

            if (DataIsPgpEncrypted ||
                _popUpService.ShowQuestion("Data seems to be decrypted. Try to decrypt anyway?") == PopUpResult.Yes)
            {
                var form = new PgpDecrypt(_popUpService, _encryptionAndSigningProvider, cbFromFile.Checked);
                if (!form.IsDisposed)
                {
                    WinFormsApplication.CenterForm(form, this);

                    form.FormClosed += PgpDecryptForm_FormClosed;
                    form.ShowDialog();
                }
            }
        }

        private void PgpDecryptForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as PgpDecrypt;
            if (form != null)
            {
                PgpPassword = form.PgpPassword;

                //FromFile = form.DecryptFromFile;
                PgpKeyFile = form.PgpPrivateKeyFile;
                PgpKeyText = form.PgpPrivateKeyText;

                var result = string.Empty;
                if (FromFile)
                {
                    result = _encryptionAndSigningProvider.GetDecryptedStringFromFile(tbData.Text, PgpKeyFile, PgpPassword);
                    lblEncryption.Text = "Decrypted with key: " + PgpKeyFile;
                }
                else
                {
                    result = _encryptionAndSigningProvider.GetDecryptedStringFromString(tbData.Text, PgpKeyText, PgpPassword);
                    lblEncryption.Text = "Decrypted with node key";
                }

                tbData.Rtf = result;
            }
        }

        private void btnPgpEncryptData_Click(object sender, EventArgs e)
        {
            FromFile = cbFromFile.Checked;

            var result = _popUpService.ShowQuestion("Do you want to encrypt as RTF? (Otherwise it would be text only.)", "Encrypt as RTF?", DefaultPopUpButton.No);
            var decryptedData = result == PopUpResult.Yes ? tbData.Rtf : tbData.Text;
            
            if (string.IsNullOrWhiteSpace(decryptedData))
            {
                _popUpService.ShowError("No data to encrypt.");
                return;
            }
            
            if (FromFile)
            {
                PgpKeyFile = _popUpService.GetPublicKeyFile();

                if (_configuration.TreeFeatures.EnableEncryptionSigning)
                {
                    PgpKeyText = File.ReadAllText(PgpKeyFile);

                    // TODO: Get the private key and password also, for encryption with signing, and do not get both because is redundant

                    var resultedText = _encryptionAndSigningProvider.EncryptAndSignString(tbData.Rtf, PgpKeyText, string.Empty, string.Empty, true);
                    resultedText = RicherTextBox.Controls.RicherTextBox.StripRTF(resultedText);

                    if (tbData.Text != resultedText)
                    {
                        data = null;
                        tbData.Text = resultedText;
                        lblEncryption.Text = "Encrypted with key: " + PgpKeyFile;
                    }
                }
                else
                {
                    using (var decryptedStream = decryptedData.ToStream())
                    {
                        using (var outputStream = new MemoryStream())
                        {
                            _encryptionAndSigningProvider.EncryptFromFile(decryptedStream, outputStream, PgpKeyFile, true, true);

                            using (var reader = new StreamReader(outputStream))
                            {
                                var resultedText = reader.ReadToEnd();
                                if (tbData.Text != resultedText)
                                {
                                    data = null;
                                    tbData.Text = resultedText;
                                    lblEncryption.Text = "Encrypted with key: " + PgpKeyFile;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                FindNodeWithPublicKey();

                var  resultedText = string.Empty;
                if (_configuration.TreeFeatures.EnableEncryptionSigning)
                {
                    // TODO: Get the private key and password also, for encryption with signing, and do not get both because is redundant

                    resultedText = _encryptionAndSigningProvider.EncryptAndSignString(tbData.Rtf, PgpKeyText, string.Empty, string.Empty, true);
                }
                else
                {
                    resultedText = _encryptionAndSigningProvider.GetEncryptedStringFromString(tbData.Rtf, PgpKeyText, true, true);
                }
                resultedText = RicherTextBox.Controls.RicherTextBox.StripRTF(resultedText);

                if (tbData.Text != resultedText)
                {
                    data = null;
                    tbData.Text = resultedText;
                    lblEncryption.Text = "Encrypted with node key";
                }
            }
        }

        private void FindNodeWithPublicKey()
        {
            var form = new SearchForm(_popUpService);

            WinFormsApplication.CenterForm(form, WinFormsApplication.MainForm);

            form.FormClosed += SearchForm_FormClosed;
            form.ShowDialog();
        }

        private void SearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as SearchForm;
            if (form != null)
            {
                var textToFind = form.TextToFind;

                if (WinFormsApplication.MainForm == null || WinFormsApplication.MainForm.TaskList == null || WinFormsApplication.MainForm.TaskList.Nodes == null)
                    return;

                var node = TreeNodeHelper.GetFirstNode(WinFormsApplication.MainForm.TaskList.Nodes, textToFind);
                if (node != null)
                {
                    var nodeData = node.GetTreeNodeData();
                    if (nodeData != null)
                    {
                        PgpKeyText = RicherTextBox.Controls.RicherTextBox.StripRTF(nodeData.Data);

                        _popUpService.ShowMessage($"Public key taken from data of node {node.Text}", $"Node {node.Text} used");
                    }
                }

                if (string.IsNullOrEmpty(PgpKeyText) && _popUpService.ShowQuestion("You did not select a valid public key node. Try to select again?") == PopUpResult.Yes)
                    FindNodeWithPublicKey();
            }
        }

        private void PopUpEditForm_DoubleClick(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            else
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        }

        private void PopUpEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            // TODO: Hide unfinished feature using a flag
            _popUpService.ShowInfo("This feature is not finished yet. It will be available in the next version.");
        }

        // TODO: hide or show this button based on graphics feature?
        private void btnShowGraphics_Click(object sender, EventArgs e)
        {
            var text = tbData.TextBox.SelectedText.Length > 0 ? tbData.TextBox.SelectedText : tbData.TextBox.Text;
            if (text.Length <= 0)
                return;

            if (_canvasForm == null || _canvasForm.IsDisposed)
                _canvasForm = _canvasFormFactory.Create(new[] { text });
            else
            {
                _canvasForm.RunTimer.Stop();
                _canvasForm.GraphicsFile.Clean();
                _canvasForm.GraphicsFile.ParseLines(new[] { text });
                _canvasForm.RunTimer.Start();
            }

            _canvasForm.Show();
        }
    }
}