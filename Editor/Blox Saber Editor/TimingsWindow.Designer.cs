namespace Sound_Space_Editor
{
    partial class TimingsWindow
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
            this.PointList = new System.Windows.Forms.DataGridView();
            this.BPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.MoveLabel = new System.Windows.Forms.Label();
            this.MoveBox = new System.Windows.Forms.NumericUpDown();
            this.MoveButton = new System.Windows.Forms.Button();
            this.ImportCH = new System.Windows.Forms.Button();
            this.ImportOSU = new System.Windows.Forms.Button();
            this.ImportADOFAI = new System.Windows.Forms.Button();
            this.OpenTapper = new System.Windows.Forms.Button();
            this.OpenBeatmap = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.BpmBox = new System.Windows.Forms.NumericUpDown();
            this.OffsetBox = new System.Windows.Forms.NumericUpDown();
            this.CurrentButton = new System.Windows.Forms.Button();
            this.BpmLabel = new System.Windows.Forms.Label();
            this.OffsetLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BpmBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PointList
            // 
            this.PointList.AllowUserToAddRows = false;
            this.PointList.AllowUserToDeleteRows = false;
            this.PointList.AllowUserToResizeColumns = false;
            this.PointList.AllowUserToResizeRows = false;
            this.PointList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.PointList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PointList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PointList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BPM,
            this.Offset});
            this.PointList.Location = new System.Drawing.Point(9, 9);
            this.PointList.Margin = new System.Windows.Forms.Padding(0);
            this.PointList.Name = "PointList";
            this.PointList.ReadOnly = true;
            this.PointList.RowHeadersVisible = false;
            this.PointList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.PointList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PointList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.PointList.Size = new System.Drawing.Size(326, 342);
            this.PointList.TabIndex = 9;
            this.PointList.SelectionChanged += new System.EventHandler(this.PointList_SelectionChanged);
            // 
            // BPM
            // 
            this.BPM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BPM.HeaderText = "BPM";
            this.BPM.Name = "BPM";
            this.BPM.ReadOnly = true;
            this.BPM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BPM.ToolTipText = "The BPM of the point";
            // 
            // Offset
            // 
            this.Offset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Offset.HeaderText = "Position (ms)";
            this.Offset.Name = "Offset";
            this.Offset.ReadOnly = true;
            this.Offset.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Offset.ToolTipText = "The position of the point in milliseconds";
            // 
            // UpdateButton
            // 
            this.UpdateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.UpdateButton.Location = new System.Drawing.Point(158, 382);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(88, 22);
            this.UpdateButton.TabIndex = 4;
            this.UpdateButton.Text = "Update Point";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // MoveLabel
            // 
            this.MoveLabel.Location = new System.Drawing.Point(9, 412);
            this.MoveLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MoveLabel.Name = "MoveLabel";
            this.MoveLabel.Size = new System.Drawing.Size(143, 16);
            this.MoveLabel.TabIndex = 10;
            this.MoveLabel.Text = "Move Selected Points (ms)";
            // 
            // MoveBox
            // 
            this.MoveBox.Location = new System.Drawing.Point(158, 410);
            this.MoveBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.MoveBox.Minimum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            -2147483648});
            this.MoveBox.Name = "MoveBox";
            this.MoveBox.Size = new System.Drawing.Size(88, 20);
            this.MoveBox.TabIndex = 11;
            // 
            // MoveButton
            // 
            this.MoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MoveButton.Location = new System.Drawing.Point(247, 409);
            this.MoveButton.Name = "MoveButton";
            this.MoveButton.Size = new System.Drawing.Size(88, 22);
            this.MoveButton.TabIndex = 12;
            this.MoveButton.Text = "Move Points";
            this.MoveButton.UseVisualStyleBackColor = true;
            this.MoveButton.Click += new System.EventHandler(this.MoveButton_Click);
            // 
            // ImportCH
            // 
            this.ImportCH.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportCH.Location = new System.Drawing.Point(175, 436);
            this.ImportCH.Name = "ImportCH";
            this.ImportCH.Size = new System.Drawing.Size(77, 47);
            this.ImportCH.TabIndex = 13;
            this.ImportCH.Text = "Paste Clone Hero Timings";
            this.ImportCH.UseVisualStyleBackColor = true;
            this.ImportCH.Click += new System.EventHandler(this.ImportCH_Click);
            // 
            // ImportOSU
            // 
            this.ImportOSU.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportOSU.Location = new System.Drawing.Point(9, 436);
            this.ImportOSU.Name = "ImportOSU";
            this.ImportOSU.Size = new System.Drawing.Size(77, 47);
            this.ImportOSU.TabIndex = 14;
            this.ImportOSU.Text = "Paste OSU Timings";
            this.ImportOSU.UseVisualStyleBackColor = true;
            this.ImportOSU.Click += new System.EventHandler(this.ImportOSU_Click);
            // 
            // ImportADOFAI
            // 
            this.ImportADOFAI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ImportADOFAI.Location = new System.Drawing.Point(92, 436);
            this.ImportADOFAI.Name = "ImportADOFAI";
            this.ImportADOFAI.Size = new System.Drawing.Size(77, 47);
            this.ImportADOFAI.TabIndex = 15;
            this.ImportADOFAI.Text = "Paste ADOFAI Timings";
            this.ImportADOFAI.UseVisualStyleBackColor = true;
            this.ImportADOFAI.Click += new System.EventHandler(this.ImportADOFAI_Click);
            // 
            // OpenTapper
            // 
            this.OpenTapper.Location = new System.Drawing.Point(92, 489);
            this.OpenTapper.Name = "OpenTapper";
            this.OpenTapper.Size = new System.Drawing.Size(160, 47);
            this.OpenTapper.TabIndex = 16;
            this.OpenTapper.Text = "Open BPM Tapper";
            this.OpenTapper.UseVisualStyleBackColor = true;
            this.OpenTapper.Click += new System.EventHandler(this.OpenTapper_Click);
            // 
            // OpenBeatmap
            // 
            this.OpenBeatmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.OpenBeatmap.Location = new System.Drawing.Point(258, 436);
            this.OpenBeatmap.Name = "OpenBeatmap";
            this.OpenBeatmap.Size = new System.Drawing.Size(77, 47);
            this.OpenBeatmap.TabIndex = 17;
            this.OpenBeatmap.Text = "Open Beatmap Timings";
            this.OpenBeatmap.UseVisualStyleBackColor = true;
            this.OpenBeatmap.Click += new System.EventHandler(this.OpenBeatmap_Click);
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AddButton.Location = new System.Drawing.Point(158, 354);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(88, 22);
            this.AddButton.TabIndex = 18;
            this.AddButton.Text = "Add Point";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DeleteButton.Location = new System.Drawing.Point(247, 354);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(88, 22);
            this.DeleteButton.TabIndex = 19;
            this.DeleteButton.Text = "Delete Point";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // BpmBox
            // 
            this.BpmBox.DecimalPlaces = 3;
            this.BpmBox.Location = new System.Drawing.Point(69, 356);
            this.BpmBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.BpmBox.Minimum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            -2147483648});
            this.BpmBox.Name = "BpmBox";
            this.BpmBox.Size = new System.Drawing.Size(88, 20);
            this.BpmBox.TabIndex = 20;
            // 
            // OffsetBox
            // 
            this.OffsetBox.Location = new System.Drawing.Point(69, 382);
            this.OffsetBox.Maximum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            0});
            this.OffsetBox.Minimum = new decimal(new int[] {
            268435455,
            1042612833,
            542101086,
            -2147483648});
            this.OffsetBox.Name = "OffsetBox";
            this.OffsetBox.Size = new System.Drawing.Size(88, 20);
            this.OffsetBox.TabIndex = 21;
            // 
            // CurrentButton
            // 
            this.CurrentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CurrentButton.Location = new System.Drawing.Point(247, 382);
            this.CurrentButton.Name = "CurrentButton";
            this.CurrentButton.Size = new System.Drawing.Size(88, 22);
            this.CurrentButton.TabIndex = 22;
            this.CurrentButton.Text = "Current Pos";
            this.CurrentButton.UseVisualStyleBackColor = true;
            this.CurrentButton.Click += new System.EventHandler(this.CurrentButton_Click);
            // 
            // BpmLabel
            // 
            this.BpmLabel.AutoSize = true;
            this.BpmLabel.Location = new System.Drawing.Point(33, 358);
            this.BpmLabel.Name = "BpmLabel";
            this.BpmLabel.Size = new System.Drawing.Size(30, 13);
            this.BpmLabel.TabIndex = 23;
            this.BpmLabel.Text = "BPM";
            // 
            // OffsetLabel
            // 
            this.OffsetLabel.AutoSize = true;
            this.OffsetLabel.Location = new System.Drawing.Point(28, 384);
            this.OffsetLabel.Name = "OffsetLabel";
            this.OffsetLabel.Size = new System.Drawing.Size(35, 13);
            this.OffsetLabel.TabIndex = 24;
            this.OffsetLabel.Text = "Offset";
            // 
            // TimingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 546);
            this.Controls.Add(this.OffsetLabel);
            this.Controls.Add(this.BpmLabel);
            this.Controls.Add(this.CurrentButton);
            this.Controls.Add(this.OffsetBox);
            this.Controls.Add(this.BpmBox);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.OpenBeatmap);
            this.Controls.Add(this.OpenTapper);
            this.Controls.Add(this.ImportADOFAI);
            this.Controls.Add(this.ImportOSU);
            this.Controls.Add(this.ImportCH);
            this.Controls.Add(this.MoveButton);
            this.Controls.Add(this.MoveBox);
            this.Controls.Add(this.MoveLabel);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.PointList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TimingsWindow";
            this.ShowIcon = false;
            this.Closing += new System.ComponentModel.CancelEventHandler(OnClosing);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BpmBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView PointList;
		private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Label MoveLabel;
        private System.Windows.Forms.NumericUpDown MoveBox;
        private System.Windows.Forms.Button MoveButton;
        private System.Windows.Forms.Button ImportCH;
        private System.Windows.Forms.Button ImportOSU;
        private System.Windows.Forms.Button ImportADOFAI;
        private System.Windows.Forms.Button OpenTapper;
        private System.Windows.Forms.Button OpenBeatmap;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.NumericUpDown BpmBox;
        private System.Windows.Forms.NumericUpDown OffsetBox;
        private System.Windows.Forms.Button CurrentButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn BPM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
        private System.Windows.Forms.Label BpmLabel;
        private System.Windows.Forms.Label OffsetLabel;
    }
}