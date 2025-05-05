namespace CimContextor
{
    partial class ClassConstraintForm
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
            this.LVConstraint = new System.Windows.Forms.ListView();
            this.ColConstName = new System.Windows.Forms.ColumnHeader();
            this.ColConstNote = new System.Windows.Forms.ColumnHeader();
            this.LabAttributName = new System.Windows.Forms.Label();
            this.ButShowDef = new System.Windows.Forms.Button();
            this.TBEditedNote = new System.Windows.Forms.TextBox();
            this.ButSaveEditedNote = new System.Windows.Forms.Button();
            this.ButDone = new System.Windows.Forms.Button();
            this.RBScratch = new System.Windows.Forms.RadioButton();
            this.RBTradConstraint = new System.Windows.Forms.RadioButton();
            this.RBCancel = new System.Windows.Forms.RadioButton();
            this.CBTradConstraint = new System.Windows.Forms.ComboBox();
            this.LabConstraintName = new System.Windows.Forms.Label();
            this.TBConstraintName = new System.Windows.Forms.TextBox();
            this.CBConstraintType = new System.Windows.Forms.ComboBox();
            this.ButDeleteConst = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LVConstraint
            // 
            this.LVConstraint.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColConstName,
            this.ColConstNote});
            this.LVConstraint.FullRowSelect = true;
            this.LVConstraint.GridLines = true;
            this.LVConstraint.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVConstraint.Location = new System.Drawing.Point(9, 23);
            this.LVConstraint.MultiSelect = false;
            this.LVConstraint.Name = "LVConstraint";
            this.LVConstraint.Size = new System.Drawing.Size(288, 104);
            this.LVConstraint.TabIndex = 0;
            this.LVConstraint.UseCompatibleStateImageBehavior = false;
            this.LVConstraint.View = System.Windows.Forms.View.Details;
            this.LVConstraint.SelectedIndexChanged += new System.EventHandler(this.LVConstraint_SelectedIndexChanged);
            // 
            // ColConstName
            // 
            this.ColConstName.Text = "Constraint name";
            this.ColConstName.Width = 107;
            // 
            // ColConstNote
            // 
            this.ColConstNote.Text = "Constraint code";
            this.ColConstNote.Width = 176;
            // 
            // LabAttributName
            // 
            this.LabAttributName.AutoSize = true;
            this.LabAttributName.Location = new System.Drawing.Point(7, 6);
            this.LabAttributName.Name = "LabAttributName";
            this.LabAttributName.Size = new System.Drawing.Size(67, 13);
            this.LabAttributName.TabIndex = 1;
            this.LabAttributName.Text = "Class name :";
            // 
            // ButShowDef
            // 
            this.ButShowDef.Location = new System.Drawing.Point(106, 133);
            this.ButShowDef.Name = "ButShowDef";
            this.ButShowDef.Size = new System.Drawing.Size(89, 24);
            this.ButShowDef.TabIndex = 3;
            this.ButShowDef.Text = "Show Def";
            this.ButShowDef.UseVisualStyleBackColor = true;
            this.ButShowDef.Click += new System.EventHandler(this.ButShowDef_Click);
            // 
            // TBEditedNote
            // 
            this.TBEditedNote.Location = new System.Drawing.Point(303, 49);
            this.TBEditedNote.Multiline = true;
            this.TBEditedNote.Name = "TBEditedNote";
            this.TBEditedNote.Size = new System.Drawing.Size(201, 78);
            this.TBEditedNote.TabIndex = 4;
            this.TBEditedNote.Visible = false;
            // 
            // ButSaveEditedNote
            // 
            this.ButSaveEditedNote.Location = new System.Drawing.Point(220, 133);
            this.ButSaveEditedNote.Name = "ButSaveEditedNote";
            this.ButSaveEditedNote.Size = new System.Drawing.Size(106, 24);
            this.ButSaveEditedNote.TabIndex = 5;
            this.ButSaveEditedNote.Text = "Save constraint";
            this.ButSaveEditedNote.UseVisualStyleBackColor = true;
            this.ButSaveEditedNote.Visible = false;
            this.ButSaveEditedNote.Click += new System.EventHandler(this.ButSaveEditedNote_Click);
            // 
            // ButDone
            // 
            this.ButDone.Location = new System.Drawing.Point(12, 133);
            this.ButDone.Name = "ButDone";
            this.ButDone.Size = new System.Drawing.Size(79, 24);
            this.ButDone.TabIndex = 6;
            this.ButDone.Text = "Done";
            this.ButDone.UseVisualStyleBackColor = true;
            this.ButDone.Click += new System.EventHandler(this.ButDone_Click);
            // 
            // RBScratch
            // 
            this.RBScratch.AutoSize = true;
            this.RBScratch.Location = new System.Drawing.Point(350, 36);
            this.RBScratch.Name = "RBScratch";
            this.RBScratch.Size = new System.Drawing.Size(157, 17);
            this.RBScratch.TabIndex = 7;
            this.RBScratch.TabStop = true;
            this.RBScratch.Text = "New constraint from scratch";
            this.RBScratch.UseVisualStyleBackColor = true;
            this.RBScratch.Visible = false;
            this.RBScratch.CheckedChanged += new System.EventHandler(this.RBScratch_CheckedChanged);
            // 
            // RBTradConstraint
            // 
            this.RBTradConstraint.AutoSize = true;
            this.RBTradConstraint.Location = new System.Drawing.Point(350, 59);
            this.RBTradConstraint.Name = "RBTradConstraint";
            this.RBTradConstraint.Size = new System.Drawing.Size(146, 17);
            this.RBTradConstraint.TabIndex = 8;
            this.RBTradConstraint.TabStop = true;
            this.RBTradConstraint.Text = "Use predefined constraint";
            this.RBTradConstraint.UseVisualStyleBackColor = true;
            this.RBTradConstraint.Visible = false;
            this.RBTradConstraint.CheckedChanged += new System.EventHandler(this.RBTradConstraint_CheckedChanged);
            // 
            // RBCancel
            // 
            this.RBCancel.AutoSize = true;
            this.RBCancel.Location = new System.Drawing.Point(350, 82);
            this.RBCancel.Name = "RBCancel";
            this.RBCancel.Size = new System.Drawing.Size(58, 17);
            this.RBCancel.TabIndex = 9;
            this.RBCancel.TabStop = true;
            this.RBCancel.Text = "Cancel";
            this.RBCancel.UseVisualStyleBackColor = true;
            this.RBCancel.Visible = false;
            this.RBCancel.CheckedChanged += new System.EventHandler(this.RBCancel_CheckedChanged);
            // 
            // CBTradConstraint
            // 
            this.CBTradConstraint.FormattingEnabled = true;
            this.CBTradConstraint.Location = new System.Drawing.Point(303, 22);
            this.CBTradConstraint.Name = "CBTradConstraint";
            this.CBTradConstraint.Size = new System.Drawing.Size(200, 21);
            this.CBTradConstraint.TabIndex = 10;
            this.CBTradConstraint.Visible = false;
            this.CBTradConstraint.SelectedIndexChanged += new System.EventHandler(this.CBTradConstraint_SelectedIndexChanged);
            // 
            // LabConstraintName
            // 
            this.LabConstraintName.AutoSize = true;
            this.LabConstraintName.Location = new System.Drawing.Point(305, 23);
            this.LabConstraintName.Name = "LabConstraintName";
            this.LabConstraintName.Size = new System.Drawing.Size(41, 13);
            this.LabConstraintName.TabIndex = 11;
            this.LabConstraintName.Text = "Name :";
            this.LabConstraintName.Visible = false;
            // 
            // TBConstraintName
            // 
            this.TBConstraintName.Location = new System.Drawing.Point(352, 21);
            this.TBConstraintName.Name = "TBConstraintName";
            this.TBConstraintName.Size = new System.Drawing.Size(150, 20);
            this.TBConstraintName.TabIndex = 12;
            this.TBConstraintName.Visible = false;
            // 
            // CBConstraintType
            // 
            this.CBConstraintType.FormattingEnabled = true;
            this.CBConstraintType.Location = new System.Drawing.Point(390, 135);
            this.CBConstraintType.Name = "CBConstraintType";
            this.CBConstraintType.Size = new System.Drawing.Size(111, 21);
            this.CBConstraintType.TabIndex = 13;
            this.CBConstraintType.Visible = false;
            // 
            // ButDeleteConst
            // 
            this.ButDeleteConst.Location = new System.Drawing.Point(356, 133);
            this.ButDeleteConst.Name = "ButDeleteConst";
            this.ButDeleteConst.Size = new System.Drawing.Size(113, 23);
            this.ButDeleteConst.TabIndex = 14;
            this.ButDeleteConst.Text = "Delete";
            this.ButDeleteConst.UseVisualStyleBackColor = true;
            this.ButDeleteConst.Visible = false;
            this.ButDeleteConst.Click += new System.EventHandler(this.ButDeleteConst_Click);
            // 
            // ClassConstraintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 167);
            this.Controls.Add(this.ButDeleteConst);
            this.Controls.Add(this.CBConstraintType);
            this.Controls.Add(this.TBConstraintName);
            this.Controls.Add(this.LabConstraintName);
            this.Controls.Add(this.CBTradConstraint);
            this.Controls.Add(this.RBCancel);
            this.Controls.Add(this.RBTradConstraint);
            this.Controls.Add(this.RBScratch);
            this.Controls.Add(this.ButDone);
            this.Controls.Add(this.ButSaveEditedNote);
            this.Controls.Add(this.TBEditedNote);
            this.Controls.Add(this.ButShowDef);
            this.Controls.Add(this.LabAttributName);
            this.Controls.Add(this.LVConstraint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ClassConstraintForm";
            this.Text = "Select a constraint";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LVConstraint;
        private System.Windows.Forms.ColumnHeader ColConstName;
        private System.Windows.Forms.Label LabAttributName;
        private System.Windows.Forms.ColumnHeader ColConstNote;
        private System.Windows.Forms.Button ButShowDef;
        private System.Windows.Forms.TextBox TBEditedNote;
        private System.Windows.Forms.Button ButSaveEditedNote;
        private System.Windows.Forms.Button ButDone;
        private System.Windows.Forms.RadioButton RBScratch;
        private System.Windows.Forms.RadioButton RBTradConstraint;
        private System.Windows.Forms.RadioButton RBCancel;
        private System.Windows.Forms.ComboBox CBTradConstraint;
        private System.Windows.Forms.Label LabConstraintName;
        private System.Windows.Forms.TextBox TBConstraintName;
        private System.Windows.Forms.ComboBox CBConstraintType;
        private System.Windows.Forms.Button ButDeleteConst;
    }
}