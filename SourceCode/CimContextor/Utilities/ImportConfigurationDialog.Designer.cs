namespace CimContextor.Utilities
{
    partial class ImportConfigurationDialog
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
            this.selectConfigLabel = new System.Windows.Forms.Label();
            this.selectConfigTB = new System.Windows.Forms.TextBox();
            this.SelectBtn = new System.Windows.Forms.Button();
            this.importBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selectConfigLabel
            // 
            this.selectConfigLabel.AutoSize = true;
            this.selectConfigLabel.Location = new System.Drawing.Point(46, 39);
            this.selectConfigLabel.Name = "selectConfigLabel";
            this.selectConfigLabel.Size = new System.Drawing.Size(178, 20);
            this.selectConfigLabel.TabIndex = 0;
            this.selectConfigLabel.Text = "Select configuration file:";
            // 
            // selectConfigTB
            // 
            this.selectConfigTB.Location = new System.Drawing.Point(47, 72);
            this.selectConfigTB.Name = "selectConfigTB";
            this.selectConfigTB.Size = new System.Drawing.Size(700, 26);
            this.selectConfigTB.TabIndex = 1;
            this.selectConfigTB.TextChanged += new System.EventHandler(this.selectConfigTB_TextChanged);
            // 
            // SelectBtn
            // 
            this.SelectBtn.Location = new System.Drawing.Point(776, 67);
            this.SelectBtn.Name = "SelectBtn";
            this.SelectBtn.Size = new System.Drawing.Size(118, 37);
            this.SelectBtn.TabIndex = 2;
            this.SelectBtn.Text = "Select File";
            this.SelectBtn.UseVisualStyleBackColor = true;
            this.SelectBtn.Click += new System.EventHandler(this.SelectBtn_Click);
            // 
            // importBtn
            // 
            this.importBtn.Enabled = false;
            this.importBtn.Location = new System.Drawing.Point(47, 126);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(118, 37);
            this.importBtn.TabIndex = 3;
            this.importBtn.Text = "Save";
            this.importBtn.UseVisualStyleBackColor = true;
            this.importBtn.Click += new System.EventHandler(this.importBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(776, 126);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(118, 37);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // ImportConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 202);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.importBtn);
            this.Controls.Add(this.SelectBtn);
            this.Controls.Add(this.selectConfigTB);
            this.Controls.Add(this.selectConfigLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ImportConfigurationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Configuration";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label selectConfigLabel;
        private System.Windows.Forms.TextBox selectConfigTB;
        private System.Windows.Forms.Button SelectBtn;
        private System.Windows.Forms.Button importBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}