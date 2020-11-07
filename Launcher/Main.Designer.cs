namespace Launcher
{
    partial class Main
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
            this.MainContainer = new System.Windows.Forms.Panel();
            this.Changelog = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.NoTasks = new System.Windows.Forms.Label();
            this.VersionSelect = new System.Windows.Forms.ComboBox();
            this.DownloadInfo = new System.Windows.Forms.Label();
            this.LaunchButton = new System.Windows.Forms.Button();
            this.DownloadProgress = new System.Windows.Forms.ProgressBar();
            this.MainContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.Controls.Add(this.Changelog);
            this.MainContainer.Controls.Add(this.panel1);
            this.MainContainer.Location = new System.Drawing.Point(16, 15);
            this.MainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Size = new System.Drawing.Size(747, 369);
            this.MainContainer.TabIndex = 0;
            // 
            // Changelog
            // 
            this.Changelog.Location = new System.Drawing.Point(0, 0);
            this.Changelog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Changelog.MinimumSize = new System.Drawing.Size(27, 25);
            this.Changelog.Name = "Changelog";
            this.Changelog.Size = new System.Drawing.Size(747, 283);
            this.Changelog.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.NoTasks);
            this.panel1.Controls.Add(this.VersionSelect);
            this.panel1.Controls.Add(this.DownloadInfo);
            this.panel1.Controls.Add(this.LaunchButton);
            this.panel1.Controls.Add(this.DownloadProgress);
            this.panel1.Location = new System.Drawing.Point(0, 290);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(747, 79);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(340, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Version:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NoTasks
            // 
            this.NoTasks.BackColor = System.Drawing.Color.Transparent;
            this.NoTasks.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.NoTasks.Location = new System.Drawing.Point(320, 53);
            this.NoTasks.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NoTasks.Name = "NoTasks";
            this.NoTasks.Size = new System.Drawing.Size(80, 20);
            this.NoTasks.TabIndex = 4;
            this.NoTasks.Text = "No Tasks";
            this.NoTasks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VersionSelect
            // 
            this.VersionSelect.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.VersionSelect.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.VersionSelect.FormattingEnabled = true;
            this.VersionSelect.Items.AddRange(new object[] {
            "Latest (1.6)",
            "1.6",
            "1.5.1b",
            "1.5.1a",
            "1.5.1",
            "1.5",
            "1.4.2f",
            "1.4.2"});
            this.VersionSelect.Location = new System.Drawing.Point(400, 17);
            this.VersionSelect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.VersionSelect.Name = "VersionSelect";
            this.VersionSelect.Size = new System.Drawing.Size(199, 24);
            this.VersionSelect.TabIndex = 3;
            this.VersionSelect.Text = "Latest (1.6)";
            // 
            // DownloadInfo
            // 
            this.DownloadInfo.AutoSize = true;
            this.DownloadInfo.BackColor = System.Drawing.Color.Transparent;
            this.DownloadInfo.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.DownloadInfo.Location = new System.Drawing.Point(1, 36);
            this.DownloadInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DownloadInfo.Name = "DownloadInfo";
            this.DownloadInfo.Size = new System.Drawing.Size(208, 17);
            this.DownloadInfo.TabIndex = 2;
            this.DownloadInfo.Text = "Downloading release zip..(69%)";
            this.DownloadInfo.Visible = false;
            // 
            // LaunchButton
            // 
            this.LaunchButton.Location = new System.Drawing.Point(613, 10);
            this.LaunchButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LaunchButton.Name = "LaunchButton";
            this.LaunchButton.Size = new System.Drawing.Size(133, 39);
            this.LaunchButton.TabIndex = 1;
            this.LaunchButton.Text = "Launch";
            this.LaunchButton.UseVisualStyleBackColor = true;
            this.LaunchButton.Click += new System.EventHandler(this.LaunchButton_Click);
            // 
            // DownloadProgress
            // 
            this.DownloadProgress.Location = new System.Drawing.Point(0, 52);
            this.DownloadProgress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DownloadProgress.Name = "DownloadProgress";
            this.DownloadProgress.Size = new System.Drawing.Size(747, 22);
            this.DownloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DownloadProgress.TabIndex = 0;
            this.DownloadProgress.Value = 69;
            this.DownloadProgress.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(779, 395);
            this.Controls.Add(this.MainContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SS Quantum Editor";
            this.Load += new System.EventHandler(this.Main_Load);
            this.MainContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainContainer;
        private System.Windows.Forms.WebBrowser Changelog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox VersionSelect;
        private System.Windows.Forms.Label DownloadInfo;
        private System.Windows.Forms.Button LaunchButton;
        private System.Windows.Forms.ProgressBar DownloadProgress;
        private System.Windows.Forms.Label NoTasks;
        private System.Windows.Forms.Label label1;
    }
}