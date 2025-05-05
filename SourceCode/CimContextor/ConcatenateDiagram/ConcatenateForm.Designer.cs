namespace CimContextor
{
    partial class ConcatenateForm
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
            this.ButConcatenate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TBPackageName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TBDiagramName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButConcatenate
            // 
            this.ButConcatenate.Location = new System.Drawing.Point(77, 84);
            this.ButConcatenate.Name = "ButConcatenate";
            this.ButConcatenate.Size = new System.Drawing.Size(86, 25);
            this.ButConcatenate.TabIndex = 0;
            this.ButConcatenate.Text = "Concatenate";
            this.ButConcatenate.UseVisualStyleBackColor = true;
            this.ButConcatenate.Click += new System.EventHandler(this.ButConcatenate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TBPackageName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 67);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Package";
            // 
            // TBPackageName
            // 
            this.TBPackageName.Location = new System.Drawing.Point(9, 36);
            this.TBPackageName.Name = "TBPackageName";
            this.TBPackageName.Size = new System.Drawing.Size(203, 20);
            this.TBPackageName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the name of the new package :";
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(314, 84);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(86, 24);
            this.ButCancel.TabIndex = 2;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.TBDiagramName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(251, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 66);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Diagram";
            // 
            // TBDiagramName
            // 
            this.TBDiagramName.Location = new System.Drawing.Point(14, 35);
            this.TBDiagramName.Name = "TBDiagramName";
            this.TBDiagramName.Size = new System.Drawing.Size(180, 20);
            this.TBDiagramName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Enter the name of the  new diagram :";
            // 
            // ConcatenateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 114);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButConcatenate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConcatenateForm";
            this.Text = "ConcatenateForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButConcatenate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TBPackageName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TBDiagramName;
        private System.Windows.Forms.Label label2;
    }
}