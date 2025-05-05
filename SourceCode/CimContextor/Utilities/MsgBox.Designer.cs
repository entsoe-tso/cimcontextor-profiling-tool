/////////////////////////////////////////////////////////////////////////////////////////
// Author: Alexander Balka
// Date: 20221118
/////////////////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CimContextor.Utilities
{
    partial class MsgBox
    {
        private Thread msgThread;
 
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textLabel = new System.Windows.Forms.Label() {
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Left = 10
            };
            this.SuspendLayout();
            // 
            // textLabel
            // 
            this.textLabel.AutoEllipsis = true;
            this.textLabel.AutoSize = false;
            this.textLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLabel.Location = new System.Drawing.Point(60, 33);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(43, 25);
            this.textLabel.TabIndex = 0;
            this.textLabel.Text = "text";
            // 
            // MsgBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 100);
            this.Controls.Add(this.textLabel);
            this.Name = "MsgBox";
            this.Text = "Note";
            this.TopMost = true;
            this.ControlBox = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label textLabel;

        public void SetText(string txt)
        {
            textLabel.Text = txt;
        }

        public void ShowDlg()
        {
            
            this.ShowDialog();
        }

        public void ShowBox(string text)
        {
            this.textLabel.Text = text;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(text.Length * 15, 100);
            msgThread = new Thread(ShowDlg);
            msgThread.Start();
        }

        public void ShowLoading()
        {
            string text = "Loading...";
            this.textLabel.Text = text;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(text.Length * 15, 100);
            msgThread = new Thread(ShowDlg);
            msgThread.Start();
        }

        public void CloseBox()
        {
            this.Dispose();
            msgThread.Abort();
        }
    }
}