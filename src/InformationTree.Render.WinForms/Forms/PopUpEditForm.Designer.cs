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

            if (disposing && tbExitPopUpAndSave != null)
            {
                tbExitPopUpAndSave.Click -= tbExitPopUpAndSave_Click;
                tbExitPopUpAndSave.Dispose();
            }

            if (disposing && tbData != null)
            {
                tbData.LinkClicked -= tbData_LinkClicked;
                tbData.DocumentKeyUp -= tbData_KeyUp;
                tbData.KeyUp -= PopUpEditForm_KeyUp;
                
                if (tbData.TextBox != null)
                {
                    tbData.TextBox.KeyUp -= PopUpEditForm_KeyUp;
                    tbData.TextBox = null;
                }
                
                tbData.Dispose();
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
            this.btnPgpEncryptData = new System.Windows.Forms.Button();
            this.btnPgpDecryptData = new System.Windows.Forms.Button();
            this.lblEncryption = new System.Windows.Forms.Label();
            this.cbFromFile = new System.Windows.Forms.CheckBox();
            this.cbKeepCrypt = new System.Windows.Forms.CheckBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnShowGraphics = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPgpEncryptData
            // 
            this.btnPgpEncryptData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgpEncryptData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPgpEncryptData.Location = new System.Drawing.Point(493, 557);
            this.btnPgpEncryptData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnPgpEncryptData.Name = "btnPgpEncryptData";
            this.btnPgpEncryptData.Size = new System.Drawing.Size(135, 27);
            this.btnPgpEncryptData.TabIndex = 2;
            this.btnPgpEncryptData.Text = "PGP Encrypt Data";
            this.btnPgpEncryptData.UseVisualStyleBackColor = true;
            this.btnPgpEncryptData.Click += new System.EventHandler(this.btnPgpEncryptData_Click);
            // 
            // btnPgpDecryptData
            // 
            this.btnPgpDecryptData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgpDecryptData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPgpDecryptData.Location = new System.Drawing.Point(636, 557);
            this.btnPgpDecryptData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnPgpDecryptData.Name = "btnPgpDecryptData";
            this.btnPgpDecryptData.Size = new System.Drawing.Size(135, 27);
            this.btnPgpDecryptData.TabIndex = 3;
            this.btnPgpDecryptData.Text = "PGP Decrypt Data";
            this.btnPgpDecryptData.UseVisualStyleBackColor = true;
            this.btnPgpDecryptData.Click += new System.EventHandler(this.btnPgpDecryptData_Click);
            // 
            // lblEncryption
            // 
            this.lblEncryption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEncryption.AutoSize = true;
            this.lblEncryption.Location = new System.Drawing.Point(37, 563);
            this.lblEncryption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEncryption.Name = "lblEncryption";
            this.lblEncryption.Size = new System.Drawing.Size(83, 15);
            this.lblEncryption.TabIndex = 5;
            this.lblEncryption.Text = "No encryption";
            // 
            // cbFromFile
            // 
            this.cbFromFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFromFile.AutoSize = true;
            this.cbFromFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFromFile.Location = new System.Drawing.Point(413, 563);
            this.cbFromFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbFromFile.Name = "cbFromFile";
            this.cbFromFile.Size = new System.Drawing.Size(70, 19);
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
            this.cbKeepCrypt.Location = new System.Drawing.Point(324, 563);
            this.cbKeepCrypt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbKeepCrypt.Name = "cbKeepCrypt";
            this.cbKeepCrypt.Size = new System.Drawing.Size(79, 19);
            this.cbKeepCrypt.TabIndex = 7;
            this.cbKeepCrypt.Text = "Keep crypt";
            this.cbKeepCrypt.UseVisualStyleBackColor = true;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCalculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalculate.Location = new System.Drawing.Point(12, 520);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(135, 27);
            this.btnCalculate.TabIndex = 8;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnShowGraphics
            // 
            this.btnShowGraphics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowGraphics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowGraphics.Location = new System.Drawing.Point(154, 520);
            this.btnShowGraphics.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowGraphics.Name = "btnShowGraphics";
            this.btnShowGraphics.Size = new System.Drawing.Size(173, 27);
            this.btnShowGraphics.TabIndex = 9;
            this.btnShowGraphics.Text = "Show graphics for selected";
            this.btnShowGraphics.UseVisualStyleBackColor = true;
            this.btnShowGraphics.Click += new System.EventHandler(this.btnShowGraphics_Click);
            // 
            // PopUpEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 598);
            this.Controls.Add(this.btnShowGraphics);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.cbKeepCrypt);
            this.Controls.Add(this.cbFromFile);
            this.Controls.Add(this.lblEncryption);
            this.Controls.Add(this.btnPgpDecryptData);
            this.Controls.Add(this.btnPgpEncryptData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "PopUpEditForm";
            this.Text = "Data edit";
            this.DoubleClick += new System.EventHandler(this.PopUpEditForm_DoubleClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PopUpEditForm_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnPgpEncryptData;
        private System.Windows.Forms.Button btnPgpDecryptData;
        private System.Windows.Forms.Label lblEncryption;
        private System.Windows.Forms.CheckBox cbFromFile;
        private System.Windows.Forms.CheckBox cbKeepCrypt;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnShowGraphics;
    }
}