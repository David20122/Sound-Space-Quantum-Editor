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
            this.ResetButton = new System.Windows.Forms.Button();
            this.DecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.DecimalPlacesLabel = new System.Windows.Forms.Label();
            this.BPMDecimals = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DecimalPlaces)).BeginInit();
            this.SuspendLayout();
            // 
            // TapButton
            // 
            this.TapButton.Location = new System.Drawing.Point(12, 103);
            this.TapButton.Name = "TapButton";
            this.TapButton.Size = new System.Drawing.Size(184, 65);
            this.TapButton.TabIndex = 0;
            this.TapButton.TabStop = false;
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
            this.BPM.TabStop = false;
            this.BPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(12, 174);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(184, 41);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.TabStop = false;
            this.ResetButton.Text = "RESET";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // DecimalPlaces
            // 
            this.DecimalPlaces.Location = new System.Drawing.Point(109, 51);
            this.DecimalPlaces.Name = "DecimalPlaces";
            this.DecimalPlaces.Size = new System.Drawing.Size(87, 20);
            this.DecimalPlaces.TabIndex = 5;
            this.DecimalPlaces.TabStop = false;
            this.DecimalPlaces.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // DecimalPlacesLabel
            // 
            this.DecimalPlacesLabel.AutoSize = true;
            this.DecimalPlacesLabel.Location = new System.Drawing.Point(12, 53);
            this.DecimalPlacesLabel.Name = "DecimalPlacesLabel";
            this.DecimalPlacesLabel.Size = new System.Drawing.Size(80, 13);
            this.DecimalPlacesLabel.TabIndex = 6;
            this.DecimalPlacesLabel.Text = "Decimal Places";
            // 
            // BPMDecimals
            // 
            this.BPMDecimals.Location = new System.Drawing.Point(12, 77);
            this.BPMDecimals.Name = "BPMDecimals";
            this.BPMDecimals.ReadOnly = true;
            this.BPMDecimals.Size = new System.Drawing.Size(184, 20);
            this.BPMDecimals.TabIndex = 7;
            this.BPMDecimals.TabStop = false;
            this.BPMDecimals.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BPMTapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 227);
            this.Controls.Add(this.BPMDecimals);
            this.Controls.Add(this.DecimalPlacesLabel);
            this.Controls.Add(this.DecimalPlaces);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.BPM);
            this.Controls.Add(this.TapButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "BPMTapper";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BPMTapper_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.DecimalPlaces)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TapButton;
        private System.Windows.Forms.TextBox BPM;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.NumericUpDown DecimalPlaces;
        private System.Windows.Forms.Label DecimalPlacesLabel;
        private System.Windows.Forms.TextBox BPMDecimals;
    }
}