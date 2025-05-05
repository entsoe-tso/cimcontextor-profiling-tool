namespace CimContextor
{
    partial class GlobalIsBasedOnForm
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
            this.CBQualifier = new System.Windows.Forms.ComboBox();
            this.TBPackageName = new System.Windows.Forms.TextBox();
            this.CBNewPackage = new System.Windows.Forms.CheckBox();
            this.ButGlobalIBO = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CBQualifier
            // 
            this.CBQualifier.FormattingEnabled = true;
            this.CBQualifier.Location = new System.Drawing.Point(174, 33);
            this.CBQualifier.Name = "CBQualifier";
            this.CBQualifier.Size = new System.Drawing.Size(140, 21);
            this.CBQualifier.TabIndex = 0;
            this.CBQualifier.SelectedIndexChanged += new System.EventHandler(this.CBQualifier_SelectedIndexChanged);
            // 
            // TBPackageName
            // 
            this.TBPackageName.Location = new System.Drawing.Point(6, 33);
            this.TBPackageName.Name = "TBPackageName";
            this.TBPackageName.Size = new System.Drawing.Size(162, 20);
            this.TBPackageName.TabIndex = 1;
            // 
            // CBNewPackage
            // 
            this.CBNewPackage.AutoSize = true;
            this.CBNewPackage.Checked = true;
            this.CBNewPackage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBNewPackage.Location = new System.Drawing.Point(6, 10);
            this.CBNewPackage.Name = "CBNewPackage";
            this.CBNewPackage.Size = new System.Drawing.Size(200, 17);
            this.CBNewPackage.TabIndex = 2;
            this.CBNewPackage.Text = "Create IsBasedOn in a new package";
            this.CBNewPackage.UseVisualStyleBackColor = true;
            this.CBNewPackage.CheckedChanged += new System.EventHandler(this.CBNewPackage_CheckedChanged);
            // 
            // ButGlobalIBO
            // 
            this.ButGlobalIBO.Location = new System.Drawing.Point(34, 59);
            this.ButGlobalIBO.Name = "ButGlobalIBO";
            this.ButGlobalIBO.Size = new System.Drawing.Size(112, 24);
            this.ButGlobalIBO.TabIndex = 3;
            this.ButGlobalIBO.Text = "Execute";
            this.ButGlobalIBO.UseVisualStyleBackColor = true;
            this.ButGlobalIBO.Click += new System.EventHandler(this.ButGlobalIBO_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(191, 60);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(110, 24);
            this.ButCancel.TabIndex = 4;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // GlobalIsBasedOnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 89);
            this.ControlBox = false;
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButGlobalIBO);
            this.Controls.Add(this.CBNewPackage);
            this.Controls.Add(this.TBPackageName);
            this.Controls.Add(this.CBQualifier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GlobalIsBasedOnForm";
            this.Text = "GlobalIsBasedOnForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CBQualifier;
        private System.Windows.Forms.TextBox TBPackageName;
        private System.Windows.Forms.CheckBox CBNewPackage;
        private System.Windows.Forms.Button ButGlobalIBO;
        private System.Windows.Forms.Button ButCancel;
    }
}