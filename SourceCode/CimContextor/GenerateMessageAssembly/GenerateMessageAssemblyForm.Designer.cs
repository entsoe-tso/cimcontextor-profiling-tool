namespace CimContextor
{
    partial class GenerateMessageAssemblyForm
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
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.TBFilePath = new System.Windows.Forms.TextBox();
            this.ButSelectFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ButExecuteMessageAssembly = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TBFilePath
            // 
            this.TBFilePath.Location = new System.Drawing.Point(12, 36);
            this.TBFilePath.Name = "TBFilePath";
            this.TBFilePath.Size = new System.Drawing.Size(312, 20);
            this.TBFilePath.TabIndex = 0;
            // 
            // ButSelectFolder
            // 
            this.ButSelectFolder.Location = new System.Drawing.Point(330, 36);
            this.ButSelectFolder.Name = "ButSelectFolder";
            this.ButSelectFolder.Size = new System.Drawing.Size(98, 20);
            this.ButSelectFolder.TabIndex = 1;
            this.ButSelectFolder.Text = "Browse";
            this.ButSelectFolder.UseVisualStyleBackColor = true;
            this.ButSelectFolder.Click += new System.EventHandler(this.ButSelectFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select the output file :";
            // 
            // ButExecuteMessageAssembly
            // 
            this.ButExecuteMessageAssembly.Location = new System.Drawing.Point(153, 62);
            this.ButExecuteMessageAssembly.Name = "ButExecuteMessageAssembly";
            this.ButExecuteMessageAssembly.Size = new System.Drawing.Size(111, 24);
            this.ButExecuteMessageAssembly.TabIndex = 3;
            this.ButExecuteMessageAssembly.Text = "Execute";
            this.ButExecuteMessageAssembly.UseVisualStyleBackColor = true;
            this.ButExecuteMessageAssembly.Click += new System.EventHandler(this.ButExecuteMessageAssembly_Click);
            // 
            // GenerateMessageAssemblyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 89);
            this.Controls.Add(this.ButExecuteMessageAssembly);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButSelectFolder);
            this.Controls.Add(this.TBFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GenerateMessageAssemblyForm";
            this.Text = "GenerateMessageAssemblyForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.TextBox TBFilePath;
        private System.Windows.Forms.Button ButSelectFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButExecuteMessageAssembly;
    }
}