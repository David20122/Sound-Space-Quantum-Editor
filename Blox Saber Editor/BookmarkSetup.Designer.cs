namespace Sound_Space_Editor
{
	partial class BookmarkSetup
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
			this.BookmarkList = new System.Windows.Forms.DataGridView();
			this.Label = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CurrentButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.BookmarkList)).BeginInit();
			this.SuspendLayout();
			// 
			// BookmarkList
			// 
			this.BookmarkList.AllowUserToResizeRows = false;
			this.BookmarkList.BackgroundColor = System.Drawing.SystemColors.Control;
			this.BookmarkList.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.BookmarkList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.BookmarkList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Label,
            this.Time});
			this.BookmarkList.Location = new System.Drawing.Point(12, 12);
			this.BookmarkList.Name = "BookmarkList";
			this.BookmarkList.RowHeadersVisible = false;
			this.BookmarkList.Size = new System.Drawing.Size(376, 398);
			this.BookmarkList.TabIndex = 1;
			this.BookmarkList.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.UpdateList);
			// 
			// Label
			// 
			this.Label.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Label.HeaderText = "Label";
			this.Label.Name = "Label";
			this.Label.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Time
			// 
			this.Time.HeaderText = "Position (ms)";
			this.Time.Name = "Time";
			// 
			// CurrentButton
			// 
			this.CurrentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.CurrentButton.Location = new System.Drawing.Point(292, 416);
			this.CurrentButton.Name = "CurrentButton";
			this.CurrentButton.Size = new System.Drawing.Size(96, 22);
			this.CurrentButton.TabIndex = 5;
			this.CurrentButton.Text = "Use Current Pos";
			this.CurrentButton.UseVisualStyleBackColor = true;
			this.CurrentButton.Click += new System.EventHandler(this.CurrentButton_Click);
			// 
			// BookmarkSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(400, 450);
			this.Controls.Add(this.CurrentButton);
			this.Controls.Add(this.BookmarkList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "BookmarkSetup";
			this.Text = "Bookmarks";
			((System.ComponentModel.ISupportInitialize)(this.BookmarkList)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView BookmarkList;
		private System.Windows.Forms.DataGridViewTextBoxColumn Label;
		private System.Windows.Forms.DataGridViewTextBoxColumn Time;
		private System.Windows.Forms.Button CurrentButton;
	}
}