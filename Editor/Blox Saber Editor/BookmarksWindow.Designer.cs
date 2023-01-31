namespace Sound_Space_Editor
{
	partial class BookmarksWindow
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
            this.TextBox = new System.Windows.Forms.RichTextBox();
            this.OffsetLabel = new System.Windows.Forms.Label();
            this.TextLabel = new System.Windows.Forms.Label();
            this.CurrentButton = new System.Windows.Forms.Button();
            this.OffsetBox = new System.Windows.Forms.NumericUpDown();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.BookmarkList = new System.Windows.Forms.DataGridView();
            this.Text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Offset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.OffsetBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BookmarkList)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBox
            // 
            this.TextBox.Location = new System.Drawing.Point(64, 354);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(88, 22);
            this.TextBox.TabIndex = 60;
            this.TextBox.Text = "";
            // 
            // OffsetLabel
            // 
            this.OffsetLabel.AutoSize = true;
            this.OffsetLabel.Location = new System.Drawing.Point(23, 384);
            this.OffsetLabel.Name = "OffsetLabel";
            this.OffsetLabel.Size = new System.Drawing.Size(35, 13);
            this.OffsetLabel.TabIndex = 59;
            this.OffsetLabel.Text = "Offset";
            // 
            // TextLabel
            // 
            this.TextLabel.AutoSize = true;
            this.TextLabel.Location = new System.Drawing.Point(28, 358);
            this.TextLabel.Name = "TextLabel";
            this.TextLabel.Size = new System.Drawing.Size(28, 13);
            this.TextLabel.TabIndex = 58;
            this.TextLabel.Text = "Text";
            // 
            // CurrentButton
            // 
            this.CurrentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.CurrentButton.Location = new System.Drawing.Point(262, 382);
            this.CurrentButton.Name = "CurrentButton";
            this.CurrentButton.Size = new System.Drawing.Size(101, 22);
            this.CurrentButton.TabIndex = 57;
            this.CurrentButton.Text = "Current Pos";
            this.CurrentButton.UseVisualStyleBackColor = true;
            this.CurrentButton.Click += new System.EventHandler(this.CurrentButton_Click);
            // 
            // OffsetBox
            // 
            this.OffsetBox.Location = new System.Drawing.Point(64, 382);
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
            this.OffsetBox.TabIndex = 56;
            this.OffsetBox.ValueChanged += new System.EventHandler(this.OffsetBox_ValueChanged);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.DeleteButton.Location = new System.Drawing.Point(262, 354);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(101, 22);
            this.DeleteButton.TabIndex = 55;
            this.DeleteButton.Text = "Delete Bookmark";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AddButton.Location = new System.Drawing.Point(153, 354);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(101, 22);
            this.AddButton.TabIndex = 54;
            this.AddButton.Text = "Add Bookmark";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // UpdateButton
            // 
            this.UpdateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.UpdateButton.Location = new System.Drawing.Point(153, 382);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(101, 22);
            this.UpdateButton.TabIndex = 53;
            this.UpdateButton.Text = "Update Bookmark";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // BookmarkList
            // 
            this.BookmarkList.AllowUserToAddRows = false;
            this.BookmarkList.AllowUserToDeleteRows = false;
            this.BookmarkList.AllowUserToResizeColumns = false;
            this.BookmarkList.AllowUserToResizeRows = false;
            this.BookmarkList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.BookmarkList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BookmarkList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BookmarkList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Text,
            this.Offset});
            this.BookmarkList.Location = new System.Drawing.Point(9, 9);
            this.BookmarkList.Margin = new System.Windows.Forms.Padding(0);
            this.BookmarkList.Name = "BookmarkList";
            this.BookmarkList.ReadOnly = true;
            this.BookmarkList.RowHeadersVisible = false;
            this.BookmarkList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.BookmarkList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.BookmarkList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.BookmarkList.Size = new System.Drawing.Size(354, 342);
            this.BookmarkList.TabIndex = 52;
            this.BookmarkList.SelectionChanged += new System.EventHandler(this.BookmarkList_SelectionChanged);
            // 
            // Text
            // 
            this.Text.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Text.HeaderText = "Text";
            this.Text.Name = "Text";
            this.Text.ReadOnly = true;
            this.Text.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Text.ToolTipText = "The BPM of the point";
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
            // BookmarksWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 411);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.OffsetLabel);
            this.Controls.Add(this.TextLabel);
            this.Controls.Add(this.CurrentButton);
            this.Controls.Add(this.OffsetBox);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.BookmarkList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "BookmarksWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.OnClosing);
            ((System.ComponentModel.ISupportInitialize)(this.OffsetBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BookmarkList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        #endregion

        private System.Windows.Forms.RichTextBox TextBox;
        private System.Windows.Forms.Label OffsetLabel;
        private System.Windows.Forms.Label TextLabel;
        private System.Windows.Forms.Button CurrentButton;
        private System.Windows.Forms.NumericUpDown OffsetBox;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.DataGridView BookmarkList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Text;
        private System.Windows.Forms.DataGridViewTextBoxColumn Offset;
    }
}