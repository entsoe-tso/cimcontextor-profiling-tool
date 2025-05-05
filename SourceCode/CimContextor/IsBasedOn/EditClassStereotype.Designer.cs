namespace CimContextor
{
    partial class EditClassStereotype
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
            this.CLBStereotype = new System.Windows.Forms.CheckedListBox();
            this.ButSave = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CLBStereotype
            // 
            this.CLBStereotype.CheckOnClick = true;
            this.CLBStereotype.FormattingEnabled = true;
            this.CLBStereotype.Location = new System.Drawing.Point(12, 10);
            this.CLBStereotype.Name = "CLBStereotype";
            this.CLBStereotype.Size = new System.Drawing.Size(275, 214);
            this.CLBStereotype.TabIndex = 0;
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(12, 230);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(92, 24);
            this.ButSave.TabIndex = 1;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(195, 230);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(92, 24);
            this.ButCancel.TabIndex = 2;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // EditClassStereotype
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 260);
            this.ControlBox = false;
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.CLBStereotype);
            this.Name = "EditClassStereotype";
            this.Text = "Select stereotypes";
            this.Load += new System.EventHandler(this.EditClassStereotype_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox CLBStereotype;
        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.Button ButCancel;
    }
}