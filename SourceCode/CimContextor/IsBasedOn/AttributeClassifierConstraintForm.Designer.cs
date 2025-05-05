namespace CimContextor
{
    partial class AttributeClassifierConstraintForm
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
            this.LVExistingConstraint = new System.Windows.Forms.ListView();
            this.ColName = new System.Windows.Forms.ColumnHeader();
            this.ColValue = new System.Windows.Forms.ColumnHeader();
            this.LabAttributeName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ButAddConstraint = new System.Windows.Forms.Button();
            this.CBAvailableConstraint = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ButEditConstraint = new System.Windows.Forms.Button();
            this.CBExistingConstraint = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButDispose = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.TBDefaultValue = new System.Windows.Forms.TextBox();
            this.CBConstantValue = new System.Windows.Forms.CheckBox();
            this.CBStaticValue = new System.Windows.Forms.CheckBox();
            this.ButDeleteConstraint = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // LVExistingConstraint
            // 
            this.LVExistingConstraint.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName,
            this.ColValue});
            this.LVExistingConstraint.GridLines = true;
            this.LVExistingConstraint.Location = new System.Drawing.Point(12, 25);
            this.LVExistingConstraint.Name = "LVExistingConstraint";
            this.LVExistingConstraint.Size = new System.Drawing.Size(254, 176);
            this.LVExistingConstraint.TabIndex = 0;
            this.LVExistingConstraint.UseCompatibleStateImageBehavior = false;
            this.LVExistingConstraint.View = System.Windows.Forms.View.Details;
            // 
            // ColName
            // 
            this.ColName.Text = "Name";
            this.ColName.Width = 134;
            // 
            // ColValue
            // 
            this.ColValue.Text = "Value";
            this.ColValue.Width = 113;
            // 
            // LabAttributeName
            // 
            this.LabAttributeName.AutoSize = true;
            this.LabAttributeName.Location = new System.Drawing.Point(9, 3);
            this.LabAttributeName.Name = "LabAttributeName";
            this.LabAttributeName.Size = new System.Drawing.Size(96, 13);
            this.LabAttributeName.TabIndex = 1;
            this.LabAttributeName.Text = "labelAttributeName";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(280, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 198);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manage constraints";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.ButAddConstraint);
            this.groupBox3.Controls.Add(this.CBAvailableConstraint);
            this.groupBox3.Location = new System.Drawing.Point(10, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(218, 68);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Add a constraint";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Available constraint :";
            // 
            // ButAddConstraint
            // 
            this.ButAddConstraint.Location = new System.Drawing.Point(164, 16);
            this.ButAddConstraint.Name = "ButAddConstraint";
            this.ButAddConstraint.Size = new System.Drawing.Size(43, 45);
            this.ButAddConstraint.TabIndex = 1;
            this.ButAddConstraint.Text = "Ok";
            this.ButAddConstraint.UseVisualStyleBackColor = true;
            this.ButAddConstraint.Click += new System.EventHandler(this.ButAddConstraint_Click);
            // 
            // CBAvailableConstraint
            // 
            this.CBAvailableConstraint.FormattingEnabled = true;
            this.CBAvailableConstraint.Location = new System.Drawing.Point(6, 40);
            this.CBAvailableConstraint.Name = "CBAvailableConstraint";
            this.CBAvailableConstraint.Size = new System.Drawing.Size(153, 21);
            this.CBAvailableConstraint.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ButDeleteConstraint);
            this.groupBox2.Controls.Add(this.ButEditConstraint);
            this.groupBox2.Controls.Add(this.CBExistingConstraint);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(10, 93);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(218, 98);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Edit a constraint";
            // 
            // ButEditConstraint
            // 
            this.ButEditConstraint.Location = new System.Drawing.Point(129, 66);
            this.ButEditConstraint.Name = "ButEditConstraint";
            this.ButEditConstraint.Size = new System.Drawing.Size(83, 22);
            this.ButEditConstraint.TabIndex = 2;
            this.ButEditConstraint.Text = "Edit";
            this.ButEditConstraint.UseVisualStyleBackColor = true;
            this.ButEditConstraint.Click += new System.EventHandler(this.ButEditConstraint_Click);
            // 
            // CBExistingConstraint
            // 
            this.CBExistingConstraint.FormattingEnabled = true;
            this.CBExistingConstraint.Location = new System.Drawing.Point(6, 39);
            this.CBExistingConstraint.Name = "CBExistingConstraint";
            this.CBExistingConstraint.Size = new System.Drawing.Size(201, 21);
            this.CBExistingConstraint.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Existing constraint :";
            // 
            // ButDispose
            // 
            this.ButDispose.Location = new System.Drawing.Point(532, 152);
            this.ButDispose.Name = "ButDispose";
            this.ButDispose.Size = new System.Drawing.Size(95, 24);
            this.ButDispose.TabIndex = 3;
            this.ButDispose.Text = "Ok";
            this.ButDispose.UseVisualStyleBackColor = true;
            this.ButDispose.Click += new System.EventHandler(this.ButDispose_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.TBDefaultValue);
            this.groupBox4.Controls.Add(this.CBConstantValue);
            this.groupBox4.Controls.Add(this.CBStaticValue);
            this.groupBox4.Location = new System.Drawing.Point(524, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(115, 143);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Attribute details";
            // 
            // TBDefaultValue
            // 
            this.TBDefaultValue.Location = new System.Drawing.Point(8, 73);
            this.TBDefaultValue.Name = "TBDefaultValue";
            this.TBDefaultValue.Size = new System.Drawing.Size(95, 20);
            this.TBDefaultValue.TabIndex = 2;
            // 
            // CBConstantValue
            // 
            this.CBConstantValue.AutoSize = true;
            this.CBConstantValue.Location = new System.Drawing.Point(9, 44);
            this.CBConstantValue.Name = "CBConstantValue";
            this.CBConstantValue.Size = new System.Drawing.Size(77, 17);
            this.CBConstantValue.TabIndex = 1;
            this.CBConstantValue.Text = "is constant";
            this.CBConstantValue.UseVisualStyleBackColor = true;
            // 
            // CBStaticValue
            // 
            this.CBStaticValue.AutoSize = true;
            this.CBStaticValue.Location = new System.Drawing.Point(9, 19);
            this.CBStaticValue.Name = "CBStaticValue";
            this.CBStaticValue.Size = new System.Drawing.Size(61, 17);
            this.CBStaticValue.TabIndex = 0;
            this.CBStaticValue.Text = "is static";
            this.CBStaticValue.UseVisualStyleBackColor = true;
            // 
            // ButDeleteConstraint
            // 
            this.ButDeleteConstraint.Location = new System.Drawing.Point(6, 66);
            this.ButDeleteConstraint.Name = "ButDeleteConstraint";
            this.ButDeleteConstraint.Size = new System.Drawing.Size(83, 22);
            this.ButDeleteConstraint.TabIndex = 2;
            this.ButDeleteConstraint.Text = "Delete";
            this.ButDeleteConstraint.UseVisualStyleBackColor = true;
            this.ButDeleteConstraint.Click += new System.EventHandler(this.CBDeleteConstraint_Click);
            // 
            // AttributeClassifierConstraintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 207);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.ButDispose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LabAttributeName);
            this.Controls.Add(this.LVExistingConstraint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeClassifierConstraintForm";
            this.Text = "AttributeClassifierConstraintForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LVExistingConstraint;
        private System.Windows.Forms.Label LabAttributeName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButAddConstraint;
        private System.Windows.Forms.ComboBox CBAvailableConstraint;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ButEditConstraint;
        private System.Windows.Forms.ComboBox CBExistingConstraint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ColumnHeader ColValue;
        private System.Windows.Forms.Button ButDispose;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox CBStaticValue;
        private System.Windows.Forms.CheckBox CBConstantValue;
        private System.Windows.Forms.TextBox TBDefaultValue;
        private System.Windows.Forms.Button ButDeleteConstraint;
    }
}