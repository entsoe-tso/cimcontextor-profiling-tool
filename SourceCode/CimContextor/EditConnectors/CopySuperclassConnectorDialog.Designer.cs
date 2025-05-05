namespace CimContextor.EditConnectors
{
    partial class CopySuperclassConnectorDialog
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
            this.explanationLB = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.createBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // explanationLB
            // 
            this.explanationLB.AutoSize = true;
            this.explanationLB.Location = new System.Drawing.Point(50, 41);
            this.explanationLB.Name = "explanationLB";
            this.explanationLB.Size = new System.Drawing.Size(448, 60);
            this.explanationLB.TabIndex = 0;
            this.explanationLB.Text = "This creates connectors from the selected element to all other \r\n\'BasedOn\' elemen" +
    "ts according to the connectors between the \r\nCIM parent classes.";
            this.explanationLB.Click += new System.EventHandler(this.explanationLB_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(394, 112);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(104, 41);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // createBtn
            // 
            this.createBtn.Location = new System.Drawing.Point(254, 112);
            this.createBtn.Name = "createBtn";
            this.createBtn.Size = new System.Drawing.Size(104, 41);
            this.createBtn.TabIndex = 3;
            this.createBtn.Text = "Create";
            this.createBtn.UseVisualStyleBackColor = true;
            this.createBtn.Click += new System.EventHandler(this.createBtn_Click);
            // 
            // CopySuperclassConnectorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 177);
            this.Controls.Add(this.createBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.explanationLB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CopySuperclassConnectorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copy Superclass Connector Dialog";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label explanationLB;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button createBtn;
    }
}