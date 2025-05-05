namespace CimContextor
{
    partial class IsBasedOnForm
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
            this.TBQualifier = new System.Windows.Forms.TextBox();
            this.ButExecIsBasedOn = new System.Windows.Forms.Button();
            this.CBQualifier = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ButCreateQualifier = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CBQualifierAllowedToAny = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ButDeleteQualifier = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ButEditAttributStereotype = new System.Windows.Forms.Button();
            this.ButEditConstraint = new System.Windows.Forms.Button();
            this.ButEditClassifier = new System.Windows.Forms.Button();
            this.ButEditCardinality = new System.Windows.Forms.Button();
            this.ButUnselectAll = new System.Windows.Forms.Button();
            this.ButSelectAll = new System.Windows.Forms.Button();
            this.LVAttribute = new System.Windows.Forms.ListView();
            this.ColCB = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColHerited = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColLowerBound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColUpperBound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CBCopyTagValues = new System.Windows.Forms.CheckBox();
            this.ButCancel = new System.Windows.Forms.Button();
            this.groupbox22 = new System.Windows.Forms.GroupBox();
            this.CBRoot = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CBInheritList = new System.Windows.Forms.ComboBox();
            this.CBAstract = new System.Windows.Forms.CheckBox();
            this.ButEditStereotype = new System.Windows.Forms.Button();
            this.CBCopyStereotype = new System.Windows.Forms.CheckBox();
            this.CBCopyParent = new System.Windows.Forms.CheckBox();
            this.ButEditClassConstraint = new System.Windows.Forms.Button();
            this.CBCopyConstraints = new System.Windows.Forms.CheckBox();
            this.CBCopyNotes = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupbox22.SuspendLayout();
            this.SuspendLayout();
            // 
            // TBQualifier
            // 
            this.TBQualifier.Location = new System.Drawing.Point(9, 29);
            this.TBQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TBQualifier.Name = "TBQualifier";
            this.TBQualifier.Size = new System.Drawing.Size(260, 26);
            this.TBQualifier.TabIndex = 0;
            // 
            // ButExecIsBasedOn
            // 
            this.ButExecIsBasedOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButExecIsBasedOn.Location = new System.Drawing.Point(883, 465);
            this.ButExecIsBasedOn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButExecIsBasedOn.Name = "ButExecIsBasedOn";
            this.ButExecIsBasedOn.Size = new System.Drawing.Size(204, 38);
            this.ButExecIsBasedOn.TabIndex = 1;
            this.ButExecIsBasedOn.Text = "Execute IsBasedOn";
            this.ButExecIsBasedOn.UseVisualStyleBackColor = true;
            this.ButExecIsBasedOn.Click += new System.EventHandler(this.ButExecuteIsBasedOn_Click);
            // 
            // CBQualifier
            // 
            this.CBQualifier.FormattingEnabled = true;
            this.CBQualifier.Location = new System.Drawing.Point(12, 38);
            this.CBQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBQualifier.Name = "CBQualifier";
            this.CBQualifier.Size = new System.Drawing.Size(253, 28);
            this.CBQualifier.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter a qualifier or create one :";
            // 
            // ButCreateQualifier
            // 
            this.ButCreateQualifier.Location = new System.Drawing.Point(12, 114);
            this.ButCreateQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCreateQualifier.Name = "ButCreateQualifier";
            this.ButCreateQualifier.Size = new System.Drawing.Size(116, 38);
            this.ButCreateQualifier.TabIndex = 4;
            this.ButCreateQualifier.Text = "Create";
            this.ButCreateQualifier.UseVisualStyleBackColor = true;
            this.ButCreateQualifier.Click += new System.EventHandler(this.ButCreateQualifier_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CBQualifierAllowedToAny);
            this.groupBox1.Controls.Add(this.ButCreateQualifier);
            this.groupBox1.Controls.Add(this.TBQualifier);
            this.groupBox1.Location = new System.Drawing.Point(14, 225);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(280, 155);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // CBQualifierAllowedToAny
            // 
            this.CBQualifierAllowedToAny.AutoSize = true;
            this.CBQualifierAllowedToAny.Checked = true;
            this.CBQualifierAllowedToAny.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBQualifierAllowedToAny.Location = new System.Drawing.Point(34, 74);
            this.CBQualifierAllowedToAny.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBQualifierAllowedToAny.Name = "CBQualifierAllowedToAny";
            this.CBQualifierAllowedToAny.Size = new System.Drawing.Size(209, 24);
            this.CBQualifierAllowedToAny.TabIndex = 5;
            this.CBQualifierAllowedToAny.Text = "Allowed to class and role";
            this.CBQualifierAllowedToAny.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ButDeleteQualifier);
            this.groupBox2.Controls.Add(this.CBQualifier);
            this.groupBox2.Location = new System.Drawing.Point(14, 49);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(280, 155);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select";
            // 
            // ButDeleteQualifier
            // 
            this.ButDeleteQualifier.Location = new System.Drawing.Point(12, 95);
            this.ButDeleteQualifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButDeleteQualifier.Name = "ButDeleteQualifier";
            this.ButDeleteQualifier.Size = new System.Drawing.Size(116, 37);
            this.ButDeleteQualifier.TabIndex = 3;
            this.ButDeleteQualifier.Text = "Delete";
            this.ButDeleteQualifier.UseVisualStyleBackColor = true;
            this.ButDeleteQualifier.Click += new System.EventHandler(this.ButDeleteQualifier_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(18, 12);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(309, 389);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Qualifier";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.ButEditAttributStereotype);
            this.groupBox4.Controls.Add(this.ButEditConstraint);
            this.groupBox4.Controls.Add(this.ButEditClassifier);
            this.groupBox4.Controls.Add(this.ButEditCardinality);
            this.groupBox4.Controls.Add(this.ButUnselectAll);
            this.groupBox4.Controls.Add(this.ButSelectAll);
            this.groupBox4.Controls.Add(this.LVAttribute);
            this.groupBox4.Location = new System.Drawing.Point(336, 12);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(705, 389);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Attribute";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // ButEditAttributStereotype
            // 
            this.ButEditAttributStereotype.Location = new System.Drawing.Point(411, 340);
            this.ButEditAttributStereotype.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditAttributStereotype.Name = "ButEditAttributStereotype";
            this.ButEditAttributStereotype.Size = new System.Drawing.Size(132, 37);
            this.ButEditAttributStereotype.TabIndex = 15;
            this.ButEditAttributStereotype.Text = "Edit stereotype";
            this.ButEditAttributStereotype.UseVisualStyleBackColor = true;
            this.ButEditAttributStereotype.Click += new System.EventHandler(this.ButEditAttributStereotype_Click);
            // 
            // ButEditConstraint
            // 
            this.ButEditConstraint.Location = new System.Drawing.Point(564, 340);
            this.ButEditConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditConstraint.Name = "ButEditConstraint";
            this.ButEditConstraint.Size = new System.Drawing.Size(132, 37);
            this.ButEditConstraint.TabIndex = 14;
            this.ButEditConstraint.Text = "Edit constraint";
            this.ButEditConstraint.UseVisualStyleBackColor = true;
            this.ButEditConstraint.Click += new System.EventHandler(this.ButEditConstraint_Click);
            // 
            // ButEditClassifier
            // 
            this.ButEditClassifier.Location = new System.Drawing.Point(564, 294);
            this.ButEditClassifier.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditClassifier.Name = "ButEditClassifier";
            this.ButEditClassifier.Size = new System.Drawing.Size(132, 37);
            this.ButEditClassifier.TabIndex = 12;
            this.ButEditClassifier.Text = "Edit type";
            this.ButEditClassifier.UseVisualStyleBackColor = true;
            this.ButEditClassifier.Click += new System.EventHandler(this.ButEditClassifier_Click);
            // 
            // ButEditCardinality
            // 
            this.ButEditCardinality.Location = new System.Drawing.Point(411, 294);
            this.ButEditCardinality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditCardinality.Name = "ButEditCardinality";
            this.ButEditCardinality.Size = new System.Drawing.Size(132, 37);
            this.ButEditCardinality.TabIndex = 11;
            this.ButEditCardinality.Text = "Edit cardinality";
            this.ButEditCardinality.UseVisualStyleBackColor = true;
            this.ButEditCardinality.Click += new System.EventHandler(this.ButEditCardinality_Click);
            // 
            // ButUnselectAll
            // 
            this.ButUnselectAll.Location = new System.Drawing.Point(9, 332);
            this.ButUnselectAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButUnselectAll.Name = "ButUnselectAll";
            this.ButUnselectAll.Size = new System.Drawing.Size(120, 37);
            this.ButUnselectAll.TabIndex = 10;
            this.ButUnselectAll.Text = "Unselect all";
            this.ButUnselectAll.UseVisualStyleBackColor = true;
            this.ButUnselectAll.Click += new System.EventHandler(this.ButUnselectAll_Click);
            // 
            // ButSelectAll
            // 
            this.ButSelectAll.Location = new System.Drawing.Point(150, 332);
            this.ButSelectAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButSelectAll.Name = "ButSelectAll";
            this.ButSelectAll.Size = new System.Drawing.Size(120, 37);
            this.ButSelectAll.TabIndex = 9;
            this.ButSelectAll.Text = "Select all";
            this.ButSelectAll.UseVisualStyleBackColor = true;
            this.ButSelectAll.Click += new System.EventHandler(this.ButSelectAll_Click);
            // 
            // LVAttribute
            // 
            this.LVAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LVAttribute.CausesValidation = false;
            this.LVAttribute.CheckBoxes = true;
            this.LVAttribute.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColCB,
            this.ColHerited,
            this.ColNameHeader,
            this.ColLowerBound,
            this.ColUpperBound});
            this.LVAttribute.FullRowSelect = true;
            this.LVAttribute.GridLines = true;
            this.LVAttribute.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVAttribute.HideSelection = false;
            this.LVAttribute.Location = new System.Drawing.Point(9, 29);
            this.LVAttribute.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LVAttribute.MultiSelect = false;
            this.LVAttribute.Name = "LVAttribute";
            this.LVAttribute.Size = new System.Drawing.Size(685, 244);
            this.LVAttribute.TabIndex = 8;
            this.LVAttribute.UseCompatibleStateImageBehavior = false;
            this.LVAttribute.View = System.Windows.Forms.View.Details;
            this.LVAttribute.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LVAttribute_ItemCheck);
            this.LVAttribute.SelectedIndexChanged += new System.EventHandler(this.LVAttribute_SelectedIndexChanged);
            // 
            // ColCB
            // 
            this.ColCB.Text = "";
            this.ColCB.Width = 22;
            // 
            // ColHerited
            // 
            this.ColHerited.Text = "Possible inheritance";
            this.ColHerited.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColHerited.Width = 120;
            // 
            // ColNameHeader
            // 
            this.ColNameHeader.Text = "Attribute name";
            this.ColNameHeader.Width = 190;
            // 
            // ColLowerBound
            // 
            this.ColLowerBound.Text = "LowerBound";
            this.ColLowerBound.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ColUpperBound
            // 
            this.ColUpperBound.Text = "UpperBound";
            this.ColUpperBound.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CBCopyTagValues
            // 
            this.CBCopyTagValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCopyTagValues.AutoSize = true;
            this.CBCopyTagValues.Checked = true;
            this.CBCopyTagValues.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyTagValues.Location = new System.Drawing.Point(9, 100);
            this.CBCopyTagValues.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyTagValues.Name = "CBCopyTagValues";
            this.CBCopyTagValues.Size = new System.Drawing.Size(212, 24);
            this.CBCopyTagValues.TabIndex = 13;
            this.CBCopyTagValues.Text = "Copy parent\'s TagValues";
            this.CBCopyTagValues.UseVisualStyleBackColor = true;
            this.CBCopyTagValues.CheckedChanged += new System.EventHandler(this.CBCopyTagValues_CheckedChanged);
            // 
            // ButCancel
            // 
            this.ButCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButCancel.Location = new System.Drawing.Point(1123, 465);
            this.ButCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(204, 38);
            this.ButCancel.TabIndex = 10;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // groupbox22
            // 
            this.groupbox22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupbox22.Controls.Add(this.CBRoot);
            this.groupbox22.Controls.Add(this.label2);
            this.groupbox22.Controls.Add(this.CBInheritList);
            this.groupbox22.Controls.Add(this.CBAstract);
            this.groupbox22.Controls.Add(this.ButEditStereotype);
            this.groupbox22.Controls.Add(this.CBCopyStereotype);
            this.groupbox22.Controls.Add(this.CBCopyParent);
            this.groupbox22.Controls.Add(this.ButEditClassConstraint);
            this.groupbox22.Controls.Add(this.CBCopyConstraints);
            this.groupbox22.Controls.Add(this.CBCopyNotes);
            this.groupbox22.Controls.Add(this.CBCopyTagValues);
            this.groupbox22.Location = new System.Drawing.Point(1059, 12);
            this.groupbox22.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupbox22.Name = "groupbox22";
            this.groupbox22.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupbox22.Size = new System.Drawing.Size(288, 434);
            this.groupbox22.TabIndex = 14;
            this.groupbox22.TabStop = false;
            this.groupbox22.Text = "Options";
            // 
            // CBRoot
            // 
            this.CBRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBRoot.AutoSize = true;
            this.CBRoot.Enabled = false;
            this.CBRoot.Location = new System.Drawing.Point(9, 285);
            this.CBRoot.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBRoot.Name = "CBRoot";
            this.CBRoot.Size = new System.Drawing.Size(133, 24);
            this.CBRoot.TabIndex = 23;
            this.CBRoot.Text = "is root (active)";
            this.CBRoot.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 357);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 20);
            this.label2.TabIndex = 22;
            this.label2.Text = "Inherit from :";
            // 
            // CBInheritList
            // 
            this.CBInheritList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBInheritList.Enabled = false;
            this.CBInheritList.FormattingEnabled = true;
            this.CBInheritList.Location = new System.Drawing.Point(9, 391);
            this.CBInheritList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBInheritList.Name = "CBInheritList";
            this.CBInheritList.Size = new System.Drawing.Size(259, 28);
            this.CBInheritList.TabIndex = 21;
            this.CBInheritList.SelectedIndexChanged += new System.EventHandler(this.CBInheritList_SelectedIndexChanged);
            // 
            // CBAstract
            // 
            this.CBAstract.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBAstract.AutoSize = true;
            this.CBAstract.Enabled = false;
            this.CBAstract.Location = new System.Drawing.Point(9, 320);
            this.CBAstract.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBAstract.Name = "CBAstract";
            this.CBAstract.Size = new System.Drawing.Size(108, 24);
            this.CBAstract.TabIndex = 20;
            this.CBAstract.Text = "is abstract";
            this.CBAstract.UseVisualStyleBackColor = true;
            // 
            // ButEditStereotype
            // 
            this.ButEditStereotype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButEditStereotype.Location = new System.Drawing.Point(45, 242);
            this.ButEditStereotype.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditStereotype.Name = "ButEditStereotype";
            this.ButEditStereotype.Size = new System.Drawing.Size(204, 34);
            this.ButEditStereotype.TabIndex = 19;
            this.ButEditStereotype.Text = "Edit class\'s stereotype";
            this.ButEditStereotype.UseVisualStyleBackColor = true;
            this.ButEditStereotype.Click += new System.EventHandler(this.ButEditStereotype_Click);
            // 
            // CBCopyStereotype
            // 
            this.CBCopyStereotype.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCopyStereotype.AutoSize = true;
            this.CBCopyStereotype.Checked = true;
            this.CBCopyStereotype.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyStereotype.Location = new System.Drawing.Point(9, 214);
            this.CBCopyStereotype.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyStereotype.Name = "CBCopyStereotype";
            this.CBCopyStereotype.Size = new System.Drawing.Size(211, 24);
            this.CBCopyStereotype.TabIndex = 18;
            this.CBCopyStereotype.Text = "Copy parent\'s stereotype";
            this.CBCopyStereotype.UseVisualStyleBackColor = true;
            this.CBCopyStereotype.CheckedChanged += new System.EventHandler(this.CBCopyStereotype_CheckedChanged);
            // 
            // CBCopyParent
            // 
            this.CBCopyParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCopyParent.AutoSize = true;
            this.CBCopyParent.Location = new System.Drawing.Point(9, 29);
            this.CBCopyParent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyParent.Name = "CBCopyParent";
            this.CBCopyParent.Size = new System.Drawing.Size(188, 24);
            this.CBCopyParent.TabIndex = 17;
            this.CBCopyParent.Text = "Copy the parent class";
            this.CBCopyParent.UseVisualStyleBackColor = true;
            // 
            // ButEditClassConstraint
            // 
            this.ButEditClassConstraint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButEditClassConstraint.Location = new System.Drawing.Point(45, 171);
            this.ButEditClassConstraint.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButEditClassConstraint.Name = "ButEditClassConstraint";
            this.ButEditClassConstraint.Size = new System.Drawing.Size(204, 34);
            this.ButEditClassConstraint.TabIndex = 16;
            this.ButEditClassConstraint.Text = "Edit class\'s constraints";
            this.ButEditClassConstraint.UseVisualStyleBackColor = true;
            this.ButEditClassConstraint.Click += new System.EventHandler(this.ButEditClassConstraint_Click);
            // 
            // CBCopyConstraints
            // 
            this.CBCopyConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCopyConstraints.AutoSize = true;
            this.CBCopyConstraints.Checked = true;
            this.CBCopyConstraints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyConstraints.Location = new System.Drawing.Point(9, 135);
            this.CBCopyConstraints.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyConstraints.Name = "CBCopyConstraints";
            this.CBCopyConstraints.Size = new System.Drawing.Size(214, 24);
            this.CBCopyConstraints.TabIndex = 15;
            this.CBCopyConstraints.Text = "Copy parent\'s constraints";
            this.CBCopyConstraints.UseVisualStyleBackColor = true;
            this.CBCopyConstraints.CheckedChanged += new System.EventHandler(this.CBCopyConstraints_CheckedChanged);
            // 
            // CBCopyNotes
            // 
            this.CBCopyNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCopyNotes.AutoSize = true;
            this.CBCopyNotes.Checked = true;
            this.CBCopyNotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBCopyNotes.Location = new System.Drawing.Point(9, 65);
            this.CBCopyNotes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyNotes.Name = "CBCopyNotes";
            this.CBCopyNotes.Size = new System.Drawing.Size(176, 24);
            this.CBCopyNotes.TabIndex = 14;
            this.CBCopyNotes.Text = "Copy parent\'s notes";
            this.CBCopyNotes.UseVisualStyleBackColor = true;
            this.CBCopyNotes.CheckedChanged += new System.EventHandler(this.CBCopyNotes_CheckedChanged);
            // 
            // IsBasedOnForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1353, 517);
            this.Controls.Add(this.groupbox22);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ButExecIsBasedOn);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "IsBasedOnForm";
            this.Text = "IsBasedOn";
            this.Load += new System.EventHandler(this.IsBasedOnForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupbox22.ResumeLayout(false);
            this.groupbox22.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TBQualifier;
        private System.Windows.Forms.Button ButExecIsBasedOn;
        private System.Windows.Forms.ComboBox CBQualifier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButCreateQualifier;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ButDeleteQualifier;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView LVAttribute;
        private System.Windows.Forms.Button ButSelectAll;
        private System.Windows.Forms.Button ButUnselectAll;
        private System.Windows.Forms.ColumnHeader ColNameHeader;
        private System.Windows.Forms.ColumnHeader ColLowerBound;
        private System.Windows.Forms.ColumnHeader ColUpperBound;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButEditCardinality;
        private System.Windows.Forms.Button ButEditClassifier;
        private System.Windows.Forms.CheckBox CBCopyTagValues;
        private System.Windows.Forms.Button ButEditConstraint;
        private System.Windows.Forms.GroupBox groupbox22;
        private System.Windows.Forms.CheckBox CBCopyConstraints;
        private System.Windows.Forms.CheckBox CBCopyNotes;
        private System.Windows.Forms.Button ButEditClassConstraint;
        private System.Windows.Forms.CheckBox CBQualifierAllowedToAny;
        private System.Windows.Forms.CheckBox CBCopyParent;
        private System.Windows.Forms.CheckBox CBCopyStereotype;
        private System.Windows.Forms.Button ButEditStereotype;
        private System.Windows.Forms.Button ButEditAttributStereotype;
        private System.Windows.Forms.CheckBox CBAstract;
        private System.Windows.Forms.ColumnHeader ColHerited;
        private System.Windows.Forms.ColumnHeader ColCB;
        private System.Windows.Forms.ComboBox CBInheritList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CBRoot;
    }
}