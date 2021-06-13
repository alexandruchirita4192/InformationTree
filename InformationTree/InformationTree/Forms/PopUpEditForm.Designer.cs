using RicherTextBox.Controls;

namespace InformationTree.Forms
{
    partial class PopUpEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopUpEditForm));
            this.tbExitPopUpAndSave = new System.Windows.Forms.Button();
            this.btnPgpEncryptData = new System.Windows.Forms.Button();
            this.btnPgpDecryptData = new System.Windows.Forms.Button();
            this.lblEncryption = new System.Windows.Forms.Label();
            this.cbFromFile = new System.Windows.Forms.CheckBox();
            this.cbKeepCrypt = new System.Windows.Forms.CheckBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnShowGraphics = new System.Windows.Forms.Button();
            this.tbData = new RicherTextBox.Controls.RicherTextBox();
            this.SuspendLayout();
            // 
            // tbExitPopUpAndSave
            // 
            this.tbExitPopUpAndSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbExitPopUpAndSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tbExitPopUpAndSave.ForeColor = System.Drawing.Color.LimeGreen;
            this.tbExitPopUpAndSave.Location = new System.Drawing.Point(667, 483);
            this.tbExitPopUpAndSave.Name = "tbExitPopUpAndSave";
            this.tbExitPopUpAndSave.Size = new System.Drawing.Size(144, 23);
            this.tbExitPopUpAndSave.TabIndex = 1;
            this.tbExitPopUpAndSave.Text = "Exit Pop-Up and Save";
            this.tbExitPopUpAndSave.UseVisualStyleBackColor = true;
            this.tbExitPopUpAndSave.Click += new System.EventHandler(this.tbExitPopUpAndSave_Click);
            // 
            // btnPgpEncryptData
            // 
            this.btnPgpEncryptData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgpEncryptData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPgpEncryptData.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnPgpEncryptData.Location = new System.Drawing.Point(423, 483);
            this.btnPgpEncryptData.Name = "btnPgpEncryptData";
            this.btnPgpEncryptData.Size = new System.Drawing.Size(116, 23);
            this.btnPgpEncryptData.TabIndex = 2;
            this.btnPgpEncryptData.Text = "PGP Encrypt Data";
            this.btnPgpEncryptData.UseVisualStyleBackColor = true;
            this.btnPgpEncryptData.Click += new System.EventHandler(this.btnPgpEncryptData_Click);
            // 
            // btnPgpDecryptData
            // 
            this.btnPgpDecryptData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgpDecryptData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPgpDecryptData.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnPgpDecryptData.Location = new System.Drawing.Point(545, 483);
            this.btnPgpDecryptData.Name = "btnPgpDecryptData";
            this.btnPgpDecryptData.Size = new System.Drawing.Size(116, 23);
            this.btnPgpDecryptData.TabIndex = 3;
            this.btnPgpDecryptData.Text = "PGP Decrypt Data";
            this.btnPgpDecryptData.UseVisualStyleBackColor = true;
            this.btnPgpDecryptData.Click += new System.EventHandler(this.btnPgpDecryptData_Click);
            // 
            // lblEncryption
            // 
            this.lblEncryption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEncryption.AutoSize = true;
            this.lblEncryption.BackColor = System.Drawing.Color.White;
            this.lblEncryption.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblEncryption.Location = new System.Drawing.Point(32, 488);
            this.lblEncryption.Name = "lblEncryption";
            this.lblEncryption.Size = new System.Drawing.Size(73, 13);
            this.lblEncryption.TabIndex = 5;
            this.lblEncryption.Text = "No encryption";
            // 
            // cbFromFile
            // 
            this.cbFromFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFromFile.AutoSize = true;
            this.cbFromFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFromFile.ForeColor = System.Drawing.Color.LimeGreen;
            this.cbFromFile.Location = new System.Drawing.Point(352, 487);
            this.cbFromFile.Name = "cbFromFile";
            this.cbFromFile.Size = new System.Drawing.Size(62, 17);
            this.cbFromFile.TabIndex = 6;
            this.cbFromFile.Text = "From file";
            this.cbFromFile.UseVisualStyleBackColor = true;
            // 
            // cbKeepCrypt
            // 
            this.cbKeepCrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbKeepCrypt.AutoSize = true;
            this.cbKeepCrypt.Checked = true;
            this.cbKeepCrypt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbKeepCrypt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbKeepCrypt.ForeColor = System.Drawing.Color.LimeGreen;
            this.cbKeepCrypt.Location = new System.Drawing.Point(272, 487);
            this.cbKeepCrypt.Name = "cbKeepCrypt";
            this.cbKeepCrypt.Size = new System.Drawing.Size(74, 17);
            this.cbKeepCrypt.TabIndex = 7;
            this.cbKeepCrypt.Text = "Keep crypt";
            this.cbKeepCrypt.UseVisualStyleBackColor = true;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCalculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalculate.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnCalculate.Location = new System.Drawing.Point(10, 451);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(116, 23);
            this.btnCalculate.TabIndex = 8;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnShowGraphics
            // 
            this.btnShowGraphics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowGraphics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowGraphics.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnShowGraphics.Location = new System.Drawing.Point(132, 451);
            this.btnShowGraphics.Name = "btnShowGraphics";
            this.btnShowGraphics.Size = new System.Drawing.Size(148, 23);
            this.btnShowGraphics.TabIndex = 9;
            this.btnShowGraphics.Text = "Show graphics for selected";
            this.btnShowGraphics.UseVisualStyleBackColor = true;
            this.btnShowGraphics.Click += new System.EventHandler(this.btnShowGraphics_Click);
            // 
            // tbData
            // 
            this.tbData.AlignCenterVisible = true;
            this.tbData.AlignLeftVisible = true;
            this.tbData.AlignRightVisible = true;
            this.tbData.AllowDrop = true;
            this.tbData.AnalysisVisible = false;
            this.tbData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbData.AutoScroll = true;
            this.tbData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tbData.BackColor = System.Drawing.Color.White;
            this.tbData.BoldVisible = true;
            this.tbData.BulletsVisible = true;
            this.tbData.CalculateVisible = true;
            this.tbData.ChooseFontVisible = true;
            this.tbData.Cursor = System.Windows.Forms.Cursors.Default;
            this.tbData.DecryptVisible = true;
            this.tbData.EncryptDecryptCategoryVisible = true;
            this.tbData.EncryptVisible = true;
            this.tbData.FindReplaceVisible = true;
            this.tbData.FontColorVisible = true;
            this.tbData.FontFamilyVisible = true;
            this.tbData.FontSizeVisible = true;
            this.tbData.ForeColor = System.Drawing.Color.LimeGreen;
            this.tbData.GlobalVisibility = true;
            this.tbData.GroupAlignmentVisible = true;
            this.tbData.GroupBoldUnderlineItalicVisible = true;
            this.tbData.GroupFontColorVisible = true;
            this.tbData.GroupFontNameAndSizeVisible = true;
            this.tbData.GroupIndentationAndBulletsVisible = true;
            this.tbData.GroupInsertVisible = true;
            this.tbData.GroupSaveAndLoadVisible = true;
            this.tbData.GroupZoomVisible = true;
            this.tbData.AnalysisVisible = true;
            this.tbData.CalculateVisible = true;
            this.tbData.INDENT = 10;
            this.tbData.IndentVisible = true;
            this.tbData.InsertPictureVisible = true;
            this.tbData.ItalicVisible = true;
            this.tbData.LoadVisible = true;
            this.tbData.Location = new System.Drawing.Point(13, 13);
            this.tbData.Margin = new System.Windows.Forms.Padding(4);
            this.tbData.Name = "tbData";
            this.tbData.OutdentVisible = true;
            this.tbData.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0\\fnil\\fcharset204 Microsoft" +
    " Sans Serif;}}\r\n{\\colortbl ;\\red50\\green205\\blue50;}\r\n\\viewkind4\\uc1\\pard\\cf1\\f0" +
    "\\fs18\\par\r\n}\r\n";
            this.tbData.SaveVisible = true;
            this.tbData.SeparatorAlignVisible = true;
            this.tbData.SeparatorBoldUnderlineItalicVisible = true;
            this.tbData.SeparatorFontColorVisible = true;
            this.tbData.SeparatorFontVisible = true;
            this.tbData.SeparatorIndentAndBulletsVisible = true;
            this.tbData.SeparatorInsertVisible = true;
            this.tbData.SeparatorSaveLoadVisible = true;
            this.tbData.Size = new System.Drawing.Size(798, 431);
            this.tbData.TabIndex = 0;
            this.tbData.ToolStripVisible = true;
            this.tbData.UnderlineVisible = true;
            this.tbData.WordWrapVisible = true;
            this.tbData.ZoomFactorTextVisible = true;
            this.tbData.ZoomInVisible = true;
            this.tbData.ZoomOutVisible = true;
            this.tbData.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.tbData_LinkClicked);
            this.tbData.DocumentKeyUp += new System.Windows.Forms.KeyEventHandler(this.tbData_KeyUp);
            this.tbData.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PopUpEditForm_KeyUp);
            // 
            // PopUpEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(823, 518);
            this.Controls.Add(this.btnShowGraphics);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.cbKeepCrypt);
            this.Controls.Add(this.cbFromFile);
            this.Controls.Add(this.lblEncryption);
            this.Controls.Add(this.btnPgpDecryptData);
            this.Controls.Add(this.btnPgpEncryptData);
            this.Controls.Add(this.tbData);
            this.Controls.Add(this.tbExitPopUpAndSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PopUpEditForm";
            this.Text = "Data edit";
            this.DoubleClick += new System.EventHandler(this.PopUpEditForm_DoubleClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PopUpEditForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button tbExitPopUpAndSave;
        private RicherTextBox.Controls.RicherTextBox tbData;
        private System.Windows.Forms.Button btnPgpEncryptData;
        private System.Windows.Forms.Button btnPgpDecryptData;
        private System.Windows.Forms.Label lblEncryption;
        private System.Windows.Forms.CheckBox cbFromFile;
        private System.Windows.Forms.CheckBox cbKeepCrypt;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnShowGraphics;
    }
}