namespace InformationTree.Forms
{
    partial class StartupAlertForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupAlertForm));
            this.tvAlertCategoryAndTasks = new System.Windows.Forms.TreeView();
            this.btnSelectTaskOrCategory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvAlertCategoryAndTasks
            // 
            this.tvAlertCategoryAndTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvAlertCategoryAndTasks.Location = new System.Drawing.Point(13, 13);
            this.tvAlertCategoryAndTasks.Name = "tvAlertCategoryAndTasks";
            this.tvAlertCategoryAndTasks.Size = new System.Drawing.Size(490, 191);
            this.tvAlertCategoryAndTasks.TabIndex = 0;
            // 
            // btnSelectTaskOrCategory
            // 
            this.btnSelectTaskOrCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTaskOrCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectTaskOrCategory.Location = new System.Drawing.Point(374, 210);
            this.btnSelectTaskOrCategory.Name = "btnSelectTaskOrCategory";
            this.btnSelectTaskOrCategory.Size = new System.Drawing.Size(129, 23);
            this.btnSelectTaskOrCategory.TabIndex = 1;
            this.btnSelectTaskOrCategory.Text = "Select task or category";
            this.btnSelectTaskOrCategory.UseVisualStyleBackColor = true;
            this.btnSelectTaskOrCategory.Click += new System.EventHandler(this.btnSelectTaskOrCategory_Click);
            // 
            // StartupAlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 245);
            this.Controls.Add(this.btnSelectTaskOrCategory);
            this.Controls.Add(this.tvAlertCategoryAndTasks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StartupAlertForm";
            this.Text = "Startup alert window";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvAlertCategoryAndTasks;
        private System.Windows.Forms.Button btnSelectTaskOrCategory;
    }
}