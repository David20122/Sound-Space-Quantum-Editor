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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.PointList = new System.Windows.Forms.DataGridView();
            this.CurrentButton = new System.Windows.Forms.Button();
            this.MoveLabel = new System.Windows.Forms.Label();
            this.MoveBox = new System.Windows.Forms.NumericUpDown();
            this.MoveButton = new System.Windows.Forms.Button();
            this.ImportCH = new System.Windows.Forms.Button();
            this.ImportOSU = new System.Windows.Forms.Button();
            this.ImportADOFAI = new System.Windows.Forms.Button();
            this.OpenTapper = new System.Windows.Forms.Button();
            this.OpenBeatmap = new System.Windows.Forms.Button();
            this.BPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remove = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PointList
            // 
            this.PointList.AllowUserToResizeColumns = false;
            this.PointList.AllowUserToResizeRows = false;
            this.PointList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.PointList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PointList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PointList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BPM,
            this.Offset,
            this.Remove});
            this.PointList.Location = new System.Drawing.Point(9, 9);
            this.PointList.Margin = new System.Windows.Forms.Padding(0);
            this.PointList.Name = "PointList";
            this.PointList.RowHeadersVisible = false;
            this.PointList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.PointList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PointList.Size = new System.Drawing.Size(326, 369);
            this.PointList.TabIndex = 9;
            this.PointList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointList_CellContentClick);
            this.PointList.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.UpdateList);
            // 
            // CurrentButton
            // 
            this.CurrentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CurrentButton.Location = new System.Drawing.Point(247, 381);
            this.CurrentButton.Name = "CurrentButton";
            this.CurrentButton.Size = new System.Drawing.Size(88, 22);
            this.CurrentButton.TabIndex = 4;
            this.CurrentButton.Text = "Current Pos";
            this.CurrentButton.UseVisualStyleBackColor = true;
            this.CurrentButton.Click += new System.EventHandler(this.CurrentButton_Click);
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
            // BPM
            // 
            this.BPM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BPM.HeaderText = "BPM";
            this.BPM.Name = "BPM";
            this.BPM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BPM.ToolTipText = "The BPM of the point";
            // 
            // Offset
            // 
            this.Offset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Offset.HeaderText = "Position (ms)";
            this.Offset.Name = "Offset";
            this.Offset.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Offset.ToolTipText = "The position of the point in milliseconds";
            // 
            // Remove
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.Remove.DefaultCellStyle = dataGridViewCellStyle1;
            this.Remove.HeaderText = "";
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Text = "";
            this.Remove.Width = 20;
            // 
            // TimingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 546);
            this.Controls.Add(this.OpenBeatmap);
            this.Controls.Add(this.OpenTapper);
            this.Controls.Add(this.ImportADOFAI);
            this.Controls.Add(this.ImportOSU);
            this.Controls.Add(this.ImportCH);
            this.Controls.Add(this.MoveButton);
            this.Controls.Add(this.MoveBox);
            this.Controls.Add(this.MoveLabel);
            this.Controls.Add(this.CurrentButton);
            this.Controls.Add(this.PointList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TimingsWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "`";
            ((System.ComponentModel.ISupportInitialize)(this.PointList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PointList;
		private System.Windows.Forms.Button CurrentButton;
        private System.Windows.Forms.Label MoveLabel;
        private System.Windows.Forms.NumericUpDown MoveBox;
        private System.Windows.Forms.Button MoveButton;
        private System.Windows.Forms.Button ImportCH;
        private System.Windows.Forms.Button ImportOSU;
        private System.Windows.Forms.Button ImportADOFAI;
        private System.Windows.Forms.Button OpenTapper;
        private System.Windows.Forms.Button OpenBeatmap;
        private System.Windows.Forms.DataGridViewTextBoxColumn BPM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
        private System.Windows.Forms.DataGridViewButtonColumn Remove;
    }
}