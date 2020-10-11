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
            this.panel1 = new System.Windows.Forms.Panel();
            this.Changelog = new System.Windows.Forms.WebBrowser();
            this.DownloadProgress = new System.Windows.Forms.ProgressBar();
            this.LaunchButton = new System.Windows.Forms.Button();
            this.DownloadInfo = new System.Windows.Forms.Label();
            this.VersionSelect = new System.Windows.Forms.ComboBox();
            this.MainContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.Controls.Add(this.Changelog);
            this.MainContainer.Controls.Add(this.panel1);
            this.MainContainer.Location = new System.Drawing.Point(12, 12);
            this.MainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Size = new System.Drawing.Size(560, 300);
            this.MainContainer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.VersionSelect);
            this.panel1.Controls.Add(this.DownloadInfo);
            this.panel1.Controls.Add(this.LaunchButton);
            this.panel1.Controls.Add(this.DownloadProgress);
            this.panel1.Location = new System.Drawing.Point(0, 236);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(560, 64);
            this.panel1.TabIndex = 1;
            // 
            // Changelog
            // 
            this.Changelog.Location = new System.Drawing.Point(0, 0);
            this.Changelog.MinimumSize = new System.Drawing.Size(20, 20);
            this.Changelog.Name = "Changelog";
            this.Changelog.Size = new System.Drawing.Size(560, 230);
            this.Changelog.TabIndex = 2;
            // 
            // DownloadProgress
            // 
            this.DownloadProgress.Location = new System.Drawing.Point(0, 42);
            this.DownloadProgress.Name = "DownloadProgress";
            this.DownloadProgress.Size = new System.Drawing.Size(560, 18);
            this.DownloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DownloadProgress.TabIndex = 0;
            this.DownloadProgress.Value = 69;
            this.DownloadProgress.Visible = false;
            // 
            // LaunchButton
            // 
            this.LaunchButton.Location = new System.Drawing.Point(460, 8);
            this.LaunchButton.Name = "LaunchButton";
            this.LaunchButton.Size = new System.Drawing.Size(100, 32);
            this.LaunchButton.TabIndex = 1;
            this.LaunchButton.Text = "Launch";
            this.LaunchButton.UseVisualStyleBackColor = true;
            // 
            // DownloadInfo
            // 
            this.DownloadInfo.AutoSize = true;
            this.DownloadInfo.BackColor = System.Drawing.Color.Transparent;
            this.DownloadInfo.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.DownloadInfo.Location = new System.Drawing.Point(1, 29);
            this.DownloadInfo.Name = "DownloadInfo";
            this.DownloadInfo.Size = new System.Drawing.Size(154, 13);
            this.DownloadInfo.TabIndex = 2;
            this.DownloadInfo.Text = "Downloading release zip..(69%)";
            this.DownloadInfo.Visible = false;
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
            this.VersionSelect.Location = new System.Drawing.Point(330, 14);
            this.VersionSelect.Name = "VersionSelect";
            this.VersionSelect.Size = new System.Drawing.Size(125, 21);
            this.VersionSelect.TabIndex = 3;
            this.VersionSelect.Text = "Latest (1.6)";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(584, 321);
            this.Controls.Add(this.MainContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main";
            this.Text = "SS Quantum Editor";
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
    }
}