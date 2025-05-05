namespace CimContextor.EditConnectors

{
    partial class EditDuplicateForXsdConnectorsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.LabName = new System.Windows.Forms.Label();
            this.ButCancel = new System.Windows.Forms.Button();
            this.LVConnectors = new System.Windows.Forms.ListView();
            this.ColNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColTargetClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColClientEndRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColSupplierEndRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColSourceClass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ButDupplicateConnectors = new System.Windows.Forms.Button();
            this.LVSubConnectors = new System.Windows.Forms.ListView();
            this.ColSubNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColSubClientEndRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColSubSupplierEndRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColAgregate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LabParentConnectorName = new System.Windows.Forms.Label();
            this.ButSubdivideAConnector = new System.Windows.Forms.Button();
            this.ButEditSubConnector = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CBCopyConstraints = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected class: ";
            // 
            // LabName
            // 
            this.LabName.AutoSize = true;
            this.LabName.Location = new System.Drawing.Point(136, 14);
            this.LabName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabName.Name = "LabName";
            this.LabName.Size = new System.Drawing.Size(0, 20);
            this.LabName.TabIndex = 1;
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(918, 479);
            this.ButCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(202, 35);
            this.ButCancel.TabIndex = 2;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // LVConnectors
            // 
            this.LVConnectors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColNumber,
            this.ColTargetClass,
            this.ColClientEndRole,
            this.ColSupplierEndRole});
            this.LVConnectors.Enabled = false;
            this.LVConnectors.FullRowSelect = true;
            this.LVConnectors.GridLines = true;
            this.LVConnectors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVConnectors.HideSelection = false;
            this.LVConnectors.Location = new System.Drawing.Point(22, 85);
            this.LVConnectors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LVConnectors.MultiSelect = false;
            this.LVConnectors.Name = "LVConnectors";
            this.LVConnectors.Size = new System.Drawing.Size(1202, 218);
            this.LVConnectors.TabIndex = 3;
            this.LVConnectors.UseCompatibleStateImageBehavior = false;
            this.LVConnectors.View = System.Windows.Forms.View.Details;
            this.LVConnectors.SelectedIndexChanged += new System.EventHandler(this.LVConnectors_SelectedIndexChanged);
            // 
            // ColNumber
            // 
            this.ColNumber.Text = "N°";
            this.ColNumber.Width = 44;
            // 
            // ColTargetClass
            // 
            this.ColTargetClass.Text = "Associated class";
            this.ColTargetClass.Width = 193;
            // 
            // ColClientEndRole
            // 
            this.ColClientEndRole.Text = "Selected class role";
            this.ColClientEndRole.Width = 199;
            // 
            // ColSupplierEndRole
            // 
            this.ColSupplierEndRole.Text = "Associated class end role";
            this.ColSupplierEndRole.Width = 184;
            // 
            // ButDupplicateConnectors
            // 
            this.ButDupplicateConnectors.Location = new System.Drawing.Point(918, 434);
            this.ButDupplicateConnectors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButDupplicateConnectors.Name = "ButDupplicateConnectors";
            this.ButDupplicateConnectors.Size = new System.Drawing.Size(202, 35);
            this.ButDupplicateConnectors.TabIndex = 4;
            this.ButDupplicateConnectors.Text = "Save";
            this.ButDupplicateConnectors.UseVisualStyleBackColor = true;
            this.ButDupplicateConnectors.Click += new System.EventHandler(this.ButDupplicateConnectors_Click);
            // 
            // LVSubConnectors
            // 
            this.LVSubConnectors.CheckBoxes = true;
            this.LVSubConnectors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColSubNumber,
            this.ColSubClientEndRole,
            this.ColSubSupplierEndRole,
            this.ColAgregate});
            this.LVSubConnectors.Enabled = false;
            this.LVSubConnectors.FullRowSelect = true;
            this.LVSubConnectors.GridLines = true;
            this.LVSubConnectors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVSubConnectors.HideSelection = false;
            this.LVSubConnectors.Location = new System.Drawing.Point(22, 346);
            this.LVSubConnectors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LVSubConnectors.MultiSelect = false;
            this.LVSubConnectors.Name = "LVSubConnectors";
            this.LVSubConnectors.Size = new System.Drawing.Size(624, 273);
            this.LVSubConnectors.TabIndex = 6;
            this.LVSubConnectors.UseCompatibleStateImageBehavior = false;
            this.LVSubConnectors.View = System.Windows.Forms.View.Details;
            // 
            // ColSubNumber
            // 
            this.ColSubNumber.Text = "N°";
            this.ColSubNumber.Width = 36;
            // 
            // ColSubClientEndRole
            // 
            this.ColSubClientEndRole.Text = "Selected class role";
            this.ColSubClientEndRole.Width = 150;
            // 
            // ColSubSupplierEndRole
            // 
            this.ColSubSupplierEndRole.Text = "Associated class end role";
            this.ColSubSupplierEndRole.Width = 150;
            // 
            // ColAgregate
            // 
            this.ColAgregate.Text = "Oriented";
            this.ColAgregate.Width = 75;
            // 
            // LabParentConnectorName
            // 
            this.LabParentConnectorName.AutoSize = true;
            this.LabParentConnectorName.Location = new System.Drawing.Point(18, 314);
            this.LabParentConnectorName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabParentConnectorName.Name = "LabParentConnectorName";
            this.LabParentConnectorName.Size = new System.Drawing.Size(196, 20);
            this.LabParentConnectorName.TabIndex = 7;
            this.LabParentConnectorName.Text = "Detail of the connector n° :";
            // 
            // ButSubdivideAConnector
            // 
            this.ButSubdivideAConnector.Location = new System.Drawing.Point(1063, 314);
            this.ButSubdivideAConnector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButSubdivideAConnector.Name = "ButSubdivideAConnector";
            this.ButSubdivideAConnector.Size = new System.Drawing.Size(163, 60);
            this.ButSubdivideAConnector.TabIndex = 8;
            this.ButSubdivideAConnector.Text = "Modify selected association";
            this.ButSubdivideAConnector.UseVisualStyleBackColor = true;
            this.ButSubdivideAConnector.Click += new System.EventHandler(this.ButSubdivideAConnector_Click);
            // 
            // ButEditSubConnector
            // 
            this.ButEditSubConnector.Location = new System.Drawing.Point(657, 414);
            this.ButEditSubConnector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditSubConnector.Name = "ButEditSubConnector";
            this.ButEditSubConnector.Size = new System.Drawing.Size(136, 74);
            this.ButEditSubConnector.TabIndex = 9;
            this.ButEditSubConnector.Text = "Modify edited association";
            this.ButEditSubConnector.UseVisualStyleBackColor = true;
            this.ButEditSubConnector.Click += new System.EventHandler(this.ButEditSubConnector_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CBCopyConstraints);
            this.groupBox1.Location = new System.Drawing.Point(657, 314);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(354, 81);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // CBCopyConstraints
            // 
            this.CBCopyConstraints.AutoSize = true;
            this.CBCopyConstraints.Checked = true;
            this.CBCopyConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyConstraints.Location = new System.Drawing.Point(15, 31);
            this.CBCopyConstraints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyConstraints.Name = "CBCopyConstraints";
            this.CBCopyConstraints.Size = new System.Drawing.Size(237, 24);
            this.CBCopyConstraints.TabIndex = 0;
            this.CBCopyConstraints.Text = "Duplicate parent\'s constraint";
            this.CBCopyConstraints.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(317, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "                                                                             ";
            // 
            // EditDuplicateForXsdConnectorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 694);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButEditSubConnector);
            this.Controls.Add(this.ButSubdivideAConnector);
            this.Controls.Add(this.LabParentConnectorName);
            this.Controls.Add(this.LVSubConnectors);
            this.Controls.Add(this.ButDupplicateConnectors);
            this.Controls.Add(this.LVConnectors);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.LabName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EditDuplicateForXsdConnectorsForm";
            this.Text = "Select the link to edit";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabName;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.ListView LVConnectors;
        private System.Windows.Forms.ColumnHeader ColClientEndRole;
        private System.Windows.Forms.ColumnHeader ColSupplierEndRole;
        private System.Windows.Forms.Button ButDupplicateConnectors;
        private System.Windows.Forms.ListView LVSubConnectors;
        private System.Windows.Forms.ColumnHeader ColSubClientEndRole;
        private System.Windows.Forms.ColumnHeader ColSubSupplierEndRole;
        private System.Windows.Forms.Label LabParentConnectorName;
        private System.Windows.Forms.ColumnHeader ColNumber;
        private System.Windows.Forms.ColumnHeader ColSubNumber;
        private System.Windows.Forms.Button ButSubdivideAConnector;
        private System.Windows.Forms.Button ButEditSubConnector;
        private System.Windows.Forms.ColumnHeader ColTargetClass;
        private System.Windows.Forms.ColumnHeader ColSourceClass;
        private System.Windows.Forms.ColumnHeader ColAgregate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CBCopyConstraints;
        private System.Windows.Forms.Label label2;
    }
}