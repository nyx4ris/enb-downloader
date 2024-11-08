namespace ENBInstall {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbGame = new System.Windows.Forms.ComboBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbVersions = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game";
            // 
            // cmbGame
            // 
            this.cmbGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGame.Enabled = false;
            this.cmbGame.FormattingEnabled = true;
            this.cmbGame.Location = new System.Drawing.Point(12, 25);
            this.cmbGame.Name = "cmbGame";
            this.cmbGame.Size = new System.Drawing.Size(170, 21);
            this.cmbGame.TabIndex = 1;
            this.cmbGame.SelectedIndexChanged += new System.EventHandler(this.cmbGame_SelectedIndexChanged);
            // 
            // btnInstall
            // 
            this.btnInstall.Enabled = false;
            this.btnInstall.Location = new System.Drawing.Point(103, 92);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(79, 23);
            this.btnInstall.TabIndex = 5;
            this.btnInstall.Text = "Download";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 97);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(79, 13);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Made by Lilith";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Version";
            // 
            // cmbVersions
            // 
            this.cmbVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVersions.Enabled = false;
            this.cmbVersions.FormattingEnabled = true;
            this.cmbVersions.Location = new System.Drawing.Point(12, 65);
            this.cmbVersions.Name = "cmbVersions";
            this.cmbVersions.Size = new System.Drawing.Size(170, 21);
            this.cmbVersions.TabIndex = 8;
            this.cmbVersions.SelectedIndexChanged += new System.EventHandler(this.cmbVersions_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(194, 127);
            this.Controls.Add(this.cmbVersions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.cmbGame);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ENB Downloader";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbGame;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbVersions;
    }
}