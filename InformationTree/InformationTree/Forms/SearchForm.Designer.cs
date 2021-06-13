namespace FormsGame.Forms
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.tbFind = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbFind
            // 
            this.tbFind.AllowDrop = true;
            this.tbFind.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFind.BackColor = System.Drawing.Color.White;
            this.tbFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.tbFind.ForeColor = System.Drawing.Color.LimeGreen;
            this.tbFind.Location = new System.Drawing.Point(12, 12);
            this.tbFind.MaximumSize = new System.Drawing.Size(772, 25);
            this.tbFind.MinimumSize = new System.Drawing.Size(772, 25);
            this.tbFind.Multiline = true;
            this.tbFind.Name = "tbFind";
            this.tbFind.Size = new System.Drawing.Size(772, 25);
            this.tbFind.TabIndex = 1;
            this.tbFind.WordWrap = false;
            this.tbFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbFind_KeyUp);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(796, 49);
            this.Controls.Add(this.tbFind);
            this.ForeColor = System.Drawing.Color.LimeGreen;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(812, 88);
            this.MinimumSize = new System.Drawing.Size(812, 88);
            this.Name = "SearchForm";
            this.Text = "Search box";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFind;
    }
}