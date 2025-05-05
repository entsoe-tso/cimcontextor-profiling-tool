namespace CimContextor
{
    partial class AttributeClassifierForm
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
            this.ColClassfierName = new System.Windows.Forms.ColumnHeader();
            this.ButCancel = new System.Windows.Forms.Button();
            this.ButSelect = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TBDefaultValue = new System.Windows.Forms.TextBox();
            this.CBConstantValue = new System.Windows.Forms.CheckBox();
            this.CBStaticValue = new System.Windows.Forms.CheckBox();
            this.ButEnum = new System.Windows.Forms.Button();
            this.LabState = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LVClassifier
            // 
            this.LVClassifier.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColClassfierName});
            this.LVClassifier.FullRowSelect = true;
            this.LVClassifier.GridLines = true;
            this.LVClassifier.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVClassifier.Location = new System.Drawing.Point(12, 31);
            this.LVClassifier.Name = "LVClassifier";
            this.LVClassifier.Size = new System.Drawing.Size(249, 127);
            this.LVClassifier.TabIndex = 0;
            this.LVClassifier.UseCompatibleStateImageBehavior = false;
            this.LVClassifier.View = System.Windows.Forms.View.Details;
            // 
            // ColClassfierName
            // 
            this.ColClassfierName.Text = "Name";
            this.ColClassfierName.Width = 243;
            // 
            // ButCancel
            // 
            this.ButCancel.Location = new System.Drawing.Point(390, 136);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(106, 22);
            this.ButCancel.TabIndex = 1;
            this.ButCancel.Text = "Cancel";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // ButSelect
            // 
            this.ButSelect.Location = new System.Drawing.Point(278, 136);
            this.ButSelect.Name = "ButSelect";
            this.ButSelect.Size = new System.Drawing.Size(106, 22);
            this.ButSelect.TabIndex = 2;
            this.ButSelect.Text = "Ok";
            this.ButSelect.UseVisualStyleBackColor = true;
            this.ButSelect.Click += new System.EventHandler(this.ButSelect_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TBDefaultValue);
            this.groupBox1.Controls.Add(this.CBConstantValue);
            this.groupBox1.Controls.Add(this.CBStaticValue);
            this.groupBox1.Location = new System.Drawing.Point(278, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 110);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attribute details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Default value :";
            // 
            // TBDefaultValue
            // 
            this.TBDefaultValue.Location = new System.Drawing.Point(9, 84);
            this.TBDefaultValue.Name = "TBDefaultValue";
            this.TBDefaultValue.Size = new System.Drawing.Size(201, 20);
            this.TBDefaultValue.TabIndex = 1;
            // 
            // CBConstantValue
            // 
            this.CBConstantValue.AutoSize = true;
            this.CBConstantValue.Location = new System.Drawing.Point(6, 19);
            this.CBConstantValue.Name = "CBConstantValue";
            this.CBConstantValue.Size = new System.Drawing.Size(77, 17);
            this.CBConstantValue.TabIndex = 0;
            this.CBConstantValue.Text = "is constant";
            this.CBConstantValue.UseVisualStyleBackColor = true;
            // 
            // CBStaticValue
            // 
            this.CBStaticValue.AutoSize = true;
            this.CBStaticValue.Location = new System.Drawing.Point(150, 19);
            this.CBStaticValue.Name = "CBStaticValue";
            this.CBStaticValue.Size = new System.Drawing.Size(61, 17);
            this.CBStaticValue.TabIndex = 2;
            this.CBStaticValue.Text = "is static";
            this.CBStaticValue.UseVisualStyleBackColor = true;
            // 
            // ButEnum
            // 
            this.ButEnum.Location = new System.Drawing.Point(114, 4);
            this.ButEnum.Name = "ButEnum";
            this.ButEnum.Size = new System.Drawing.Size(106, 22);
            this.ButEnum.TabIndex = 4;
            this.ButEnum.Text = "Show enum";
            this.ButEnum.UseVisualStyleBackColor = true;
            this.ButEnum.Click += new System.EventHandler(this.ButEnum_Click);
            // 
            // LabState
            // 
            this.LabState.AutoSize = true;
            this.LabState.Location = new System.Drawing.Point(10, 9);
            this.LabState.Name = "LabState";
            this.LabState.Size = new System.Drawing.Size(71, 13);
            this.LabState.TabIndex = 5;
            this.LabState.Text = "Datatype list :";
            // 
            // AttributeClassifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 227);
            this.ControlBox = false;
            this.Controls.Add(this.LabState);
            this.Controls.Add(this.ButEnum);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ButSelect);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.LVClassifier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeClassifierForm";
            this.Text = "Select a new type";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView LVClassifier;
        private System.Windows.Forms.ColumnHeader ColClassfierName;
        private System.Windows.Forms.Button ButCancel;
        private System.Windows.Forms.Button ButSelect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox CBConstantValue;
        private System.Windows.Forms.TextBox TBDefaultValue;
        private System.Windows.Forms.CheckBox CBStaticValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButEnum;
        private System.Windows.Forms.Label LabState;
    }
}