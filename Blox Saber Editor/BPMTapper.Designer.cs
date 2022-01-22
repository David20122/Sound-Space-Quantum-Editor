namespace Sound_Space_Editor
{
    partial class BPMTapper
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
            this.TapButton = new System.Windows.Forms.Button();
            this.BPM = new System.Windows.Forms.TextBox();
            this.CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TapButton
            // 
            this.TapButton.Location = new System.Drawing.Point(12, 50);
            this.TapButton.Name = "TapButton";
            this.TapButton.Size = new System.Drawing.Size(184, 65);
            this.TapButton.TabIndex = 0;
            this.TapButton.Text = "TAP";
            this.TapButton.UseVisualStyleBackColor = true;
            this.TapButton.Click += new System.EventHandler(this.TapButton_Click);
            // 
            // BPM
            // 
            this.BPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.5F);
            this.BPM.Location = new System.Drawing.Point(12, 12);
            this.BPM.Name = "BPM";
            this.BPM.ReadOnly = true;
            this.BPM.Size = new System.Drawing.Size(184, 32);
            this.BPM.TabIndex = 1;
            this.BPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(109, 168);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(87, 26);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(12, 168);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(87, 26);
            this.OKButton.TabIndex = 3;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 121);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(184, 41);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "RESET";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // BPMTapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 206);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.BPM);
            this.Controls.Add(this.TapButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BPMTapper";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TapButton;
        private System.Windows.Forms.TextBox BPM;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button ResetButton;
    }
}