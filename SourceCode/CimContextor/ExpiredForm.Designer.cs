namespace CimContextor
{
    partial class ExpiredForm
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
            this.ButClose = new System.Windows.Forms.Button();
            this.TBExpired = new System.Windows.Forms.TextBox();
            this.LabExpirationDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButClose
            // 
            this.ButClose.Location = new System.Drawing.Point(176, 170);
            this.ButClose.Name = "ButClose";
            this.ButClose.Size = new System.Drawing.Size(97, 20);
            this.ButClose.TabIndex = 0;
            this.ButClose.Text = "Ok";
            this.ButClose.UseVisualStyleBackColor = true;
            this.ButClose.Click += new System.EventHandler(this.ButClose_Click);
            // 
            // TBExpired
            // 
            this.TBExpired.Font = new System.Drawing.Font("Matura MT Script Capitals", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBExpired.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.TBExpired.Location = new System.Drawing.Point(9, 34);
            this.TBExpired.Multiline = true;
            this.TBExpired.Name = "TBExpired";
            this.TBExpired.ReadOnly = true;
            this.TBExpired.Size = new System.Drawing.Size(438, 118);
            this.TBExpired.TabIndex = 1;
            // 
            // LabExpirationDate
            // 
            this.LabExpirationDate.AutoSize = true;
            this.LabExpirationDate.Location = new System.Drawing.Point(12, 9);
            this.LabExpirationDate.Name = "LabExpirationDate";
            this.LabExpirationDate.Size = new System.Drawing.Size(83, 13);
            this.LabExpirationDate.TabIndex = 2;
            this.LabExpirationDate.Text = "Expiration date :";
            // 
            // ExpiredForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 197);
            this.ControlBox = false;
            this.Controls.Add(this.LabExpirationDate);
            this.Controls.Add(this.TBExpired);
            this.Controls.Add(this.ButClose);
            this.Name = "ExpiredForm";
            this.Text = "ExpiredForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButClose;
        private System.Windows.Forms.TextBox TBExpired;
        private System.Windows.Forms.Label LabExpirationDate;
    }
}