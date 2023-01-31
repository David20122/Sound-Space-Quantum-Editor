
namespace Sound_Space_Editor
{
    partial class ExportSSPM
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
            this.MapIDLabel = new System.Windows.Forms.Label();
            this.MapIDBox = new System.Windows.Forms.TextBox();
            this.SongNameLabel = new System.Windows.Forms.Label();
            this.SongNameBox = new System.Windows.Forms.TextBox();
            this.MappersLabel = new System.Windows.Forms.Label();
            this.MappersSubLabel = new System.Windows.Forms.Label();
            this.FinishButton = new System.Windows.Forms.Button();
            this.MappersBox = new System.Windows.Forms.TextBox();
            this.CoverPathBox = new System.Windows.Forms.TextBox();
            this.SelectButton = new System.Windows.Forms.Button();
            this.CoverPathLabel = new System.Windows.Forms.Label();
            this.UseCoverCheckbox = new System.Windows.Forms.CheckBox();
            this.DifficultyBox = new System.Windows.Forms.ComboBox();
            this.DifficultyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MapIDLabel
            // 
            this.MapIDLabel.AutoSize = true;
            this.MapIDLabel.Location = new System.Drawing.Point(11, 15);
            this.MapIDLabel.Name = "MapIDLabel";
            this.MapIDLabel.Size = new System.Drawing.Size(42, 13);
            this.MapIDLabel.TabIndex = 0;
            this.MapIDLabel.Text = "Map ID";
            // 
            // MapIDBox
            // 
            this.MapIDBox.Location = new System.Drawing.Point(80, 12);
            this.MapIDBox.Name = "MapIDBox";
            this.MapIDBox.ReadOnly = true;
            this.MapIDBox.Size = new System.Drawing.Size(236, 20);
            this.MapIDBox.TabIndex = 1;
            // 
            // SongNameLabel
            // 
            this.SongNameLabel.AutoSize = true;
            this.SongNameLabel.Location = new System.Drawing.Point(11, 41);
            this.SongNameLabel.Name = "SongNameLabel";
            this.SongNameLabel.Size = new System.Drawing.Size(63, 13);
            this.SongNameLabel.TabIndex = 2;
            this.SongNameLabel.Text = "Song Name";
            // 
            // SongNameBox
            // 
            this.SongNameBox.Location = new System.Drawing.Point(80, 38);
            this.SongNameBox.Name = "SongNameBox";
            this.SongNameBox.Size = new System.Drawing.Size(236, 20);
            this.SongNameBox.TabIndex = 3;
            this.SongNameBox.TextChanged += new System.EventHandler(this.SongNameBox_TextChanged);
            // 
            // MappersLabel
            // 
            this.MappersLabel.AutoSize = true;
            this.MappersLabel.Location = new System.Drawing.Point(11, 107);
            this.MappersLabel.Name = "MappersLabel";
            this.MappersLabel.Size = new System.Drawing.Size(54, 13);
            this.MappersLabel.TabIndex = 5;
            this.MappersLabel.Text = "Mapper(s)";
            // 
            // MappersSubLabel
            // 
            this.MappersSubLabel.AutoSize = true;
            this.MappersSubLabel.Location = new System.Drawing.Point(11, 132);
            this.MappersSubLabel.Name = "MappersSubLabel";
            this.MappersSubLabel.Size = new System.Drawing.Size(70, 13);
            this.MappersSubLabel.TabIndex = 6;
            this.MappersSubLabel.Text = "(One per line)";
            // 
            // FinishButton
            // 
            this.FinishButton.Location = new System.Drawing.Point(14, 248);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(302, 36);
            this.FinishButton.TabIndex = 7;
            this.FinishButton.Text = "Finish";
            this.FinishButton.UseVisualStyleBackColor = true;
            this.FinishButton.Click += new System.EventHandler(this.FinishButton_Click);
            // 
            // MappersBox
            // 
            this.MappersBox.Location = new System.Drawing.Point(80, 91);
            this.MappersBox.Multiline = true;
            this.MappersBox.Name = "MappersBox";
            this.MappersBox.Size = new System.Drawing.Size(236, 101);
            this.MappersBox.TabIndex = 8;
            this.MappersBox.TextChanged += new System.EventHandler(this.MappersBox_TextChanged);
            // 
            // CoverPathBox
            // 
            this.CoverPathBox.Location = new System.Drawing.Point(80, 198);
            this.CoverPathBox.Name = "CoverPathBox";
            this.CoverPathBox.ReadOnly = true;
            this.CoverPathBox.Size = new System.Drawing.Size(176, 20);
            this.CoverPathBox.TabIndex = 9;
            this.CoverPathBox.Text = "Default";
            // 
            // SelectButton
            // 
            this.SelectButton.Location = new System.Drawing.Point(262, 194);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(54, 26);
            this.SelectButton.TabIndex = 10;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // CoverPathLabel
            // 
            this.CoverPathLabel.AutoSize = true;
            this.CoverPathLabel.Location = new System.Drawing.Point(11, 201);
            this.CoverPathLabel.Name = "CoverPathLabel";
            this.CoverPathLabel.Size = new System.Drawing.Size(67, 13);
            this.CoverPathLabel.TabIndex = 11;
            this.CoverPathLabel.Text = "Cover Image";
            // 
            // UseCoverCheckbox
            // 
            this.UseCoverCheckbox.AutoSize = true;
            this.UseCoverCheckbox.Checked = true;
            this.UseCoverCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseCoverCheckbox.Location = new System.Drawing.Point(80, 225);
            this.UseCoverCheckbox.Name = "UseCoverCheckbox";
            this.UseCoverCheckbox.Size = new System.Drawing.Size(76, 17);
            this.UseCoverCheckbox.TabIndex = 12;
            this.UseCoverCheckbox.Text = "Use Cover";
            this.UseCoverCheckbox.UseVisualStyleBackColor = true;
            // 
            // DifficultyBox
            // 
            this.DifficultyBox.FormattingEnabled = true;
            this.DifficultyBox.Items.AddRange(new object[] {
            "N/A",
            "Easy",
            "Medium",
            "Hard",
            "Logic",
            "Tasukete"});
            this.DifficultyBox.Location = new System.Drawing.Point(80, 64);
            this.DifficultyBox.Name = "DifficultyBox";
            this.DifficultyBox.Size = new System.Drawing.Size(236, 21);
            this.DifficultyBox.TabIndex = 13;
            this.DifficultyBox.Text = "N/A";
            // 
            // DifficultyLabel
            // 
            this.DifficultyLabel.AutoSize = true;
            this.DifficultyLabel.Location = new System.Drawing.Point(11, 67);
            this.DifficultyLabel.Name = "DifficultyLabel";
            this.DifficultyLabel.Size = new System.Drawing.Size(47, 13);
            this.DifficultyLabel.TabIndex = 14;
            this.DifficultyLabel.Text = "Difficulty";
            // 
            // ExportSSPM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 295);
            this.Controls.Add(this.DifficultyLabel);
            this.Controls.Add(this.DifficultyBox);
            this.Controls.Add(this.UseCoverCheckbox);
            this.Controls.Add(this.CoverPathLabel);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.CoverPathBox);
            this.Controls.Add(this.MappersBox);
            this.Controls.Add(this.FinishButton);
            this.Controls.Add(this.MappersSubLabel);
            this.Controls.Add(this.MappersLabel);
            this.Controls.Add(this.SongNameBox);
            this.Controls.Add(this.SongNameLabel);
            this.Controls.Add(this.MapIDBox);
            this.Controls.Add(this.MapIDLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExportSSPM";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ExportSSPM_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MapIDLabel;
        private System.Windows.Forms.TextBox MapIDBox;
        private System.Windows.Forms.Label SongNameLabel;
        private System.Windows.Forms.TextBox SongNameBox;
        private System.Windows.Forms.Label MappersLabel;
        private System.Windows.Forms.Label MappersSubLabel;
        private System.Windows.Forms.Button FinishButton;
        private System.Windows.Forms.TextBox MappersBox;
        private System.Windows.Forms.TextBox CoverPathBox;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Label CoverPathLabel;
        private System.Windows.Forms.CheckBox UseCoverCheckbox;
        private System.Windows.Forms.ComboBox DifficultyBox;
        private System.Windows.Forms.Label DifficultyLabel;
    }
}