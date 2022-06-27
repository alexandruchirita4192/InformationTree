using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using InformationTree.Domain;
using InformationTree.Domain.Entities;
using InformationTree.Domain.Entities.Graphics;
using InformationTree.Domain.Extensions;
using InformationTree.Domain.Requests;
using InformationTree.Domain.Responses;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using MediatR;
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
        private readonly IConfigurationReader _configurationReader;
        private readonly IMediator _mediator;
        private readonly ICachingService _cachingService;
        private readonly Configuration _configuration;

        private Button tbExitPopUpAndSave;
        private RicherTextBox.Controls.RicherTextBox tbData;

        #endregion Fields

        #region Properties

        #region Data

        /// <summary>
        /// Data field to override what is returned from the pop up in case it is not empty.
        /// </summary>
        private string data;

        /// <summary>
        /// Returns the data from the RicherTextBox editor of the pop up.
        /// </summary>
        public string Data
        {
            get
            {
                if (data.IsNotEmpty())
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
                return tbData.Text.IsPgpEncrypted();
            }
        }

        #endregion Data

        private bool FromFile { get { return cbFromFile.Checked; } }

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
            IConfigurationReader configurationReader,
            IMediator mediator,
            ICachingService cachingService)
        {
            _canvasFormFactory = canvasFormFactory;
            _popUpService = popUpService;
            _configurationReader = configurationReader;
            _mediator = mediator;
            _cachingService = cachingService;

            _configuration = _configurationReader.GetConfiguration();

            InitializeComponent();
            InitializeRicherTextBoxControlAndSaveAndCloseButton();
            HideComponentsBasedOnFeatures();

            FormClosing += tbExitPopUpAndSave_Click;

            if (tbData != null && tbData.TextBox != null)
                tbData.TextBox.AllowDrop = true;
        }

        private void HideComponentsBasedOnFeatures()
        {
            if (_configuration == null)
                return;

            SetVisibleAndEnabled(btnCalculate, _configuration.RicherTextBoxFeatures.EnableCalculation);
            SetVisibleAndEnabled(btnShowGraphics, _configuration.ApplicationFeatures.EnableExtraGraphics);
            SetVisibleAndEnabled(btnPgpEncryptData, _configuration.TreeFeatures.EnableManualEncryption);
            SetVisibleAndEnabled(btnPgpDecryptData, _configuration.TreeFeatures.EnableManualEncryption);
            SetVisibleAndEnabled(btnCalculate, _configuration.RicherTextBoxFeatures.EnableCalculation);
            SetVisibleAndEnabled(cbKeepCrypt, _configuration.TreeFeatures.EnableManualEncryption);
            SetVisibleAndEnabled(cbFromFile, _configuration.TreeFeatures.EnableManualEncryption);
        }

        private void SetVisibleAndEnabled(Control control, bool visibleAndEnabled)
        {
            if (control == null)
                return;

            control.Visible = visibleAndEnabled;
            control.Enabled = visibleAndEnabled;
        }

        public PopUpEditForm(
            ICanvasFormFactory canvasFormFactory,
            IPopUpService popUpService,
            IConfigurationReader configurationReader,
            IMediator mediator,
            ICachingService cachingService,
            string title,
            string data)
            : this(canvasFormFactory, popUpService, configurationReader, mediator, cachingService)
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
            if (richTextBoxForeColor == Constants.Colors.DefaultBackGroundColor)
                tbData.ForeColor = Constants.Colors.DefaultForeGroundColor;

            var richTextBoxBackColor = tbData.BackColor;
            if (richTextBoxBackColor != Constants.Colors.DefaultBackGroundColor)
                tbData.BackColor = Constants.Colors.DefaultBackGroundColor;

            UpdateRichTextBoxSelectionColor();
        }

        private void UpdateRichTextBoxSelectionColor()
        {
            if (tbData?.TextBox == null)
                return;

            if (tbData.TextBox.SelectionColor != Constants.Colors.DefaultForeGroundColor)
                tbData.TextBox.SelectionColor = Constants.Colors.DefaultForeGroundColor;

            if (tbData.TextBox.SelectionBackColor != Constants.Colors.DefaultBackGroundColor)
                tbData.TextBox.SelectionBackColor = Constants.Colors.DefaultBackGroundColor;
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

            tbData = new RicherTextBox.Controls.RicherTextBox(_popUpService, _configurationReader, _mediator)
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
                if (!_configuration.RicherTextBoxFeatures.EnableTable)
                    return target;

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

            tbData.CalculateFunction = (target) =>
            {
                if (btnCalculate != null)
                    btnCalculate.PerformClick();

                return tbData.Rtf;
            };

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
                    // Prepare clipboard image from clipboard by resizing it to fit in RichTextBox current size
                    var request = new PrepareClipboardImagePasteRequest
                    {
                        MaxWidth = tbData.Width,
                        MaxHeight = tbData.Height
                    };
                    Task.Run(async () => await _mediator.Send(request));
                    
                    return base.ProcessCmdKey(ref msg, keyData);

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private async void tbExitPopUpAndSave_Click(object sender, EventArgs e)
        {
            var request = new PopUpEditFormExitPopUpAndSaveClickRequest
            {
                ExitPopUpAndSaveTextBox = tbExitPopUpAndSave,
                Form = this,
                Sender = sender
            };
            await _mediator.Send(request);
        }

        private async void tbData_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var request = new PopUpEditFormDataLinkClickedRequest
            {
                LinkText = e.LinkText
            };
            await _mediator.Send(request);
        }

        private async void tbData_KeyUp(object sender, KeyEventArgs e)
        {
            var request = new PopUpEditFormDataKeyUpRequest
            {
                DataRicherTextBox = tbData,
                KeyData = (int)e.KeyData
            };
            await _mediator.Send(request);
        }

        private async void btnPgpDecryptData_Click(object sender, EventArgs e)
        {
            data = cbKeepCrypt.Checked ? Data : null;

            var request = new PgpEncryptDecryptDataForRicherTextBoxRequest
            {
                DataRicherTextBox = tbData,
                EncryptionLabel = lblEncryption,
                ActionType = PgpActionType.Decrypt,
                FromFile = FromFile,
                DataIsPgpEncrypted = DataIsPgpEncrypted,
                FormToCenterTo = this
            };

            if (await _mediator.Send(request) is not PgpEncryptDecryptDataForRicherTextBoxResponse response)
                return;
            if (response.DataIsNull)
                data = null;
        }

        private async void btnPgpEncryptData_Click(object sender, EventArgs e)
        {
            var request = new PgpEncryptDecryptDataForRicherTextBoxRequest
            {
                DataRicherTextBox = tbData,
                EncryptionLabel = lblEncryption,
                ActionType = PgpActionType.Encrypt,
                FromFile = FromFile,
                DataIsPgpEncrypted = DataIsPgpEncrypted,
                FormToCenterTo = this
            };

            if (await _mediator.Send(request) is not PgpEncryptDecryptDataForRicherTextBoxResponse response)
                return;
            if (response.DataIsNull)
                data = null;
        }

        private async void PopUpEditForm_DoubleClick(object sender, EventArgs e)
        {
            var request = new FormMouseDoubleClickRequest
            {
                Form = this
            };
            await _mediator.Send(request);
        }
        
        private async void PopUpEditForm_KeyUp(object sender, KeyEventArgs e)
        {
            var request = new FormKeyUpRequest
            {
                Form = this,
                KeyData = (int)e.KeyData
            };
            await _mediator.Send(request);
        }

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            var request = new PopUpEditFormCalculateClickRequest();
            await _mediator.Send(request);
        }

        private async void btnShowGraphics_Click(object sender, EventArgs e)
        {
            var text = tbData.TextBox.SelectedText.Length > 0 ? tbData.TextBox.SelectedText : tbData.TextBox.Text;
            if (text.Length <= 0)
                return;
            
            var request = new ShowCanvasPopUpRequest
            {
                FigureLines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
            };
            await _mediator.Send(request);
        }
    }
}