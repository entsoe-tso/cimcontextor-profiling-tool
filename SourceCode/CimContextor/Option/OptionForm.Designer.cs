namespace CimContextor
{
    partial class OptionForm
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
            this.CBIsBasedOn = new System.Windows.Forms.CheckBox();
            this.GBFunctionnality = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.GBAddinSettings = new System.Windows.Forms.GroupBox();
            this.csvDelimLB = new System.Windows.Forms.Label();
            this.csvDelimComboBox = new System.Windows.Forms.ComboBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.CBEnableConcreteInheritanceInProfiles = new System.Windows.Forms.CheckBox();
            this.EnablePropertyGrouping = new System.Windows.Forms.CheckBox();
            this.CBCopyParentElement = new System.Windows.Forms.CheckBox();
            this.CBDataEnumQualifierNeeded = new System.Windows.Forms.CheckBox();
            this.CBDefaultBackgroundColor = new System.Windows.Forms.CheckBox();
            this.CBWarning = new System.Windows.Forms.CheckBox();
            this.CBConfirm = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.ButSelectColor = new System.Windows.Forms.Button();
            this.ButSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CBLog = new System.Windows.Forms.CheckBox();
            this.CBAutoNames = new System.Windows.Forms.CheckBox();
            this.CDBackground = new System.Windows.Forms.ColorDialog();
            this.CBWG13Ancesters = new System.Windows.Forms.CheckBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.GBFunctionnality.SuspendLayout();
            this.GBAddinSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CBIsBasedOn
            // 
            this.CBIsBasedOn.AutoSize = true;
            this.CBIsBasedOn.Checked = true;
            this.CBIsBasedOn.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBIsBasedOn.Location = new System.Drawing.Point(9, 29);
            this.CBIsBasedOn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBIsBasedOn.Name = "CBIsBasedOn";
            this.CBIsBasedOn.Size = new System.Drawing.Size(288, 24);
            this.CBIsBasedOn.TabIndex = 0;
            this.CBIsBasedOn.Text = "Activate the IsBasedOn Funtionality";
            this.CBIsBasedOn.UseVisualStyleBackColor = true;
            // 
            // GBFunctionnality
            // 
            this.GBFunctionnality.Controls.Add(this.radioButton2);
            this.GBFunctionnality.Controls.Add(this.radioButton1);
            this.GBFunctionnality.Controls.Add(this.CBIsBasedOn);
            this.GBFunctionnality.Location = new System.Drawing.Point(14, 19);
            this.GBFunctionnality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GBFunctionnality.Name = "GBFunctionnality";
            this.GBFunctionnality.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GBFunctionnality.Size = new System.Drawing.Size(364, 125);
            this.GBFunctionnality.TabIndex = 3;
            this.GBFunctionnality.TabStop = false;
            this.GBFunctionnality.Text = "Manage Functionnality";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(9, 98);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(273, 24);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Associations for hierachical Model";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.Visible = false;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.RB2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(8, 64);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(239, 24);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Associations for graph Model";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Visible = false;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.RB1_CheckedChanged);
            // 
            // GBAddinSettings
            // 
            this.GBAddinSettings.Controls.Add(this.csvDelimLB);
            this.GBAddinSettings.Controls.Add(this.csvDelimComboBox);
            this.GBAddinSettings.Controls.Add(this.checkBox3);
            this.GBAddinSettings.Controls.Add(this.checkBox1);
            this.GBAddinSettings.Controls.Add(this.CBEnableConcreteInheritanceInProfiles);
            this.GBAddinSettings.Controls.Add(this.EnablePropertyGrouping);
            this.GBAddinSettings.Controls.Add(this.CBCopyParentElement);
            this.GBAddinSettings.Controls.Add(this.CBDataEnumQualifierNeeded);
            this.GBAddinSettings.Controls.Add(this.CBDefaultBackgroundColor);
            this.GBAddinSettings.Controls.Add(this.CBWarning);
            this.GBAddinSettings.Controls.Add(this.CBConfirm);
            this.GBAddinSettings.Location = new System.Drawing.Point(15, 151);
            this.GBAddinSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GBAddinSettings.Name = "GBAddinSettings";
            this.GBAddinSettings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GBAddinSettings.Size = new System.Drawing.Size(364, 498);
            this.GBAddinSettings.TabIndex = 4;
            this.GBAddinSettings.TabStop = false;
            this.GBAddinSettings.Text = "Add-in Settings";
            this.GBAddinSettings.Enter += new System.EventHandler(this.GBAddinSettings_Enter);
            // 
            // csvDelimLB
            // 
            this.csvDelimLB.AutoSize = true;
            this.csvDelimLB.Location = new System.Drawing.Point(138, 436);
            this.csvDelimLB.Name = "csvDelimLB";
            this.csvDelimLB.Size = new System.Drawing.Size(108, 20);
            this.csvDelimLB.TabIndex = 16;
            this.csvDelimLB.Text = "CSV Delimiter";
            // 
            // csvDelimComboBox
            // 
            this.csvDelimComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.csvDelimComboBox.FormattingEnabled = true;
            this.csvDelimComboBox.Location = new System.Drawing.Point(10, 433);
            this.csvDelimComboBox.Name = "csvDelimComboBox";
            this.csvDelimComboBox.Size = new System.Drawing.Size(121, 28);
            this.csvDelimComboBox.TabIndex = 15;
            this.csvDelimComboBox.SelectedIndexChanged += new System.EventHandler(this.csvDelimSelected);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBox3.Location = new System.Drawing.Point(8, 295);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(278, 24);
            this.checkBox3.TabIndex = 14;
            this.checkBox3.Text = "W13AutomaticAnscestersInProfile";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBox1.Location = new System.Drawing.Point(10, 259);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(176, 24);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "WG13 profiling style";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_2);
            // 
            // CBEnableConcreteInheritanceInProfiles
            // 
            this.CBEnableConcreteInheritanceInProfiles.AutoSize = true;
            this.CBEnableConcreteInheritanceInProfiles.Checked = true;
            this.CBEnableConcreteInheritanceInProfiles.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBEnableConcreteInheritanceInProfiles.Location = new System.Drawing.Point(10, 222);
            this.CBEnableConcreteInheritanceInProfiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBEnableConcreteInheritanceInProfiles.Name = "CBEnableConcreteInheritanceInProfiles";
            this.CBEnableConcreteInheritanceInProfiles.Size = new System.Drawing.Size(304, 24);
            this.CBEnableConcreteInheritanceInProfiles.TabIndex = 12;
            this.CBEnableConcreteInheritanceInProfiles.Text = "Enable concrete inheritance in profiles";
            this.CBEnableConcreteInheritanceInProfiles.UseVisualStyleBackColor = true;
            this.CBEnableConcreteInheritanceInProfiles.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // EnablePropertyGrouping
            // 
            this.EnablePropertyGrouping.AutoSize = true;
            this.EnablePropertyGrouping.Checked = true;
            this.EnablePropertyGrouping.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.EnablePropertyGrouping.Location = new System.Drawing.Point(9, 188);
            this.EnablePropertyGrouping.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.EnablePropertyGrouping.Name = "EnablePropertyGrouping";
            this.EnablePropertyGrouping.Size = new System.Drawing.Size(222, 24);
            this.EnablePropertyGrouping.TabIndex = 11;
            this.EnablePropertyGrouping.Text = "Enable Property Grouping ";
            this.EnablePropertyGrouping.UseVisualStyleBackColor = true;
            this.EnablePropertyGrouping.CheckedChanged += new System.EventHandler(this.EnablePropertyGrouping_CheckedChanged);
            // 
            // CBCopyParentElement
            // 
            this.CBCopyParentElement.AutoSize = true;
            this.CBCopyParentElement.Checked = true;
            this.CBCopyParentElement.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBCopyParentElement.Location = new System.Drawing.Point(10, 126);
            this.CBCopyParentElement.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBCopyParentElement.Name = "CBCopyParentElement";
            this.CBCopyParentElement.Size = new System.Drawing.Size(209, 24);
            this.CBCopyParentElement.TabIndex = 10;
            this.CBCopyParentElement.Text = "Copy the parent element";
            this.CBCopyParentElement.UseVisualStyleBackColor = true;
            // 
            // CBDataEnumQualifierNeeded
            // 
            this.CBDataEnumQualifierNeeded.AutoSize = true;
            this.CBDataEnumQualifierNeeded.Checked = true;
            this.CBDataEnumQualifierNeeded.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBDataEnumQualifierNeeded.Location = new System.Drawing.Point(9, 94);
            this.CBDataEnumQualifierNeeded.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBDataEnumQualifierNeeded.Name = "CBDataEnumQualifierNeeded";
            this.CBDataEnumQualifierNeeded.Size = new System.Drawing.Size(260, 24);
            this.CBDataEnumQualifierNeeded.TabIndex = 9;
            this.CBDataEnumQualifierNeeded.Text = "Must qualify datatype and enum";
            this.CBDataEnumQualifierNeeded.UseVisualStyleBackColor = true;
            // 
            // CBDefaultBackgroundColor
            // 
            this.CBDefaultBackgroundColor.AutoSize = true;
            this.CBDefaultBackgroundColor.Checked = true;
            this.CBDefaultBackgroundColor.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBDefaultBackgroundColor.Location = new System.Drawing.Point(10, 155);
            this.CBDefaultBackgroundColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBDefaultBackgroundColor.Name = "CBDefaultBackgroundColor";
            this.CBDefaultBackgroundColor.Size = new System.Drawing.Size(319, 24);
            this.CBDefaultBackgroundColor.TabIndex = 8;
            this.CBDefaultBackgroundColor.Text = "Use default color for IsBasedOn classes";
            this.CBDefaultBackgroundColor.UseVisualStyleBackColor = true;
            this.CBDefaultBackgroundColor.CheckedChanged += new System.EventHandler(this.CBDefaultBackgroundColor_CheckedChanged);
            // 
            // CBWarning
            // 
            this.CBWarning.AutoSize = true;
            this.CBWarning.Checked = true;
            this.CBWarning.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBWarning.Location = new System.Drawing.Point(9, 65);
            this.CBWarning.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBWarning.Name = "CBWarning";
            this.CBWarning.Size = new System.Drawing.Size(282, 24);
            this.CBWarning.TabIndex = 6;
            this.CBWarning.Text = "Enable warning (recommended on)";
            this.CBWarning.UseVisualStyleBackColor = true;
            // 
            // CBConfirm
            // 
            this.CBConfirm.AutoSize = true;
            this.CBConfirm.Checked = true;
            this.CBConfirm.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBConfirm.Location = new System.Drawing.Point(9, 29);
            this.CBConfirm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBConfirm.Name = "CBConfirm";
            this.CBConfirm.Size = new System.Drawing.Size(240, 24);
            this.CBConfirm.TabIndex = 5;
            this.CBConfirm.Text = "Confirm IsBasedOn\'s actions";
            this.CBConfirm.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.checkBox2.Location = new System.Drawing.Point(22, 479);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(168, 24);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "NavigationEnabled";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // ButSelectColor
            // 
            this.ButSelectColor.Location = new System.Drawing.Point(319, 672);
            this.ButSelectColor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButSelectColor.Name = "ButSelectColor";
            this.ButSelectColor.Size = new System.Drawing.Size(150, 43);
            this.ButSelectColor.TabIndex = 7;
            this.ButSelectColor.Text = "Select a color";
            this.ButSelectColor.UseVisualStyleBackColor = true;
            this.ButSelectColor.Click += new System.EventHandler(this.ButSelectColor_Click);
            // 
            // ButSave
            // 
            this.ButSave.Location = new System.Drawing.Point(25, 745);
            this.ButSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ButSave.Name = "ButSave";
            this.ButSave.Size = new System.Drawing.Size(112, 43);
            this.ButSave.TabIndex = 5;
            this.ButSave.Text = "Save";
            this.ButSave.UseVisualStyleBackColor = true;
            this.ButSave.Click += new System.EventHandler(this.ButSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CBLog);
            this.groupBox1.Location = new System.Drawing.Point(22, 659);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(225, 60);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // CBLog
            // 
            this.CBLog.AutoSize = true;
            this.CBLog.Checked = true;
            this.CBLog.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBLog.Location = new System.Drawing.Point(9, 27);
            this.CBLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBLog.Name = "CBLog";
            this.CBLog.Size = new System.Drawing.Size(193, 24);
            this.CBLog.TabIndex = 0;
            this.CBLog.Text = "Log IsBasedOn action";
            this.CBLog.UseVisualStyleBackColor = true;
            // 
            // CBAutoNames
            // 
            this.CBAutoNames.AutoSize = true;
            this.CBAutoNames.Checked = true;
            this.CBAutoNames.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBAutoNames.Location = new System.Drawing.Point(24, 515);
            this.CBAutoNames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBAutoNames.Name = "CBAutoNames";
            this.CBAutoNames.Size = new System.Drawing.Size(164, 24);
            this.CBAutoNames.TabIndex = 15;
            this.CBAutoNames.Text = "RolesAutoNaming";
            this.CBAutoNames.UseVisualStyleBackColor = true;
            this.CBAutoNames.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // CBWG13Ancesters
            // 
            this.CBWG13Ancesters.AutoSize = true;
            this.CBWG13Ancesters.Checked = true;
            this.CBWG13Ancesters.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.CBWG13Ancesters.Location = new System.Drawing.Point(25, 551);
            this.CBWG13Ancesters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CBWG13Ancesters.Name = "CBWG13Ancesters";
            this.CBWG13Ancesters.Size = new System.Drawing.Size(270, 24);
            this.CBWG13Ancesters.TabIndex = 16;
            this.CBWG13Ancesters.Text = "W13AutomaticAnscesterInProfile";
            this.CBWG13Ancesters.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(357, 745);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(112, 43);
            this.cancelBtn.TabIndex = 17;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // OptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 810);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.CBWG13Ancesters);
            this.Controls.Add(this.CBAutoNames);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.ButSelectColor);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButSave);
            this.Controls.Add(this.GBAddinSettings);
            this.Controls.Add(this.GBFunctionnality);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "OptionForm";
            this.Text = "Option";
            this.GBFunctionnality.ResumeLayout(false);
            this.GBFunctionnality.PerformLayout();
            this.GBAddinSettings.ResumeLayout(false);
            this.GBAddinSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CBIsBasedOn;
        private System.Windows.Forms.GroupBox GBFunctionnality;
        private System.Windows.Forms.GroupBox GBAddinSettings;
        private System.Windows.Forms.CheckBox CBConfirm;
        private System.Windows.Forms.Button ButSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CBLog;
        private System.Windows.Forms.CheckBox CBWarning;
        private System.Windows.Forms.Button ButSelectColor;
        private System.Windows.Forms.ColorDialog CDBackground;
        private System.Windows.Forms.CheckBox CBDefaultBackgroundColor;
        private System.Windows.Forms.CheckBox CBDataEnumQualifierNeeded;
        private System.Windows.Forms.CheckBox CBCopyParentElement;
        private System.Windows.Forms.CheckBox EnablePropertyGrouping;
        private System.Windows.Forms.CheckBox CBEnableConcreteInheritanceInProfiles;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox CBAutoNames;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox CBWG13Ancesters;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label csvDelimLB;
        private System.Windows.Forms.ComboBox csvDelimComboBox;
    }
}