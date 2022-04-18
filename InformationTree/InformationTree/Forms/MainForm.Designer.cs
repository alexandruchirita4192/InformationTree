using System.Windows.Forms;

namespace InformationTree.Forms
{
    partial class MainForm
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

            if (disposing)
                this.MouseWheel -= tvTaskList_MouseClick;

            if (disposing && _timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            if (disposing && _randomTimer != null)
            {
                _randomTimer.Stop();
                _randomTimer.Elapsed -= RandomTimer_Elapsed;
                _randomTimer = null;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnNoTask = new System.Windows.Forms.Button();
            this.tvTaskList = new System.Windows.Forms.TreeView();
            this.gbTaskList = new System.Windows.Forms.GroupBox();
            this.tbSearchBox = new System.Windows.Forms.TextBox();
            this.btnAddTask = new System.Windows.Forms.Button();
            this.gbTask = new System.Windows.Forms.GroupBox();
            this.tbTaskName = new System.Windows.Forms.TextBox();
            this.btnUpdateText = new System.Windows.Forms.Button();
            this.pbPercentComplete = new System.Windows.Forms.ProgressBar();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblCompletePercent = new System.Windows.Forms.Label();
            this.nudCompleteProgress = new System.Windows.Forms.NumericUpDown();
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
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnCollapse = new System.Windows.Forms.Button();
            this.btnCalculatePercentage = new System.Windows.Forms.Button();
            this.btnCalculatePercentage2 = new System.Windows.Forms.Button();
            this.btnToggleCompletedTasks = new System.Windows.Forms.Button();
            this.btnMoveToNextUnfinished = new System.Windows.Forms.Button();
            this.btnMoveTaskUp = new System.Windows.Forms.Button();
            this.btnMoveTaskDown = new System.Windows.Forms.Button();
            this.btnResetException = new System.Windows.Forms.Button();
            this.btnDoNotSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tbTreeChange = new System.Windows.Forms.TabControl();
            this.tbTaskEdit = new System.Windows.Forms.TabPage();
            this.gbCompletedPercent = new System.Windows.Forms.GroupBox();
            this.gbTaskOperations = new System.Windows.Forms.GroupBox();
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
            this.tbTaskDetails = new System.Windows.Forms.TabPage();
            this.btnShowStartupAlertForm = new System.Windows.Forms.Button();
            this.gbTaskDetails = new System.Windows.Forms.GroupBox();
            this.tbLastChangeDate = new System.Windows.Forms.TextBox();
            this.lblLastChangeDate = new System.Windows.Forms.Label();
            this.cbIsStartupAlert = new System.Windows.Forms.CheckBox();
            this.lblIsStartupAlert = new System.Windows.Forms.Label();
            this.tbCategory = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.tbDataSize = new System.Windows.Forms.TextBox();
            this.lblDataSize = new System.Windows.Forms.Label();
            this.tbAddedDate = new System.Windows.Forms.TextBox();
            this.lblUrgency = new System.Windows.Forms.Label();
            this.lblAddedDate = new System.Windows.Forms.Label();
            this.btnUpdateLink = new System.Windows.Forms.Button();
            this.nudUrgency = new System.Windows.Forms.NumericUpDown();
            this.tbAddedNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbLink = new System.Windows.Forms.TextBox();
            this.lblAddedNumber = new System.Windows.Forms.Label();
            this.tbTasksChange = new System.Windows.Forms.TabPage();
            this.lblUnchanged = new System.Windows.Forms.Label();
            this.gbTreeToXML = new System.Windows.Forms.GroupBox();
            this.btnMakeSubTreeWithChildrenOfSelectedNode = new System.Windows.Forms.Button();
            this.btnGoToDefaultTree = new System.Windows.Forms.Button();
            this.cbUseSelectedNode = new System.Windows.Forms.CheckBox();
            this.btnMoveNode = new System.Windows.Forms.Button();
            this.lblUrgencyNumber = new System.Windows.Forms.Label();
            this.lblNumberOfTask = new System.Windows.Forms.Label();
            this.nudShowFromUrgencyNumber = new System.Windows.Forms.NumericUpDown();
            this.btnShowFromToUrgencyNumber = new System.Windows.Forms.Button();
            this.nudShowToUrgencyNumber = new System.Windows.Forms.NumericUpDown();
            this.nudShowFromNumber = new System.Windows.Forms.NumericUpDown();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.btnShowFromToNumberOfTask = new System.Windows.Forms.Button();
            this.nudShowUntilNumber = new System.Windows.Forms.NumericUpDown();
            this.tbGraphics = new System.Windows.Forms.TabPage();
            this.gbGenerate = new System.Windows.Forms.GroupBox();
            this.cbLog = new System.Windows.Forms.CheckBox();
            this.btnGenerateFiguresAndExec = new System.Windows.Forms.Button();
            this.nudComputeType = new System.Windows.Forms.NumericUpDown();
            this.lblComputeType = new System.Windows.Forms.Label();
            this.cbUseDefaults = new System.Windows.Forms.CheckBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.nudIterations = new System.Windows.Forms.NumericUpDown();
            this.nudTheta = new System.Windows.Forms.NumericUpDown();
            this.nudRadius = new System.Windows.Forms.NumericUpDown();
            this.nudPoints = new System.Windows.Forms.NumericUpDown();
            this.nudY = new System.Windows.Forms.NumericUpDown();
            this.nudNumber = new System.Windows.Forms.NumericUpDown();
            this.nudX = new System.Windows.Forms.NumericUpDown();
            this.lblIterations = new System.Windows.Forms.Label();
            this.lblTheta = new System.Windows.Forms.Label();
            this.lblRadius = new System.Windows.Forms.Label();
            this.lblPoints = new System.Windows.Forms.Label();
            this.lblNumber = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.btnDeleteCanvas = new System.Windows.Forms.Button();
            this.btnExecCommand = new System.Windows.Forms.Button();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.btnShowCanvasPopUp = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.lblChangeTreeType = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showChildrenAsListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExportToRtf = new System.Windows.Forms.Button();
            this.gbTaskList.SuspendLayout();
            this.gbTask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCompleteProgress)).BeginInit();
            this.gbTimeSpent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMilliseconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).BeginInit();
            this.tbTreeChange.SuspendLayout();
            this.tbTaskEdit.SuspendLayout();
            this.gbCompletedPercent.SuspendLayout();
            this.gbTaskOperations.SuspendLayout();
            this.gbStyleChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
            this.tbTaskDetails.SuspendLayout();
            this.gbTaskDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUrgency)).BeginInit();
            this.tbTasksChange.SuspendLayout();
            this.gbTreeToXML.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowFromUrgencyNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowToUrgencyNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowFromNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowUntilNumber)).BeginInit();
            this.tbGraphics.SuspendLayout();
            this.gbGenerate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudComputeType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTheta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNoTask
            // 
            this.btnNoTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNoTask.Location = new System.Drawing.Point(4, 7);
            this.btnNoTask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNoTask.Name = "btnNoTask";
            this.btnNoTask.Size = new System.Drawing.Size(70, 61);
            this.btnNoTask.TabIndex = 1;
            this.btnNoTask.Text = "Clear selection for tasks";
            this.btnNoTask.UseVisualStyleBackColor = false;
            this.btnNoTask.Click += new System.EventHandler(this.btnNoTask_Click);
            // 
            // tvTaskList
            // 
            this.tvTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvTaskList.HideSelection = false;
            this.tvTaskList.Location = new System.Drawing.Point(14, 20);
            this.tvTaskList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tvTaskList.Name = "tvTaskList";
            this.tvTaskList.Size = new System.Drawing.Size(1109, 636);
            this.tvTaskList.TabIndex = 3;
            // 
            // gbTaskList
            // 
            this.gbTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTaskList.Controls.Add(this.tbSearchBox);
            this.gbTaskList.Controls.Add(this.tvTaskList);
            this.gbTaskList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.gbTaskList.Location = new System.Drawing.Point(14, 14);
            this.gbTaskList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskList.Name = "gbTaskList";
            this.gbTaskList.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskList.Size = new System.Drawing.Size(1124, 663);
            this.gbTaskList.TabIndex = 4;
            this.gbTaskList.TabStop = false;
            this.gbTaskList.Text = "Tasks Tree";
            // 
            // tbSearchBox
            // 
            this.tbSearchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchBox.Location = new System.Drawing.Point(840, 32);
            this.tbSearchBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbSearchBox.Name = "tbSearchBox";
            this.tbSearchBox.Size = new System.Drawing.Size(250, 20);
            this.tbSearchBox.TabIndex = 4;
            this.tbSearchBox.DoubleClick += new System.EventHandler(this.tbSearchBox_DoubleClick);
            this.tbSearchBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbSearchBox_KeyUp);
            // 
            // btnAddTask
            // 
            this.btnAddTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddTask.Location = new System.Drawing.Point(9, 22);
            this.btnAddTask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(66, 27);
            this.btnAddTask.TabIndex = 5;
            this.btnAddTask.Text = "&Add";
            this.btnAddTask.UseVisualStyleBackColor = true;
            this.btnAddTask.Click += new System.EventHandler(this.btnAddTask_Click);
            // 
            // gbTask
            // 
            this.gbTask.Controls.Add(this.tbTaskName);
            this.gbTask.Location = new System.Drawing.Point(4, 3);
            this.gbTask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTask.Name = "gbTask";
            this.gbTask.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTask.Size = new System.Drawing.Size(233, 194);
            this.gbTask.TabIndex = 6;
            this.gbTask.TabStop = false;
            this.gbTask.Text = "Selected task";
            // 
            // tbTaskName
            // 
            this.tbTaskName.Location = new System.Drawing.Point(7, 22);
            this.tbTaskName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTaskName.Multiline = true;
            this.tbTaskName.Name = "tbTaskName";
            this.tbTaskName.Size = new System.Drawing.Size(219, 164);
            this.tbTaskName.TabIndex = 7;
            this.tbTaskName.DoubleClick += new System.EventHandler(this.tbTaskName_DoubleClick);
            this.tbTaskName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            // 
            // btnUpdateText
            // 
            this.btnUpdateText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateText.Location = new System.Drawing.Point(83, 22);
            this.btnUpdateText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnUpdateText.Name = "btnUpdateText";
            this.btnUpdateText.Size = new System.Drawing.Size(66, 27);
            this.btnUpdateText.TabIndex = 11;
            this.btnUpdateText.Text = "&Update";
            this.btnUpdateText.UseVisualStyleBackColor = true;
            this.btnUpdateText.Click += new System.EventHandler(this.btnUpdateText_Click);
            // 
            // pbPercentComplete
            // 
            this.pbPercentComplete.ForeColor = System.Drawing.Color.Black;
            this.pbPercentComplete.Location = new System.Drawing.Point(10, 50);
            this.pbPercentComplete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pbPercentComplete.Name = "pbPercentComplete";
            this.pbPercentComplete.Size = new System.Drawing.Size(215, 27);
            this.pbPercentComplete.TabIndex = 10;
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(157, 22);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(66, 27);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblCompletePercent
            // 
            this.lblCompletePercent.AutoSize = true;
            this.lblCompletePercent.ForeColor = System.Drawing.Color.Black;
            this.lblCompletePercent.Location = new System.Drawing.Point(18, 24);
            this.lblCompletePercent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCompletePercent.Name = "lblCompletePercent";
            this.lblCompletePercent.Size = new System.Drawing.Size(80, 15);
            this.lblCompletePercent.TabIndex = 9;
            this.lblCompletePercent.Text = "Complete (%)";
            // 
            // nudCompleteProgress
            // 
            this.nudCompleteProgress.AllowDrop = true;
            this.nudCompleteProgress.Location = new System.Drawing.Point(108, 20);
            this.nudCompleteProgress.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudCompleteProgress.Name = "nudCompleteProgress";
            this.nudCompleteProgress.Size = new System.Drawing.Size(117, 23);
            this.nudCompleteProgress.TabIndex = 6;
            this.nudCompleteProgress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudCompleteProgress.ValueChanged += new System.EventHandler(this.nudCompleteProgress_ValueChanged);
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
            this.gbTimeSpent.Location = new System.Drawing.Point(7, 347);
            this.gbTimeSpent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTimeSpent.Name = "gbTimeSpent";
            this.gbTimeSpent.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTimeSpent.Size = new System.Drawing.Size(233, 211);
            this.gbTimeSpent.TabIndex = 7;
            this.gbTimeSpent.TabStop = false;
            this.gbTimeSpent.Text = "Time spent";
            // 
            // nudMilliseconds
            // 
            this.nudMilliseconds.AllowDrop = true;
            this.nudMilliseconds.Location = new System.Drawing.Point(110, 108);
            this.nudMilliseconds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudMilliseconds.Name = "nudMilliseconds";
            this.nudMilliseconds.Size = new System.Drawing.Size(117, 23);
            this.nudMilliseconds.TabIndex = 10;
            this.nudMilliseconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudSeconds
            // 
            this.nudSeconds.AllowDrop = true;
            this.nudSeconds.Location = new System.Drawing.Point(110, 78);
            this.nudSeconds.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudSeconds.Name = "nudSeconds";
            this.nudSeconds.Size = new System.Drawing.Size(117, 23);
            this.nudSeconds.TabIndex = 9;
            this.nudSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudMinutes
            // 
            this.nudMinutes.AllowDrop = true;
            this.nudMinutes.Location = new System.Drawing.Point(110, 50);
            this.nudMinutes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudMinutes.Name = "nudMinutes";
            this.nudMinutes.Size = new System.Drawing.Size(117, 23);
            this.nudMinutes.TabIndex = 8;
            this.nudMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nudHours
            // 
            this.nudHours.AllowDrop = true;
            this.nudHours.Location = new System.Drawing.Point(110, 21);
            this.nudHours.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudHours.Name = "nudHours";
            this.nudHours.Size = new System.Drawing.Size(117, 23);
            this.nudHours.TabIndex = 7;
            this.nudHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblMilliseconds
            // 
            this.lblMilliseconds.AutoSize = true;
            this.lblMilliseconds.Location = new System.Drawing.Point(12, 111);
            this.lblMilliseconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMilliseconds.Name = "lblMilliseconds";
            this.lblMilliseconds.Size = new System.Drawing.Size(73, 15);
            this.lblMilliseconds.TabIndex = 5;
            this.lblMilliseconds.Text = "Milliseconds";
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(12, 81);
            this.lblSeconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(51, 15);
            this.lblSeconds.TabIndex = 4;
            this.lblSeconds.Text = "Seconds";
            // 
            // lblMinutes
            // 
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Location = new System.Drawing.Point(12, 52);
            this.lblMinutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(50, 15);
            this.lblMinutes.TabIndex = 3;
            this.lblMinutes.Text = "Minutes";
            // 
            // btnStopCounting
            // 
            this.btnStopCounting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopCounting.Location = new System.Drawing.Point(12, 178);
            this.btnStopCounting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStopCounting.Name = "btnStopCounting";
            this.btnStopCounting.Size = new System.Drawing.Size(215, 27);
            this.btnStopCounting.TabIndex = 2;
            this.btnStopCounting.Text = "St&op counting (update)";
            this.btnStopCounting.UseVisualStyleBackColor = true;
            this.btnStopCounting.Click += new System.EventHandler(this.btnStopCounting_Click);
            // 
            // btnStartCounting
            // 
            this.btnStartCounting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartCounting.Location = new System.Drawing.Point(12, 144);
            this.btnStartCounting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnStartCounting.Name = "btnStartCounting";
            this.btnStartCounting.Size = new System.Drawing.Size(215, 27);
            this.btnStartCounting.TabIndex = 1;
            this.btnStartCounting.Text = "St&art counting";
            this.btnStartCounting.UseVisualStyleBackColor = true;
            this.btnStartCounting.Click += new System.EventHandler(this.btnStartCounting_Click);
            // 
            // lblHours
            // 
            this.lblHours.AutoSize = true;
            this.lblHours.Location = new System.Drawing.Point(12, 23);
            this.lblHours.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHours.Name = "lblHours";
            this.lblHours.Size = new System.Drawing.Size(39, 15);
            this.lblHours.TabIndex = 0;
            this.lblHours.Text = "Hours";
            // 
            // btnExpand
            // 
            this.btnExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpand.Location = new System.Drawing.Point(127, 207);
            this.btnExpand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(121, 27);
            this.btnExpand.TabIndex = 8;
            this.btnExpand.Text = "Expand";
            this.btnExpand.UseVisualStyleBackColor = false;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnCollapse
            // 
            this.btnCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCollapse.Location = new System.Drawing.Point(127, 173);
            this.btnCollapse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCollapse.Name = "btnCollapse";
            this.btnCollapse.Size = new System.Drawing.Size(121, 27);
            this.btnCollapse.TabIndex = 9;
            this.btnCollapse.Text = "Collapse";
            this.btnCollapse.UseVisualStyleBackColor = false;
            this.btnCollapse.Click += new System.EventHandler(this.btnCollapse_Click);
            // 
            // btnCalculatePercentage
            // 
            this.btnCalculatePercentage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalculatePercentage.Location = new System.Drawing.Point(4, 74);
            this.btnCalculatePercentage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCalculatePercentage.Name = "btnCalculatePercentage";
            this.btnCalculatePercentage.Size = new System.Drawing.Size(121, 85);
            this.btnCalculatePercentage.TabIndex = 10;
            this.btnCalculatePercentage.Text = "Calculate percentage from leafs (to selected node)";
            this.btnCalculatePercentage.UseVisualStyleBackColor = false;
            this.btnCalculatePercentage.Click += new System.EventHandler(this.btnCalculatePercentage_Click);
            // 
            // btnCalculatePercentage2
            // 
            this.btnCalculatePercentage2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalculatePercentage2.Location = new System.Drawing.Point(128, 74);
            this.btnCalculatePercentage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCalculatePercentage2.Name = "btnCalculatePercentage2";
            this.btnCalculatePercentage2.Size = new System.Drawing.Size(121, 85);
            this.btnCalculatePercentage2.TabIndex = 11;
            this.btnCalculatePercentage2.Text = "Calculate percentage from selected node (to leafs)";
            this.btnCalculatePercentage2.UseVisualStyleBackColor = false;
            this.btnCalculatePercentage2.Click += new System.EventHandler(this.btnCalculatePercentage2_Click);
            // 
            // btnToggleCompletedTasks
            // 
            this.btnToggleCompletedTasks.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleCompletedTasks.Location = new System.Drawing.Point(80, 7);
            this.btnToggleCompletedTasks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnToggleCompletedTasks.Name = "btnToggleCompletedTasks";
            this.btnToggleCompletedTasks.Size = new System.Drawing.Size(169, 27);
            this.btnToggleCompletedTasks.TabIndex = 13;
            this.btnToggleCompletedTasks.Text = "Toggle completed tasks";
            this.btnToggleCompletedTasks.UseVisualStyleBackColor = false;
            this.btnToggleCompletedTasks.Click += new System.EventHandler(this.btnToggleCompletedTasks_Click);
            // 
            // btnMoveToNextUnfinished
            // 
            this.btnMoveToNextUnfinished.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveToNextUnfinished.Location = new System.Drawing.Point(80, 40);
            this.btnMoveToNextUnfinished.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMoveToNextUnfinished.Name = "btnMoveToNextUnfinished";
            this.btnMoveToNextUnfinished.Size = new System.Drawing.Size(169, 27);
            this.btnMoveToNextUnfinished.TabIndex = 14;
            this.btnMoveToNextUnfinished.Text = "Move to next unfinished";
            this.btnMoveToNextUnfinished.UseVisualStyleBackColor = false;
            this.btnMoveToNextUnfinished.Click += new System.EventHandler(this.btnMoveToNextUnfinished_Click);
            // 
            // btnMoveTaskUp
            // 
            this.btnMoveTaskUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveTaskUp.Location = new System.Drawing.Point(4, 173);
            this.btnMoveTaskUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMoveTaskUp.Name = "btnMoveTaskUp";
            this.btnMoveTaskUp.Size = new System.Drawing.Size(121, 27);
            this.btnMoveTaskUp.TabIndex = 15;
            this.btnMoveTaskUp.Text = "Move task &up";
            this.btnMoveTaskUp.UseVisualStyleBackColor = false;
            this.btnMoveTaskUp.Click += new System.EventHandler(this.btnMoveTaskUp_Click);
            // 
            // btnMoveTaskDown
            // 
            this.btnMoveTaskDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveTaskDown.Location = new System.Drawing.Point(4, 207);
            this.btnMoveTaskDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMoveTaskDown.Name = "btnMoveTaskDown";
            this.btnMoveTaskDown.Size = new System.Drawing.Size(121, 27);
            this.btnMoveTaskDown.TabIndex = 16;
            this.btnMoveTaskDown.Text = "Move task &down";
            this.btnMoveTaskDown.UseVisualStyleBackColor = false;
            this.btnMoveTaskDown.Click += new System.EventHandler(this.btnMoveTaskDown_Click);
            // 
            // btnResetException
            // 
            this.btnResetException.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetException.Location = new System.Drawing.Point(4, 603);
            this.btnResetException.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnResetException.Name = "btnResetException";
            this.btnResetException.Size = new System.Drawing.Size(121, 27);
            this.btnResetException.TabIndex = 17;
            this.btnResetException.Text = "Reset exception";
            this.btnResetException.UseVisualStyleBackColor = false;
            this.btnResetException.Click += new System.EventHandler(this.btnResetException_Click);
            // 
            // btnDoNotSave
            // 
            this.btnDoNotSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoNotSave.Location = new System.Drawing.Point(128, 603);
            this.btnDoNotSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDoNotSave.Name = "btnDoNotSave";
            this.btnDoNotSave.Size = new System.Drawing.Size(121, 27);
            this.btnDoNotSave.TabIndex = 21;
            this.btnDoNotSave.Text = "Do not save";
            this.btnDoNotSave.UseVisualStyleBackColor = false;
            this.btnDoNotSave.Click += new System.EventHandler(this.btnDoNotSave_Click);
            // 
            // tbTreeChange
            // 
            this.tbTreeChange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTreeChange.Controls.Add(this.tbTaskEdit);
            this.tbTreeChange.Controls.Add(this.tbTaskDetails);
            this.tbTreeChange.Controls.Add(this.tbTasksChange);
            this.tbTreeChange.Controls.Add(this.tbGraphics);
            this.tbTreeChange.Cursor = System.Windows.Forms.Cursors.Cross;
            this.tbTreeChange.Location = new System.Drawing.Point(1144, 14);
            this.tbTreeChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTreeChange.Name = "tbTreeChange";
            this.tbTreeChange.SelectedIndex = 0;
            this.tbTreeChange.Size = new System.Drawing.Size(262, 663);
            this.tbTreeChange.TabIndex = 5;
            this.tbTreeChange.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tbTreeChange_MouseDoubleClick);
            // 
            // tbTaskEdit
            // 
            this.tbTaskEdit.AutoScroll = true;
            this.tbTaskEdit.Controls.Add(this.gbCompletedPercent);
            this.tbTaskEdit.Controls.Add(this.gbTaskOperations);
            this.tbTaskEdit.Controls.Add(this.gbStyleChange);
            this.tbTaskEdit.Controls.Add(this.gbTask);
            this.tbTaskEdit.Location = new System.Drawing.Point(4, 24);
            this.tbTaskEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTaskEdit.Name = "tbTaskEdit";
            this.tbTaskEdit.Size = new System.Drawing.Size(254, 635);
            this.tbTaskEdit.TabIndex = 2;
            this.tbTaskEdit.Text = "Edit";
            this.tbTaskEdit.DoubleClick += new System.EventHandler(this.tbTask_DoubleClick);
            // 
            // gbCompletedPercent
            // 
            this.gbCompletedPercent.Controls.Add(this.lblCompletePercent);
            this.gbCompletedPercent.Controls.Add(this.nudCompleteProgress);
            this.gbCompletedPercent.Controls.Add(this.pbPercentComplete);
            this.gbCompletedPercent.Location = new System.Drawing.Point(4, 298);
            this.gbCompletedPercent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbCompletedPercent.Name = "gbCompletedPercent";
            this.gbCompletedPercent.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbCompletedPercent.Size = new System.Drawing.Size(233, 85);
            this.gbCompletedPercent.TabIndex = 15;
            this.gbCompletedPercent.TabStop = false;
            this.gbCompletedPercent.Text = "Completion percent";
            // 
            // gbTaskOperations
            // 
            this.gbTaskOperations.BackColor = System.Drawing.SystemColors.Control;
            this.gbTaskOperations.Controls.Add(this.btnExportToRtf);
            this.gbTaskOperations.Controls.Add(this.btnAddTask);
            this.gbTaskOperations.Controls.Add(this.btnUpdateText);
            this.gbTaskOperations.Controls.Add(this.btnDelete);
            this.gbTaskOperations.ForeColor = System.Drawing.Color.Black;
            this.gbTaskOperations.Location = new System.Drawing.Point(4, 204);
            this.gbTaskOperations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskOperations.Name = "gbTaskOperations";
            this.gbTaskOperations.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskOperations.Size = new System.Drawing.Size(233, 87);
            this.gbTaskOperations.TabIndex = 14;
            this.gbTaskOperations.TabStop = false;
            this.gbTaskOperations.Text = "Task operations";
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
            this.gbStyleChange.Location = new System.Drawing.Point(4, 390);
            this.gbStyleChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbStyleChange.Name = "gbStyleChange";
            this.gbStyleChange.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbStyleChange.Size = new System.Drawing.Size(233, 240);
            this.gbStyleChange.TabIndex = 13;
            this.gbStyleChange.TabStop = false;
            this.gbStyleChange.Text = "Style change";
            // 
            // tbTextColor
            // 
            this.tbTextColor.Location = new System.Drawing.Point(125, 177);
            this.tbTextColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTextColor.Name = "tbTextColor";
            this.tbTextColor.Size = new System.Drawing.Size(101, 23);
            this.tbTextColor.TabIndex = 14;
            // 
            // tbBackgroundColor
            // 
            this.tbBackgroundColor.Location = new System.Drawing.Point(125, 207);
            this.tbBackgroundColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbBackgroundColor.Name = "tbBackgroundColor";
            this.tbBackgroundColor.Size = new System.Drawing.Size(101, 23);
            this.tbBackgroundColor.TabIndex = 14;
            // 
            // lblTextColor
            // 
            this.lblTextColor.AutoSize = true;
            this.lblTextColor.ForeColor = System.Drawing.Color.Black;
            this.lblTextColor.Location = new System.Drawing.Point(12, 180);
            this.lblTextColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTextColor.Name = "lblTextColor";
            this.lblTextColor.Size = new System.Drawing.Size(58, 15);
            this.lblTextColor.TabIndex = 4;
            this.lblTextColor.Text = "Text color";
            // 
            // lblBackgroundColor
            // 
            this.lblBackgroundColor.AutoSize = true;
            this.lblBackgroundColor.ForeColor = System.Drawing.Color.Black;
            this.lblBackgroundColor.Location = new System.Drawing.Point(12, 210);
            this.lblBackgroundColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBackgroundColor.Name = "lblBackgroundColor";
            this.lblBackgroundColor.Size = new System.Drawing.Size(101, 15);
            this.lblBackgroundColor.TabIndex = 13;
            this.lblBackgroundColor.Text = "Background color";
            // 
            // clbStyle
            // 
            this.clbStyle.CheckOnClick = true;
            this.clbStyle.FormattingEnabled = true;
            this.clbStyle.Location = new System.Drawing.Point(12, 78);
            this.clbStyle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clbStyle.Name = "clbStyle";
            this.clbStyle.Size = new System.Drawing.Size(214, 94);
            this.clbStyle.TabIndex = 12;
            // 
            // cbFontFamily
            // 
            this.cbFontFamily.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFontFamily.FormattingEnabled = true;
            this.cbFontFamily.Location = new System.Drawing.Point(82, 17);
            this.cbFontFamily.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbFontFamily.Name = "cbFontFamily";
            this.cbFontFamily.Size = new System.Drawing.Size(144, 23);
            this.cbFontFamily.TabIndex = 11;
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.ForeColor = System.Drawing.Color.Black;
            this.lblFontSize.Location = new System.Drawing.Point(8, 51);
            this.lblFontSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(53, 15);
            this.lblFontSize.TabIndex = 9;
            this.lblFontSize.Text = "Font size";
            // 
            // lblFontFamily
            // 
            this.lblFontFamily.AutoSize = true;
            this.lblFontFamily.ForeColor = System.Drawing.Color.Black;
            this.lblFontFamily.Location = new System.Drawing.Point(8, 22);
            this.lblFontFamily.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFontFamily.Name = "lblFontFamily";
            this.lblFontFamily.Size = new System.Drawing.Size(67, 15);
            this.lblFontFamily.TabIndex = 8;
            this.lblFontFamily.Text = "Font family";
            // 
            // nudFontSize
            // 
            this.nudFontSize.AllowDrop = true;
            this.nudFontSize.Location = new System.Drawing.Point(82, 48);
            this.nudFontSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudFontSize.Name = "nudFontSize";
            this.nudFontSize.Size = new System.Drawing.Size(145, 23);
            this.nudFontSize.TabIndex = 6;
            this.nudFontSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbTaskDetails
            // 
            this.tbTaskDetails.Controls.Add(this.btnShowStartupAlertForm);
            this.tbTaskDetails.Controls.Add(this.gbTaskDetails);
            this.tbTaskDetails.Controls.Add(this.gbTimeSpent);
            this.tbTaskDetails.Location = new System.Drawing.Point(4, 24);
            this.tbTaskDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTaskDetails.Name = "tbTaskDetails";
            this.tbTaskDetails.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTaskDetails.Size = new System.Drawing.Size(254, 635);
            this.tbTaskDetails.TabIndex = 1;
            this.tbTaskDetails.Text = "Details";
            // 
            // btnShowStartupAlertForm
            // 
            this.btnShowStartupAlertForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowStartupAlertForm.Location = new System.Drawing.Point(15, 599);
            this.btnShowStartupAlertForm.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowStartupAlertForm.Name = "btnShowStartupAlertForm";
            this.btnShowStartupAlertForm.Size = new System.Drawing.Size(215, 27);
            this.btnShowStartupAlertForm.TabIndex = 15;
            this.btnShowStartupAlertForm.Text = "&Show startup alert form";
            this.btnShowStartupAlertForm.UseVisualStyleBackColor = true;
            this.btnShowStartupAlertForm.Click += new System.EventHandler(this.btnShowStartupAlertForm_Click);
            // 
            // gbTaskDetails
            // 
            this.gbTaskDetails.Controls.Add(this.tbLastChangeDate);
            this.gbTaskDetails.Controls.Add(this.lblLastChangeDate);
            this.gbTaskDetails.Controls.Add(this.cbIsStartupAlert);
            this.gbTaskDetails.Controls.Add(this.lblIsStartupAlert);
            this.gbTaskDetails.Controls.Add(this.tbCategory);
            this.gbTaskDetails.Controls.Add(this.lblCategory);
            this.gbTaskDetails.Controls.Add(this.tbDataSize);
            this.gbTaskDetails.Controls.Add(this.lblDataSize);
            this.gbTaskDetails.Controls.Add(this.tbAddedDate);
            this.gbTaskDetails.Controls.Add(this.lblUrgency);
            this.gbTaskDetails.Controls.Add(this.lblAddedDate);
            this.gbTaskDetails.Controls.Add(this.btnUpdateLink);
            this.gbTaskDetails.Controls.Add(this.nudUrgency);
            this.gbTaskDetails.Controls.Add(this.tbAddedNumber);
            this.gbTaskDetails.Controls.Add(this.label1);
            this.gbTaskDetails.Controls.Add(this.tbLink);
            this.gbTaskDetails.Controls.Add(this.lblAddedNumber);
            this.gbTaskDetails.Location = new System.Drawing.Point(5, 7);
            this.gbTaskDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskDetails.Name = "gbTaskDetails";
            this.gbTaskDetails.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTaskDetails.Size = new System.Drawing.Size(233, 333);
            this.gbTaskDetails.TabIndex = 8;
            this.gbTaskDetails.TabStop = false;
            this.gbTaskDetails.Text = "Task details";
            // 
            // tbLastChangeDate
            // 
            this.tbLastChangeDate.Location = new System.Drawing.Point(96, 111);
            this.tbLastChangeDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbLastChangeDate.Name = "tbLastChangeDate";
            this.tbLastChangeDate.ReadOnly = true;
            this.tbLastChangeDate.Size = new System.Drawing.Size(129, 23);
            this.tbLastChangeDate.TabIndex = 31;
            // 
            // lblLastChangeDate
            // 
            this.lblLastChangeDate.AutoSize = true;
            this.lblLastChangeDate.Location = new System.Drawing.Point(15, 117);
            this.lblLastChangeDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastChangeDate.Name = "lblLastChangeDate";
            this.lblLastChangeDate.Size = new System.Drawing.Size(70, 15);
            this.lblLastChangeDate.TabIndex = 30;
            this.lblLastChangeDate.Text = "Last change";
            // 
            // cbIsStartupAlert
            // 
            this.cbIsStartupAlert.AutoSize = true;
            this.cbIsStartupAlert.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbIsStartupAlert.Location = new System.Drawing.Point(152, 205);
            this.cbIsStartupAlert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbIsStartupAlert.Name = "cbIsStartupAlert";
            this.cbIsStartupAlert.Size = new System.Drawing.Size(15, 14);
            this.cbIsStartupAlert.TabIndex = 29;
            this.cbIsStartupAlert.UseVisualStyleBackColor = false;
            // 
            // lblIsStartupAlert
            // 
            this.lblIsStartupAlert.AutoSize = true;
            this.lblIsStartupAlert.Location = new System.Drawing.Point(16, 205);
            this.lblIsStartupAlert.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIsStartupAlert.Name = "lblIsStartupAlert";
            this.lblIsStartupAlert.Size = new System.Drawing.Size(81, 15);
            this.lblIsStartupAlert.TabIndex = 28;
            this.lblIsStartupAlert.Text = "Is startup alert";
            // 
            // tbCategory
            // 
            this.tbCategory.Location = new System.Drawing.Point(96, 173);
            this.tbCategory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbCategory.Name = "tbCategory";
            this.tbCategory.Size = new System.Drawing.Size(129, 23);
            this.tbCategory.TabIndex = 27;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(15, 177);
            this.lblCategory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(55, 15);
            this.lblCategory.TabIndex = 26;
            this.lblCategory.Text = "Category";
            // 
            // tbDataSize
            // 
            this.tbDataSize.Location = new System.Drawing.Point(96, 228);
            this.tbDataSize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbDataSize.Name = "tbDataSize";
            this.tbDataSize.ReadOnly = true;
            this.tbDataSize.Size = new System.Drawing.Size(129, 23);
            this.tbDataSize.TabIndex = 25;
            // 
            // lblDataSize
            // 
            this.lblDataSize.AutoSize = true;
            this.lblDataSize.Location = new System.Drawing.Point(16, 232);
            this.lblDataSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDataSize.Name = "lblDataSize";
            this.lblDataSize.Size = new System.Drawing.Size(75, 15);
            this.lblDataSize.TabIndex = 24;
            this.lblDataSize.Text = "Data size[+c]";
            // 
            // tbAddedDate
            // 
            this.tbAddedDate.Location = new System.Drawing.Point(96, 81);
            this.tbAddedDate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbAddedDate.Name = "tbAddedDate";
            this.tbAddedDate.ReadOnly = true;
            this.tbAddedDate.Size = new System.Drawing.Size(129, 23);
            this.tbAddedDate.TabIndex = 22;
            // 
            // lblUrgency
            // 
            this.lblUrgency.AutoSize = true;
            this.lblUrgency.Location = new System.Drawing.Point(18, 145);
            this.lblUrgency.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUrgency.Name = "lblUrgency";
            this.lblUrgency.Size = new System.Drawing.Size(68, 15);
            this.lblUrgency.TabIndex = 18;
            this.lblUrgency.Text = "Urgency nr.";
            // 
            // lblAddedDate
            // 
            this.lblAddedDate.AutoSize = true;
            this.lblAddedDate.Location = new System.Drawing.Point(16, 84);
            this.lblAddedDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddedDate.Name = "lblAddedDate";
            this.lblAddedDate.Size = new System.Drawing.Size(68, 15);
            this.lblAddedDate.TabIndex = 20;
            this.lblAddedDate.Text = "Added date";
            // 
            // btnUpdateLink
            // 
            this.btnUpdateLink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateLink.Location = new System.Drawing.Point(9, 300);
            this.btnUpdateLink.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnUpdateLink.Name = "btnUpdateLink";
            this.btnUpdateLink.Size = new System.Drawing.Size(215, 27);
            this.btnUpdateLink.TabIndex = 9;
            this.btnUpdateLink.Text = "&Update link && data";
            this.btnUpdateLink.UseVisualStyleBackColor = false;
            this.btnUpdateLink.Click += new System.EventHandler(this.btnUpdateText_Click);
            // 
            // nudUrgency
            // 
            this.nudUrgency.AllowDrop = true;
            this.nudUrgency.BackColor = System.Drawing.SystemColors.Control;
            this.nudUrgency.ForeColor = System.Drawing.Color.Black;
            this.nudUrgency.Location = new System.Drawing.Point(96, 141);
            this.nudUrgency.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudUrgency.Name = "nudUrgency";
            this.nudUrgency.Size = new System.Drawing.Size(130, 23);
            this.nudUrgency.TabIndex = 17;
            this.nudUrgency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbAddedNumber
            // 
            this.tbAddedNumber.Location = new System.Drawing.Point(96, 51);
            this.tbAddedNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbAddedNumber.Name = "tbAddedNumber";
            this.tbAddedNumber.ReadOnly = true;
            this.tbAddedNumber.Size = new System.Drawing.Size(129, 23);
            this.tbAddedNumber.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Link";
            // 
            // tbLink
            // 
            this.tbLink.Location = new System.Drawing.Point(55, 22);
            this.tbLink.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbLink.Name = "tbLink";
            this.tbLink.Size = new System.Drawing.Size(170, 23);
            this.tbLink.TabIndex = 0;
            // 
            // lblAddedNumber
            // 
            this.lblAddedNumber.AutoSize = true;
            this.lblAddedNumber.Location = new System.Drawing.Point(16, 54);
            this.lblAddedNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddedNumber.Name = "lblAddedNumber";
            this.lblAddedNumber.Size = new System.Drawing.Size(59, 15);
            this.lblAddedNumber.TabIndex = 19;
            this.lblAddedNumber.Text = "Added nr.";
            // 
            // tbTasksChange
            // 
            this.tbTasksChange.Controls.Add(this.lblUnchanged);
            this.tbTasksChange.Controls.Add(this.gbTreeToXML);
            this.tbTasksChange.Controls.Add(this.btnMoveNode);
            this.tbTasksChange.Controls.Add(this.lblUrgencyNumber);
            this.tbTasksChange.Controls.Add(this.lblNumberOfTask);
            this.tbTasksChange.Controls.Add(this.nudShowFromUrgencyNumber);
            this.tbTasksChange.Controls.Add(this.btnShowFromToUrgencyNumber);
            this.tbTasksChange.Controls.Add(this.nudShowToUrgencyNumber);
            this.tbTasksChange.Controls.Add(this.nudShowFromNumber);
            this.tbTasksChange.Controls.Add(this.btnNoTask);
            this.tbTasksChange.Controls.Add(this.btnCollapse);
            this.tbTasksChange.Controls.Add(this.btnExpand);
            this.tbTasksChange.Controls.Add(this.btnCalculatePercentage2);
            this.tbTasksChange.Controls.Add(this.btnMoveToNextUnfinished);
            this.tbTasksChange.Controls.Add(this.btnCalculatePercentage);
            this.tbTasksChange.Controls.Add(this.btnDoNotSave);
            this.tbTasksChange.Controls.Add(this.btnToggleCompletedTasks);
            this.tbTasksChange.Controls.Add(this.btnMoveTaskUp);
            this.tbTasksChange.Controls.Add(this.btnResetException);
            this.tbTasksChange.Controls.Add(this.btnShowAll);
            this.tbTasksChange.Controls.Add(this.btnMoveTaskDown);
            this.tbTasksChange.Controls.Add(this.btnShowFromToNumberOfTask);
            this.tbTasksChange.Controls.Add(this.nudShowUntilNumber);
            this.tbTasksChange.Location = new System.Drawing.Point(4, 24);
            this.tbTasksChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTasksChange.Name = "tbTasksChange";
            this.tbTasksChange.Size = new System.Drawing.Size(254, 635);
            this.tbTasksChange.TabIndex = 3;
            this.tbTasksChange.Text = "Change";
            // 
            // lblUnchanged
            // 
            this.lblUnchanged.AutoSize = true;
            this.lblUnchanged.Location = new System.Drawing.Point(9, 584);
            this.lblUnchanged.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUnchanged.Name = "lblUnchanged";
            this.lblUnchanged.Size = new System.Drawing.Size(163, 15);
            this.lblUnchanged.TabIndex = 33;
            this.lblUnchanged.Text = "Tree unchanged (do not save)";
            this.lblUnchanged.Click += new System.EventHandler(this.lblUnchanged_Click);
            // 
            // gbTreeToXML
            // 
            this.gbTreeToXML.Controls.Add(this.btnMakeSubTreeWithChildrenOfSelectedNode);
            this.gbTreeToXML.Controls.Add(this.btnGoToDefaultTree);
            this.gbTreeToXML.Controls.Add(this.cbUseSelectedNode);
            this.gbTreeToXML.Location = new System.Drawing.Point(5, 240);
            this.gbTreeToXML.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTreeToXML.Name = "gbTreeToXML";
            this.gbTreeToXML.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gbTreeToXML.Size = new System.Drawing.Size(233, 119);
            this.gbTreeToXML.TabIndex = 29;
            this.gbTreeToXML.TabStop = false;
            this.gbTreeToXML.Text = "Tree to XML";
            // 
            // btnMakeSubTreeWithChildrenOfSelectedNode
            // 
            this.btnMakeSubTreeWithChildrenOfSelectedNode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMakeSubTreeWithChildrenOfSelectedNode.ForeColor = System.Drawing.Color.Black;
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Location = new System.Drawing.Point(7, 48);
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Name = "btnMakeSubTreeWithChildrenOfSelectedNode";
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Size = new System.Drawing.Size(215, 27);
            this.btnMakeSubTreeWithChildrenOfSelectedNode.TabIndex = 10;
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Text = "Make sub-tree with children";
            this.btnMakeSubTreeWithChildrenOfSelectedNode.UseVisualStyleBackColor = true;
            this.btnMakeSubTreeWithChildrenOfSelectedNode.Click += new System.EventHandler(this.btnMakeSubTreeWithChildrenOfSelectedNode_Click);
            // 
            // btnGoToDefaultTree
            // 
            this.btnGoToDefaultTree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoToDefaultTree.Location = new System.Drawing.Point(7, 82);
            this.btnGoToDefaultTree.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGoToDefaultTree.Name = "btnGoToDefaultTree";
            this.btnGoToDefaultTree.Size = new System.Drawing.Size(215, 27);
            this.btnGoToDefaultTree.TabIndex = 12;
            this.btnGoToDefaultTree.Text = "Go to default &tree";
            this.btnGoToDefaultTree.UseVisualStyleBackColor = true;
            this.btnGoToDefaultTree.Click += new System.EventHandler(this.btnGoToDefaultTree_Click);
            // 
            // cbUseSelectedNode
            // 
            this.cbUseSelectedNode.AutoSize = true;
            this.cbUseSelectedNode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbUseSelectedNode.Location = new System.Drawing.Point(15, 22);
            this.cbUseSelectedNode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbUseSelectedNode.Name = "cbUseSelectedNode";
            this.cbUseSelectedNode.Size = new System.Drawing.Size(121, 19);
            this.cbUseSelectedNode.TabIndex = 11;
            this.cbUseSelectedNode.Text = "Use selected node";
            this.cbUseSelectedNode.UseVisualStyleBackColor = false;
            // 
            // btnMoveNode
            // 
            this.btnMoveNode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveNode.Location = new System.Drawing.Point(68, 367);
            this.btnMoveNode.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMoveNode.Name = "btnMoveNode";
            this.btnMoveNode.Size = new System.Drawing.Size(121, 27);
            this.btnMoveNode.TabIndex = 28;
            this.btnMoveNode.Text = "&Move node";
            this.btnMoveNode.UseVisualStyleBackColor = false;
            this.btnMoveNode.Click += new System.EventHandler(this.btnMoveNode_Click);
            // 
            // lblUrgencyNumber
            // 
            this.lblUrgencyNumber.AutoSize = true;
            this.lblUrgencyNumber.Location = new System.Drawing.Point(5, 397);
            this.lblUrgencyNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUrgencyNumber.Name = "lblUrgencyNumber";
            this.lblUrgencyNumber.Size = new System.Drawing.Size(99, 15);
            this.lblUrgencyNumber.TabIndex = 27;
            this.lblUrgencyNumber.Text = "Urgency number:";
            // 
            // lblNumberOfTask
            // 
            this.lblNumberOfTask.AutoSize = true;
            this.lblNumberOfTask.Location = new System.Drawing.Point(5, 474);
            this.lblNumberOfTask.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNumberOfTask.Name = "lblNumberOfTask";
            this.lblNumberOfTask.Size = new System.Drawing.Size(92, 15);
            this.lblNumberOfTask.TabIndex = 26;
            this.lblNumberOfTask.Text = "Number of task:";
            // 
            // nudShowFromUrgencyNumber
            // 
            this.nudShowFromUrgencyNumber.Location = new System.Drawing.Point(5, 415);
            this.nudShowFromUrgencyNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudShowFromUrgencyNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudShowFromUrgencyNumber.Name = "nudShowFromUrgencyNumber";
            this.nudShowFromUrgencyNumber.Size = new System.Drawing.Size(119, 23);
            this.nudShowFromUrgencyNumber.TabIndex = 25;
            this.nudShowFromUrgencyNumber.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // btnShowFromToUrgencyNumber
            // 
            this.btnShowFromToUrgencyNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowFromToUrgencyNumber.Location = new System.Drawing.Point(4, 443);
            this.btnShowFromToUrgencyNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowFromToUrgencyNumber.Name = "btnShowFromToUrgencyNumber";
            this.btnShowFromToUrgencyNumber.Size = new System.Drawing.Size(246, 29);
            this.btnShowFromToUrgencyNumber.TabIndex = 24;
            this.btnShowFromToUrgencyNumber.Text = "Show from X to Y";
            this.btnShowFromToUrgencyNumber.UseVisualStyleBackColor = false;
            this.btnShowFromToUrgencyNumber.Click += new System.EventHandler(this.btnShowFromToUrgencyNumber_Click);
            // 
            // nudShowToUrgencyNumber
            // 
            this.nudShowToUrgencyNumber.Location = new System.Drawing.Point(130, 415);
            this.nudShowToUrgencyNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudShowToUrgencyNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudShowToUrgencyNumber.Name = "nudShowToUrgencyNumber";
            this.nudShowToUrgencyNumber.Size = new System.Drawing.Size(119, 23);
            this.nudShowToUrgencyNumber.TabIndex = 23;
            this.nudShowToUrgencyNumber.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // nudShowFromNumber
            // 
            this.nudShowFromNumber.Location = new System.Drawing.Point(5, 493);
            this.nudShowFromNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudShowFromNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudShowFromNumber.Name = "nudShowFromNumber";
            this.nudShowFromNumber.Size = new System.Drawing.Size(119, 23);
            this.nudShowFromNumber.TabIndex = 22;
            this.nudShowFromNumber.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // btnShowAll
            // 
            this.btnShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAll.Location = new System.Drawing.Point(4, 552);
            this.btnShowAll.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(246, 27);
            this.btnShowAll.TabIndex = 20;
            this.btnShowAll.Text = "Show &all";
            this.btnShowAll.UseVisualStyleBackColor = false;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // btnShowFromToNumberOfTask
            // 
            this.btnShowFromToNumberOfTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowFromToNumberOfTask.Location = new System.Drawing.Point(4, 520);
            this.btnShowFromToNumberOfTask.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowFromToNumberOfTask.Name = "btnShowFromToNumberOfTask";
            this.btnShowFromToNumberOfTask.Size = new System.Drawing.Size(246, 27);
            this.btnShowFromToNumberOfTask.TabIndex = 19;
            this.btnShowFromToNumberOfTask.Text = "Show &from X to Y";
            this.btnShowFromToNumberOfTask.UseVisualStyleBackColor = false;
            this.btnShowFromToNumberOfTask.Click += new System.EventHandler(this.btnShowFromToNumberOfTask_Click);
            // 
            // nudShowUntilNumber
            // 
            this.nudShowUntilNumber.Location = new System.Drawing.Point(130, 493);
            this.nudShowUntilNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.nudShowUntilNumber.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudShowUntilNumber.Name = "nudShowUntilNumber";
            this.nudShowUntilNumber.Size = new System.Drawing.Size(119, 23);
            this.nudShowUntilNumber.TabIndex = 18;
            this.nudShowUntilNumber.Value = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            // 
            // tbGraphics
            // 
            this.tbGraphics.Controls.Add(this.gbGenerate);
            this.tbGraphics.Controls.Add(this.btnDeleteCanvas);
            this.tbGraphics.Controls.Add(this.btnExecCommand);
            this.tbGraphics.Controls.Add(this.tbCommand);
            this.tbGraphics.Controls.Add(this.btnShowCanvasPopUp);
            this.tbGraphics.Location = new System.Drawing.Point(4, 24);
            this.tbGraphics.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbGraphics.Name = "tbGraphics";
            this.tbGraphics.Size = new System.Drawing.Size(254, 635);
            this.tbGraphics.TabIndex = 4;
            this.tbGraphics.Text = "Graphics";
            // 
            // gbGenerate
            // 
            this.gbGenerate.Controls.Add(this.cbLog);
            this.gbGenerate.Controls.Add(this.btnGenerateFiguresAndExec);
            this.gbGenerate.Controls.Add(this.nudComputeType);
            this.gbGenerate.Controls.Add(this.lblComputeType);
            this.gbGenerate.Controls.Add(this.cbUseDefaults);
            this.gbGenerate.Controls.Add(this.btnGenerate);
            this.gbGenerate.Controls.Add(this.nudIterations);
            this.gbGenerate.Controls.Add(this.nudTheta);
            this.gbGenerate.Controls.Add(this.nudRadius);
            this.gbGenerate.Controls.Add(this.nudPoints);
            this.gbGenerate.Controls.Add(this.nudY);
            this.gbGenerate.Controls.Add(this.nudNumber);
            this.gbGenerate.Controls.Add(this.nudX);
            this.gbGenerate.Controls.Add(this.lblIterations);
            this.gbGenerate.Controls.Add(this.lblTheta);
            this.gbGenerate.Controls.Add(this.lblRadius);
            this.gbGenerate.Controls.Add(this.lblPoints);
            this.gbGenerate.Controls.Add(this.lblNumber);
            this.gbGenerate.Controls.Add(this.lblY);
            this.gbGenerate.Controls.Add(this.lblX);
            this.gbGenerate.Location = new System.Drawing.Point(15, 293);
            this.gbGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.gbGenerate.Name = "gbGenerate";
            this.gbGenerate.Padding = new System.Windows.Forms.Padding(2);
            this.gbGenerate.Size = new System.Drawing.Size(237, 338);
            this.gbGenerate.TabIndex = 19;
            this.gbGenerate.TabStop = false;
            this.gbGenerate.Text = "Generate figures";
            // 
            // cbLog
            // 
            this.cbLog.AutoSize = true;
            this.cbLog.Location = new System.Drawing.Point(159, 130);
            this.cbLog.Margin = new System.Windows.Forms.Padding(2);
            this.cbLog.Name = "cbLog";
            this.cbLog.Size = new System.Drawing.Size(46, 19);
            this.cbLog.TabIndex = 24;
            this.cbLog.Text = "Log";
            this.cbLog.UseVisualStyleBackColor = true;
            this.cbLog.CheckedChanged += new System.EventHandler(this.cbLog_CheckedChanged);
            // 
            // btnGenerateFiguresAndExec
            // 
            this.btnGenerateFiguresAndExec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateFiguresAndExec.ForeColor = System.Drawing.Color.Black;
            this.btnGenerateFiguresAndExec.Location = new System.Drawing.Point(6, 306);
            this.btnGenerateFiguresAndExec.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGenerateFiguresAndExec.Name = "btnGenerateFiguresAndExec";
            this.btnGenerateFiguresAndExec.Size = new System.Drawing.Size(225, 27);
            this.btnGenerateFiguresAndExec.TabIndex = 23;
            this.btnGenerateFiguresAndExec.Text = "Generate &figures && exec";
            this.btnGenerateFiguresAndExec.UseVisualStyleBackColor = true;
            this.btnGenerateFiguresAndExec.Click += new System.EventHandler(this.btnGenerateFiguresAndExec_Click);
            // 
            // nudComputeType
            // 
            this.nudComputeType.Location = new System.Drawing.Point(89, 245);
            this.nudComputeType.Margin = new System.Windows.Forms.Padding(2);
            this.nudComputeType.Name = "nudComputeType";
            this.nudComputeType.Size = new System.Drawing.Size(105, 23);
            this.nudComputeType.TabIndex = 22;
            // 
            // lblComputeType
            // 
            this.lblComputeType.AutoSize = true;
            this.lblComputeType.Location = new System.Drawing.Point(15, 247);
            this.lblComputeType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblComputeType.Name = "lblComputeType";
            this.lblComputeType.Size = new System.Drawing.Size(60, 15);
            this.lblComputeType.TabIndex = 21;
            this.lblComputeType.Text = "Calc. Type";
            // 
            // cbUseDefaults
            // 
            this.cbUseDefaults.AutoSize = true;
            this.cbUseDefaults.Location = new System.Drawing.Point(58, 130);
            this.cbUseDefaults.Margin = new System.Windows.Forms.Padding(2);
            this.cbUseDefaults.Name = "cbUseDefaults";
            this.cbUseDefaults.Size = new System.Drawing.Size(90, 19);
            this.cbUseDefaults.TabIndex = 20;
            this.cbUseDefaults.Text = "Use defaults";
            this.cbUseDefaults.UseVisualStyleBackColor = true;
            this.cbUseDefaults.CheckedChanged += new System.EventHandler(this.cbUseDefaults_CheckedChanged);
            // 
            // btnGenerate
            // 
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.ForeColor = System.Drawing.Color.Black;
            this.btnGenerate.Location = new System.Drawing.Point(6, 277);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(225, 27);
            this.btnGenerate.TabIndex = 19;
            this.btnGenerate.Text = "&Generate figures";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // nudIterations
            // 
            this.nudIterations.Location = new System.Drawing.Point(89, 212);
            this.nudIterations.Margin = new System.Windows.Forms.Padding(2);
            this.nudIterations.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.nudIterations.Name = "nudIterations";
            this.nudIterations.Size = new System.Drawing.Size(105, 23);
            this.nudIterations.TabIndex = 12;
            // 
            // nudTheta
            // 
            this.nudTheta.DecimalPlaces = 4;
            this.nudTheta.Location = new System.Drawing.Point(89, 186);
            this.nudTheta.Margin = new System.Windows.Forms.Padding(2);
            this.nudTheta.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTheta.Name = "nudTheta";
            this.nudTheta.Size = new System.Drawing.Size(105, 23);
            this.nudTheta.TabIndex = 11;
            // 
            // nudRadius
            // 
            this.nudRadius.DecimalPlaces = 4;
            this.nudRadius.Location = new System.Drawing.Point(89, 159);
            this.nudRadius.Margin = new System.Windows.Forms.Padding(2);
            this.nudRadius.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.Size = new System.Drawing.Size(105, 23);
            this.nudRadius.TabIndex = 10;
            this.nudRadius.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudPoints
            // 
            this.nudPoints.Location = new System.Drawing.Point(89, 104);
            this.nudPoints.Margin = new System.Windows.Forms.Padding(2);
            this.nudPoints.Name = "nudPoints";
            this.nudPoints.Size = new System.Drawing.Size(105, 23);
            this.nudPoints.TabIndex = 9;
            // 
            // nudY
            // 
            this.nudY.DecimalPlaces = 4;
            this.nudY.Location = new System.Drawing.Point(89, 52);
            this.nudY.Margin = new System.Windows.Forms.Padding(2);
            this.nudY.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudY.Name = "nudY";
            this.nudY.Size = new System.Drawing.Size(105, 23);
            this.nudY.TabIndex = 8;
            this.nudY.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // nudNumber
            // 
            this.nudNumber.Location = new System.Drawing.Point(89, 77);
            this.nudNumber.Margin = new System.Windows.Forms.Padding(2);
            this.nudNumber.Name = "nudNumber";
            this.nudNumber.Size = new System.Drawing.Size(105, 23);
            this.nudNumber.TabIndex = 8;
            this.nudNumber.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // nudX
            // 
            this.nudX.DecimalPlaces = 4;
            this.nudX.Location = new System.Drawing.Point(89, 25);
            this.nudX.Margin = new System.Windows.Forms.Padding(2);
            this.nudX.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudX.Name = "nudX";
            this.nudX.Size = new System.Drawing.Size(105, 23);
            this.nudX.TabIndex = 7;
            this.nudX.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // lblIterations
            // 
            this.lblIterations.AutoSize = true;
            this.lblIterations.Location = new System.Drawing.Point(15, 213);
            this.lblIterations.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIterations.Name = "lblIterations";
            this.lblIterations.Size = new System.Drawing.Size(56, 15);
            this.lblIterations.TabIndex = 6;
            this.lblIterations.Text = "Iterations";
            // 
            // lblTheta
            // 
            this.lblTheta.AutoSize = true;
            this.lblTheta.Location = new System.Drawing.Point(15, 187);
            this.lblTheta.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTheta.Name = "lblTheta";
            this.lblTheta.Size = new System.Drawing.Size(36, 15);
            this.lblTheta.TabIndex = 5;
            this.lblTheta.Text = "Theta";
            // 
            // lblRadius
            // 
            this.lblRadius.AutoSize = true;
            this.lblRadius.Location = new System.Drawing.Point(15, 162);
            this.lblRadius.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRadius.Name = "lblRadius";
            this.lblRadius.Size = new System.Drawing.Size(42, 15);
            this.lblRadius.TabIndex = 4;
            this.lblRadius.Text = "Radius";
            // 
            // lblPoints
            // 
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(15, 106);
            this.lblPoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Size = new System.Drawing.Size(40, 15);
            this.lblPoints.TabIndex = 3;
            this.lblPoints.Text = "Points";
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(15, 80);
            this.lblNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(51, 15);
            this.lblNumber.TabIndex = 2;
            this.lblNumber.Text = "Number";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(15, 53);
            this.lblY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 15);
            this.lblY.TabIndex = 1;
            this.lblY.Text = "Y";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(15, 28);
            this.lblX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 15);
            this.lblX.TabIndex = 0;
            this.lblX.Text = "X";
            // 
            // btnDeleteCanvas
            // 
            this.btnDeleteCanvas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteCanvas.Location = new System.Drawing.Point(15, 260);
            this.btnDeleteCanvas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteCanvas.Name = "btnDeleteCanvas";
            this.btnDeleteCanvas.Size = new System.Drawing.Size(237, 27);
            this.btnDeleteCanvas.TabIndex = 18;
            this.btnDeleteCanvas.Text = "&Delete canvas";
            this.btnDeleteCanvas.UseVisualStyleBackColor = false;
            this.btnDeleteCanvas.Click += new System.EventHandler(this.btnDeleteCanvas_Click);
            // 
            // btnExecCommand
            // 
            this.btnExecCommand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecCommand.Location = new System.Drawing.Point(15, 193);
            this.btnExecCommand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExecCommand.Name = "btnExecCommand";
            this.btnExecCommand.Size = new System.Drawing.Size(237, 27);
            this.btnExecCommand.TabIndex = 17;
            this.btnExecCommand.Text = "&Exec command";
            this.btnExecCommand.UseVisualStyleBackColor = false;
            this.btnExecCommand.Click += new System.EventHandler(this.btnExecCommand_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCommand.Location = new System.Drawing.Point(15, 7);
            this.tbCommand.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbCommand.Multiline = true;
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbCommand.Size = new System.Drawing.Size(238, 179);
            this.tbCommand.TabIndex = 16;
            // 
            // btnShowCanvasPopUp
            // 
            this.btnShowCanvasPopUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowCanvasPopUp.Location = new System.Drawing.Point(15, 226);
            this.btnShowCanvasPopUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnShowCanvasPopUp.Name = "btnShowCanvasPopUp";
            this.btnShowCanvasPopUp.Size = new System.Drawing.Size(237, 27);
            this.btnShowCanvasPopUp.TabIndex = 15;
            this.btnShowCanvasPopUp.Text = "Show &canvas pop-up";
            this.btnShowCanvasPopUp.UseVisualStyleBackColor = false;
            this.btnShowCanvasPopUp.Click += new System.EventHandler(this.btnShowCanvasPopUp_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // lblChangeTreeType
            // 
            this.lblChangeTreeType.AutoSize = true;
            this.lblChangeTreeType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline) 
                | System.Drawing.FontStyle.Strikeout))), System.Drawing.GraphicsUnit.Point);
            this.lblChangeTreeType.Location = new System.Drawing.Point(0, 673);
            this.lblChangeTreeType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblChangeTreeType.Name = "lblChangeTreeType";
            this.lblChangeTreeType.Size = new System.Drawing.Size(11, 13);
            this.lblChangeTreeType.TabIndex = 5;
            this.lblChangeTreeType.Text = ".";
            this.lblChangeTreeType.Click += new System.EventHandler(this.lblChangeTreeType_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showChildrenAsListToolStripMenuItem,
            this.editToolStripMenuItem,
            this.encryptToolStripMenuItem,
            this.decryptToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 92);
            // 
            // showChildrenAsListToolStripMenuItem
            // 
            this.showChildrenAsListToolStripMenuItem.Name = "showChildrenAsListToolStripMenuItem";
            this.showChildrenAsListToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.showChildrenAsListToolStripMenuItem.Text = "Show children as list";
            this.showChildrenAsListToolStripMenuItem.Click += new System.EventHandler(this.showChildrenAsListToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // encryptToolStripMenuItem
            // 
            this.encryptToolStripMenuItem.Name = "encryptToolStripMenuItem";
            this.encryptToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.encryptToolStripMenuItem.Text = "Encrypt";
            this.encryptToolStripMenuItem.Click += new System.EventHandler(this.encryptToolStripMenuItem_Click);
            // 
            // decryptToolStripMenuItem
            // 
            this.decryptToolStripMenuItem.Name = "decryptToolStripMenuItem";
            this.decryptToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.decryptToolStripMenuItem.Text = "Decrypt";
            this.decryptToolStripMenuItem.Click += new System.EventHandler(this.decryptToolStripMenuItem_Click);
            // 
            // btnExportToRtf
            // 
            this.btnExportToRtf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportToRtf.Location = new System.Drawing.Point(8, 54);
            this.btnExportToRtf.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExportToRtf.Name = "btnExportToRtf";
            this.btnExportToRtf.Size = new System.Drawing.Size(215, 27);
            this.btnExportToRtf.TabIndex = 12;
            this.btnExportToRtf.Text = "&Export to RTF";
            this.btnExportToRtf.UseVisualStyleBackColor = true;
            this.btnExportToRtf.Click += new System.EventHandler(this.btnExportToRtf_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1414, 689);
            this.Controls.Add(this.lblChangeTreeType);
            this.Controls.Add(this.tbTreeChange);
            this.Controls.Add(this.gbTaskList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "Tasks";
            this.DoubleClick += new System.EventHandler(this.MainForm_DoubleClick);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.gbTaskList.ResumeLayout(false);
            this.gbTaskList.PerformLayout();
            this.gbTask.ResumeLayout(false);
            this.gbTask.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCompleteProgress)).EndInit();
            this.gbTimeSpent.ResumeLayout(false);
            this.gbTimeSpent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMilliseconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHours)).EndInit();
            this.tbTreeChange.ResumeLayout(false);
            this.tbTaskEdit.ResumeLayout(false);
            this.gbCompletedPercent.ResumeLayout(false);
            this.gbCompletedPercent.PerformLayout();
            this.gbTaskOperations.ResumeLayout(false);
            this.gbStyleChange.ResumeLayout(false);
            this.gbStyleChange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
            this.tbTaskDetails.ResumeLayout(false);
            this.gbTaskDetails.ResumeLayout(false);
            this.gbTaskDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUrgency)).EndInit();
            this.tbTasksChange.ResumeLayout(false);
            this.tbTasksChange.PerformLayout();
            this.gbTreeToXML.ResumeLayout(false);
            this.gbTreeToXML.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowFromUrgencyNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowToUrgencyNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowFromNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShowUntilNumber)).EndInit();
            this.tbGraphics.ResumeLayout(false);
            this.tbGraphics.PerformLayout();
            this.gbGenerate.ResumeLayout(false);
            this.gbGenerate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudComputeType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTheta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnNoTask;
        private System.Windows.Forms.TreeView tvTaskList;
        private System.Windows.Forms.GroupBox gbTaskList;
        private System.Windows.Forms.Button btnAddTask;
        private System.Windows.Forms.GroupBox gbTask;
        private System.Windows.Forms.NumericUpDown nudCompleteProgress;
        private System.Windows.Forms.TextBox tbTaskName;
        private System.Windows.Forms.Label lblCompletePercent;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox gbTimeSpent;
        private System.Windows.Forms.ProgressBar pbPercentComplete;
        private System.Windows.Forms.Button btnStopCounting;
        private System.Windows.Forms.Button btnStartCounting;
        protected internal System.Windows.Forms.Label lblHours;
        protected internal System.Windows.Forms.Label lblMilliseconds;
        protected internal System.Windows.Forms.Label lblSeconds;
        protected internal System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.NumericUpDown nudMilliseconds;
        private System.Windows.Forms.NumericUpDown nudSeconds;
        private System.Windows.Forms.NumericUpDown nudMinutes;
        private System.Windows.Forms.NumericUpDown nudHours;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnCollapse;
        private System.Windows.Forms.Button btnCalculatePercentage;
        private System.Windows.Forms.Button btnCalculatePercentage2;
        private System.Windows.Forms.Button btnToggleCompletedTasks;
        private System.Windows.Forms.Button btnMoveToNextUnfinished;
        private System.Windows.Forms.Button btnUpdateText;
        private System.Windows.Forms.Button btnMoveTaskUp;
        private System.Windows.Forms.Button btnMoveTaskDown;
        private System.Windows.Forms.TextBox tbSearchBox;
        private System.Windows.Forms.Button btnResetException;
        private System.Windows.Forms.Button btnDoNotSave;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tbTaskEdit;
        private System.Windows.Forms.TabPage tbTaskDetails;
        private System.Windows.Forms.TabPage tbTasksChange;
        private System.Windows.Forms.NumericUpDown nudFontSize;
        private System.Windows.Forms.Label lblFontFamily;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.ComboBox cbFontFamily;
        private System.Windows.Forms.CheckedListBox clbStyle;
        private System.Windows.Forms.Label lblBackgroundColor;
        private System.Windows.Forms.Label lblTextColor;
        private System.Windows.Forms.TextBox tbBackgroundColor;
        private System.Windows.Forms.TextBox tbTextColor;
        private System.Windows.Forms.GroupBox gbStyleChange;
        private System.Windows.Forms.GroupBox gbTaskOperations;
        private System.Windows.Forms.GroupBox gbCompletedPercent;
        private System.Windows.Forms.NumericUpDown nudShowFromUrgencyNumber;
        private System.Windows.Forms.Button btnShowFromToUrgencyNumber;
        private System.Windows.Forms.NumericUpDown nudShowToUrgencyNumber;
        private System.Windows.Forms.Label lblUrgencyNumber;
        private System.Windows.Forms.GroupBox gbTaskDetails;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbLink;
        private System.Windows.Forms.Button btnUpdateLink;
        private System.Windows.Forms.NumericUpDown nudUrgency;
        private System.Windows.Forms.Label lblUrgency;
        private System.Windows.Forms.Label lblAddedNumber;
        private System.Windows.Forms.Label lblAddedDate;
        private System.Windows.Forms.TextBox tbAddedDate;
        private System.Windows.Forms.TextBox tbAddedNumber;
        private System.Windows.Forms.TextBox tbDataSize;
        private System.Windows.Forms.Label lblDataSize;
        private System.Windows.Forms.Button btnMoveNode;
        private System.Windows.Forms.TabControl tbTreeChange;
        private System.Windows.Forms.NumericUpDown nudShowUntilNumber;
        private System.Windows.Forms.Button btnShowFromToNumberOfTask;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.NumericUpDown nudShowFromNumber;
        private System.Windows.Forms.Label lblNumberOfTask;
        private System.Windows.Forms.CheckBox cbUseSelectedNode;
        private System.Windows.Forms.Button btnGoToDefaultTree;
        private System.Windows.Forms.Button btnMakeSubTreeWithChildrenOfSelectedNode;
        private System.Windows.Forms.GroupBox gbTreeToXML;
        private System.Windows.Forms.Label lblIsStartupAlert;
        private System.Windows.Forms.TextBox tbCategory;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.CheckBox cbIsStartupAlert;
        private System.Windows.Forms.TextBox tbLastChangeDate;
        private System.Windows.Forms.Label lblLastChangeDate;
        private System.Windows.Forms.Button btnShowStartupAlertForm;
        private System.Windows.Forms.TabPage tbGraphics;
        private System.Windows.Forms.Button btnShowCanvasPopUp;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.Button btnExecCommand;
        private System.Windows.Forms.Button btnDeleteCanvas;
        private System.Windows.Forms.GroupBox gbGenerate;
        private System.Windows.Forms.NumericUpDown nudIterations;
        private System.Windows.Forms.NumericUpDown nudTheta;
        private System.Windows.Forms.NumericUpDown nudRadius;
        private System.Windows.Forms.NumericUpDown nudPoints;
        private System.Windows.Forms.NumericUpDown nudY;
        private System.Windows.Forms.NumericUpDown nudNumber;
        private System.Windows.Forms.NumericUpDown nudX;
        private System.Windows.Forms.Label lblIterations;
        private System.Windows.Forms.Label lblTheta;
        private System.Windows.Forms.Label lblRadius;
        private System.Windows.Forms.Label lblPoints;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox cbUseDefaults;
        private System.Windows.Forms.NumericUpDown nudComputeType;
        private System.Windows.Forms.Label lblComputeType;
        private System.Windows.Forms.Button btnGenerateFiguresAndExec;
        private System.Windows.Forms.CheckBox cbLog;
        private System.Windows.Forms.Label lblUnchanged;
        private System.Windows.Forms.Label lblChangeTreeType;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showChildrenAsListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encryptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decryptToolStripMenuItem;
        private Button btnExportToRtf;
    }
}

