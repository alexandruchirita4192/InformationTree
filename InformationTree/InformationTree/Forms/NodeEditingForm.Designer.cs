namespace FormsGame.Forms
{
    partial class NodeEditingForm
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
            this.gbTimeSpent = new System.Windows.Forms.GroupBox();
            this.nudMilliseconds = new System.Windows.Forms.NumericUpDown();
            this.nudSeconds = new System.Windows.Forms.NumericUpDown();
            this.nudMinutes = new System.Windows.Forms.NumericUpDown();
            this.nudHours = new System.Windows.Forms.NumericUpDown();
            this.lblMilliseconds = new System.Windows.Forms.Label();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.lblMinutes = new System.Windows.Forms.Label();
            this.btnStopCounting = new System.Windows.Forms.Button();
            this.btnStartCounting = new System.Windows.Forms.Button();
            this.lblHours = new System.Windows.Forms.Label();
            this.gbStyleChange = new System.Windows.Forms.GroupBox();
            this.tbTextColor = new System.Windows.Forms.TextBox();
            this.tbBackgroundColor = new System.Windows.Forms.TextBox();
            this.lblTextColor = new System.Windows.Forms.Label();
            this.lblBackgroundColor = new System.Windows.Forms.Label();
            this.clbStyle = new System.Windows.Forms.CheckedListBox();
            this.cbFontFamily = new System.Windows.Forms.ComboBox();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.lblFontFamily = new System.Windows.Forms.Label();
            this.nudFontSize = new System.Windows.Forms.NumericUpDown();
            this.gbTimeSpent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMilliseconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).BeginInit();
            this.gbStyleChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // gbTimeSpent
            // 
            this.gbTimeSpent.Controls.Add(this.nudMilliseconds);
            this.gbTimeSpent.Controls.Add(this.nudSeconds);
            this.gbTimeSpent.Controls.Add(this.nudMinutes);
            this.gbTimeSpent.Controls.Add(this.nudHours);
            this.gbTimeSpent.Controls.Add(this.lblMilliseconds);
            this.gbTimeSpent.Controls.Add(this.lblSeconds);
            this.gbTimeSpent.Controls.Add(this.lblMinutes);
            this.gbTimeSpent.Controls.Add(this.btnStopCounting);
            this.gbTimeSpent.Controls.Add(this.btnStartCounting);
            this.gbTimeSpent.Controls.Add(this.lblHours);
            this.gbTimeSpent.Enabled = false;
            this.gbTimeSpent.Location = new System.Drawing.Point(206, 123);
            this.gbTimeSpent.Name = "gbTimeSpent";
            this.gbTimeSpent.Size = new System.Drawing.Size(200, 183);
            this.gbTimeSpent.TabIndex = 8;
            this.gbTimeSpent.TabStop = false;
            this.gbTimeSpent.Text = "Time spent";
            // 
            // nudMilliseconds
            // 
            this.nudMilliseconds.AllowDrop = true;
            this.nudMilliseconds.Location = new System.Drawing.Point(94, 94);
            this.nudMilliseconds.Name = "nudMilliseconds";
            this.nudMilliseconds.Size = new System.Drawing.Size(100, 20);
            this.nudMilliseconds.TabIndex = 10;
            this.nudMilliseconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudSeconds
            // 
            this.nudSeconds.AllowDrop = true;
            this.nudSeconds.Location = new System.Drawing.Point(94, 68);
            this.nudSeconds.Name = "nudSeconds";
            this.nudSeconds.Size = new System.Drawing.Size(100, 20);
            this.nudSeconds.TabIndex = 9;
            this.nudSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudMinutes
            // 
            this.nudMinutes.AllowDrop = true;
            this.nudMinutes.Location = new System.Drawing.Point(94, 43);
            this.nudMinutes.Name = "nudMinutes";
            this.nudMinutes.Size = new System.Drawing.Size(100, 20);
            this.nudMinutes.TabIndex = 8;
            this.nudMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudHours
            // 
            this.nudHours.AllowDrop = true;
            this.nudHours.Location = new System.Drawing.Point(94, 18);
            this.nudHours.Name = "nudHours";
            this.nudHours.Size = new System.Drawing.Size(100, 20);
            this.nudHours.TabIndex = 7;
            this.nudHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblMilliseconds
            // 
            this.lblMilliseconds.AutoSize = true;
            this.lblMilliseconds.Location = new System.Drawing.Point(10, 96);
            this.lblMilliseconds.Name = "lblMilliseconds";
            this.lblMilliseconds.Size = new System.Drawing.Size(64, 13);
            this.lblMilliseconds.TabIndex = 5;
            this.lblMilliseconds.Text = "Milliseconds";
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(10, 70);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(49, 13);
            this.lblSeconds.TabIndex = 4;
            this.lblSeconds.Text = "Seconds";
            // 
            // lblMinutes
            // 
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Location = new System.Drawing.Point(10, 45);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(44, 13);
            this.lblMinutes.TabIndex = 3;
            this.lblMinutes.Text = "Minutes";
            // 
            // btnStopCounting
            // 
            this.btnStopCounting.Location = new System.Drawing.Point(10, 154);
            this.btnStopCounting.Name = "btnStopCounting";
            this.btnStopCounting.Size = new System.Drawing.Size(184, 23);
            this.btnStopCounting.TabIndex = 2;
            this.btnStopCounting.Text = "Stop counting (update)";
            this.btnStopCounting.UseVisualStyleBackColor = true;
            // 
            // btnStartCounting
            // 
            this.btnStartCounting.Location = new System.Drawing.Point(10, 125);
            this.btnStartCounting.Name = "btnStartCounting";
            this.btnStartCounting.Size = new System.Drawing.Size(184, 23);
            this.btnStartCounting.TabIndex = 1;
            this.btnStartCounting.Text = "Start counting";
            this.btnStartCounting.UseVisualStyleBackColor = true;
            // 
            // lblHours
            // 
            this.lblHours.AutoSize = true;
            this.lblHours.Location = new System.Drawing.Point(10, 20);
            this.lblHours.Name = "lblHours";
            this.lblHours.Size = new System.Drawing.Size(35, 13);
            this.lblHours.TabIndex = 0;
            this.lblHours.Text = "Hours";
            // 
            // gbStyleChange
            // 
            this.gbStyleChange.Controls.Add(this.tbTextColor);
            this.gbStyleChange.Controls.Add(this.tbBackgroundColor);
            this.gbStyleChange.Controls.Add(this.lblTextColor);
            this.gbStyleChange.Controls.Add(this.lblBackgroundColor);
            this.gbStyleChange.Controls.Add(this.clbStyle);
            this.gbStyleChange.Controls.Add(this.cbFontFamily);
            this.gbStyleChange.Controls.Add(this.lblFontSize);
            this.gbStyleChange.Controls.Add(this.lblFontFamily);
            this.gbStyleChange.Controls.Add(this.nudFontSize);
            this.gbStyleChange.Location = new System.Drawing.Point(423, 123);
            this.gbStyleChange.Name = "gbStyleChange";
            this.gbStyleChange.Size = new System.Drawing.Size(200, 205);
            this.gbStyleChange.TabIndex = 13;
            this.gbStyleChange.TabStop = false;
            this.gbStyleChange.Text = "Style change";
            // 
            // tbTextColor
            // 
            this.tbTextColor.Location = new System.Drawing.Point(107, 153);
            this.tbTextColor.Name = "tbTextColor";
            this.tbTextColor.Size = new System.Drawing.Size(87, 20);
            this.tbTextColor.TabIndex = 14;
            // 
            // tbBackgroundColor
            // 
            this.tbBackgroundColor.Location = new System.Drawing.Point(107, 179);
            this.tbBackgroundColor.Name = "tbBackgroundColor";
            this.tbBackgroundColor.Size = new System.Drawing.Size(87, 20);
            this.tbBackgroundColor.TabIndex = 14;
            // 
            // lblTextColor
            // 
            this.lblTextColor.AutoSize = true;
            this.lblTextColor.Location = new System.Drawing.Point(10, 156);
            this.lblTextColor.Name = "lblTextColor";
            this.lblTextColor.Size = new System.Drawing.Size(54, 13);
            this.lblTextColor.TabIndex = 4;
            this.lblTextColor.Text = "Text color";
            // 
            // lblBackgroundColor
            // 
            this.lblBackgroundColor.AutoSize = true;
            this.lblBackgroundColor.Location = new System.Drawing.Point(10, 182);
            this.lblBackgroundColor.Name = "lblBackgroundColor";
            this.lblBackgroundColor.Size = new System.Drawing.Size(91, 13);
            this.lblBackgroundColor.TabIndex = 13;
            this.lblBackgroundColor.Text = "Background color";
            // 
            // clbStyle
            // 
            this.clbStyle.CheckOnClick = true;
            this.clbStyle.FormattingEnabled = true;
            this.clbStyle.Location = new System.Drawing.Point(10, 68);
            this.clbStyle.Name = "clbStyle";
            this.clbStyle.Size = new System.Drawing.Size(184, 79);
            this.clbStyle.TabIndex = 12;
            // 
            // cbFontFamily
            // 
            this.cbFontFamily.FormattingEnabled = true;
            this.cbFontFamily.Location = new System.Drawing.Point(70, 15);
            this.cbFontFamily.Name = "cbFontFamily";
            this.cbFontFamily.Size = new System.Drawing.Size(124, 21);
            this.cbFontFamily.TabIndex = 11;
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(7, 44);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(51, 13);
            this.lblFontSize.TabIndex = 9;
            this.lblFontSize.Text = "Font Size";
            // 
            // lblFontFamily
            // 
            this.lblFontFamily.AutoSize = true;
            this.lblFontFamily.Location = new System.Drawing.Point(7, 19);
            this.lblFontFamily.Name = "lblFontFamily";
            this.lblFontFamily.Size = new System.Drawing.Size(57, 13);
            this.lblFontFamily.TabIndex = 8;
            this.lblFontFamily.Text = "Font family";
            // 
            // nudFontSize
            // 
            this.nudFontSize.AllowDrop = true;
            this.nudFontSize.Location = new System.Drawing.Point(70, 42);
            this.nudFontSize.Name = "nudFontSize";
            this.nudFontSize.Size = new System.Drawing.Size(124, 20);
            this.nudFontSize.TabIndex = 6;
            this.nudFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // NodeEditingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 428);
            this.Controls.Add(this.gbStyleChange);
            this.Controls.Add(this.gbTimeSpent);
            this.Name = "NodeEditingForm";
            this.Text = "NodeEditingForm";
            this.gbTimeSpent.ResumeLayout(false);
            this.gbTimeSpent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMilliseconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).EndInit();
            this.gbStyleChange.ResumeLayout(false);
            this.gbStyleChange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTimeSpent;
        private System.Windows.Forms.NumericUpDown nudMilliseconds;
        private System.Windows.Forms.NumericUpDown nudSeconds;
        private System.Windows.Forms.NumericUpDown nudMinutes;
        private System.Windows.Forms.NumericUpDown nudHours;
        protected internal System.Windows.Forms.Label lblMilliseconds;
        protected internal System.Windows.Forms.Label lblSeconds;
        protected internal System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.Button btnStopCounting;
        private System.Windows.Forms.Button btnStartCounting;
        protected internal System.Windows.Forms.Label lblHours;
        private System.Windows.Forms.GroupBox gbStyleChange;
        private System.Windows.Forms.TextBox tbTextColor;
        private System.Windows.Forms.TextBox tbBackgroundColor;
        private System.Windows.Forms.Label lblTextColor;
        private System.Windows.Forms.Label lblBackgroundColor;
        private System.Windows.Forms.CheckedListBox clbStyle;
        private System.Windows.Forms.ComboBox cbFontFamily;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.Label lblFontFamily;
        private System.Windows.Forms.NumericUpDown nudFontSize;
    }
}