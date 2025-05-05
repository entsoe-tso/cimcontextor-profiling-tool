namespace CimContextor
{
    partial class EnumClassifierForm
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
            this.LVClassifier = new System.Windows.Forms.ListView();
            this.ColName = new System.Windows.Forms.ColumnHeader();
            this.ColMember = new System.Windows.Forms.ColumnHeader();
            this.ButCancel = new System.Windows.Forms.Button();
            this.ButOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LVClassifier
            // 
            this.LVClassifier.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName,
            this.ColMember});
            this.LVClassifier.GridLines = true;
            this.LVClassifier.Location = new System.Drawing.Point(9, 6);
            this.LVClassifier.Name = "LVClassifier";
            this.LVClassifier.Size = new System.Drawing.Size(431, 123);
            this.LVClassifier.TabIndex = 0;
            this.LVClassifier.UseCompatibleStateImageBehavior = false;
            this.LVClassifier.View = System.Windows.Forms.View.Details;
            // 
            // ColName
            // 
            this.ColName.Text = "Enum name";
            this.ColName.Width = 86;
            // 
            // ColMember
            // 
            this.ColMember.Text = "Member";
            this.ColMember.Width = 332;
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(340, 135);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(98, 23);
            this.ButCancel.TabIndex = 1;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // ButOk
            // 
            this.ButOk.Location = new System.Drawing.Point(9, 135);
            this.ButOk.Name = "ButOk";
            this.ButOk.Size = new System.Drawing.Size(98, 23);
            this.ButOk.TabIndex = 2;
            this.ButOk.Text = "Ok";
            this.ButOk.UseVisualStyleBackColor = true;
            this.ButOk.Click += new System.EventHandler(this.ButOk_Click);
            // 
            // EnumClassifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 161);
            this.Controls.Add(this.ButOk);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.LVClassifier);
            this.Name = "EnumClassifierForm";
            this.Text = "EnumClassifierForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView LVClassifier;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButOk;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ColumnHeader ColMember;
    }
}