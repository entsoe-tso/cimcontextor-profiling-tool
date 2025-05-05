using CimContextor.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace CimContextor.Option
{
    partial class AboutForm
    {
        private static readonly string licAgreementText = "Copyright [2023] [ENTSO-E]\r\n\r\n \r\n\r\nLicensed under the Apache License, Version 2.0 (the \"License\");\r\n\r\nyou may not use this file except in compliance with the License.\r\n\r\nYou may obtain a copy of the License at\r\n\r\n \r\n\r\n    http://www.apache.org/licenses/LICENSE-2.0\r\n\r\n \r\n\r\nUnless required by applicable law or agreed to in writing, software\r\n\r\ndistributed under the License is distributed on an \"AS IS\" BASIS,\r\n\r\nWITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.\r\n\r\nSee the License for the specific language governing permissions and\r\n\r\nlimitations under the License.";
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

        public void FillForm(string version, bool withLicense, string licKey)
        {
            Bitmap logoImg = FileManager.GetImageByName("CimContextor.Option.", "entose_logo-h-pos.png");
            if (logoImg != null)
            {
                this.pictureBox1.Image = logoImg;
            }

            this.versionLb.Text += version;

            if (withLicense)
            {
                string expiryDate = CimContextor.Utilities.LicenseManager.GetExpiryDate(licKey); 
                string convDate = DateTime.ParseExact(expiryDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd MMM yyyy");
                this.licenseTextLb.Text += convDate;
            }
            else
            {
                this.licenseTextLb.Text += "Unlimited License";
            }

            this.agrrementTB.Text = licAgreementText;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.productLb = new System.Windows.Forms.Label();
            this.versionLb = new System.Windows.Forms.Label();
            this.copyrightLb = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lineLb = new System.Windows.Forms.Label();
            this.licenseTextLb = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.agrrementTB = new System.Windows.Forms.TextBox();
            this.closeBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::CimContextor.Properties.Resources.entose_logo_h_pos;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(433, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // productLb
            // 
            this.productLb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.productLb.AutoSize = true;
            this.productLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productLb.Location = new System.Drawing.Point(627, 47);
            this.productLb.Name = "productLb";
            this.productLb.Size = new System.Drawing.Size(365, 50);
            this.productLb.TabIndex = 1;
            this.productLb.Text = "Enterprise Architect Add-in by ENTSO-E\r\nOriginal Author: Zamiren";
            // 
            // versionLb
            // 
            this.versionLb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLb.AutoSize = true;
            this.versionLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLb.Location = new System.Drawing.Point(627, 113);
            this.versionLb.Name = "versionLb";
            this.versionLb.Size = new System.Drawing.Size(169, 25);
            this.versionLb.TabIndex = 2;
            this.versionLb.Text = "Program Version: ";
            // 
            // copyrightLb
            // 
            this.copyrightLb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightLb.AutoSize = true;
            this.copyrightLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyrightLb.Location = new System.Drawing.Point(627, 156);
            this.copyrightLb.Name = "copyrightLb";
            this.copyrightLb.Size = new System.Drawing.Size(122, 25);
            this.copyrightLb.TabIndex = 3;
            this.copyrightLb.Text = "Copyright by";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 7);
            this.linkLabel1.Location = new System.Drawing.Point(746, 155);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(102, 25);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "ENTSO-E";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lineLb
            // 
            this.lineLb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lineLb.AutoSize = true;
            this.lineLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineLb.Location = new System.Drawing.Point(627, 184);
            this.lineLb.Name = "lineLb";
            this.lineLb.Size = new System.Drawing.Size(353, 25);
            this.lineLb.TabIndex = 5;
            this.lineLb.Text = "_______________________________";
            // 
            // licenseTextLb
            // 
            this.licenseTextLb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.licenseTextLb.AutoSize = true;
            this.licenseTextLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.licenseTextLb.Location = new System.Drawing.Point(627, 221);
            this.licenseTextLb.Name = "licenseTextLb";
            this.licenseTextLb.Size = new System.Drawing.Size(155, 25);
            this.licenseTextLb.TabIndex = 6;
            this.licenseTextLb.Text = "License Expiry:  ";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(627, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "CimConteXtor";
            // 
            // agrrementTB
            // 
            this.agrrementTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.agrrementTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.agrrementTB.Location = new System.Drawing.Point(24, 309);
            this.agrrementTB.Margin = new System.Windows.Forms.Padding(8);
            this.agrrementTB.Multiline = true;
            this.agrrementTB.Name = "agrrementTB";
            this.agrrementTB.ReadOnly = true;
            this.agrrementTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.agrrementTB.Size = new System.Drawing.Size(968, 270);
            this.agrrementTB.TabIndex = 8;
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.Location = new System.Drawing.Point(888, 620);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(104, 45);
            this.closeBtn.TabIndex = 9;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 268);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "LICENSE AGREEMENT";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 689);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.agrrementTB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.licenseTextLb);
            this.Controls.Add(this.lineLb);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.copyrightLb);
            this.Controls.Add(this.versionLb);
            this.Controls.Add(this.productLb);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label productLb;
        private System.Windows.Forms.Label versionLb;
        private System.Windows.Forms.Label copyrightLb;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label lineLb;
        private System.Windows.Forms.Label licenseTextLb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox agrrementTB;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Label label2;
    }
}