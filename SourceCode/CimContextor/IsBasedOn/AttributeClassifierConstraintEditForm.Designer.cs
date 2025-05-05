namespace CimContextor
{
    partial class AttributeClassifierConstraintEditForm
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
            this.ButCancel = new System.Windows.Forms.Button();
            this.ButSave = new System.Windows.Forms.Button();
            this.LabConstraintName = new System.Windows.Forms.Label();
            this.TBNotes = new System.Windows.Forms.TextBox();
            this.TBComment = new System.Windows.Forms.TextBox();
            this.CB = new System.Windows.Forms.ComboBox();
            this.LabDefinedValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(425, 202);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(82, 27);
            this.ButCancel.TabIndex = 0;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(6, 202);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(82, 27);
            this.ButSave.TabIndex = 1;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // LabConstraintName
            // 
            this.LabConstraintName.AutoSize = true;
            this.LabConstraintName.Location = new System.Drawing.Point(3, 9);
            this.LabConstraintName.Name = "LabConstraintName";
            this.LabConstraintName.Size = new System.Drawing.Size(61, 13);
            this.LabConstraintName.TabIndex = 2;
            this.LabConstraintName.Text = "NeverSeen";
            // 
            // TBNotes
            // 
            this.TBNotes.Location = new System.Drawing.Point(6, 123);
            this.TBNotes.Multiline = true;
            this.TBNotes.Name = "TBNotes";
            this.TBNotes.Size = new System.Drawing.Size(501, 73);
            this.TBNotes.TabIndex = 3;
            // 
            // TBComment
            // 
            this.TBComment.Location = new System.Drawing.Point(6, 29);
            this.TBComment.Multiline = true;
            this.TBComment.Name = "TBComment";
            this.TBComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TBComment.Size = new System.Drawing.Size(500, 65);
            this.TBComment.TabIndex = 4;
            this.TBComment.Text = "No comments";
            // 
            // CB
            // 
            this.CB.FormattingEnabled = true;
            this.CB.Location = new System.Drawing.Point(6, 123);
            this.CB.Name = "CB";
            this.CB.Size = new System.Drawing.Size(501, 21);
            this.CB.TabIndex = 5;
            // 
            // LabDefinedValue
            // 
            this.LabDefinedValue.AutoSize = true;
            this.LabDefinedValue.Location = new System.Drawing.Point(3, 97);
            this.LabDefinedValue.Name = "LabDefinedValue";
            this.LabDefinedValue.Size = new System.Drawing.Size(96, 13);
            this.LabDefinedValue.TabIndex = 6;
            this.LabDefinedValue.Text = "Possible value list :";
            // 
            // AttributeClassifierConstraintEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 233);
            this.Controls.Add(this.LabDefinedValue);
            this.Controls.Add(this.CB);
            this.Controls.Add(this.TBComment);
            this.Controls.Add(this.TBNotes);
            this.Controls.Add(this.LabConstraintName);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.ButCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeClassifierConstraintEditForm";
            this.Text = "AttributeClassifierConstraintEditForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.Label LabConstraintName;
        private System.Windows.Forms.TextBox TBNotes;
        private System.Windows.Forms.TextBox TBComment;
        private System.Windows.Forms.ComboBox CB;
        private System.Windows.Forms.Label LabDefinedValue;
    }
}