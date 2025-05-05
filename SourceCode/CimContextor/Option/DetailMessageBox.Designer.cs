namespace CimContextor
{
    partial class DetailMessageBox
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
            this.ButOk = new System.Windows.Forms.Button();
            this.ButDetails = new System.Windows.Forms.Button();
            this.LVObjectError = new System.Windows.Forms.ListView();
            this.ColString = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LabelError = new System.Windows.Forms.Label();
            this.LabMain = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButOk
            // 
            this.ButOk.Location = new System.Drawing.Point(222, 84);
            this.ButOk.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ButOk.Name = "ButOk";
            this.ButOk.Size = new System.Drawing.Size(156, 40);
            this.ButOk.TabIndex = 1;
            this.ButOk.Text = "Ok";
            this.ButOk.UseVisualStyleBackColor = true;
            this.ButOk.Click += new System.EventHandler(this.ButOk_Click);
            // 
            // ButDetails
            // 
            this.ButDetails.Location = new System.Drawing.Point(19, 84);
            this.ButDetails.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ButDetails.Name = "ButDetails";
            this.ButDetails.Size = new System.Drawing.Size(156, 40);
            this.ButDetails.TabIndex = 2;
            this.ButDetails.Text = "Details";
            this.ButDetails.UseVisualStyleBackColor = true;
            this.ButDetails.Click += new System.EventHandler(this.ButDetails_Click);
            // 
            // LVObjectError
            // 
            this.LVObjectError.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColString});
            this.LVObjectError.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LVObjectError.Location = new System.Drawing.Point(5, 136);
            this.LVObjectError.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.LVObjectError.Name = "LVObjectError";
            this.LVObjectError.Size = new System.Drawing.Size(1655, 298);
            this.LVObjectError.TabIndex = 3;
            this.LVObjectError.UseCompatibleStateImageBehavior = false;
            this.LVObjectError.View = System.Windows.Forms.View.Details;
            // 
            // ColString
            // 
            this.ColString.Text = "";
            this.ColString.Width = 630;
            // 
            // LabelError
            // 
            this.LabelError.AutoSize = true;
            this.LabelError.Location = new System.Drawing.Point(14, 362);
            this.LabelError.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.LabelError.Name = "LabelError";
            this.LabelError.Size = new System.Drawing.Size(113, 25);
            this.LabelError.TabIndex = 5;
            this.LabelError.Text = "neverseen";
            // 
            // LabMain
            // 
            this.LabMain.AutoSize = true;
            this.LabMain.Location = new System.Drawing.Point(30, 21);
            this.LabMain.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.LabMain.Name = "LabMain";
            this.LabMain.Size = new System.Drawing.Size(113, 25);
            this.LabMain.TabIndex = 6;
            this.LabMain.Text = "neverseen";
            // 
            // DetailMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1700, 805);
            this.ControlBox = false;
            this.Controls.Add(this.LabMain);
            this.Controls.Add(this.LabelError);
            this.Controls.Add(this.LVObjectError);
            this.Controls.Add(this.ButDetails);
            this.Controls.Add(this.ButOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "DetailMessageBox";
            this.Text = "DetailMessageBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButOk;
        private System.Windows.Forms.Button ButDetails;
        private System.Windows.Forms.ListView LVObjectError;
        private System.Windows.Forms.Label LabelError;
        private System.Windows.Forms.Label LabMain;
        private System.Windows.Forms.ColumnHeader ColString;
    }
}