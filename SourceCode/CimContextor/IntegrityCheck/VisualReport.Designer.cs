namespace CimContextor
{
    partial class VisualReport
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
            this.ButExit = new System.Windows.Forms.Button();
            this.LVCheckedElements = new System.Windows.Forms.ListView();
            this.ColName = new System.Windows.Forms.ColumnHeader();
            this.LVSelectedItem = new System.Windows.Forms.ListView();
            this.colError = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // ButExit
            // 
            this.ButExit.Location = new System.Drawing.Point(288, 229);
            this.ButExit.Name = "ButExit";
            this.ButExit.Size = new System.Drawing.Size(90, 25);
            this.ButExit.TabIndex = 0;
            this.ButExit.Text = "Exit";
            this.ButExit.UseVisualStyleBackColor = true;
            this.ButExit.Click += new System.EventHandler(this.ButExit_Click);
            // 
            // LVCheckedElements
            // 
            this.LVCheckedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName});
            this.LVCheckedElements.FullRowSelect = true;
            this.LVCheckedElements.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LVCheckedElements.Location = new System.Drawing.Point(9, 10);
            this.LVCheckedElements.MultiSelect = false;
            this.LVCheckedElements.Name = "LVCheckedElements";
            this.LVCheckedElements.Size = new System.Drawing.Size(352, 161);
            this.LVCheckedElements.TabIndex = 1;
            this.LVCheckedElements.UseCompatibleStateImageBehavior = false;
            this.LVCheckedElements.View = System.Windows.Forms.View.Details;
            this.LVCheckedElements.SelectedIndexChanged += new System.EventHandler(this.LVCheckedElements_SelectedIndexChanged);
            // 
            // ColName
            // 
            this.ColName.Text = "Checked element ";
            this.ColName.Width = 342;
            // 
            // LVSelectedItem
            // 
            this.LVSelectedItem.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colError});
            this.LVSelectedItem.Location = new System.Drawing.Point(379, 13);
            this.LVSelectedItem.MultiSelect = false;
            this.LVSelectedItem.Name = "LVSelectedItem";
            this.LVSelectedItem.Size = new System.Drawing.Size(342, 157);
            this.LVSelectedItem.TabIndex = 2;
            this.LVSelectedItem.UseCompatibleStateImageBehavior = false;
            this.LVSelectedItem.View = System.Windows.Forms.View.Details;
            this.LVSelectedItem.SelectedIndexChanged += new System.EventHandler(this.LVSelectedItem_SelectedIndexChanged);
            // 
            // colError
            // 
            this.colError.Text = "Error detail";
            this.colError.Width = 331;
            // 
            // VisualReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 266);
            this.ControlBox = false;
            this.Controls.Add(this.LVSelectedItem);
            this.Controls.Add(this.LVCheckedElements);
            this.Controls.Add(this.ButExit);
            this.Name = "VisualReport";
            this.Text = "VisualReport";
            this.Load += new System.EventHandler(this.VisualReport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButExit;
        private System.Windows.Forms.ListView LVCheckedElements;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ListView LVSelectedItem;
        private System.Windows.Forms.ColumnHeader colError;
    }
}