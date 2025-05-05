namespace CimContextor.Integrity_Checking
{
    partial class CheckResultsDisplay
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
            this.saveCSVBn = new System.Windows.Forms.Button();
            this.closeBn = new System.Windows.Forms.Button();
            this.resultsTB = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // saveCSVBn
            // 
            this.saveCSVBn.Enabled = false;
            this.saveCSVBn.Location = new System.Drawing.Point(51, 470);
            this.saveCSVBn.Name = "saveCSVBn";
            this.saveCSVBn.Size = new System.Drawing.Size(180, 48);
            this.saveCSVBn.TabIndex = 1;
            this.saveCSVBn.Text = "Save to CSV File";
            this.saveCSVBn.UseVisualStyleBackColor = true;
            this.saveCSVBn.Click += new System.EventHandler(this.SaveToCSV);
            // 
            // closeBn
            // 
            this.closeBn.Location = new System.Drawing.Point(546, 470);
            this.closeBn.Name = "closeBn";
            this.closeBn.Size = new System.Drawing.Size(180, 48);
            this.closeBn.TabIndex = 2;
            this.closeBn.Text = "Close";
            this.closeBn.UseVisualStyleBackColor = true;
            this.closeBn.Click += new System.EventHandler(this.CloseDisplay);
            // 
            // resultsTB
            // 
            this.resultsTB.Location = new System.Drawing.Point(51, 39);
            this.resultsTB.Name = "resultsTB";
            this.resultsTB.ReadOnly = true;
            this.resultsTB.Size = new System.Drawing.Size(675, 395);
            this.resultsTB.TabIndex = 3;
            this.resultsTB.Text = "";
            this.resultsTB.WordWrap = false;
            // 
            // CheckResultsDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 544);
            this.Controls.Add(this.resultsTB);
            this.Controls.Add(this.closeBn);
            this.Controls.Add(this.saveCSVBn);
            this.Name = "CheckResultsDisplay";
            this.Text = "CheckResultsDisplay";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button saveCSVBn;
        private System.Windows.Forms.Button closeBn;
        private System.Windows.Forms.RichTextBox resultsTB;
    }
}