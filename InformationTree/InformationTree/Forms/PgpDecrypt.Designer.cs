namespace InformationTree.Forms
{
    partial class PgpDecrypt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PgpDecrypt));
            this.mtbPgpDecrypt = new System.Windows.Forms.MaskedTextBox();
            this.lblDecryptingPassword = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mtbPgpDecrypt
            // 
            this.mtbPgpDecrypt.Location = new System.Drawing.Point(12, 32);
            this.mtbPgpDecrypt.Name = "mtbPgpDecrypt";
            this.mtbPgpDecrypt.Size = new System.Drawing.Size(277, 20);
            this.mtbPgpDecrypt.TabIndex = 0;
            this.mtbPgpDecrypt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mtbPgpDecrypt_KeyUp);
            // 
            // lblDecryptingPassword
            // 
            this.lblDecryptingPassword.AutoSize = true;
            this.lblDecryptingPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDecryptingPassword.Location = new System.Drawing.Point(8, 9);
            this.lblDecryptingPassword.Name = "lblDecryptingPassword";
            this.lblDecryptingPassword.Size = new System.Drawing.Size(209, 18);
            this.lblDecryptingPassword.TabIndex = 1;
            this.lblDecryptingPassword.Text = "Password for PGP decryption:";
            this.lblDecryptingPassword.Click += new System.EventHandler(this.lblDecryptingPassword_Click);
            // 
            // PgpDecrypt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 59);
            this.Controls.Add(this.lblDecryptingPassword);
            this.Controls.Add(this.mtbPgpDecrypt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(324, 98);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(324, 98);
            this.Name = "PgpDecrypt";
            this.Text = "Decrypt data files";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox mtbPgpDecrypt;
        private System.Windows.Forms.Label lblDecryptingPassword;
    }
}