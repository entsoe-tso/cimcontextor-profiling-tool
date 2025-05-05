namespace CimContextor
{
    partial class AttributeCardinalityForm
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
            this.LabAttributeName = new System.Windows.Forms.Label();
            this.LabLowerBound = new System.Windows.Forms.Label();
            this.LabUpperBound = new System.Windows.Forms.Label();
            this.CBLowerBound = new System.Windows.Forms.ComboBox();
            this.CBUpperBound = new System.Windows.Forms.ComboBox();
            this.ButSave = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabAttributeName
            // 
            this.LabAttributeName.AutoSize = true;
            this.LabAttributeName.Location = new System.Drawing.Point(16, 10);
            this.LabAttributeName.Name = "LabAttributeName";
            this.LabAttributeName.Size = new System.Drawing.Size(74, 13);
            this.LabAttributeName.TabIndex = 0;
            this.LabAttributeName.Text = "AttributName :";
            // 
            // LabLowerBound
            // 
            this.LabLowerBound.AutoSize = true;
            this.LabLowerBound.Location = new System.Drawing.Point(15, 30);
            this.LabLowerBound.Name = "LabLowerBound";
            this.LabLowerBound.Size = new System.Drawing.Size(75, 13);
            this.LabLowerBound.TabIndex = 1;
            this.LabLowerBound.Text = "Lower bound :";
            // 
            // LabUpperBound
            // 
            this.LabUpperBound.AutoSize = true;
            this.LabUpperBound.Location = new System.Drawing.Point(123, 30);
            this.LabUpperBound.Name = "LabUpperBound";
            this.LabUpperBound.Size = new System.Drawing.Size(75, 13);
            this.LabUpperBound.TabIndex = 2;
            this.LabUpperBound.Text = "Upper bound :";
            // 
            // CBLowerBound
            // 
            this.CBLowerBound.FormattingEnabled = true;
            this.CBLowerBound.Location = new System.Drawing.Point(18, 46);
            this.CBLowerBound.Name = "CBLowerBound";
            this.CBLowerBound.Size = new System.Drawing.Size(72, 21);
            this.CBLowerBound.TabIndex = 3;
            // 
            // CBUpperBound
            // 
            this.CBUpperBound.FormattingEnabled = true;
            this.CBUpperBound.Location = new System.Drawing.Point(126, 46);
            this.CBUpperBound.Name = "CBUpperBound";
            this.CBUpperBound.Size = new System.Drawing.Size(72, 21);
            this.CBUpperBound.TabIndex = 4;
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(19, 79);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(85, 22);
            this.ButSave.TabIndex = 5;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(113, 79);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(85, 22);
            this.ButCancel.TabIndex = 6;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // AttributeCardinalityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 143);
            this.ControlBox = false;
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.CBUpperBound);
            this.Controls.Add(this.CBLowerBound);
            this.Controls.Add(this.LabUpperBound);
            this.Controls.Add(this.LabLowerBound);
            this.Controls.Add(this.LabAttributeName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeCardinalityForm";
            this.Text = "Select a new Cardinality";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabAttributeName;
        private System.Windows.Forms.Label LabLowerBound;
        private System.Windows.Forms.Label LabUpperBound;
        private System.Windows.Forms.ComboBox CBLowerBound;
        private System.Windows.Forms.ComboBox CBUpperBound;
        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.Button ButCancel;
    }
}