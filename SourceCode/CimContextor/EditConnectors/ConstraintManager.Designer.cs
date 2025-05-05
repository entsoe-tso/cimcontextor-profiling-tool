namespace CimContextor.EditConnectors
{
    partial class  ConstraintManagerForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.TBNotes = new System.Windows.Forms.TextBox();
            this.LNotes = new System.Windows.Forms.Label();
            this.ButCreateConstraint = new System.Windows.Forms.Button();
            this.TBType = new System.Windows.Forms.TextBox();
            this.LType = new System.Windows.Forms.Label();
            this.LName = new System.Windows.Forms.Label();
            this.CBAllowedToAny = new System.Windows.Forms.CheckBox();
            this.TBCreateConstraint = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.CBConstraint = new System.Windows.Forms.ComboBox();
            this.ButDeleteConstraint = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(512, 254);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "                   Manage Constraint";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.TBNotes);
            this.groupBox7.Controls.Add(this.LNotes);
            this.groupBox7.Controls.Add(this.ButCreateConstraint);
            this.groupBox7.Controls.Add(this.TBType);
            this.groupBox7.Controls.Add(this.LType);
            this.groupBox7.Controls.Add(this.LName);
            this.groupBox7.Controls.Add(this.CBAllowedToAny);
            this.groupBox7.Controls.Add(this.TBCreateConstraint);
            this.groupBox7.Location = new System.Drawing.Point(6, 20);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox7.Size = new System.Drawing.Size(332, 224);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Create";
            // 
            // TBNotes
            // 
            this.TBNotes.Location = new System.Drawing.Point(73, 139);
            this.TBNotes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TBNotes.Name = "TBNotes";
            this.TBNotes.Size = new System.Drawing.Size(250, 26);
            this.TBNotes.TabIndex = 7;
            // 
            // LNotes
            // 
            this.LNotes.AutoSize = true;
            this.LNotes.Location = new System.Drawing.Point(9, 145);
            this.LNotes.Name = "LNotes";
            this.LNotes.Size = new System.Drawing.Size(55, 20);
            this.LNotes.TabIndex = 6;
            this.LNotes.Text = "Notes:";
            // 
            // ButCreateConstraint
            // 
            this.ButCreateConstraint.Location = new System.Drawing.Point(69, 176);
            this.ButCreateConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCreateConstraint.Name = "ButCreateConstraint";
            this.ButCreateConstraint.Size = new System.Drawing.Size(150, 38);
            this.ButCreateConstraint.TabIndex = 1;
            this.ButCreateConstraint.Text = "Create constraint";
            this.ButCreateConstraint.UseVisualStyleBackColor = true;
            this.ButCreateConstraint.Click += new System.EventHandler(this.ButCreateConstraint_Click);
            // 
            // TBType
            // 
            this.TBType.Location = new System.Drawing.Point(73, 102);
            this.TBType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TBType.Name = "TBType";
            this.TBType.Size = new System.Drawing.Size(146, 26);
            this.TBType.TabIndex = 5;
            this.TBType.Text = "OCL";
            this.TBType.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // LType
            // 
            this.LType.AutoSize = true;
            this.LType.Location = new System.Drawing.Point(6, 106);
            this.LType.Name = "LType";
            this.LType.Size = new System.Drawing.Size(47, 20);
            this.LType.TabIndex = 4;
            this.LType.Text = "Type:";
            this.LType.Click += new System.EventHandler(this.label2_Click);
            // 
            // LName
            // 
            this.LName.AutoSize = true;
            this.LName.Location = new System.Drawing.Point(6, 69);
            this.LName.Name = "LName";
            this.LName.Size = new System.Drawing.Size(55, 20);
            this.LName.TabIndex = 3;
            this.LName.Text = "Name:";
            // 
            // CBAllowedToAny
            // 
            this.CBAllowedToAny.AutoSize = true;
            this.CBAllowedToAny.Checked = true;
            this.CBAllowedToAny.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBAllowedToAny.Location = new System.Drawing.Point(9, 29);
            this.CBAllowedToAny.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBAllowedToAny.Name = "CBAllowedToAny";
            this.CBAllowedToAny.Size = new System.Drawing.Size(139, 24);
            this.CBAllowedToAny.TabIndex = 2;
            this.CBAllowedToAny.Text = "Allowed to Any";
            this.CBAllowedToAny.UseVisualStyleBackColor = true;
            // 
            // TBCreateConstraint
            // 
            this.TBCreateConstraint.Location = new System.Drawing.Point(73, 65);
            this.TBCreateConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TBCreateConstraint.Name = "TBCreateConstraint";
            this.TBCreateConstraint.Size = new System.Drawing.Size(146, 26);
            this.TBCreateConstraint.TabIndex = 0;
            this.TBCreateConstraint.TextChanged += new System.EventHandler(this.TBCreateConstraint_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.CBConstraint);
            this.groupBox6.Controls.Add(this.ButDeleteConstraint);
            this.groupBox6.Location = new System.Drawing.Point(346, 20);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Size = new System.Drawing.Size(166, 224);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Delete";
            // 
            // CBConstraint
            // 
            this.CBConstraint.FormattingEnabled = true;
            this.CBConstraint.Location = new System.Drawing.Point(17, 65);
            this.CBConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBConstraint.Name = "CBConstraint";
            this.CBConstraint.Size = new System.Drawing.Size(130, 28);
            this.CBConstraint.TabIndex = 4;
            this.CBConstraint.SelectedIndexChanged += new System.EventHandler(this.CBConstraint_SelectedIndexChanged);
            // 
            // ButDeleteConstraint
            // 
            this.ButDeleteConstraint.Location = new System.Drawing.Point(9, 176);
            this.ButDeleteConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButDeleteConstraint.Name = "ButDeleteConstraint";
            this.ButDeleteConstraint.Size = new System.Drawing.Size(147, 38);
            this.ButDeleteConstraint.TabIndex = 3;
            this.ButDeleteConstraint.Text = "Delete Constraint";
            this.ButDeleteConstraint.UseVisualStyleBackColor = true;
            this.ButDeleteConstraint.Click += new System.EventHandler(this.ButDeleteConstraint_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(370, 284);
            this.ButCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(147, 38);
            this.ButCancel.TabIndex = 7;
            this.ButCancel.Text = "Close";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // ConstraintManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 342);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ConstraintManagerForm";
            this.Text = "Manage Constraint";
            this.groupBox1.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButDeleteConstraint;
        private System.Windows.Forms.Button ButCreateConstraint;
        private System.Windows.Forms.TextBox TBCreateConstraint;
        private System.Windows.Forms.CheckBox CBAllowedToAny;
        private System.Windows.Forms.ComboBox CBConstraint;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label LType;
        private System.Windows.Forms.Label LName;
        private System.Windows.Forms.TextBox TBType;
        private System.Windows.Forms.Label LNotes;
        private System.Windows.Forms.TextBox TBNotes;
    }
}