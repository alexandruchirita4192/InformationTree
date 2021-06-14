namespace InformationTree.Forms
{
    partial class CanvasPopUpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CanvasPopUpForm));
            this.SuspendLayout();
            // 
            // CanvasPopUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(434, 411);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CanvasPopUpForm";
            this.Text = "Canvas pop-up";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasPopUpForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CanvasPopUpForm_KeyDown);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CanvasPopUpForm_MouseDoubleClick);
            this.Resize += new System.EventHandler(this.CanvasPopUpForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}