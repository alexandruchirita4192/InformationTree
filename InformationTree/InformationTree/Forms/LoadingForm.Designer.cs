namespace InformationTree.Forms
{
    partial class LoadingForm
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

            // Timer dispose
            if (disposing && Timer != null)
                Timer.Elapsed -= T_Elapsed;

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbFileLoad = new System.Windows.Forms.ProgressBar();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pbLoadingGraphics = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingGraphics)).BeginInit();
            this.SuspendLayout();
            // 
            // pbFileLoad
            // 
            this.pbFileLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFileLoad.Location = new System.Drawing.Point(59, 457);
            this.pbFileLoad.Name = "pbFileLoad";
            this.pbFileLoad.Size = new System.Drawing.Size(291, 23);
            this.pbFileLoad.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbFileLoad.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Location = new System.Drawing.Point(12, 441);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(394, 13);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Loading (processing XML file)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pbLoadingGraphics
            // 
            this.pbLoadingGraphics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLoadingGraphics.Location = new System.Drawing.Point(12, 12);
            this.pbLoadingGraphics.Name = "pbLoadingGraphics";
            this.pbLoadingGraphics.Size = new System.Drawing.Size(394, 426);
            this.pbLoadingGraphics.TabIndex = 3;
            this.pbLoadingGraphics.TabStop = false;
            this.pbLoadingGraphics.Paint += new System.Windows.Forms.PaintEventHandler(this.pbLoadingGraphics_Paint);
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 492);
            this.Controls.Add(this.pbLoadingGraphics);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbFileLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadingForm";
            this.Text = "LoadingForm";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingGraphics)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar pbFileLoad;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pbLoadingGraphics;
    }
}