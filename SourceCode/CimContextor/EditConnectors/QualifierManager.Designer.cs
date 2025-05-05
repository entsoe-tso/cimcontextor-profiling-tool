namespace CimContextor.EditConnectors
{
    partial class   QualifierManagerForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.CBAllowedToAny = new System.Windows.Forms.CheckBox();
            this.TBCreateQualifier = new System.Windows.Forms.TextBox();
            this.ButCreateQualifier = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.CBQualifier = new System.Windows.Forms.ComboBox();
            this.ButDeleteQualifier = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(399, 164);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "                          Manage qualifier";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.CBAllowedToAny);
            this.groupBox7.Controls.Add(this.TBCreateQualifier);
            this.groupBox7.Controls.Add(this.ButCreateQualifier);
            this.groupBox7.Location = new System.Drawing.Point(5, 16);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox7.Size = new System.Drawing.Size(203, 140);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Create";
            // 
            // CBAllowedToAny
            // 
            this.CBAllowedToAny.AutoSize = true;
            this.CBAllowedToAny.Checked = true;
            this.CBAllowedToAny.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBAllowedToAny.Location = new System.Drawing.Point(9, 34);
            this.CBAllowedToAny.Margin = new System.Windows.Forms.Padding(4);
            this.CBAllowedToAny.Name = "CBAllowedToAny";
            this.CBAllowedToAny.Size = new System.Drawing.Size(186, 21);
            this.CBAllowedToAny.TabIndex = 2;
            this.CBAllowedToAny.Text = "Allowed to class and role";
            this.CBAllowedToAny.UseVisualStyleBackColor = true;
            // 
            // TBCreateQualifier
            // 
            this.TBCreateQualifier.Location = new System.Drawing.Point(0, 63);
            this.TBCreateQualifier.Margin = new System.Windows.Forms.Padding(4);
            this.TBCreateQualifier.Name = "TBCreateQualifier";
            this.TBCreateQualifier.Size = new System.Drawing.Size(180, 22);
            this.TBCreateQualifier.TabIndex = 0;
            // 
            // ButCreateQualifier
            // 
            this.ButCreateQualifier.Location = new System.Drawing.Point(22, 102);
            this.ButCreateQualifier.Margin = new System.Windows.Forms.Padding(4);
            this.ButCreateQualifier.Name = "ButCreateQualifier";
            this.ButCreateQualifier.Size = new System.Drawing.Size(133, 30);
            this.ButCreateQualifier.TabIndex = 1;
            this.ButCreateQualifier.Text = "Create qualifier";
            this.ButCreateQualifier.UseVisualStyleBackColor = true;
            this.ButCreateQualifier.Click += new System.EventHandler(this.ButCreateQualifier_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.CBQualifier);
            this.groupBox6.Controls.Add(this.ButDeleteQualifier);
            this.groupBox6.Location = new System.Drawing.Point(236, 16);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(147, 148);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Delete";
            // 
            // CBQualifier
            // 
            this.CBQualifier.FormattingEnabled = true;
            this.CBQualifier.Location = new System.Drawing.Point(8, 63);
            this.CBQualifier.Margin = new System.Windows.Forms.Padding(4);
            this.CBQualifier.Name = "CBQualifier";
            this.CBQualifier.Size = new System.Drawing.Size(131, 24);
            this.CBQualifier.TabIndex = 4;
            // 
            // ButDeleteQualifier
            // 
            this.ButDeleteQualifier.Location = new System.Drawing.Point(12, 102);
            this.ButDeleteQualifier.Margin = new System.Windows.Forms.Padding(4);
            this.ButDeleteQualifier.Name = "ButDeleteQualifier";
            this.ButDeleteQualifier.Size = new System.Drawing.Size(131, 30);
            this.ButDeleteQualifier.TabIndex = 3;
            this.ButDeleteQualifier.Text = "Delete qualifier";
            this.ButDeleteQualifier.UseVisualStyleBackColor = true;
            this.ButDeleteQualifier.Click += new System.EventHandler(this.ButDeleteQualifier_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(150, 185);
            this.ButCancel.Margin = new System.Windows.Forms.Padding(4);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(80, 44);
            this.ButCancel.TabIndex = 7;
            this.ButCancel.Text = "Close";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // QualifierManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 269);
            this.ControlBox = false;
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "QualifierManagerForm";
            this.Text = "Manage Qualifier";
            this.groupBox1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButDeleteQualifier;
        private System.Windows.Forms.Button ButCreateQualifier;
        private System.Windows.Forms.TextBox TBCreateQualifier;
        private System.Windows.Forms.CheckBox CBAllowedToAny;
        private System.Windows.Forms.ComboBox CBQualifier;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}