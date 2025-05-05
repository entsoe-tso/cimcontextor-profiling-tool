namespace CimContextor.EditConnectors
{
    partial class EditSubdivideForOptimConnectorForm
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
            this.LabClientRole = new System.Windows.Forms.Label();
            this.LabSupplierRole = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LSContainment = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CBByRefSource = new System.Windows.Forms.CheckBox();
            this.CBAgregateSource = new System.Windows.Forms.CheckBox();
            this.TBClientRole = new System.Windows.Forms.TextBox();
            this.CBClientQualifier = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.LTContainment = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CBAgregateTarget = new System.Windows.Forms.CheckBox();
            this.CBByRefTarget = new System.Windows.Forms.CheckBox();
            this.TBSupplierRole = new System.Windows.Forms.TextBox();
            this.CBSupplierQualifier = new System.Windows.Forms.ComboBox();
            this.AddSubdividedConnector = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CBUBSupplierCardinality = new System.Windows.Forms.ComboBox();
            this.CBUBClientCardinality = new System.Windows.Forms.ComboBox();
            this.CBLBClientCardinality = new System.Windows.Forms.ComboBox();
            this.CBLBSupplierCardinality = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.CBCopyNotes = new System.Windows.Forms.CheckBox();
            this.CBCopyTagValues = new System.Windows.Forms.CheckBox();
            this.CBCopyStereotype = new System.Windows.Forms.CheckBox();
            this.CBStereotype = new System.Windows.Forms.ComboBox();
            this.GB = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.GB.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabClientRole
            // 
            this.LabClientRole.AutoSize = true;
            this.LabClientRole.Location = new System.Drawing.Point(9, 30);
            this.LabClientRole.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabClientRole.Name = "LabClientRole";
            this.LabClientRole.Size = new System.Drawing.Size(105, 20);
            this.LabClientRole.TabIndex = 1;
            this.LabClientRole.Text = "Parent\'s role :";
            // 
            // LabSupplierRole
            // 
            this.LabSupplierRole.AutoSize = true;
            this.LabSupplierRole.Location = new System.Drawing.Point(9, 30);
            this.LabSupplierRole.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabSupplierRole.Name = "LabSupplierRole";
            this.LabSupplierRole.Size = new System.Drawing.Size(105, 20);
            this.LabSupplierRole.TabIndex = 2;
            this.LabSupplierRole.Text = "Parent\'s role :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.LSContainment);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.CBByRefSource);
            this.groupBox2.Controls.Add(this.CBAgregateSource);
            this.groupBox2.Controls.Add(this.TBClientRole);
            this.groupBox2.Controls.Add(this.CBClientQualifier);
            this.groupBox2.Controls.Add(this.LabClientRole);
            this.groupBox2.Location = new System.Drawing.Point(18, 14);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(234, 261);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Class role";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // LSContainment
            // 
            this.LSContainment.AutoSize = true;
            this.LSContainment.Location = new System.Drawing.Point(65, 190);
            this.LSContainment.Name = "LSContainment";
            this.LSContainment.Size = new System.Drawing.Size(100, 20);
            this.LSContainment.TabIndex = 6;
            this.LSContainment.Text = "Containment";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "class role";
            // 
            // CBByRefSource
            // 
            this.CBByRefSource.AutoSize = true;
            this.CBByRefSource.Location = new System.Drawing.Point(14, 225);
            this.CBByRefSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBByRefSource.Name = "CBByRefSource";
            this.CBByRefSource.Size = new System.Drawing.Size(169, 24);
            this.CBByRefSource.TabIndex = 3;
            this.CBByRefSource.Text = "Containment by ref";
            this.CBByRefSource.UseVisualStyleBackColor = true;
            this.CBByRefSource.CheckedChanged += new System.EventHandler(this.CBByRefSource_CheckedChanged);
            // 
            // CBAgregateSource
            // 
            this.CBAgregateSource.AutoSize = true;
            this.CBAgregateSource.Location = new System.Drawing.Point(12, 189);
            this.CBAgregateSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBAgregateSource.Name = "CBAgregateSource";
            this.CBAgregateSource.Size = new System.Drawing.Size(57, 24);
            this.CBAgregateSource.TabIndex = 2;
            this.CBAgregateSource.Text = "AS";
            this.CBAgregateSource.UseVisualStyleBackColor = true;
            this.CBAgregateSource.CheckedChanged += new System.EventHandler(this.CBAgregateSource_CheckedChanged);
            // 
            // TBClientRole
            // 
            this.TBClientRole.Location = new System.Drawing.Point(10, 138);
            this.TBClientRole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TBClientRole.Name = "TBClientRole";
            this.TBClientRole.Size = new System.Drawing.Size(214, 26);
            this.TBClientRole.TabIndex = 1;
            this.TBClientRole.TabStop = false;
            // 
            // CBClientQualifier
            // 
            this.CBClientQualifier.FormattingEnabled = true;
            this.CBClientQualifier.Location = new System.Drawing.Point(9, 86);
            this.CBClientQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBClientQualifier.Name = "CBClientQualifier";
            this.CBClientQualifier.Size = new System.Drawing.Size(214, 28);
            this.CBClientQualifier.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LTContainment);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.CBAgregateTarget);
            this.groupBox3.Controls.Add(this.CBByRefTarget);
            this.groupBox3.Controls.Add(this.TBSupplierRole);
            this.groupBox3.Controls.Add(this.CBSupplierQualifier);
            this.groupBox3.Controls.Add(this.LabSupplierRole);
            this.groupBox3.Location = new System.Drawing.Point(261, 14);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(233, 261);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Other end role";
            // 
            // LTContainment
            // 
            this.LTContainment.AutoSize = true;
            this.LTContainment.Location = new System.Drawing.Point(65, 190);
            this.LTContainment.Name = "LTContainment";
            this.LTContainment.Size = new System.Drawing.Size(100, 20);
            this.LTContainment.TabIndex = 7;
            this.LTContainment.Text = "Containment";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Other class role";
            // 
            // CBAgregateTarget
            // 
            this.CBAgregateTarget.AutoSize = true;
            this.CBAgregateTarget.Location = new System.Drawing.Point(12, 189);
            this.CBAgregateTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBAgregateTarget.Name = "CBAgregateTarget";
            this.CBAgregateTarget.Size = new System.Drawing.Size(55, 24);
            this.CBAgregateTarget.TabIndex = 4;
            this.CBAgregateTarget.Text = "AT";
            this.CBAgregateTarget.UseVisualStyleBackColor = true;
            this.CBAgregateTarget.CheckedChanged += new System.EventHandler(this.CBAgregateTarget_CheckedChanged);
            // 
            // CBByRefTarget
            // 
            this.CBByRefTarget.AutoSize = true;
            this.CBByRefTarget.Location = new System.Drawing.Point(12, 225);
            this.CBByRefTarget.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBByRefTarget.Name = "CBByRefTarget";
            this.CBByRefTarget.Size = new System.Drawing.Size(169, 24);
            this.CBByRefTarget.TabIndex = 3;
            this.CBByRefTarget.Text = "Containment by ref";
            this.CBByRefTarget.UseVisualStyleBackColor = true;
            this.CBByRefTarget.CheckedChanged += new System.EventHandler(this.CBByRefTarget_CheckedChanged);
            // 
            // TBSupplierRole
            // 
            this.TBSupplierRole.Location = new System.Drawing.Point(9, 138);
            this.TBSupplierRole.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TBSupplierRole.Name = "TBSupplierRole";
            this.TBSupplierRole.Size = new System.Drawing.Size(212, 26);
            this.TBSupplierRole.TabIndex = 1;
            // 
            // CBSupplierQualifier
            // 
            this.CBSupplierQualifier.FormattingEnabled = true;
            this.CBSupplierQualifier.Location = new System.Drawing.Point(9, 86);
            this.CBSupplierQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBSupplierQualifier.Name = "CBSupplierQualifier";
            this.CBSupplierQualifier.Size = new System.Drawing.Size(212, 28);
            this.CBSupplierQualifier.TabIndex = 0;
            // 
            // AddSubdividedConnector
            // 
            this.AddSubdividedConnector.Location = new System.Drawing.Point(30, 458);
            this.AddSubdividedConnector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddSubdividedConnector.Name = "AddSubdividedConnector";
            this.AddSubdividedConnector.Size = new System.Drawing.Size(198, 55);
            this.AddSubdividedConnector.TabIndex = 6;
            this.AddSubdividedConnector.Text = "Save";
            this.AddSubdividedConnector.UseVisualStyleBackColor = true;
            this.AddSubdividedConnector.Click += new System.EventHandler(this.AddSubdividedConnector_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(553, 458);
            this.ButCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(198, 55);
            this.ButCancel.TabIndex = 7;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.CBUBSupplierCardinality);
            this.groupBox4.Controls.Add(this.CBUBClientCardinality);
            this.groupBox4.Controls.Add(this.CBLBClientCardinality);
            this.groupBox4.Controls.Add(this.CBLBSupplierCardinality);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(18, 299);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(479, 91);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Cardinality";
            // 
            // CBUBSupplierCardinality
            // 
            this.CBUBSupplierCardinality.FormattingEnabled = true;
            this.CBUBSupplierCardinality.Location = new System.Drawing.Point(341, 51);
            this.CBUBSupplierCardinality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBUBSupplierCardinality.Name = "CBUBSupplierCardinality";
            this.CBUBSupplierCardinality.Size = new System.Drawing.Size(90, 28);
            this.CBUBSupplierCardinality.TabIndex = 6;
            // 
            // CBUBClientCardinality
            // 
            this.CBUBClientCardinality.FormattingEnabled = true;
            this.CBUBClientCardinality.Location = new System.Drawing.Point(111, 49);
            this.CBUBClientCardinality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBUBClientCardinality.Name = "CBUBClientCardinality";
            this.CBUBClientCardinality.Size = new System.Drawing.Size(90, 28);
            this.CBUBClientCardinality.TabIndex = 5;
            // 
            // CBLBClientCardinality
            // 
            this.CBLBClientCardinality.FormattingEnabled = true;
            this.CBLBClientCardinality.Location = new System.Drawing.Point(14, 49);
            this.CBLBClientCardinality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBLBClientCardinality.Name = "CBLBClientCardinality";
            this.CBLBClientCardinality.Size = new System.Drawing.Size(90, 28);
            this.CBLBClientCardinality.TabIndex = 4;
            // 
            // CBLBSupplierCardinality
            // 
            this.CBLBSupplierCardinality.FormattingEnabled = true;
            this.CBLBSupplierCardinality.Location = new System.Drawing.Point(231, 51);
            this.CBLBSupplierCardinality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBLBSupplierCardinality.Name = "CBLBSupplierCardinality";
            this.CBLBSupplierCardinality.Size = new System.Drawing.Size(90, 28);
            this.CBLBSupplierCardinality.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Selected class cardinality :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Other end cardinality :";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.CBCopyNotes);
            this.groupBox5.Controls.Add(this.CBCopyTagValues);
            this.groupBox5.Location = new System.Drawing.Point(503, 14);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox5.Size = new System.Drawing.Size(250, 99);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Options";
            // 
            // CBCopyNotes
            // 
            this.CBCopyNotes.AutoSize = true;
            this.CBCopyNotes.Checked = true;
            this.CBCopyNotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyNotes.Location = new System.Drawing.Point(9, 60);
            this.CBCopyNotes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyNotes.Name = "CBCopyNotes";
            this.CBCopyNotes.Size = new System.Drawing.Size(176, 24);
            this.CBCopyNotes.TabIndex = 1;
            this.CBCopyNotes.Text = "Copy parent\'s notes";
            this.CBCopyNotes.UseVisualStyleBackColor = true;
            // 
            // CBCopyTagValues
            // 
            this.CBCopyTagValues.AutoSize = true;
            this.CBCopyTagValues.Checked = true;
            this.CBCopyTagValues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyTagValues.Location = new System.Drawing.Point(9, 25);
            this.CBCopyTagValues.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyTagValues.Name = "CBCopyTagValues";
            this.CBCopyTagValues.Size = new System.Drawing.Size(239, 24);
            this.CBCopyTagValues.TabIndex = 0;
            this.CBCopyTagValues.Text = "Copy parent\'s TaggedValues";
            this.CBCopyTagValues.UseVisualStyleBackColor = true;
            // 
            // CBCopyStereotype
            // 
            this.CBCopyStereotype.AutoSize = true;
            this.CBCopyStereotype.Checked = true;
            this.CBCopyStereotype.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyStereotype.Location = new System.Drawing.Point(9, 25);
            this.CBCopyStereotype.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyStereotype.Name = "CBCopyStereotype";
            this.CBCopyStereotype.Size = new System.Drawing.Size(212, 24);
            this.CBCopyStereotype.TabIndex = 2;
            this.CBCopyStereotype.Text = "Copy Parent\'s stereotype";
            this.CBCopyStereotype.UseVisualStyleBackColor = true;
            this.CBCopyStereotype.CheckedChanged += new System.EventHandler(this.CBCopyStereotype_CheckedChanged);
            // 
            // CBStereotype
            // 
            this.CBStereotype.Enabled = false;
            this.CBStereotype.FormattingEnabled = true;
            this.CBStereotype.Location = new System.Drawing.Point(9, 59);
            this.CBStereotype.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBStereotype.Name = "CBStereotype";
            this.CBStereotype.Size = new System.Drawing.Size(241, 28);
            this.CBStereotype.TabIndex = 10;
            // 
            // GB
            // 
            this.GB.Controls.Add(this.CBCopyStereotype);
            this.GB.Controls.Add(this.CBStereotype);
            this.GB.Location = new System.Drawing.Point(503, 129);
            this.GB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GB.Name = "GB";
            this.GB.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GB.Size = new System.Drawing.Size(268, 100);
            this.GB.TabIndex = 11;
            this.GB.TabStop = false;
            this.GB.Text = "Stereotype";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(553, 334);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 55);
            this.button1.TabIndex = 12;
            this.button1.Text = "Manage qualifier";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BUTManageQualifier_Click);
            // 
            // EditSubdivideForOptimConnectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 581);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.GB);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.AddSubdividedConnector);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EditSubdivideForOptimConnectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SubdivideConnectorForm";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.GB.ResumeLayout(false);
            this.GB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabClientRole;
        private System.Windows.Forms.Label LabSupplierRole;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox CBClientQualifier;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox CBSupplierQualifier;
        private System.Windows.Forms.TextBox TBClientRole;
        private System.Windows.Forms.TextBox TBSupplierRole;
        private System.Windows.Forms.Button AddSubdividedConnector;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox CBLBClientCardinality;
        private System.Windows.Forms.ComboBox CBLBSupplierCardinality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CBUBClientCardinality;
        private System.Windows.Forms.ComboBox CBUBSupplierCardinality;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox CBCopyNotes;
        private System.Windows.Forms.CheckBox CBCopyTagValues;
        private System.Windows.Forms.CheckBox CBCopyStereotype;
        private System.Windows.Forms.ComboBox CBStereotype;
        private System.Windows.Forms.GroupBox GB;
        private System.Windows.Forms.CheckBox CBAgregateSource;
        private System.Windows.Forms.CheckBox CBByRefTarget;
        private System.Windows.Forms.CheckBox CBAgregateTarget;
        private System.Windows.Forms.CheckBox CBByRefSource;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label LSContainment;
        private System.Windows.Forms.Label LTContainment;
    }
}