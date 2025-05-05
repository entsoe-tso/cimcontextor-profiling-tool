namespace CimContextor
{
    partial class DefForm
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
            this.TBDef = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TBDef
            // 
            this.TBDef.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBDef.HideSelection = false;
            this.TBDef.Location = new System.Drawing.Point(12, 12);
            this.TBDef.Multiline = true;
            this.TBDef.Name = "TBDef";
            this.TBDef.ReadOnly = true;
            this.TBDef.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TBDef.Size = new System.Drawing.Size(643, 242);
            this.TBDef.TabIndex = 5;
            this.TBDef.TextChanged += new System.EventHandler(this.TBDef_TextChanged);
            // 
            // DefForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 266);
            this.Controls.Add(this.TBDef);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DefForm";
            this.Text = "Copy and paste this data if needed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TBDef;

    }
}