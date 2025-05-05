namespace CimContextor
{
    partial class EditAttributeStereotype
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
            this.ButSave = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.CLBStereotype = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(12, 232);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(95, 24);
            this.ButSave.TabIndex = 0;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(186, 232);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(94, 24);
            this.ButCancel.TabIndex = 1;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // CLBStereotype
            // 
            this.CLBStereotype.CheckOnClick = true;
            this.CLBStereotype.FormattingEnabled = true;
            this.CLBStereotype.Location = new System.Drawing.Point(12, 11);
            this.CLBStereotype.Name = "CLBStereotype";
            this.CLBStereotype.Size = new System.Drawing.Size(267, 214);
            this.CLBStereotype.TabIndex = 2;
            // 
            // EditAttributeStereotype
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.Controls.Add(this.CLBStereotype);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButSave);
            this.Name = "EditAttributeStereotype";
            this.Text = "EditAttributeStereotype";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.CheckedListBox CLBStereotype;
    }
}