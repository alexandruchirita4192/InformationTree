namespace RicherTextBox.Controls
{
    partial class TableControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.btnSaveTableData = new System.Windows.Forms.Button();
            this.btnAddColumn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(0, 4);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(276, 62);
            this.dataGridView.TabIndex = 0;
            // 
            // btnSaveTableData
            // 
            this.btnSaveTableData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveTableData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveTableData.Location = new System.Drawing.Point(3, 70);
            this.btnSaveTableData.Name = "btnSaveTableData";
            this.btnSaveTableData.Size = new System.Drawing.Size(119, 23);
            this.btnSaveTableData.TabIndex = 1;
            this.btnSaveTableData.Text = "Save table data";
            this.btnSaveTableData.UseVisualStyleBackColor = true;
            this.btnSaveTableData.Click += new System.EventHandler(this.btnSaveTableData_Click);
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddColumn.Location = new System.Drawing.Point(128, 70);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(119, 23);
            this.btnAddColumn.TabIndex = 2;
            this.btnAddColumn.Text = "Add column";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // TableControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddColumn);
            this.Controls.Add(this.btnSaveTableData);
            this.Controls.Add(this.dataGridView);
            this.Name = "TableControl";
            this.Size = new System.Drawing.Size(276, 97);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnSaveTableData;
        private System.Windows.Forms.Button btnAddColumn;
    }
}
