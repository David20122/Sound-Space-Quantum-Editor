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
			this.RemoveButton = new System.Windows.Forms.Button();
			this.UpdateButton = new System.Windows.Forms.Button();
			this.AddButton = new System.Windows.Forms.Button();
			this.BPMLabel = new System.Windows.Forms.Label();
			this.OffsetLabel = new System.Windows.Forms.Label();
			this.OffsetBox = new System.Windows.Forms.TextBox();
			this.BPMBox = new System.Windows.Forms.TextBox();
			this.BPM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.PointList)).BeginInit();
			this.SuspendLayout();
			// 
			// PointList
			// 
			this.PointList.AllowUserToAddRows = false;
			this.PointList.AllowUserToDeleteRows = false;
			this.PointList.BackgroundColor = System.Drawing.SystemColors.Control;
			this.PointList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.PointList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.PointList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BPM,
            this.Offset});
			this.PointList.Location = new System.Drawing.Point(9, 9);
			this.PointList.Margin = new System.Windows.Forms.Padding(0);
			this.PointList.MultiSelect = false;
			this.PointList.Name = "PointList";
			this.PointList.ReadOnly = true;
			this.PointList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.PointList.Size = new System.Drawing.Size(326, 435);
			this.PointList.TabIndex = 9;
			this.PointList.SelectionChanged += new System.EventHandler(this.PointList_SelectionChanged);
			// 
			// RemoveButton
			// 
			this.RemoveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.RemoveButton.Location = new System.Drawing.Point(247, 448);
			this.RemoveButton.Name = "RemoveButton";
			this.RemoveButton.Size = new System.Drawing.Size(88, 22);
			this.RemoveButton.TabIndex = 5;
			this.RemoveButton.Text = "Remove Point";
			this.RemoveButton.UseVisualStyleBackColor = true;
			this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
			// 
			// UpdateButton
			// 
			this.UpdateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.UpdateButton.Location = new System.Drawing.Point(158, 470);
			this.UpdateButton.Name = "UpdateButton";
			this.UpdateButton.Size = new System.Drawing.Size(88, 22);
			this.UpdateButton.TabIndex = 4;
			this.UpdateButton.Text = "Update Point";
			this.UpdateButton.UseVisualStyleBackColor = true;
			this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
			// 
			// AddButton
			// 
			this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.AddButton.Location = new System.Drawing.Point(158, 448);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(88, 22);
			this.AddButton.TabIndex = 3;
			this.AddButton.Text = "Add Point";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// BPMLabel
			// 
			this.BPMLabel.Location = new System.Drawing.Point(9, 453);
			this.BPMLabel.Margin = new System.Windows.Forms.Padding(0);
			this.BPMLabel.Name = "BPMLabel";
			this.BPMLabel.Size = new System.Drawing.Size(68, 16);
			this.BPMLabel.TabIndex = 5;
			this.BPMLabel.Text = "BPM";
			// 
			// OffsetLabel
			// 
			this.OffsetLabel.Location = new System.Drawing.Point(9, 474);
			this.OffsetLabel.Margin = new System.Windows.Forms.Padding(0);
			this.OffsetLabel.Name = "OffsetLabel";
			this.OffsetLabel.Size = new System.Drawing.Size(68, 16);
			this.OffsetLabel.TabIndex = 7;
			this.OffsetLabel.Text = "Position (ms)";
			// 
			// OffsetBox
			// 
			this.OffsetBox.Location = new System.Drawing.Point(80, 471);
			this.OffsetBox.Name = "OffsetBox";
			this.OffsetBox.Size = new System.Drawing.Size(72, 20);
			this.OffsetBox.TabIndex = 2;
			// 
			// BPMBox
			// 
			this.BPMBox.Location = new System.Drawing.Point(80, 449);
			this.BPMBox.Name = "BPMBox";
			this.BPMBox.Size = new System.Drawing.Size(72, 20);
			this.BPMBox.TabIndex = 1;
			// 
			// BPM
			// 
			this.BPM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.BPM.HeaderText = "BPM";
			this.BPM.Name = "BPM";
			this.BPM.ReadOnly = true;
			this.BPM.ToolTipText = "The BPM of the point";
			// 
			// Offset
			// 
			this.Offset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Offset.HeaderText = "Position (ms)";
			this.Offset.Name = "Offset";
			this.Offset.ReadOnly = true;
			this.Offset.ToolTipText = "The position of the point in milliseconds";
			// 
			// TimingsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 501);
			this.Controls.Add(this.BPMBox);
			this.Controls.Add(this.OffsetBox);
			this.Controls.Add(this.OffsetLabel);
			this.Controls.Add(this.BPMLabel);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.UpdateButton);
			this.Controls.Add(this.RemoveButton);
			this.Controls.Add(this.PointList);
			this.Name = "TimingsWindow";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Timing Setup Panel";
			((System.ComponentModel.ISupportInitialize)(this.PointList)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView PointList;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Label BPMLabel;
        private System.Windows.Forms.Label OffsetLabel;
        private System.Windows.Forms.TextBox OffsetBox;
        private System.Windows.Forms.TextBox BPMBox;
		private System.Windows.Forms.DataGridViewTextBoxColumn BPM;
		private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
	}
}